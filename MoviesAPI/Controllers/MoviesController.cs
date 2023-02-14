using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Models;
using MoviesAPI.Repositories;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMoviesRepository _moviesRepository;

        public MoviesController(IMoviesRepository moviesRepository)
        {
            _moviesRepository = moviesRepository;
        }

        [HttpGet]
        public IActionResult GetMovies(string title = "", int yearOfRelease = 0, string genre = "")
        {
            var movies = _moviesRepository.GetMovies(title, yearOfRelease, genre);

            return Ok(movies);
        }

        [HttpGet("top5")]
        public IActionResult GetTop5MoviesByTotalUserRatings()
        {
            var movies = _moviesRepository.GetTop5MoviesByTotalUserRatings();

            return Ok(movies);
        }

        [HttpGet("top5/{userId}")]
        public IActionResult GetTop5UserRatedMovies(int userId)
        {
            var movies = _moviesRepository.GetTop5UserRatedMovies(userId);

            return Ok(movies);
        }

        [HttpPost("rating")]
        public IActionResult AddOrUpdateMovieRating(int userId, int movieId, float rating)
        {
            try
            {
                var returnCode = _moviesRepository.AddOrUpdateMovieRating(userId, movieId, rating);

                if (returnCode == 200)
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return BadRequest("An error occurred while adding or updating the movie rating");
        }
    }
}
