﻿using System;
using BandAPI.Data.Entities;
using AutoMapper;
using BandAPI.Helpers;
using BandAPI.Models;

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
        }
    }
}
