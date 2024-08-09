﻿using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Papara.Business.DTOs.Category;
using Papara.Business.Response;
using Papara.Data.Entities;
using Papara.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Papara.Business.Commands.UpdateCategory
{
    internal class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, ResponseHandler<CategoryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;
        private readonly IValidator<CategoryRequest> _validator;

        public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache memoryCache, IDistributedCache distributedCache, IValidator<CategoryRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
            _validator = validator;
        }

        public async Task<ResponseHandler<CategoryResponse>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ResponseHandler<CategoryResponse>(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var category = await _unitOfWork.CategoryRepository.GetById(request.Id);
            if (category == null)
                return new ResponseHandler<CategoryResponse>("Category not found.");

            _mapper.Map(request.Request, category);
            category.Url = GenerateSlug(category.Name);

            _unitOfWork.CategoryRepository.Update(category);
            await _unitOfWork.CompleteWithTransaction();

            _memoryCache.Remove("CategoryList");
            //await _distributedCache.RemoveAsync("CategoryList");

            var response = _mapper.Map<CategoryResponse>(category);
            return new ResponseHandler<CategoryResponse>(response);
        }

        private string GenerateSlug(string name)
        {
            string slug = name.ToLowerInvariant();

            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", string.Empty);
            slug = Regex.Replace(slug, @"\s+", " ").Trim();
            slug = slug.Substring(0, slug.Length <= 45 ? slug.Length : 45).Trim();
            slug = Regex.Replace(slug, @"\s", "-");

            return slug;
        }
    }
}
