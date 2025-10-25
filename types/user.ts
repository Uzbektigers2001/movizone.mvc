export interface User {
  id: string
  email: string
  username: string
  firstName: string
  lastName: string
  avatar?: string
  role: "user" | "admin"
  subscription?: {
    type: "free" | "premium"
    expiresAt?: string
  }
  preferences: {
    language: "uz" | "ru" | "en"
    quality: "auto" | "480p" | "720p" | "1080p"
    autoplay: boolean
  }
  watchHistory: string[]
  favorites: string[]
  playlists: string[]
  createdAt: string
  updatedAt: string
}

export interface AuthResponse {
  user: User
  token: string
  refreshToken: string
}

export interface LoginRequest {
  email: string
  password: string
}

export interface RegisterRequest {
  email: string
  username: string
  firstName: string
  lastName: string
  password: string
  confirmPassword: string
}
