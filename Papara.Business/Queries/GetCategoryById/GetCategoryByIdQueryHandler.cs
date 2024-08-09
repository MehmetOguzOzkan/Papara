using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Papara.Business.DTOs.Category;
using Papara.Business.Response;
using Papara.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Queries.GetCategoryById
{
    internal class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, ResponseHandler<CategoryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;        
        }

        public async Task<ResponseHandler<CategoryResponse>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.CategoryRepository.GetById(request.Id);
            var mapped = _mapper.Map<CategoryResponse>(entity);
            return new ResponseHandler<CategoryResponse>(mapped);
        }
    }
}
