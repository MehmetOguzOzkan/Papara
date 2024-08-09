using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Papara.Business.Commands.CreateCategory;
using Papara.Business.DTOs.Category;
using Papara.Business.DTOs.Product;
using Papara.Business.DTOs.ProductCategory;
using Papara.Business.Queries.GetCategoryById;
using Papara.Business.Response;
using Papara.Data.Entities;
using Papara.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Papara.Business.Commands.CreateProduct
{
    internal class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ResponseHandler<ProductResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;
        private readonly IValidator<ProductRequest> _validator;

        public CreateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache memoryCache, IDistributedCache distributedCache, IValidator<ProductRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
            _validator = validator;
        }

        public async Task<ResponseHandler<ProductResponse>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ResponseHandler<ProductResponse>(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var mapped = _mapper.Map<Product>(request.Request);

            await _unitOfWork.ProductRepository.Insert(mapped);
            await _unitOfWork.CompleteWithTransaction();

            if (request.Request.CategoryIds != null && request.Request.CategoryIds.Any())
            {
                foreach (var categoryId in request.Request.CategoryIds)
                {
                    var productCategoryRequest = new ProductCategoryRequest
                    {
                        ProductId = mapped.Id,
                        CategoryId = categoryId
                    };
                    var mappedProductCategory = _mapper.Map<ProductCategory>(productCategoryRequest);
                    await _unitOfWork.ProductCategoryRepository.Insert(mappedProductCategory);
                }
            }

            await _unitOfWork.CompleteWithTransaction();

            _memoryCache.Remove("ProductList");
            //await _distributedCache.RemoveAsync("ProductList");

            var product = await _unitOfWork.ProductRepository.GetById(
                mapped.Id,
                $"{nameof(Product.ProductCategories)}.{nameof(ProductCategory.Category)}"
            ); 
            var response = _mapper.Map<ProductResponse>(product);
            return new ResponseHandler<ProductResponse>(response);
        }
    }
}
