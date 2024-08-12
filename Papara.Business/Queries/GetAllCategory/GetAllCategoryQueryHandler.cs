using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Papara.Business.DTOs.Category;
using Papara.Business.Response;
using Papara.Data.Entities;
using Papara.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Queries.GetAllCategory
{
    public class GetAllCategoryQueryHandler : IRequestHandler<GetAllCategoryQuery, ResponseHandler<IEnumerable<CategoryResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;

        public GetAllCategoryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _memoryCache = memoryCache;
        }

        public async Task<ResponseHandler<IEnumerable<CategoryResponse>>> Handle(GetAllCategoryQuery request, CancellationToken cancellationToken)
        {
            var checkResult = _memoryCache.TryGetValue("CategoryList", out ResponseHandler<IEnumerable<CategoryResponse>> cacheData);
            if (checkResult)
                return cacheData;

            IEnumerable<Category> entityList = await _unitOfWork.CategoryRepository.GetAll();
            var mappedList = _mapper.Map<IEnumerable<CategoryResponse>>(entityList);
            var response = new ResponseHandler<IEnumerable<CategoryResponse>>(mappedList);

            if (entityList.Any())
            {
                var cacheOptions = new MemoryCacheEntryOptions()
                {
                    Priority = CacheItemPriority.High,
                    AbsoluteExpiration = DateTime.Now.AddDays(1),
                    SlidingExpiration = TimeSpan.FromHours(1)
                };
                _memoryCache.Set("CategoryList", response, cacheOptions);
            }

            return response;
        }
    }
}
