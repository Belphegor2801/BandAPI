using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;

namespace BandAPI.Data
{
    public class DatabaseFactory: IDatabaseFactory
    {
        private readonly DbContext _dataContext;
        private readonly IConfiguration _configuration;

        public DatabaseFactory()
        {
            _dataContext = new BandAlbumDBContext();
        }

        public DbContext GetDbContext()
        {
            return _dataContext;
        }
    }
}