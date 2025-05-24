import axios from 'axios';
import { Movie, Comment, Genre } from '../types/movie';

const API_URL = 'http://localhost:5083/api';

const api = axios.create({
    baseURL: API_URL,
    headers: {
        'Content-Type': 'application/json',
    },
    withCredentials: false // Disable credentials to prevent cookie-based auth
});

// Add request interceptor to add auth token
api.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('token');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => {
        console.error('Request interceptor error:', error);
        return Promise.reject(error);
    }
);

// Add response interceptor to handle token expiration
api.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response?.status === 401) {
            // Check if token is expired
            const token = localStorage.getItem('token');
            if (token) {
                try {
                    const payload = JSON.parse(atob(token.split('.')[1]));
                    const expirationTime = payload.exp * 1000; // Convert to milliseconds
                    if (Date.now() >= expirationTime) {
                        console.log('Token expired, redirecting to login');
                        localStorage.removeItem('token');
                        window.location.href = '/login';
                        return Promise.reject(error);
                    }
                } catch (e) {
                    console.error('Error parsing token:', e);
                }
            }
        }
        return Promise.reject(error);
    }
);

export const movieApi = {
    getLatestMovies: () => api.get<Movie[]>('/movies/latest'),
    getTopRatedMovies: () => api.get<Movie[]>('/movies/top-rated'),
    searchMovies: (query?: string, genre?: string) => 
        api.get<Movie[]>('/movies/search', { params: { query, genre } }),
    getMovieDetails: (id: number) => api.get<Movie>(`/movies/${id}`),
    getGenres: () => api.get<Genre[]>('/movies/genres'),
};

export const commentApi = {
    getMovieComments: (movieId: number) => 
        api.get<Comment[]>(`/comments/movie/${movieId}`),
    addComment: (comment: Omit<Comment, 'id' | 'createdAt' | 'user'>) => 
        api.post<Comment>('/comments', comment),
    updateComment: (id: number, content: string) => 
        api.put(`/comments/${id}`, { content }),
    deleteComment: (id: number) => api.delete(`/comments/${id}`),
};

export const authApi = {
    login: (email: string, password: string) => 
        api.post('/auth/login', { email, password }),
    register: (email: string, password: string, userName: string, confirmPassword: string) => 
        api.post('/auth/register', { email, password, userName, confirmPassword }),
};

export default api; 