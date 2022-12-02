using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using BandAPI.Data;
using BandAPI.Data.Entities;
using BandAPI.Services;
using BandAPI.Models;
using BandAPI.Common;
using AutoMapper;

namespace BandAPI.Controllers
{
    [ApiController]
    [Route("api/albums")]
    [ResponseCache(CacheProfileName = "90SecondsCacheProfile")]
    public class AlbumsController : ControllerBase
    {
        private readonly IBandAlbumRepository _bandAlbumRepository;

        public AlbumsController(IBandAlbumRepository BandAlbumRepository)
        {
            _bandAlbumRepository = BandAlbumRepository ?? throw new ArgumentNullException(nameof(BandAlbumRepository));
        }

        // GET: api/Bands
        [HttpGet]
        public async Task<IActionResult> GetAlbumsForBand(Guid bandId)
        {
            var result = await _bandAlbumRepository.GetAlbums(bandId);

            return new ObjectResult(result) { StatusCode = (int)result.Code };
        }

        [HttpGet("{albumId}", Name = "GetAlbumForBand")]
        [ResponseCache(Duration = 120)]
        public async Task<IActionResult> GetAlbumForBand(Guid bandId, Guid albumId)
        {
            var result = await _bandAlbumRepository.GetAlbum(bandId, albumId);

            return new ObjectResult(result) { StatusCode = (int)result.Code };
        }

        [HttpPost]
        public async Task<IActionResult> CreateAlbumForBand(Guid bandId, [FromBody] AlbumForCreatingDto album)
        {
            var result = await _bandAlbumRepository.AddAlbum(bandId, album);
            return Helper.TransformData(result);
        }

        [HttpPut("{albumId}")]
        public async Task<IActionResult> UpdateAlbumForBand(Guid bandId, Guid albumId, [FromBody] AlbumForUpdatingDto album)
        {
            var result = await _bandAlbumRepository.UpdateAlbum(bandId, albumId, album);
            return Helper.TransformData(result);
        }

        [HttpPatch("{albumId}")]
        public async Task<IActionResult> PartiallyUpdateAlbumForBand(Guid bandId, Guid albumId,
           [FromBody] JsonPatchDocument<AlbumForUpdatingDto> patchDocument)
        {
            var result = await _bandAlbumRepository.PartiallyUpdateAlbumForBand(bandId, albumId, patchDocument);
            return Helper.TransformData(result);
        }

        [HttpDelete("{albumId}")]
        public async Task<IActionResult> DeleteAlbumForBand(Guid bandId, Guid albumId)
        {
            var result = await _bandAlbumRepository.DeleteAlbum(bandId, albumId);
            return Helper.TransformData(result);
        }
    }
}