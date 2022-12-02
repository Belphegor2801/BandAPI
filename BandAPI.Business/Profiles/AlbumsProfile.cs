using System;
using BandAPI.Data.Entities;
using BandAPI.Models;
using BandAPI.Common;
using AutoMapper;

namespace BandAPI.Business.Profiles
{
    public class AlbumsProfile: Profile
    {
        public AlbumsProfile()
        {
            CreateMap<Album, AlbumDto>().ReverseMap();
            CreateMap<AlbumForCreatingDto, Album>();
            CreateMap<AlbumForUpdatingDto, Album>().ReverseMap();
        }
    }
}
