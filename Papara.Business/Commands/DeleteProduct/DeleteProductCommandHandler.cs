using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Papara.Business.Response;
using Papara.Data.Entities;
using Papara.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Commands.DeleteProduct
{
    internal class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, ResponseHandler>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;

        public DeleteProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache memoryCache, IDistributedCache distributedCache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
        }

        public async Task<ResponseHandler> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.ProductRepository.GetById(request.Id, $"{nameof(Product.ProductCategories)}.{nameof(ProductCategory.Category)}");
            if (product == null)
                return new ResponseHandler("Product not found.");

            var existingProductCategories = product.ProductCategories.ToList();
            foreach (var productCategory in existingProductCategories)
            {
                _unitOfWork.ProductCategoryRepository.Delete(productCategory);
            }
            await _unitOfWork.CompleteWithTransaction();

            _unitOfWork.ProductRepository.SoftDelete(product);
            await _unitOfWork.CompleteWithTransaction();

            _memoryCache.Remove("ProductList");
            //await _distributedCache.RemoveAsync("ProductList");

            return new ResponseHandler("Product deleted successfully.");
        }
    }
}
