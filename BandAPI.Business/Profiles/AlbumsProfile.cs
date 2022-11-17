using System;
using BandAPI.Data.Entities;
using AutoMapper;
using BandAPI.Helpers;

namespace BandAPI.Business.Profiles
{
    public class AlbumsProfile: Profile
    {
        public AlbumsProfile()
        {
            CreateMap<Album, Models.AlbumDto>().ReverseMap();
        }
    }
}
