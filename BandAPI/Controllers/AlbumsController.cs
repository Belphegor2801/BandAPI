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
using BandAPI.Helpers;
using AutoMapper;

namespace BandAPI.Controllers
{
    [Route("api/albums")]
    [ApiController]
    public class AlbumsController : ControllerBase
    {
        private readonly IBandAlbumRepository _bandAlbumRepository;
        private readonly IMapper _mapper;

        public AlbumsController(IBandAlbumRepository BandAlbumRepository, IMapper mapper)
        {
            _bandAlbumRepository = BandAlbumRepository ?? throw new ArgumentNullException(nameof(BandAlbumRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // GET: api/Bands
        [HttpGet]
        public ActionResult<IEnumerable<AlbumDto>> GetAlbumsForBand(Guid bandId)
        {
            if (!_bandAlbumRepository.BandExists(bandId))
                return NotFound();

            var albumsFromRepo = _bandAlbumRepository.GetAlbums(bandId);
            return Ok(_mapper.Map<IEnumerable<AlbumDto>>(albumsFromRepo));
        }

        [HttpGet("{albumId}", Name = "GetAlbumForBand")]
        [ResponseCache(Duration = 120)]
        public ActionResult<AlbumDto> GetAlbumForBand(Guid bandId, Guid albumId)
        {
            if (!_bandAlbumRepository.BandExists(bandId))
                return NotFound();

            var albumFromRepo = _bandAlbumRepository.GetAlbum(bandId, albumId);
            if (albumFromRepo == null)
                return NotFound();

            return Ok(_mapper.Map<AlbumDto>(albumFromRepo));
        }

        [HttpPost]
        public ActionResult<AlbumDto> CreateAlbumForBand(Guid bandId, [FromBody] AlbumForCreatingDto album)
        {
            if (!_bandAlbumRepository.BandExists(bandId)) return NotFound();
            var albumEntity = _mapper.Map<Album>(album);
            _bandAlbumRepository.AddAlbum(bandId, albumEntity);
            _bandAlbumRepository.Save();

            var albumToReturn = _mapper.Map<AlbumDto>(albumEntity);
            return CreatedAtRoute("GetAlbumForBand", new { bandId = bandId, albumId = albumToReturn.Id }, albumToReturn);
        }

        [HttpPut("{albumId}")]
        public ActionResult UpdateAlbumForBand(Guid bandId, Guid albumId, [FromBody] AlbumForUpdatingDto album)
        {
            if (!_bandAlbumRepository.BandExists(bandId)) return NotFound();

            var albumFromRepo = _bandAlbumRepository.GetAlbum(bandId, albumId);
            if (albumFromRepo == null) return NotFound();

            _mapper.Map(album, albumFromRepo);
            _bandAlbumRepository.UpdateAlbum(albumFromRepo);
            _bandAlbumRepository.Save();

            return NoContent();
        }

        [HttpPatch]
        public ActionResult PartiallyUpdateAlbumForBand(Guid bandId, Guid albumId, JsonPatchDocument<AlbumForUpdatingDto> patchDocument)
        {
            if (!_bandAlbumRepository.BandExists(bandId)) return NotFound();

            var albumFromRepo = _bandAlbumRepository.GetAlbum(bandId, albumId);
            if (albumFromRepo == null) return NotFound();

            var albumToPatch = _mapper.Map<AlbumForUpdatingDto>(albumFromRepo);
            patchDocument.ApplyTo(albumToPatch);

            if (!TryValidateModel(albumToPatch)) return ValidationProblem(ModelState);

            _mapper.Map(albumToPatch, albumFromRepo);
            _bandAlbumRepository.UpdateAlbum(albumFromRepo);
            _bandAlbumRepository.Save();

            return NoContent();
        }

        [HttpDelete("{albumId}")]
        public ActionResult DeleteAlbumForBand(Guid bandId, Guid albumId)
        {
            if (!_bandAlbumRepository.BandExists(bandId)) return NotFound();

            var albumFromRepo = _bandAlbumRepository.GetAlbum(bandId, albumId);
            if (albumFromRepo == null) return NotFound();

            _bandAlbumRepository.DeleteAlbum(albumFromRepo);
            _bandAlbumRepository.Save();

            return NoContent();
        }
    }
}