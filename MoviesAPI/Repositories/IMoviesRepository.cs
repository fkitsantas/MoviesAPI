using MoviesAPI.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MoviesAPI.Repositories
{
    public interface IMoviesRepository
    {
        IQueryable<Movie> GetMovies(string title = "", int yearOfRelease = 0, string genre = "");
        IQueryable<Movie> GetTop5MoviesByTotalUserRatings();
        IQueryable<Movie> GetTop5UserRatedMovies(int userId);
        int AddOrUpdateMovieRating(int userId, int movieId, float rating);
    }
    public class MoviesRepository : IMoviesRepository
    {
        private readonly MoviesAPIDbContext _context;

        public MoviesRepository(MoviesAPIDbContext context)
        {
            _context = context;
        }

        public IQueryable<Movie> GetMovies(string title = "", int yearOfRelease = 0, string genre = "")
        {
            var query = _context.Movies.AsQueryable();

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(m => m.title.Contains(title));
            }

            if (yearOfRelease != 0)
            {
                query = query.Where(m => m.yearOfRelease == yearOfRelease);
            }

            if (!string.IsNullOrEmpty(genre))
            {
                query = query.Where(m => m.genre.Contains(genre));
            }

            return query;
        }

        public IQueryable<Movie> GetTop5MoviesByTotalUserRatings()
        {
            var result = this._context.Movies
                .OrderByDescending(m => m.rating)
                .Take(5)
                .AsNoTracking();

            return result;
        }

        public IQueryable<Movie> GetTop5UserRatedMovies(int userId)
        {
            var result = this._context.Ratings
                .Where(r => r.user_id == userId)
                .OrderByDescending(r => r.rating)
                .Take(5)
                .Select(r => r.Movie)
                .AsNoTracking();

            return result;
        }

        public int AddOrUpdateMovieRating(int userId, int movieId, float rating)
        {
            var returnCode = new SqlParameter
            {
                ParameterName = "@returnCode",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };

            var userIdParameter = new SqlParameter("@user_id", userId);
            var movieIdParameter = new SqlParameter("@movie_id", movieId);
            var ratingParameter = new SqlParameter("@rating", rating);

            _context.Database.ExecuteSqlRaw("EXEC @returnCode = proc_add_or_update_movie_rating @user_id, @movie_id, @rating",
                returnCode,
                userIdParameter,
                movieIdParameter,
                ratingParameter);

            switch ((int)returnCode.Value)
            {
                case 404:
                    throw new Exception("User or movie not found");
                case 400:
                    throw new Exception("Invalid rating value");
                case 200:
                    return (int)returnCode.Value;
                default:
                    throw new Exception("An error occurred while adding or updating the movie rating");
            }
        }

    }
}
