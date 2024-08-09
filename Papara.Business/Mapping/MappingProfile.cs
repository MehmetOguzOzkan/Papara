using AutoMapper;
using Papara.Business.DTOs.Category;
using Papara.Business.DTOs.Coupon;
using Papara.Business.DTOs.Order;
using Papara.Business.DTOs.OrderDetail;
using Papara.Business.DTOs.Product;
using Papara.Business.DTOs.ProductCategory;
using Papara.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Category Mapping
            CreateMap<CategoryRequest, Category>()
                .ForMember(dest => dest.Url, opt => opt.Ignore());
            CreateMap<Category, CategoryResponse>();

            // Product Mapping
            CreateMap<ProductRequest, Product>()
                .ForMember(dest => dest.ProductCategories, opt => opt.Ignore());
            CreateMap<Product, ProductResponse>()
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.ProductCategories.Select(pc => pc.Category)));

            // ProductCategory Mapping
            CreateMap<ProductCategory, ProductCategoryRequest>().ReverseMap();

            // Coupon Mapping
            CreateMap<CouponRequest, Coupon>()
                .ForMember(dest => dest.Code, opt => opt.Ignore());
            CreateMap<Coupon, CouponResponse>();

            // Order Mapping
            CreateMap<OrderRequest, Order>()
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
                .ForMember(dest => dest.OrderDate, opt => opt.Ignore())
                .ForMember(dest => dest.Code, opt => opt.Ignore());
            CreateMap<Order, OrderResponse>();

            // OrderDetail Mapping
            CreateMap<OrderDetailRequest, OrderDetail>();
            CreateMap<OrderDetail, OrderDetailResponse>()
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product));
            CreateMap<Product, OrderDetailProductResponse>();
        }
    }
}
