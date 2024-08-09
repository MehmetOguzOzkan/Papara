using AutoMapper;
using MediatR;
using Papara.Business.DTOs.Category;
using Papara.Business.DTOs.Product;
using Papara.Business.Queries.GetCategoryById;
using Papara.Business.Response;
using Papara.Data.Entities;
using Papara.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Queries.GetProductById
{
    internal class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ResponseHandler<ProductResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetProductByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseHandler<ProductResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.ProductRepository.GetById(request.Id, $"{nameof(Product.ProductCategories)}.{nameof(ProductCategory.Category)}");
            var mapped = _mapper.Map<ProductResponse>(entity);
            return new ResponseHandler<ProductResponse>(mapped);
        }
    }
}
