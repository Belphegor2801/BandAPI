using System;
using System.Text.Json;
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
    [ApiController]
    [Route("api/bands")]
    public class BandsController : ControllerBase
    {
        private readonly IBandAlbumRepository _bandAlbumRepository;

        public BandsController(IBandAlbumRepository BandAlbumRepository)
        {
            _bandAlbumRepository = BandAlbumRepository ?? throw new ArgumentNullException(nameof(BandAlbumRepository));
        }

        // GET: api/Bands
        [HttpGet(Name = "GetBands")]
        [HttpHead]
        public async Task<IActionResult> GetBands([FromQuery] BandsResourceParameters bandsResourceParameters)
        {
            var result = await _bandAlbumRepository.GetBands(bandsResourceParameters);

            return new ObjectResult(result) { StatusCode = (int)result.Code };
        }

        [HttpGet("{bandId}", Name = "GetBand")]
        public async Task<IActionResult> GetBand(Guid bandId, string fields)
        {
            var result = await _bandAlbumRepository.GetBand(bandId, fields);
            return new ObjectResult(result) { StatusCode = (int)result.Code };
        }

        [HttpPost(Name = "AddBand")]
        public async Task<IActionResult> CreateBand([FromBody] BandForCreatingDto band)
        {
            var result = await _bandAlbumRepository.AddBand(band);
            return new ObjectResult(result) { StatusCode = (int)result.Code };
        }

        [HttpOptions]
        public ActionResult GetBandsOptions()
        {
            Response.Headers.Add("Allow", "GET, POST, DELETE, HEAD, OPTIONS");
            return Ok();
        }

        [HttpDelete("{bandId}")]
        public async Task<ActionResult> DeleteBand(Guid bandId)
        {
            var result = await _bandAlbumRepository.DeleteBand(bandId);
            return new ObjectResult(result) { StatusCode = (int)result.Code };
        }
    }
}
