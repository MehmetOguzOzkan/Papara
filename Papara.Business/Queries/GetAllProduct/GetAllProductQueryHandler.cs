using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Papara.Business.DTOs.Category;
using Papara.Business.DTOs.Product;
using Papara.Business.Queries.GetAllCategory;
using Papara.Business.Response;
using Papara.Data.Entities;
using Papara.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Queries.GetAllProduct
{
    internal class GetAllProductQueryHandler : IRequestHandler<GetAllProductQuery, ResponseHandler<IEnumerable<ProductResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;

        public GetAllProductQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _memoryCache = memoryCache;
        }

        public async Task<ResponseHandler<IEnumerable<ProductResponse>>> Handle(GetAllProductQuery request, CancellationToken cancellationToken)
        {
            var checkResult = _memoryCache.TryGetValue("ProductList", out ResponseHandler<IEnumerable<ProductResponse>> cacheData);
            if (checkResult)
                return cacheData;

            IEnumerable<Product> entityList = await _unitOfWork.ProductRepository.GetAll($"{nameof(Product.ProductCategories)}.{nameof(ProductCategory.Category)}");
            var mappedList = _mapper.Map<IEnumerable<ProductResponse>>(entityList);
            var response = new ResponseHandler<IEnumerable<ProductResponse>>(mappedList);

            if (entityList.Any()) {
                var cacheOptions = new MemoryCacheEntryOptions()
                {
                    Priority = CacheItemPriority.High,
                    AbsoluteExpiration = DateTime.Now.AddDays(1),
                    SlidingExpiration = TimeSpan.FromHours(1)
                };
                _memoryCache.Set("ProductList", response, cacheOptions);
            }

            return response;
        }
    }
}
