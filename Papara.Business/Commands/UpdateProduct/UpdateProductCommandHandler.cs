using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Papara.Business.DTOs.Product;
using Papara.Business.DTOs.ProductCategory;
using Papara.Business.Response;
using Papara.Data.Entities;
using Papara.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Commands.UpdateProduct
{
    internal class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ResponseHandler<ProductResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;
        private readonly IValidator<ProductRequest> _validator;

        public UpdateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache memoryCache, IDistributedCache distributedCache, IValidator<ProductRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
            _validator = validator;
        }

        public async Task<ResponseHandler<ProductResponse>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ResponseHandler<ProductResponse>(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var product = await _unitOfWork.ProductRepository.GetById(request.Id, $"{nameof(Product.ProductCategories)}.{nameof(ProductCategory.Category)}");
            if (product == null)
                return new ResponseHandler<ProductResponse>("Product not found.");

            _mapper.Map(request.Request, product);

            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.CompleteWithTransaction();

            var existingProductCategories = product.ProductCategories.ToList();
            foreach (var productCategory in existingProductCategories)
            {
                _unitOfWork.ProductCategoryRepository.Delete(productCategory);
            }
            await _unitOfWork.CompleteWithTransaction();

            if (request.Request.CategoryIds != null && request.Request.CategoryIds.Any())
            {
                foreach (var categoryId in request.Request.CategoryIds)
                {
                    var productCategoryRequest = new ProductCategoryRequest
                    {
                        ProductId = product.Id,
                        CategoryId = categoryId
                    };
                    var mappedProductCategory = _mapper.Map<ProductCategory>(productCategoryRequest);
                    await _unitOfWork.ProductCategoryRepository.Insert(mappedProductCategory);
                }
            }
            await _unitOfWork.CompleteWithTransaction();

            _memoryCache.Remove("ProductList");

            var updatedProduct = await _unitOfWork.ProductRepository.GetById(product.Id, $"{nameof(Product.ProductCategories)}.{nameof(ProductCategory.Category)}");
            var response = _mapper.Map<ProductResponse>(updatedProduct);
            return new ResponseHandler<ProductResponse>(response);
        }
    }
}
