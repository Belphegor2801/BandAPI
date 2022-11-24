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
        private readonly IMapper _mapper;

        public BandCollectionsController(IBandAlbumRepository BandAlbumRepository, IMapper mapper)
        {
            _bandAlbumRepository = BandAlbumRepository ?? throw new ArgumentNullException(nameof(BandAlbumRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("({ids})", Name = "GetBandCollection")]
        public IActionResult GetBandCollection([FromRoute][ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids == null) return BadRequest();

            var bandEntities = _bandAlbumRepository.GetBands(ids);

            if (ids.Count() != bandEntities.Count()) return NotFound();

            var bandToReturn = _mapper.Map<IEnumerable<BandDto>>(bandEntities);

            return Ok(bandToReturn);
        }

        [HttpPost]
        public ActionResult<IEnumerable<BandDto>> CreateBandCollection([FromBody] IEnumerable<BandForCreatingDto> bandCollection)
        {
            var bandEntities = _mapper.Map<IEnumerable<Band>>(bandCollection);

            foreach(var band in bandEntities)
            {
                _bandAlbumRepository.AddBand(band);
            }
            _bandAlbumRepository.Save();

            var bandCollectionToReturn = _mapper.Map<IEnumerable<BandDto>>(bandEntities);
            var IdsString = string.Join(",", bandCollectionToReturn.Select(b => b.Id));

            return CreatedAtRoute("GetBandCollection", new { ids = IdsString }, bandCollectionToReturn);
        }
    }
}
