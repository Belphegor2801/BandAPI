using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BandAPI.Data.Entities
{
    public class Band
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public DateTime Founded { get; set; }

        [Required]
        [MaxLength(50)]
        public string MainGenre { get; set; }

        public ICollection<Album> Albums { get; set; } = new List<Album>();
    }
}
