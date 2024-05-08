using DataCollector.Dtos.Category;
using DataCollector.Dtos.Store;
using DataCollector.Models;
using AutoMapper;
using System.Globalization;

namespace DataCollector.Data
{
    public class MapperProfiles : Profile
    {
        public MapperProfiles()
        {
            var turkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");

            #region Category
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();

            // Assign the TimeZoneInfo instance to a local variable

            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src =>
                    TimeZoneInfo.ConvertTimeToUtc(src.CreationDate, turkeyTimeZone)
                    /*.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture)*/))
                .ReverseMap();
            //    CreateMap<Category, CategoryDto>()
            //        .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src =>
            //TimeZoneInfo.ConvertTimeToUtc(src.CreationDate, TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time"))
            //.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture)))
            //        .ReverseMap();


            #endregion

            #region Store
            CreateMap<CreateStoreDto, Store>();
            CreateMap<UpdateStoreDto, Store>();
            CreateMap<StoreDto, Store>();

            CreateMap<Store, StoreForExportDto>()
                 .ForMember(dest => dest.CreatorName, opt => opt.MapFrom(src => src.CreatedByUser.FullName))
                 .ForMember(dest => dest.Assets, opt => opt.MapFrom(src => src.StorePhotos));

            CreateMap<Store, StoreDto>()
                .ForMember(dest => dest.CreatorName, opt => opt.MapFrom(src => src.CreatedByUser.FullName))
                .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src =>
                    TimeZoneInfo.ConvertTimeToUtc(src.CreationDate, turkeyTimeZone)
                   /* .ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture)*/))
            .ForMember(dest => dest.Assets, opt => opt.MapFrom(src => src.StorePhotos));

            CreateMap<StorePhoto, StorePhotoDto>()
                .ForMember(dest => dest.AssetId, opt => opt.MapFrom(src => src.Id))
                  .ForMember(dest => dest.AssetURL, opt => opt.MapFrom(src => src.PhotoURL));
            #endregion
        }
    }
}
