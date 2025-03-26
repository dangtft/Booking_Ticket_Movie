using Booking_Movie_Tickets.Data;
using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.Models.Movies;
using Microsoft.EntityFrameworkCore;
using Booking_Movie_Tickets.DTOs.Movies.Request;
using Booking_Movie_Tickets.DTOs.Movies.Response;
using Booking_Movie_Tickets.DTOs.Others;
using System.Data;

namespace Booking_Movie_Tickets.Services
{
    public class MovieService : IMovieService
    {
        private readonly BookingDbContext _context;
        private readonly ILogger<MovieService> _logger;
        public MovieService(BookingDbContext context, ILogger<MovieService> logger)
        {
            _context = context;
            _logger = logger;
        }

        //Lấy toàn bộ phim
        public async Task<PagedResult<MoviesResponse>> GetMoviesAsync(MovieFilter movieFilter)
        {
            try
            {
                var moviesData = new List<MoviesResponse>();
                var movieDict = new Dictionary<Guid, MoviesResponse>();

                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "proc_GetMovies";
                        command.CommandType = CommandType.StoredProcedure;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var movieId = reader.GetGuid(reader.GetOrdinal("Id"));

                                if (!movieDict.TryGetValue(movieId, out var existingMovie))
                                {
                                    existingMovie = new MoviesResponse
                                    {
                                        MovieId = movieId,
                                        Title = reader["Title"]?.ToString() ?? "No Title",
                                        Nation = reader["Nation"]?.ToString() ?? "Unknown",
                                        Genres = new List<string>(),
                                        ImageMovie = new List<string>(),
                                        Status = reader["Status"]?.ToString() ?? "Unknown",
                                        Rating = reader["Rating"] != DBNull.Value ? (float)reader["Rating"] : 0,
                                    };

                                    movieDict[movieId] = existingMovie;
                                }

                                var genre = reader["GenreName"]?.ToString();
                                if (!string.IsNullOrEmpty(genre) && !existingMovie.Genres.Contains(genre))
                                {
                                    existingMovie.Genres.Add(genre);
                                }

                                var mediaUrl = reader["MediaURL"] as string;
                                var mediaType = reader["MediaType"]?.ToString();
                                if (!string.IsNullOrEmpty(mediaUrl) && mediaType == "Image" && !existingMovie.ImageMovie.Contains(mediaUrl))
                                {
                                    existingMovie.ImageMovie.Add(mediaUrl);
                                }
                            }
                        }
                    }
                }

                var moviesList = movieDict.Values.AsQueryable();

                var totalCount = moviesList.Count();
                var totalPages = (int)Math.Ceiling((double)totalCount / movieFilter.PageSize);

                // Search: Lọc phim theo Title
                if (!string.IsNullOrEmpty(movieFilter.Search))
                {
                    var searchTerm = movieFilter.Search.ToLower();
                    moviesList = moviesList.Where(m => m.Title.ToLower().Contains(searchTerm));
                }

                // Sort: Sắp xếp theo yêu cầu
                moviesList = movieFilter.Sort switch
                {
                    "title_asc" => moviesList.OrderBy(m => m.Title),
                    "title_desc" => moviesList.OrderByDescending(m => m.Title),
                    "nation_asc" => moviesList.OrderBy(m => m.Nation),
                    "nation_desc" => moviesList.OrderByDescending(m => m.Nation),
                    _ => moviesList
                };

                // Phân trang
                var pagedMovies = moviesList.Skip((movieFilter.Page - 1) * movieFilter.PageSize)
                                            .Take(movieFilter.PageSize)
                                            .ToList();

                return new PagedResult<MoviesResponse>
                {
                    Page = movieFilter.Page,
                    PageSize = movieFilter.PageSize,
                    TotalPages = totalPages,
                    TotalCount = totalCount,
                    Data = pagedMovies
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách phim.");
                throw;
            }
        }

        public async Task<MovieDetailResponse> GetMovieById(Guid movieId)
        {
            var movieData = await _context.Movies
                .Where(m => m.Id == movieId && !m.IsDeleted)
                .Select(m => new MovieDetailResponse
                {
                    MovieId = m.Id,
                    Title = m.Title,
                    Description = m.Description,
                    Nation = m.Nation,
                    Duration = m.Duration,
                    Rating = m.Rating,
                    Status = m.Status,
                    ReleaseDate = m.ReleaseDate,
                    AgeRating = m.AgeRating != null
                        ? $"{m.AgeRating.RatingLabel} - {m.AgeRating.Description}"
                        : "Không có xếp hạng",
                    Genres = m.MovieGenres.Select(mg => mg.Genre.GenreName).Distinct().ToList()
                })
                .FirstOrDefaultAsync();

            return movieData;
        }

        #region CRUD Movie
        // Thêm phim
        public async Task<Movie> AddMovieAsync(MovieRequest request)
        {
            try
            {
                var movie = new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = request.Title,
                    Description = request.Description,
                    Nation = request.Nation,
                    Duration = request.Duration,
                    AgeRatingId = request.AgeRatingId,
                    Rating = request.Rating,
                    ReleaseDate = request.ReleaseDate,
                    MovieGenres = request.GenreIds?.Select(gid => new MovieGenre { GenreId = gid }).ToList() ?? new List<MovieGenre>()
                };

                _context.Movies.Add(movie);
                await _context.SaveChangesAsync();

                // Thêm suất chiếu nếu có
                if (request.Showtimes != null && request.Showtimes.Any())
                {
                    var showtimes = request.Showtimes.Select(s => new Showtime
                    {
                        Id = Guid.NewGuid(),
                        MovieId = movie.Id,
                        RoomId = s.RoomId,
                        Date = s.Date,
                        Time = s.Time,
                        Price = s.Price,
                        IsDeleted = false
                    }).ToList();

                    _context.Showtimes.AddRange(showtimes);
                    await _context.SaveChangesAsync();
                }

                // Thêm MovieMedia nếu có
                if (request.MovieMedias != null && request.MovieMedias.Any())
                {
                    var movieMedias = request.MovieMedias.Select(m => new MovieMedia
                    {
                        Id = Guid.NewGuid(),
                        MovieId = movie.Id,
                        Description = m.Description,
                        MediaType = m.MediaType,
                        MediaURL = m.MediaURL,
                        IsDeleted = false
                    }).ToList();

                    _context.MovieMedias.AddRange(movieMedias);
                    await _context.SaveChangesAsync();
                }

                return movie;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Sửa phim
        public async Task<Movie> UpdateMovieAsync(Guid id, MovieRequest request)
        {
            try
            {
                var movie = await _context.Movies
                    .Include(m => m.MovieGenres)
                    .Include(m => m.Showtimes)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (movie == null)
                {
                    throw new Exception("Phim không tồn tại.");
                }

                movie.Title = request.Title;
                movie.Description = request.Description;
                movie.Nation = request.Nation;
                movie.Duration = request.Duration;
                movie.AgeRatingId = request.AgeRatingId;
                movie.Rating = request.Rating;
                movie.ReleaseDate = request.ReleaseDate;

                // Cập nhật thể loại
                var currentGenreIds = movie.MovieGenres.Select(mg => mg.GenreId).ToList();
                var newGenreIds = request.GenreIds?.Except(currentGenreIds).ToList() ?? new List<Guid>();
                var removedGenreIds = currentGenreIds.Except(request.GenreIds ?? new List<Guid>()).ToList();

                if (removedGenreIds.Any())
                {
                    var genresToRemove = movie.MovieGenres.Where(mg => removedGenreIds.Contains(mg.GenreId)).ToList();
                    _context.MovieGenres.RemoveRange(genresToRemove);
                }

                foreach (var genreId in newGenreIds)
                {
                    movie.MovieGenres.Add(new MovieGenre { MovieId = id, GenreId = genreId });
                }

                // Cập nhật suất chiếu
                var existingShowtimes = movie.Showtimes.ToList();
                _context.Showtimes.RemoveRange(existingShowtimes);

                if (request.Showtimes != null && request.Showtimes.Any())
                {
                    var newShowtimes = request.Showtimes.Select(s => new Showtime
                    {
                        Id = Guid.NewGuid(),
                        MovieId = id,
                        RoomId = s.RoomId,
                        Date = s.Date,
                        Time = s.Time,
                        Price = s.Price,
                        IsDeleted = false
                    }).ToList();

                    _context.Showtimes.AddRange(newShowtimes);
                }

                await _context.SaveChangesAsync();
                return movie;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Xóa phim theo Id
        public async Task<bool> DeleteMovieAsync(Guid id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null || movie.IsDeleted)
                return false;

            movie.IsDeleted = true;
            await _context.SaveChangesAsync();

            return true;
        }

        #endregion
    }
}