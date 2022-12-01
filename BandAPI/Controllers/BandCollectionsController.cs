using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
    [Route("api/bandcollections")]
    public class BandCollectionsController : ControllerBase
    {
        private readonly IBandAlbumRepository _bandAlbumRepository;

        public BandCollectionsController(IBandAlbumRepository BandAlbumRepository)
        {
            _bandAlbumRepository = BandAlbumRepository ?? throw new ArgumentNullException(nameof(BandAlbumRepository));
        }

        [HttpGet("({ids})", Name = "GetBandCollection")]
        public async Task<IActionResult> GetBandCollection([FromRoute][ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            var result = await _bandAlbumRepository.GetBands(ids);

            return new ObjectResult(result) { StatusCode = (int)result.Code };
        }

        [HttpPost]
        public async Task<ActionResult> CreateBandCollection([FromBody] IEnumerable<BandForCreatingDto> bandCollection)
        {
            var result = await _bandAlbumRepository.AddBands(bandCollection);

            return new ObjectResult(result) { StatusCode = (int)result.Code };
        }
    }
}
