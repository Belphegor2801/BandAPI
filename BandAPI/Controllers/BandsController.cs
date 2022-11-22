using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BandAPI.Data;
using BandAPI.Data.Entities;
using BandAPI.Services;
using BandAPI.Models;
using BandAPI.Helpers;
using AutoMapper;

namespace BandAPI.Controllers
{
    [Route("api/bands")]
    [ApiController]
    public class BandsController : ControllerBase
    {
        private readonly IBandAlbumRepository _bandAlbumRepository;
        private readonly IMapper _mapper;

        public BandsController(IBandAlbumRepository BandAlbumRepository, IMapper mapper)
        {
            _bandAlbumRepository = BandAlbumRepository ?? throw new ArgumentNullException(nameof(BandAlbumRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // GET: api/Bands
        [HttpGet(Name = "GetBands")]
        [HttpHead]
        public ActionResult<IEnumerable<BandDto>> GetBands([FromQuery] BandsResourceParameters bandsResourceParameters)
        {
            var bandsFromRepo = _bandAlbumRepository.GetBands(bandsResourceParameters);

            return Ok(_mapper.Map<IEnumerable<BandDto>>(bandsFromRepo));
        }

        [HttpGet("{bandId}", Name = "GetBand")]
        public ActionResult GetBand(Guid bandId)
        {
            var bandFromRepo = _bandAlbumRepository.GetBand(bandId);

            if (bandFromRepo == null)
                return NotFound();

            return new JsonResult(bandFromRepo);
        }

        [HttpPost(Name = "AddBand")]
        public ActionResult CreateBand([FromBody] BandForCreatingDto band)
        {
            var bandEntity = _mapper.Map<Band>(band);
            _bandAlbumRepository.AddBand(bandEntity);
            _bandAlbumRepository.Save();

            var bandToReturn = _mapper.Map<BandDto>(bandEntity);

            return CreatedAtRoute("GetBand", new { bandId = bandToReturn.Id }, bandToReturn);
        }

        [HttpOptions]
        public IActionResult GetBandsOptions()
        {
            Response.Headers.Add("Allow", "GET, POST, DELETE, HEAD, OPTIONS");
            return Ok();
        }

        [HttpDelete("{bandId}")]
        public ActionResult DeleteBand(Guid bandId)
        {
            var bandFromRepo = _bandAlbumRepository.GetBand(bandId);
            if (bandFromRepo == null) return NotFound();

            _bandAlbumRepository.DeleteBand(bandFromRepo);
            _bandAlbumRepository.Save();

            return NoContent();
        }
    }
}
