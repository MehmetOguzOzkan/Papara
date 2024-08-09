using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Papara.Business.DTOs.Category;
using Papara.Business.Response;
using Papara.Data.Entities;
using Papara.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Queries.GetAllCategoryFromCache
{
    internal class GetAllCategoryFromCacheQueryHandler : IRequestHandler<GetAllCategoryFromCacheQuery, ResponseHandler<IEnumerable<CategoryResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;

        public GetAllCategoryFromCacheQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }

        public async Task<ResponseHandler<IEnumerable<CategoryResponse>>> Handle(GetAllCategoryFromCacheQuery request, CancellationToken cancellationToken)
        {
            var checkResult = await _distributedCache.GetAsync("CategoryList");
            if (checkResult != null)
            {
                string json = Encoding.UTF8.GetString(checkResult);
                var responseObject = JsonConvert.DeserializeObject<IEnumerable<CategoryResponse>>(json);
                return new ResponseHandler<IEnumerable<CategoryResponse>>(responseObject);
            }

            IEnumerable<Category> categoryList = await _unitOfWork.CategoryRepository.GetAll();
            var mappedList = _mapper.Map<IEnumerable<CategoryResponse>>(categoryList);
            var response = new ResponseHandler<IEnumerable<CategoryResponse>>(mappedList);

            if (categoryList.Any())
            {
                string responseString = JsonConvert.SerializeObject(mappedList);
                byte[] responseArray = Encoding.UTF8.GetBytes(responseString);
                var cacheOptions = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddDays(1),
                    SlidingExpiration = TimeSpan.FromHours(1)
                };
                await _distributedCache.SetAsync("CategoryList", responseArray, cacheOptions);
            }

            return response;

        }
    }
}
