using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Models
{
    public class User
    {
        [Key]
        public int user_id { get; set; }

        [Required]
        [MaxLength(255)]
        public string firstName { get; set; }

        [Required]
        [MaxLength(255)]
        public string lastName { get; set; }

        public virtual ICollection<Rating> Ratings { get; set; }
    }
}
