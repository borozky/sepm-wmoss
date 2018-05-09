
interface Theater {
    id: number
    name?: string
    capacity?: number
    address: string
    movieSessions?: MovieSession[]
}

interface MovieSession {
    id: number
    ticketPrice: number
    scheduledAt: string
    theater?: Theater
    theaterId: number
    movie?: Movie
    movieId: number
    scheduledById?: number
    movie?: Movie
    theater?: Theater
    scheduledBy: any
}

interface Movie {
    $id?: string,
    id: number
    title: string
    releaseYear?: number
    genre?: string
    classification?: string
    rating?: number
    posterFileName?: string
    runtimeMinutes?: number
    description?: string
    movieSessions?: MovieSession[]
}

interface ExpressBookingState {
    theaters: Theater[],
    movies: Movie[],
    sessions: MovieSession[],
    selectedMovieId?: number,
    selectedTheaterId?: number,
    selectedSessionId?: number
}
interface MoviesAndSessions {
    movieSessions?: MovieSession[],
    movies?: Movie[]
}