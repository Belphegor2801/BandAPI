using BandAPI.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BandAPI.Data;
using BandAPI.Helpers;

namespace BandAPI.Services
{
    public class BandAlbumRepository : IBandAlbumRepository
    {
        private readonly BandAlbumDBContext _dbcontext;

        public BandAlbumRepository(BandAlbumDBContext DBContext)
        {
            _dbcontext = DBContext ?? throw new ArgumentNullException(nameof(DBContext));
        }

        void IBandAlbumRepository.AddAlbum(Guid bandId, Album album)
        {
            if (bandId == Guid.Empty)
                throw new ArgumentNullException(nameof(bandId));
            if (album == null)
                throw new ArgumentNullException(nameof(album));

            album.BandId = bandId;
            _dbcontext.Albums.Add(album);
        }

        void IBandAlbumRepository.AddBand(Band band)
        {
            if (band == null)
                throw new ArgumentNullException(nameof(band));

            _dbcontext.Bands.Add(band);
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

        void IBandAlbumRepository.DeleteAlbum(Album album)
        {
            if (album == null)
                throw new ArgumentNullException(nameof(album));

            _dbcontext.Albums.Remove(album);
        }

        void IBandAlbumRepository.DeleteBand(Band band)
        {
            if (band == null)
                throw new ArgumentNullException(nameof(band));

            _dbcontext.Bands.Remove(band);
        }

        Album IBandAlbumRepository.GetAlbum(Guid bandId, Guid albumId)
        {
            if (bandId == Guid.Empty)
                throw new ArgumentNullException(nameof(bandId));
            if (albumId == Guid.Empty)
                throw new ArgumentNullException(nameof(albumId));

            return _dbcontext.Albums.Where(a => a.BandId == bandId && a.Id == albumId).FirstOrDefault();
        }

        IEnumerable<Album> IBandAlbumRepository.GetAlbums(Guid bandId)
        {
            if (bandId == Guid.Empty)
                throw new ArgumentNullException(nameof(bandId));

            return _dbcontext.Albums.Where(a => a.BandId == bandId).OrderBy(a => a.Title).ToList();
        }

        Band IBandAlbumRepository.GetBand(Guid bandId)
        {
            if (bandId == Guid.Empty)
                throw new ArgumentNullException(nameof(bandId));

            return _dbcontext.Bands.FirstOrDefault(b => b.Id == bandId);

        }

        IEnumerable<Band> IBandAlbumRepository.GetBands()
        {
            return _dbcontext.Bands.ToList();
        }

        IEnumerable<Band> IBandAlbumRepository.GetBands(IEnumerable<Guid> bandIds)
        {
            if (bandIds == null)
                throw new ArgumentNullException(nameof(bandIds));

            return _dbcontext.Bands.Where(b => bandIds.Contains(b.Id)).OrderBy(b => b.Name).ToList();
        }

        IEnumerable<Band> IBandAlbumRepository.GetBands(BandsResourceParameters bandsResourceParameters)
        {
            if (bandsResourceParameters == null)
                throw new ArgumentNullException(nameof(bandsResourceParameters));

            var collection = _dbcontext.Bands as IQueryable<Band>;

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

            return collection;
        }

        bool IBandAlbumRepository.Save()
        {
            return (_dbcontext.SaveChanges() >= 0);
        }

        void IBandAlbumRepository.UpdateAlbum(Album album)
        {
            throw new NotImplementedException();
        }

        void IBandAlbumRepository.UpdateBand(Band band)
        {
            throw new NotImplementedException();
        }
    }
}
