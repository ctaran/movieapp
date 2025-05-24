import React, { useState, useEffect } from 'react';
import { 
    Box, 
    TextField, 
    FormControl, 
    InputLabel, 
    Select, 
    MenuItem, 
    Typography,
    CircularProgress,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Paper
} from '@mui/material';
import { movieApi } from '../services/api';
import { Movie, Genre } from '../types/movie';

const Search: React.FC = () => {
    const [query, setQuery] = useState('');
    const [selectedGenre, setSelectedGenre] = useState<string>('');
    const [genres, setGenres] = useState<Genre[]>([]);
    const [movies, setMovies] = useState<Movie[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchGenres = async () => {
            try {
                const response = await movieApi.getGenres();
                setGenres(response.data);
            } catch (err) {
                console.error('Error fetching genres:', err);
                setError('Failed to fetch genres. Please try again later.');
            }
        };

        fetchGenres();
    }, []);

    useEffect(() => {
        const searchMovies = async () => {
            if (!query && !selectedGenre) return;

            setLoading(true);
            setError(null);

            try {
                const response = await movieApi.searchMovies(query, selectedGenre);
                setMovies(response.data);
            } catch (err) {
                setError('Failed to search movies. Please try again later.');
                console.error('Error searching movies:', err);
            } finally {
                setLoading(false);
            }
        };

        const debounceTimer = setTimeout(searchMovies, 500);
        return () => clearTimeout(debounceTimer);
    }, [query, selectedGenre]);

    return (
        <Box>
            <Typography variant="h4" gutterBottom>
                Search Movies
            </Typography>
            
            <Box sx={{ display: 'flex', gap: 2, mb: 4 }}>
                <TextField
                    fullWidth
                    label="Search by title"
                    value={query}
                    onChange={(e) => setQuery(e.target.value)}
                    placeholder="Enter movie title..."
                />
                <FormControl fullWidth margin="normal">
                    <InputLabel>Genre</InputLabel>
                    <Select
                        value={selectedGenre}
                        onChange={(e) => setSelectedGenre(e.target.value)}
                        label="Genre"
                    >
                        <MenuItem value="">All Genres</MenuItem>
                        {genres.map((genre) => (
                            <MenuItem key={genre.id} value={genre.name}>
                                {genre.name}
                            </MenuItem>
                        ))}
                    </Select>
                </FormControl>
            </Box>

            {loading && (
                <Box sx={{ display: 'flex', justifyContent: 'center', my: 4 }}>
                    <CircularProgress />
                </Box>
            )}

            {error && (
                <Typography color="error" sx={{ my: 2 }}>
                    {error}
                </Typography>
            )}

            {!loading && !error && movies.length === 0 && (query || selectedGenre) && (
                <Typography sx={{ my: 2 }}>
                    No movies found matching your search criteria.
                </Typography>
            )}

            {!loading && !error && movies.length > 0 && (
                <TableContainer component={Paper}>
                    <Table>
                        <TableHead>
                            <TableRow>
                                <TableCell>Poster</TableCell>
                                <TableCell>Title</TableCell>
                                <TableCell>Rating</TableCell>
                                <TableCell>Release Date</TableCell>
                                <TableCell>Genres</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {movies.map((movie) => (
                                <TableRow key={movie.id}>
                                    <TableCell>
                                        <img
                                            src={`https://image.tmdb.org/t/p/w200${movie.posterPath}`}
                                            alt={movie.title}
                                            style={{ width: '100px', height: 'auto' }}
                                        />
                                    </TableCell>
                                    <TableCell>{movie.title}</TableCell>
                                    <TableCell>{movie.voteAverage.toFixed(1)}</TableCell>
                                    <TableCell>{new Date(movie.releaseDate).toLocaleDateString()}</TableCell>
                                    <TableCell>{movie.genres?.join(', ') || 'N/A'}</TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </TableContainer>
            )}
        </Box>
    );
};

export default Search; 