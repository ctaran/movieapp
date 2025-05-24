using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Core.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Overview { get; set; } = string.Empty;
        public string PosterPath { get; set; } = string.Empty;
        public string BackdropPath { get; set; } = string.Empty;
        public string ReleaseDate { get; set; } = string.Empty;
        public double VoteAverage { get; set; }
        public int VoteCount { get; set; }
        public double Popularity { get; set; }
        public List<int> GenreIds { get; set; } = new();
        public List<string> Genres { get; set; } = new();
        public string OriginalLanguage { get; set; } = string.Empty;
        public string OriginalTitle { get; set; } = string.Empty;
        public bool Adult { get; set; }
        public bool Video { get; set; }
        public List<CastMember> Cast { get; set; } = new();
        public List<Comment>? Comments { get; set; }
    }

    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class Collection
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PosterPath { get; set; } = string.Empty;
        public string BackdropPath { get; set; } = string.Empty;
    }

    public class ProductionCompany
    {
        public int Id { get; set; }
        public string LogoPath { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string OriginCountry { get; set; } = string.Empty;
    }

    public class ProductionCountry
    {
        public string IsoCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class SpokenLanguage
    {
        public string EnglishName { get; set; } = string.Empty;
        public string IsoCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class CastMember
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Character { get; set; } = string.Empty;
        public string? ProfilePath { get; set; }
    }
} 