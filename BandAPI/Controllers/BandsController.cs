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
    [Route("api/bands")]
    [ApiController]
    public class BandsController : ControllerBase
    {
        private readonly IBandAlbumRepository _bandAlbumRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;

        public BandsController(IBandAlbumRepository BandAlbumRepository, IMapper mapper,
                                IPropertyMappingService propertyMappingService)
        {
            _bandAlbumRepository = BandAlbumRepository ?? throw new ArgumentNullException(nameof(BandAlbumRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
        }

        // GET: api/Bands
        [HttpGet(Name = "GetBands")]
        [HttpHead]
        public ActionResult<IEnumerable<BandDto>> GetBands([FromQuery] BandsResourceParameters bandsResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExists<BandDto, Band>(bandsResourceParameters.OrderBy)) return BadRequest();

            var bandsFromRepo = _bandAlbumRepository.GetBands(bandsResourceParameters);

            var previousPageLink = bandsFromRepo.HasPrevious ? 
                CreateBandUri(bandsResourceParameters, UriType.PreviousPage) : null;
            
            var nextPageLink = bandsFromRepo.HasNext ? 
                CreateBandUri(bandsResourceParameters, UriType.NextPage) : null;

            var metaData = new
            {
                totalCount = bandsFromRepo.TotalCount,
                pageSize = bandsFromRepo.PageSize,
                currentPage = bandsFromRepo.CurrentPage,
                totalPages = bandsFromRepo.TotalPages,
                previousPageLink = previousPageLink,
                nextPageLink = nextPageLink
            };

            // Decoding: \u0026 -> &
            JsonSerializerOptions jso = new JsonSerializerOptions();
            jso.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

            Response.Headers.Add("Pagination", JsonSerializer.Serialize(metaData, jso));

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

        private string CreateBandUri(BandsResourceParameters bandsResourceParameters, UriType uriType)
        {
            switch (uriType)
            {
                case UriType.PreviousPage:
                    return Url.Link("GetBands", new
                    {
                        orderBy = bandsResourceParameters.OrderBy,
                        pageNumber = bandsResourceParameters.PageNumber - 1,
                        pageSize = bandsResourceParameters.PageSize,
                        mainGenre = bandsResourceParameters.MainGenre,
                        searchQuery = bandsResourceParameters.SearchQuery
                    });
                case UriType.NextPage:
                    return Url.Link("GetBands", new
                    {
                        orderBy = bandsResourceParameters.OrderBy,
                        pageNumber = bandsResourceParameters.PageNumber + 1,
                        pageSize = bandsResourceParameters.PageSize,
                        mainGenre = bandsResourceParameters.MainGenre,
                        searchQuery = bandsResourceParameters.SearchQuery
                    });
                default:
                    return Url.Link("GetBands", new
                    {
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
