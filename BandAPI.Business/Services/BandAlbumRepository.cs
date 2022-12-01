using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Threading.Tasks;
using BandAPI.Data;
using BandAPI.Data.Entities;
using BandAPI.Helpers;
using BandAPI.Models;
using BandAPI.Common;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;

namespace BandAPI.Services
{
    public class BandAlbumRepository : IBandAlbumRepository
    {
        private readonly BandAlbumDBContext _dbcontext;
        private readonly IMapper _mapper;

        public BandAlbumRepository(BandAlbumDBContext DBContext, IMapper mapper)
        {
            _dbcontext = DBContext ?? throw new ArgumentNullException(nameof(DBContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        async Task<Response> IBandAlbumRepository.GetAlbum(Guid bandId, Guid albumId)
        {
            if (bandId == Guid.Empty)
                return new ResponseError(System.Net.HttpStatusCode.NotFound, "BandId is null!");
            if (albumId == Guid.Empty)
                return new ResponseError(System.Net.HttpStatusCode.NotFound, "AlbumId is null!");
            var albumToReturn = _dbcontext.Albums.Where(a => a.BandId == bandId && a.Id == albumId).FirstOrDefault();
            if (albumToReturn == null) 
                return new ResponseError(System.Net.HttpStatusCode.NotFound, "Album not found!");
            return new ResponseObject<Album>(albumToReturn);
        }

        async Task<Response> IBandAlbumRepository.GetAlbums(Guid bandId)
        {
            if (bandId == Guid.Empty)
                return new ResponseError(System.Net.HttpStatusCode.NotFound, "BandId not found!");

            var albumsFromRepo = _dbcontext.Albums.Where(a => a.BandId == bandId).OrderBy(a => a.Title).ToList();
            if (albumsFromRepo == null) 
                return new ResponseError(System.Net.HttpStatusCode.NotFound, "Album not found!");
            var albumsToReturn = _mapper.Map<IEnumerable<AlbumDto>>(albumsFromRepo);
            return new ResponseList<AlbumDto>(albumsToReturn.ToList());
        }

        async Task<Response> IBandAlbumRepository.AddAlbum(Guid bandId, AlbumForCreatingDto album)
        {
            if (!((IBandAlbumRepository)this).BandExists(bandId))
                return new ResponseError(System.Net.HttpStatusCode.NotFound, "Band not found!");
            if (album == null)
                return new ResponseError(System.Net.HttpStatusCode.NotFound, "Album not found!");

            var albumEntity = _mapper.Map<Album>(album);
            albumEntity.BandId = bandId;
            _dbcontext.Albums.Add(albumEntity);
            _dbcontext.SaveChanges();

            return new Response(System.Net.HttpStatusCode.OK, "Success!");
        }

        async Task<Response> IBandAlbumRepository.UpdateAlbum(Guid bandId, Guid albumId,  AlbumForUpdatingDto album)
        {
            if (!((IBandAlbumRepository)this).BandExists(bandId))
                return new ResponseError(System.Net.HttpStatusCode.NotFound, "BandId not found!");

            var albumFromRepo = _dbcontext.Albums.Where(a => a.BandId == bandId && a.Id == albumId).FirstOrDefault();
            if (albumFromRepo == null) // Add album if not exist
            {
                var albumToAdd = _mapper.Map<Album>(album);
                albumToAdd.Id = albumId;
                albumToAdd.BandId = bandId;
                _dbcontext.Add(albumToAdd);
                _dbcontext.SaveChanges();

                var albumToReturn = _mapper.Map<AlbumDto>(albumToAdd);
                return new ResponseUpdate(System.Net.HttpStatusCode.OK, "Success", albumFromRepo.Id);
            }

            _mapper.Map(album, albumFromRepo);
            _dbcontext.Albums.Update(albumFromRepo);
            _dbcontext.SaveChanges();

            return new ResponseUpdate(System.Net.HttpStatusCode.OK, "Success", albumFromRepo.Id);
            //throw new NotImplementedException();
        }

        async Task<Response> IBandAlbumRepository.PartiallyUpdateAlbumForBand(Guid bandId, Guid albumId, JsonPatchDocument<AlbumForUpdatingDto> patchDocument)
        {
            if (!((IBandAlbumRepository)this).BandExists(bandId))
                return new ResponseError(System.Net.HttpStatusCode.NotFound, "BandId not found!");

            var albumFromRepo = _dbcontext.Albums.Where(a => a.BandId == bandId && a.Id == albumId).FirstOrDefault();
            if (albumFromRepo == null)
            {
                var albumDto = new AlbumForUpdatingDto();
                patchDocument.ApplyTo(albumDto);
                var albumToAdd = _mapper.Map<Album>(albumDto);
                albumToAdd.Id = albumId;

                _dbcontext.Albums.Add(albumToAdd);
                _dbcontext.SaveChanges();

                var albumToReturn = _mapper.Map<AlbumDto>(albumToAdd);

                return new ResponseUpdate(System.Net.HttpStatusCode.OK, "Success", albumFromRepo.Id);
            }

            var albumToPatch = _mapper.Map<AlbumForUpdatingDto>(albumFromRepo);
            patchDocument.ApplyTo(albumToPatch);

            _mapper.Map(albumToPatch, albumFromRepo);
            _dbcontext.Albums.Update(albumFromRepo);
            _dbcontext.SaveChanges();

            return new ResponseUpdate(System.Net.HttpStatusCode.OK, "Success", albumFromRepo.Id);
        }

        async Task<Response> IBandAlbumRepository.DeleteAlbum(Guid bandId, Guid albumId)
        {
            if (!((IBandAlbumRepository)this).BandExists(bandId))
                return new ResponseError(System.Net.HttpStatusCode.NotFound, "BandId not found!");

            var albumFromRepo = _dbcontext.Albums.Where(a => a.BandId == bandId && a.Id == albumId).FirstOrDefault();
            if (albumFromRepo == null)
                return new ResponseError(System.Net.HttpStatusCode.NotFound, "Album not found!");

            _dbcontext.Albums.Remove(albumFromRepo);
            _dbcontext.SaveChanges();

            return new ResponseDelete(System.Net.HttpStatusCode.OK, "Success", albumId, albumFromRepo.Title);
        }


        async Task<Response> IBandAlbumRepository.GetBand(Guid bandId, string fields)
        {
            if (bandId == Guid.Empty)
                return new ResponseError(System.Net.HttpStatusCode.NotFound, "BandId not found!");

            var bandFromRepo = _dbcontext.Bands.FirstOrDefault(b => b.Id == bandId);
            if (bandFromRepo == null)
                return new ResponseError(System.Net.HttpStatusCode.NotFound, "Band not found!");

            var result = _mapper.Map<BandDto>(bandFromRepo).ShapeData(fields);

            return new ResponseObject<ExpandoObject>(result);

        }

        async Task<Response> IBandAlbumRepository.GetBands()
        {
            return new ResponseList<Band>(_dbcontext.Bands.ToList());
        }

        async Task<Response> IBandAlbumRepository.GetBands(IEnumerable<Guid> bandIds)
        {
            if (bandIds == null)
                return new ResponseError(System.Net.HttpStatusCode.NotFound, "BandId not found!");

            return new ResponseList<Band>(_dbcontext.Bands.Where(b => bandIds.Contains(b.Id)).OrderBy(b => b.Name).ToList());
        }

        async Task<Response> IBandAlbumRepository.GetBands(BandsResourceParameters bandsResourceParameters)
        {
            if (bandsResourceParameters == null)
                return new ResponseError(System.Net.HttpStatusCode.NotFound, "Bands's resouces parameters not found!");

            var collection = _dbcontext.Bands as IQueryable<Band>;

            if (!string.IsNullOrWhiteSpace(bandsResourceParameters.OrderBy))
            {
                collection = collection.ApplySorting(bandsResourceParameters.OrderBy);
            }

            if (!string.IsNullOrWhiteSpace(bandsResourceParameters.MainGenre))
            {
                var mainGenre = bandsResourceParameters.MainGenre.Trim();
                collection = collection.Where(b => b.MainGenre == mainGenre);
            }

            if (!string.IsNullOrWhiteSpace(bandsResourceParameters.SearchQuery))
            {
                var searchQuery = bandsResourceParameters.SearchQuery.Trim();
                collection = collection.Where(b => b.Name.Contains(searchQuery));
            }
            var paginationRequest = new PaginationRequest()
            {
                Fields = bandsResourceParameters.Fields,
                Page = bandsResourceParameters.PageNumber,
                Size = bandsResourceParameters.PageSize,
                Sort = bandsResourceParameters.OrderBy
            };

            var bandsFromRepo = collection.GetPage<Band>(paginationRequest);
            var bandsToReturn = _mapper.Map<IEnumerable<BandDto>>(bandsFromRepo.Content).ShapeData(bandsResourceParameters.Fields);

            var bandsPagnigation = new Pagination<ExpandoObject>()
            {
                Content = bandsToReturn.ToList(),
                Page = bandsFromRepo.Page,
                Size = bandsFromRepo.Size,
                TotalPages = bandsFromRepo.TotalPages,
                TotalElements = bandsFromRepo.TotalElements,
                NumberOfElements = bandsFromRepo.NumberOfElements
            };
            

            return new ResponsePagination<ExpandoObject>(bandsPagnigation);
        }

        async Task<Response> IBandAlbumRepository.AddBand(BandForCreatingDto band)
        {
            if (band == null)
                return new ResponseError(System.Net.HttpStatusCode.NotFound, "Band not found!");
            var bandEntity = _mapper.Map<Band>(band);
            _dbcontext.Bands.Add(bandEntity);
            _dbcontext.SaveChanges();
            return new Response(System.Net.HttpStatusCode.OK, "Success!");
        }

        async Task<Response> IBandAlbumRepository.AddBands(IEnumerable<BandForCreatingDto> bandCollection)
        {
            var bandEntities = _mapper.Map<IEnumerable<Band>>(bandCollection);

            foreach (var band in bandCollection)
            {
                _dbcontext.Add(band);
            }
            _dbcontext.SaveChanges();

            var bandCollectionToReturn = _mapper.Map<IEnumerable<BandDto>>(bandEntities);
            var IdsString = string.Join(",", bandCollectionToReturn.Select(b => b.Id));

            return new Response(System.Net.HttpStatusCode.OK, "Success!");
        }

        async Task<Response> IBandAlbumRepository.UpdateBand(Band band)
        {
            return new ResponseUpdate(System.Net.HttpStatusCode.OK, "Success", band.Id);
            //throw new NotImplementedException();
        }

        async Task<Response> IBandAlbumRepository.DeleteBand(Guid bandId)
        {
            var bandFromRepo =  _dbcontext.Bands.FirstOrDefault(b => b.Id == bandId);
            if (bandFromRepo == null)
                return new ResponseError(System.Net.HttpStatusCode.NotFound, "Band not found!");

            _dbcontext.Bands.Remove(bandFromRepo);
            _dbcontext.SaveChanges();
            return new ResponseDelete(System.Net.HttpStatusCode.OK, "Success", bandId, bandFromRepo.Name);
        }

        bool IBandAlbumRepository.AlbumExists(Guid albumId)
        {
            if (albumId == Guid.Empty)
                throw new ArgumentNullException(nameof(albumId));

            return _dbcontext.Albums.Any(a => a.Id == albumId);
        }

        bool IBandAlbumRepository.BandExists(Guid bandId)
        {
            if (bandId == Guid.Empty)
                throw new ArgumentNullException(nameof(bandId));

            return _dbcontext.Bands.Any(a => a.Id == bandId);
        }

        bool IBandAlbumRepository.Save()
        {
            return (_dbcontext.SaveChanges() >= 0);
        }



    }
}
