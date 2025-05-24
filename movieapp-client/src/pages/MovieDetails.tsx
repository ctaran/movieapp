import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
    Box,
    Typography,
    Paper,
    Rating,
    Divider,
    TextField,
    Button,
    List,
    ListItem,
    ListItemText,
    ListItemAvatar,
    Avatar,
    CircularProgress,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableRow,
    Snackbar,
    Alert
} from '@mui/material';
import { movieApi, commentApi } from '../services/api';
import { Movie, Comment } from '../types/movie';
import { useAuth } from '../contexts/AuthContext';

const MovieDetails: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const { isAuthenticated } = useAuth();
    const [movie, setMovie] = useState<Movie | null>(null);
    const [comments, setComments] = useState<Comment[]>([]);
    const [newComment, setNewComment] = useState('');
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [snackbar, setSnackbar] = useState<{ open: boolean; message: string; severity: 'error' | 'success' }>({
        open: false,
        message: '',
        severity: 'error'
    });

    useEffect(() => {
        const fetchMovieDetails = async () => {
            try {
                const [movieResponse, commentsResponse] = await Promise.all([
                    movieApi.getMovieDetails(Number(id)),
                    commentApi.getMovieComments(Number(id))
                ]);
                console.log('Movie data received:', movieResponse.data);
                console.log('Movie genres:', movieResponse.data.genres);
                console.log('Movie genreIds:', movieResponse.data.genreIds);
                setMovie(movieResponse.data);
                setComments(commentsResponse.data);
            } catch (err) {
                setError('Failed to fetch movie details. Please try again later.');
                console.error('Error fetching movie details:', err);
            } finally {
                setLoading(false);
            }
        };

        fetchMovieDetails();
    }, [id]);

    const handleAddComment = async () => {
        if (!newComment.trim()) return;

        if (!isAuthenticated) {
            setSnackbar({
                open: true,
                message: 'Please log in to add a comment',
                severity: 'error'
            });
            return;
        }

        try {
            const response = await commentApi.addComment({
                movieId: Number(id),
                content: newComment.trim()
            });
            setComments([...comments, response.data]);
            setNewComment('');
            setSnackbar({
                open: true,
                message: 'Comment added successfully',
                severity: 'success'
            });
        } catch (err: any) {
            console.error('Error adding comment:', err);
            if (err.response?.status === 401) {
                setSnackbar({
                    open: true,
                    message: 'Please log in to add a comment',
                    severity: 'error'
                });
            } else {
                setSnackbar({
                    open: true,
                    message: 'Failed to add comment. Please try again.',
                    severity: 'error'
                });
            }
        }
    };

    if (loading) {
        return (
            <Box sx={{ display: 'flex', justifyContent: 'center', my: 4 }}>
                <CircularProgress />
            </Box>
        );
    }

    if (error || !movie) {
        return (
            <Typography color="error" sx={{ my: 2 }}>
                {error || 'Movie not found'}
            </Typography>
        );
    }

    return (
        <Box>
            {/* Back to Home Button */}
            <button
                onClick={() => navigate('/')}
                className="mb-4 px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition-all duration-200 font-medium shadow-md"
            >
                ← Back to Home
            </button>
            <TableContainer component={Paper}>
                <Table>
                    <TableBody>
                        <TableRow>
                            <TableCell width="40%">
                                <img
                                    src={`https://image.tmdb.org/t/p/w500${movie.posterPath}`}
                                    alt={movie.title}
                                    style={{ width: '100%', height: 'auto' }}
                                />
                            </TableCell>
                            <TableCell>
                                <Typography variant="h4" gutterBottom>
                                    {movie.title}
                                </Typography>
                                <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                                    <Rating
                                        value={movie.voteAverage / 2}
                                        precision={0.5}
                                        readOnly
                                        size="large"
                                    />
                                    <Typography variant="body1" sx={{ ml: 2 }}>
                                        {movie.voteAverage.toFixed(1)} / 10
                                    </Typography>
                                </Box>
                                <Typography variant="body1" paragraph>
                                    {movie.overview}
                                </Typography>
                                <Typography variant="subtitle1" gutterBottom>
                                    Release Date: {new Date(movie.releaseDate).toLocaleDateString()}
                                </Typography>
                                <Typography variant="subtitle1">
                                    Genres: {movie.genres && movie.genres.length > 0 
                                        ? movie.genres.join(', ')
                                        : 'No genres available'}
                                </Typography>
                                {movie.cast && movie.cast.length > 0 && (
                                    <Box sx={{ mt: 2 }}>
                                        <Typography variant="subtitle1" gutterBottom>
                                            Main Cast:
                                        </Typography>
                                        <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 2 }}>
                                            {movie.cast.map((actor) => (
                                                <Box key={actor.id} sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                                                    {actor.profilePath ? (
                                                        <img
                                                            src={`https://image.tmdb.org/t/p/w92${actor.profilePath}`}
                                                            alt={actor.name}
                                                            style={{ width: 46, height: 69, borderRadius: 4 }}
                                                        />
                                                    ) : (
                                                        <Avatar sx={{ width: 46, height: 69 }}>{actor.name[0]}</Avatar>
                                                    )}
                                                    <Box>
                                                        <Typography variant="body2" sx={{ fontWeight: 'bold' }}>
                                                            {actor.name}
                                                        </Typography>
                                                        <Typography variant="caption" color="text.secondary">
                                                            {actor.character}
                                                        </Typography>
                                                    </Box>
                                                </Box>
                                            ))}
                                        </Box>
                                    </Box>
                                )}
                            </TableCell>
                        </TableRow>
                    </TableBody>
                </Table>
            </TableContainer>

            <Divider sx={{ my: 4 }} />

            <Typography variant="h5" gutterBottom>
                Comments
            </Typography>

            {isAuthenticated ? (
                <Box sx={{ mb: 4 }}>
                    <TextField
                        fullWidth
                        multiline
                        rows={3}
                        value={newComment}
                        onChange={(e) => setNewComment(e.target.value)}
                        placeholder="Write a comment..."
                        sx={{ mb: 2 }}
                    />
                    <Button
                        variant="contained"
                        onClick={handleAddComment}
                        disabled={!newComment.trim()}
                    >
                        Post Comment
                    </Button>
                </Box>
            ) : (
                <Typography color="text.secondary" sx={{ mb: 4 }}>
                    Please log in to leave a comment.
                </Typography>
            )}

            <List>
                {comments.map((comment) => (
                    <ListItem key={comment.id} alignItems="flex-start">
                        <ListItemAvatar>
                            <Avatar>{comment.user?.userName?.[0] || 'U'}</Avatar>
                        </ListItemAvatar>
                        <ListItemText
                            primary={comment.user?.userName || 'Anonymous'}
                            secondary={
                                <>
                                    <Typography
                                        component="span"
                                        variant="body2"
                                        color="text.primary"
                                    >
                                        {new Date(comment.createdAt).toLocaleString()}
                                    </Typography>
                                    <Typography variant="body1" sx={{ mt: 1 }}>
                                        {comment.content}
                                    </Typography>
                                </>
                            }
                        />
                    </ListItem>
                ))}
            </List>

            <Snackbar
                open={snackbar.open}
                autoHideDuration={6000}
                onClose={() => setSnackbar(prev => ({ ...prev, open: false }))}
            >
                <Alert 
                    onClose={() => setSnackbar(prev => ({ ...prev, open: false }))} 
                    severity={snackbar.severity}
                >
                    {snackbar.message}
                </Alert>
            </Snackbar>
        </Box>
    );
};

export default MovieDetails; 