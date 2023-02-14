using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesAPI.Models
{
    public class Rating
    {
        [Key]
        [Column(Order = 1)]
        public int user_id { get; set; }

        [Key]
        [Column(Order = 2)]
        public int movie_id { get; set; }

        [Required]
        public float rating { get; set; }

        public virtual User User { get; set; }
        public virtual Movie Movie { get; set; }
    }
}
