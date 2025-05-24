import React from 'react';
import { Movie } from '../types/movie';

interface MovieDetailsModalProps {
  movie: Movie;
  onClose: () => void;
  loading?: boolean;
}

const MovieDetailsModal: React.FC<MovieDetailsModalProps> = ({ movie, onClose, loading }) => {
  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-70">
      <div className="bg-white rounded-lg shadow-lg max-w-2xl w-full relative p-6">
        <button
          className="absolute top-2 right-2 text-gray-500 hover:text-gray-700 text-2xl font-bold"
          onClick={onClose}
          aria-label="Close"
        >
          &times;
        </button>
        {loading ? (
          <div className="flex justify-center items-center h-40">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
          </div>
        ) : (
          <div className="flex flex-col md:flex-row gap-6">
            <img
              src={movie.posterPath ? `https://image.tmdb.org/t/p/w300${movie.posterPath}` : '/fallback.jpg'}
              alt={movie.title || 'No title'}
              className="rounded w-40 h-auto mx-auto md:mx-0"
            />
            <div className="flex-1">
              <h2 className="text-2xl font-bold mb-2">{movie.title || 'No title'}</h2>
              {movie.backdropPath && (
                <img
                  src={`https://image.tmdb.org/t/p/w500${movie.backdropPath}`}
                  alt="Backdrop"
                  className="rounded mb-2 w-full max-h-40 object-cover"
                />
              )}
              <p className="text-gray-700 mb-4">{movie.overview || 'No description available.'}</p>
              <div className="text-sm text-gray-500 mb-2">
                <strong>Release Date:</strong> {movie.releaseDate || 'N/A'}
              </div>
              <div className="text-sm text-gray-500 mb-2">
                <strong>Genres:</strong> {movie.genres && movie.genres.length > 0 ? movie.genres.join(', ') : 'N/A'}
              </div>
              <div className="text-sm text-gray-500 mb-2">
                <strong>Rating:</strong> {movie.voteAverage ? movie.voteAverage.toFixed(1) : 'N/A'}
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default MovieDetailsModal; 