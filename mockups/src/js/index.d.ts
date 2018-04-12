
interface Theater {
    id: number
    name?: string
    capacity?: number
    address: string
}

interface MovieSession {
    id: number
    ticketPrice: number
    scheduledAt: string
    theaterId: number
    movieId: number
    scheduledById?: number
    movie?: Movie
    theater?: Theater
    scheduledBy: any
}

interface Movie {
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
    sessions: MovieSession[]
}