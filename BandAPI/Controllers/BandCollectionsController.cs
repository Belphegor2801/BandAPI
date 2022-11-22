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
    [Route("api/bandcollections")]
    [ApiController]
    public class BandCollectionsController : ControllerBase
    {
        private readonly IBandAlbumRepository _bandAlbumRepository;
        private readonly IMapper _mapper;

        public BandCollectionsController(IBandAlbumRepository BandAlbumRepository, IMapper mapper)
        {
            _bandAlbumRepository = BandAlbumRepository ?? throw new ArgumentNullException(nameof(BandAlbumRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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

            return Ok();
        }
    }
}
