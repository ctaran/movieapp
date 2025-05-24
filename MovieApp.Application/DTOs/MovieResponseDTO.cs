namespace MovieApp.Application.DTOs
{
    public class MovieResponseDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Overview { get; set; }
        public required string PosterPath { get; set; }
        public required string BackdropPath { get; set; }
        public required string ReleaseDate { get; set; }
        public double VoteAverage { get; set; }
        public int VoteCount { get; set; }
        public double Popularity { get; set; }
        public required int[] GenreIds { get; set; }
        public required string OriginalLanguage { get; set; }
        public required string OriginalTitle { get; set; }
        public bool Adult { get; set; }
        public bool Video { get; set; }
    }
} 