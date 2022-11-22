using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BandAPI.Models
{
    public class BandForCreatingDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public DateTime Founded { get; set; }

        [Required]
        [MaxLength(50)]
        public string MainGenre { get; set; }
        public ICollection<AlbumForCreatingDto> Albums { get; set; } = new List<AlbumForCreatingDto>();
    }
}
