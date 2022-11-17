using System;
using BandAPI.Data.Entities;
using AutoMapper;
using BandAPI.Helpers;

namespace BandAPI.Profiles
{
    public class BandsProfile : Profile
    {
        public BandsProfile()
        {
            CreateMap<Band, Models.BandDto>()
                .ForMember(
                    dest => dest.FoundedYearsAgo,
                    opt => opt.MapFrom(src => $"{src.Founded.ToString("yyyy")} ({src.Founded.GetYearsAgo()}) years ago"));
        }
    }
}
