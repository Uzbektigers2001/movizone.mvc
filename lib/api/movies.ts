import type { Movie } from "@/types/movie"

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000/api"

interface GetMoviesParams {
  page?: number
  limit?: number
  genre?: string
  year?: number
  rating?: number
  search?: string
}

interface GetMoviesResponse {
  data: Movie[]
  total: number
  page: number
  totalPages: number
}

// Mock data for fallback when API is not available
const mockMovies: Movie[] = [
  {
    id: "1",
    title: "The Dark Knight",
    titleUz: "Qorong'u Ritsar",
    titleRu: "Темный рыцарь",
    description: "When the menace known as the Joker wreaks havoc and chaos on the people of Gotham...",
    descriptionUz: "Joker nomi bilan tanilgan tahdid Gotham aholisiga vahima va tartibsizlik olib kelganda...",
    descriptionRu: "Когда угроза, известная как Джокер, сеет хаос среди жителей Готэма...",
    poster: "/main/img/covers/cover1.jpg",
    backdrop: "/main/img/bg/details__bg.jpg",
    trailer: "https://www.youtube.com/watch?v=EXeTwQWrcwY",
    streamUrl: "",
    duration: 152,
    releaseYear: 2008,
    rating: 9.0,
    genres: ["Action", "Crime", "Drama"],
    director: "Christopher Nolan",
    cast: ["Christian Bale", "Heath Ledger", "Aaron Eckhart"],
    country: "USA",
    language: "English",
    quality: "HD",
    views: 1250000,
    likes: 98500,
    dislikes: 1500,
    createdAt: "2024-01-01T00:00:00Z",
    updatedAt: "2024-01-01T00:00:00Z",
  },
  {
    id: "2",
    title: "Inception",
    titleUz: "Boshlanish",
    titleRu: "Начало",
    description: "A thief who steals corporate secrets through dream-sharing technology...",
    descriptionUz: "Tush almashish texnologiyasi orqali korporativ sirlarni o'g'irlaydigan o'g'ri...",
    descriptionRu: "Вор, который крадет корпоративные секреты с помощью технологии совместных снов...",
    poster: "/main/img/covers/cover2.jpg",
    backdrop: "/main/img/bg/slide__bg-1.jpg",
    trailer: "https://www.youtube.com/watch?v=YoHD9XEInc0",
    streamUrl: "",
    duration: 148,
    releaseYear: 2010,
    rating: 8.8,
    genres: ["Action", "Sci-Fi", "Thriller"],
    director: "Christopher Nolan",
    cast: ["Leonardo DiCaprio", "Marion Cotillard", "Tom Hardy"],
    country: "USA",
    language: "English",
    quality: "HD",
    views: 980000,
    likes: 87500,
    dislikes: 2100,
    createdAt: "2024-01-01T00:00:00Z",
    updatedAt: "2024-01-01T00:00:00Z",
  },
  {
    id: "3",
    title: "Interstellar",
    titleUz: "Yulduzlararo",
    titleRu: "Интерстеллар",
    description: "A team of explorers travel through a wormhole in space...",
    descriptionUz: "Tadqiqotchilar jamoasi kosmosda qurt teshigi orqali sayohat qiladi...",
    descriptionRu: "Команда исследователей путешествует через червоточину в космосе...",
    poster: "/main/img/covers/cover3.jpg",
    backdrop: "/main/img/bg/slide__bg-2.jpg",
    trailer: "https://www.youtube.com/watch?v=zSWdZVtXT7E",
    streamUrl: "",
    duration: 169,
    releaseYear: 2014,
    rating: 8.6,
    genres: ["Adventure", "Drama", "Sci-Fi"],
    director: "Christopher Nolan",
    cast: ["Matthew McConaughey", "Anne Hathaway", "Jessica Chastain"],
    country: "USA",
    language: "English",
    quality: "HD",
    views: 750000,
    likes: 65000,
    dislikes: 1800,
    createdAt: "2024-01-01T00:00:00Z",
    updatedAt: "2024-01-01T00:00:00Z",
  },
]

export const getMovies = async (params: GetMoviesParams = {}): Promise<GetMoviesResponse> => {
  try {
    const searchParams = new URLSearchParams()

    Object.entries(params).forEach(([key, value]) => {
      if (value !== undefined) {
        searchParams.append(key, value.toString())
      }
    })

    const response = await fetch(`${API_BASE_URL}/movies?${searchParams}`, {
      next: { revalidate: 300 },
      headers: {
        "Content-Type": "application/json",
      },
    })

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }

    return await response.json()
  } catch (error) {
    console.warn("API not available, using mock data:", error)

    // Return mock data when API is not available
    const { page = 1, limit = 18 } = params
    const startIndex = (page - 1) * limit
    const endIndex = startIndex + limit
    const paginatedMovies = mockMovies.slice(startIndex, endIndex)

    return {
      data: paginatedMovies,
      total: mockMovies.length,
      page,
      totalPages: Math.ceil(mockMovies.length / limit),
    }
  }
}

export const getMovieById = async (id: string): Promise<Movie | null> => {
  try {
    const response = await fetch(`${API_BASE_URL}/movies/${id}`, {
      next: { revalidate: 300 },
      headers: {
        "Content-Type": "application/json",
      },
    })

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }

    return await response.json()
  } catch (error) {
    console.warn("API not available, using mock data:", error)
    return mockMovies.find((movie) => movie.id === id) || null
  }
}

export const getRelatedMovies = async (movieId: string): Promise<Movie[]> => {
  try {
    const response = await fetch(`${API_BASE_URL}/movies/${movieId}/related`, {
      next: { revalidate: 300 },
      headers: {
        "Content-Type": "application/json",
      },
    })

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }

    return await response.json()
  } catch (error) {
    console.warn("API not available, using mock data:", error)
    return mockMovies.filter((movie) => movie.id !== movieId).slice(0, 6)
  }
}

export const getFeaturedMovies = async (): Promise<Movie[]> => {
  try {
    const response = await fetch(`${API_BASE_URL}/movies/featured`, {
      next: { revalidate: 300 },
      headers: {
        "Content-Type": "application/json",
      },
    })

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }

    return await response.json()
  } catch (error) {
    console.warn("API not available, using mock data:", error)
    return mockMovies.slice(0, 3)
  }
}
