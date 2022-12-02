using Microsoft.EntityFrameworkCore;

namespace BandAPI.Data
{
    public interface IDatabaseFactory
    {
        DbContext GetDbContext();
    }
}