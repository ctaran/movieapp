export interface Movie {
    id: number;
    title: string;
    overview: string;
    posterPath: string;
    backdropPath: string;
    releaseDate: string;
    voteAverage: number;
    voteCount: number;
    popularity: number;
    genreIds: number[];
    genres?: string[];
    originalLanguage: string;
    originalTitle: string;
    adult: boolean;
    video: boolean;
    comments?: Comment[] | null;
    cast?: CastMember[];
}

export interface Genre {
    id: number;
    name: string;
}

export interface CastMember {
    id: number;
    name: string;
    character: string;
    profilePath?: string;
}

export interface Comment {
    id: number;
    movieId: number;
    userId?: string;
    content: string;
    createdAt: string;
    user?: {
        userName: string;
    };
}

export interface User {
    id: string;
    userName: string;
    email: string;
} 