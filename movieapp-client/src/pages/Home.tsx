import React, { useState, useEffect, useRef } from 'react';
import { Movie } from '../types/movie';
import { getLatestMovies, getTopRatedMovies, searchMovies, getMovieDetails } from '../services/movieService';
import { LoadingSpinner } from '../components/LoadingSpinner';
import { ErrorMessage } from '../components/ErrorMessage';
import MovieDetailsModal from '../components/MovieDetailsModal';
import { Link } from 'react-router-dom';

// Dummy genres for now; replace with API call if needed
const GENRES = [
  'Action', 'Adventure', 'Animation', 'Comedy', 'Crime', 'Documentary',
  'Drama', 'Family', 'Fantasy', 'History', 'Horror', 'Music', 'Mystery',
  'Romance', 'Science Fiction', 'TV Movie', 'Thriller', 'War', 'Western'
];

export const Home: React.FC = () => {
  const [movies, setMovies] = useState<Movie[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [viewMode, setViewMode] = useState<'latest' | 'topRated' | 'search'>('latest');
  const [searchName, setSearchName] = useState('');
  const [searchGenre, setSearchGenre] = useState('');
  const hasFetched = useRef(false);
  const [selectedMovie, setSelectedMovie] = useState<Movie | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [modalLoading, setModalLoading] = useState(false);

  useEffect(() => {
    if (viewMode === 'search') return;
    const fetchMovies = async () => {
      if (hasFetched.current) return;
      hasFetched.current = true;
      setLoading(true);
      setError(null);
      try {
        const movieData = viewMode === 'latest' 
          ? await getLatestMovies()
          : await getTopRatedMovies();
        setMovies(movieData);
      } catch (err) {
        setError('Failed to fetch movies. Please try again later.');
        console.error('Error fetching movies:', err);
      } finally {
        setLoading(false);
      }
    };
    fetchMovies();
  }, [viewMode]);

  const handleViewModeChange = (mode: 'latest' | 'topRated' | 'search') => {
    setViewMode(mode);
    hasFetched.current = false;
    setError(null);
    if (mode !== 'search') setSearchName('');
  };

  const handleSearch = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    try {
      const results = await searchMovies(searchName, searchGenre);
      setMovies(results);
    } catch (err) {
      setError('Failed to search movies. Please try again later.');
    } finally {
      setLoading(false);
    }
  };

  const handlePosterClick = async (movieId: number) => {
    setModalLoading(true);
    try {
      const details = await getMovieDetails(movieId);
      console.log('Fetched movie details:', details);
      setSelectedMovie(details);
      setModalOpen(true);
    } catch (err) {
      setError('Failed to fetch movie details.');
    } finally {
      setModalLoading(false);
    }
  };

  const handleCloseModal = () => {
    setModalOpen(false);
    setSelectedMovie(null);
  };

  let title = 'Latest Movies';
  if (viewMode === 'topRated') title = 'Top Rated Movies';
  if (viewMode === 'search') title = 'Search Results';

  return (
    <div className="container mx-auto px-4 py-8">
      {/* Switch */}
      <div className="flex justify-start items-center mb-6 space-x-4">
        <button
          onClick={() => handleViewModeChange('latest')}
          className={`px-6 py-3 rounded-lg font-medium transition-all duration-200 transform hover:scale-105 ${
            viewMode === 'latest' 
              ? 'bg-blue-600 text-white shadow-lg shadow-blue-600/30' 
              : 'bg-gray-200 text-gray-700 hover:bg-gray-300 hover:shadow-md'
          }`}
        >
          Latest
        </button>
        <button
          onClick={() => handleViewModeChange('topRated')}
          className={`px-6 py-3 rounded-lg font-medium transition-all duration-200 transform hover:scale-105 ${
            viewMode === 'topRated' 
              ? 'bg-blue-600 text-white shadow-lg shadow-blue-600/30' 
              : 'bg-gray-200 text-gray-700 hover:bg-gray-300 hover:shadow-md'
          }`}
        >
          Top Rated
        </button>
        <button
          onClick={() => handleViewModeChange('search')}
          className={`px-6 py-3 rounded-lg font-medium transition-all duration-200 transform hover:scale-105 ${
            viewMode === 'search' 
              ? 'bg-blue-600 text-white shadow-lg shadow-blue-600/30' 
              : 'bg-gray-200 text-gray-700 hover:bg-gray-300 hover:shadow-md'
          }`}
        >
          Search
        </button>
      </div>

      {/* Title */}
      <h1 className="text-3xl font-bold text-white mb-8">{title}</h1>

      {/* Search Form */}
      {viewMode === 'search' && (
        <form onSubmit={handleSearch} className="flex flex-wrap items-end gap-6 mb-8 bg-white p-6 rounded-xl shadow-md">
          <div className="flex-1 min-w-[250px]">
            <label className="block text-sm font-semibold text-gray-700 mb-2">Movie Name</label>
            <input
              type="text"
              value={searchName}
              onChange={e => setSearchName(e.target.value)}
              className="w-full border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all duration-200 bg-white text-gray-900"
              placeholder="Enter movie name..."
            />
          </div>
          <div className="flex-1 min-w-[250px]">
            <label className="block text-sm font-semibold text-gray-700 mb-2">Genre</label>
            <select
              value={searchGenre}
              onChange={e => setSearchGenre(e.target.value)}
              className="w-full border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all duration-200 bg-white text-gray-900"
            >
              <option value="">All Genres</option>
              {GENRES.map(g => (
                <option key={g} value={g}>{g}</option>
              ))}
            </select>
          </div>
          <button
            type="submit"
            className="px-8 py-3 bg-blue-600 text-white rounded-lg font-medium hover:bg-blue-700 transition-all duration-200 transform hover:scale-105 hover:shadow-lg shadow-blue-600/30"
          >
            Search
          </button>
        </form>
      )}

      {loading ? (
        <LoadingSpinner />
      ) : error ? (
        <ErrorMessage message={error} />
      ) : (
        <div className="overflow-x-auto">
          <table className="min-w-full bg-white shadow-md rounded-lg">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">Poster</th>
                <th className="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">Title</th>
                <th className="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">Rating</th>
                <th className="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">Genres</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {movies.map((movie) => (
                <tr key={movie.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 align-top text-center" style={{ width: 120 }}>
                    <Link to={`/movie/${movie.id}`}>
                      <img
                        src={movie.posterPath ? `https://image.tmdb.org/t/p/w200${movie.posterPath}` : '/fallback.jpg'}
                        alt={movie.title || 'No title'}
                        className="h-32 w-20 object-cover rounded mx-auto cursor-pointer hover:opacity-80 transition"
                      />
                    </Link>
                  </td>
                  <td className="px-6 py-4 align-top text-center" style={{ minWidth: 200 }}>
                    <div className="text-sm font-medium text-gray-900">{movie.title}</div>
                  </td>
                  <td className="px-6 py-4 align-top text-center" style={{ minWidth: 100 }}>
                    <div className="flex items-center justify-center gap-2">
                      <svg className="w-5 h-5 text-yellow-400" fill="currentColor" viewBox="0 0 20 20">
                        <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                      </svg>
                      <span className="text-sm text-gray-600">{movie.voteAverage.toFixed(1)}</span>
                    </div>
                  </td>
                  <td className="px-6 py-4 align-top text-center" style={{ minWidth: 200 }}>
                    <div className="text-sm text-gray-500">
                      {movie.genres && movie.genres.length > 0
                        ? movie.genres.join(', ')
                        : 'No genres available'}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {/* Always render the modal at the top level if open */}
      {modalOpen && selectedMovie && (
        <MovieDetailsModal movie={selectedMovie} onClose={handleCloseModal} loading={modalLoading} />
      )}
    </div>
  );
}; 