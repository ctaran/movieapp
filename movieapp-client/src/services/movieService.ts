import { Movie } from '../types/movie';
import { movieApi } from './api';

export const getLatestMovies = async (): Promise<Movie[]> => {
  const response = await movieApi.getLatestMovies();
  return response.data;
};

export const getTopRatedMovies = async (): Promise<Movie[]> => {
  const response = await movieApi.getTopRatedMovies();
  return response.data;
};

export const searchMovies = async (query?: string, genre?: string): Promise<Movie[]> => {
  // Only include params that are set
  const params: any = {};
  if (query) params.query = query;
  if (genre) params.genre = genre;
  const response = await movieApi.searchMovies(params.query, params.genre);
  return response.data;
};

export const getMovieDetails = async (id: number): Promise<Movie> => {
  const response = await movieApi.getMovieDetails(id);
  return response.data;
}; 