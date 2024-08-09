using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Papara.Business.Commands.UpdateCategory;
using Papara.Business.DTOs.Category;
using Papara.Business.Response;
using Papara.Data.Entities;
using Papara.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Commands.DeleteCategory
{
    internal class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, ResponseHandler>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;

        public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache memoryCache, IDistributedCache distributedCache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
        }


        public async Task<ResponseHandler> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.CategoryRepository.GetById(request.Id);
            if (category == null)
                return new ResponseHandler("Category not found.");

            var productsInCategory = await _unitOfWork.ProductCategoryRepository.Where(pc => pc.CategoryId == request.Id);
            if (productsInCategory.Any())
                return new ResponseHandler("Cannot delete category. There are products associated with this category.");

            await _unitOfWork.CategoryRepository.SoftDelete(request.Id);
            await _unitOfWork.CompleteWithTransaction();

            _memoryCache.Remove("CategoryList");
            //await _distributedCache.RemoveAsync("CategoryList");

            return new ResponseHandler("Category deleted successfully");
        }
    }
}
