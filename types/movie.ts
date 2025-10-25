export interface Movie {
  id: string
  title: string
  titleUz: string
  titleRu: string
  description: string
  descriptionUz: string
  descriptionRu: string
  poster: string
  backdrop: string
  trailer: string
  streamUrl: string
  duration: number
  releaseYear: number
  rating: number
  genres: string[]
  director: string
  cast: string[]
  country: string
  language: string
  quality: string
  views: number
  likes: number
  dislikes: number
  createdAt: string
  updatedAt: string
}

export interface Genre {
  id: string
  name: string
  nameUz: string
  nameRu: string
}

export interface Comment {
  id: string
  movieId: string
  userId: string
  userName: string
  userAvatar?: string
  content: string
  rating: number
  createdAt: string
  updatedAt: string
}

export interface Playlist {
  id: string
  userId: string
  name: string
  description?: string
  movies: Movie[]
  isPublic: boolean
  createdAt: string
  updatedAt: string
}
