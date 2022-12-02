using System.Linq;
using System.Dynamic;
using BandAPI.Data.Entities;
using AutoMapper;
using BandAPI.Models;
using BandAPI.Common;

namespace BandAPI.Profiles
{
    public class BandsProfile : Profile
    {
        public BandsProfile()
        {
            CreateMap<Band, BandDto>()
                .ForMember(
                    dest => dest.FoundedYearsAgo,
                    opt => opt.MapFrom(src => $"{src.Founded.ToString("yyyy")} ({src.Founded.GetYearsAgo()}) years ago"));
            CreateMap<BandForCreatingDto, Band>();
            CreateMap<Band, ExpandoObject>();
            CreateMap<Pagination<Band>, Pagination<BandDto>>();
        }
    }
}
