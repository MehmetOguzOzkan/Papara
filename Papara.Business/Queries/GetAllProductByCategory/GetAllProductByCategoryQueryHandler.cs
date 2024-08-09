using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Papara.Business.DTOs.Product;
using Papara.Business.Queries.GetAllProduct;
using Papara.Business.Response;
using Papara.Data.Entities;
using Papara.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Queries.GetAllProductByCategory
{
    internal class GetAllProductByCategoryQueryHandler : IRequestHandler<GetAllProductByCategoryQuery, ResponseHandler<IEnumerable<ProductResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllProductByCategoryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseHandler<IEnumerable<ProductResponse>>> Handle(GetAllProductByCategoryQuery request, CancellationToken cancellationToken)
        {
            var productCategories = await _unitOfWork.ProductCategoryRepository.Where(x => x.CategoryId == request.CategoryId);
            var productIds = productCategories.Select(pc => pc.ProductId).Distinct().ToList();

            var products = await _unitOfWork.ProductRepository.Where(p => productIds.Contains(p.Id), $"{nameof(Product.ProductCategories)}.{nameof(ProductCategory.Category)}");

            var productResponses = _mapper.Map<IEnumerable<ProductResponse>>(products);
            
            return new ResponseHandler<IEnumerable<ProductResponse>>(productResponses);
        }
    }
}
