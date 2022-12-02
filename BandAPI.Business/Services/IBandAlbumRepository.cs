using System;
using System.Collections.Generic;
using BandAPI.Data.Entities;
using BandAPI.Common;
using BandAPI.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;

namespace BandAPI.Services
{
    public interface IBandAlbumRepository
    {
        Task<Response> GetAlbums(Guid bandId);
        Task<Response> GetAlbum(Guid bandId, Guid albumId);
        Task<Response> AddAlbum(Guid bandId, AlbumForCreatingDto album);
        Task<Response> UpdateAlbum(Guid bandId, Guid albumId, AlbumForUpdatingDto album);
        Task<Response> PartiallyUpdateAlbumForBand(Guid bandId, Guid albumId, JsonPatchDocument<AlbumForUpdatingDto> patchDocument);
        Task<Response> DeleteAlbum(Guid bandId, Guid albumId);

        Task<Response> GetBands();
        Task<Response> GetBand(Guid bandId, string fields);
        Task<Response> GetBands(IEnumerable<Guid> bandIds);
        Task<Response> GetBands(BandsResourceParameters bandsResourceParameters);
        Task<Response> AddBand(BandForCreatingDto band);
        Task<Response> UpdateBand(Band band);
        Task<Response> DeleteBand(Guid bandId);
        Task<Response> AddBands(IEnumerable<BandForCreatingDto> bandCollection);


        bool BandExists(Guid bandId);
        bool AlbumExists(Guid albumId);
        bool Save();
    }
}
