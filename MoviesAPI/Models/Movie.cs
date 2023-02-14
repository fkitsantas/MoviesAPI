using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Models
{
    public class Movie
    {
        [Key]
        public int movie_id { get; set; }

        [Required]
        [MaxLength(255)]
        public string title { get; set; }

        [Required]
        public int yearOfRelease { get; set; }

        [Required]
        [MaxLength(255)]
        public string genre { get; set; }

        public float rating { get; set; }

        public virtual ICollection<Rating> Ratings { get; set; }
    }
}
