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

            var previousPageLink = bandsResourceParameters.PageNumber > 1?
                CreateBandUri(bandsResourceParameters, UriType.PreviousPage) : null;

            var nextPageLink = bandsResourceParameters.PageNumber < bandsResourceParameters.PageSize?
                CreateBandUri(bandsResourceParameters, UriType.NextPage) : null;

            var metaData = new
            {
                pageSize = bandsResourceParameters.PageSize,
                currentPage = bandsResourceParameters.PageNumber,
                totalPages = bandsResourceParameters.PageSize,
                previousPageLink = previousPageLink,
                nextPageLink = nextPageLink
            };

            // Decoding: \u0026 -> &
            JsonSerializerOptions jso = new JsonSerializerOptions();
            jso.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

            Response.Headers.Add("Pagination", JsonSerializer.Serialize(metaData, jso));

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

        private string CreateBandUri(BandsResourceParameters bandsResourceParameters, UriType uriType)
        {
            switch (uriType)
            {
                case UriType.PreviousPage:
                    return Url.Link("GetBands", new
                    {
                        fields = bandsResourceParameters.Fields,
                        orderBy = bandsResourceParameters.OrderBy,
                        pageNumber = bandsResourceParameters.PageNumber - 1,
                        pageSize = bandsResourceParameters.PageSize,
                        mainGenre = bandsResourceParameters.MainGenre,
                        searchQuery = bandsResourceParameters.SearchQuery
                    });
                case UriType.NextPage:
                    return Url.Link("GetBands", new
                    {
                        fields = bandsResourceParameters.Fields,
                        orderBy = bandsResourceParameters.OrderBy,
                        pageNumber = bandsResourceParameters.PageNumber + 1,
                        pageSize = bandsResourceParameters.PageSize,
                        mainGenre = bandsResourceParameters.MainGenre,
                        searchQuery = bandsResourceParameters.SearchQuery
                    });
                default:
                    return Url.Link("GetBands", new
                    {
                        fields = bandsResourceParameters.Fields,
                        orderBy = bandsResourceParameters.OrderBy,
                        pageNumber = bandsResourceParameters.PageNumber,
                        pageSize = bandsResourceParameters.PageSize,
                        mainGenre = bandsResourceParameters.MainGenre,
                        searchQuery = bandsResourceParameters.SearchQuery
                    });
            }
        }
    }
}
