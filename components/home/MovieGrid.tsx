"use client"

import { useState } from "react"
import Image from "next/image"
import Link from "next/link"
import { Play, Star, Heart } from "lucide-react"
import type { Movie } from "@/types/movie"

interface MovieGridProps {
  movies: Movie[]
}

export default function MovieGrid({ movies }: MovieGridProps) {
  const [favorites, setFavorites] = useState<Set<string>>(new Set())

  const toggleFavorite = (movieId: string) => {
    setFavorites((prev) => {
      const newFavorites = new Set(prev)
      if (newFavorites.has(movieId)) {
        newFavorites.delete(movieId)
      } else {
        newFavorites.add(movieId)
      }
      return newFavorites
    })
  }

  if (movies.length === 0) {
    return (
      <div className="text-center py-12">
        <p className="text-gray-400 text-lg">No movies available at the moment.</p>
      </div>
    )
  }

  return (
    <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-6 gap-4">
      {movies.map((movie) => (
        <div key={movie.id} className="group relative">
          <Link href={`/movie/${movie.id}`}>
            <div className="relative aspect-[2/3] rounded-lg overflow-hidden bg-gray-800">
              <Image
                src={movie.poster || "/placeholder.svg"}
                alt={movie.title}
                fill
                className="object-cover transition-transform duration-300 group-hover:scale-105"
                sizes="(max-width: 768px) 50vw, (max-width: 1024px) 33vw, 16vw"
              />

              <div className="absolute inset-0 bg-black/0 group-hover:bg-black/50 transition-colors duration-300" />

              <div className="absolute inset-0 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity duration-300">
                <div className="bg-red-600 rounded-full p-3">
                  <Play className="w-6 h-6 text-white" />
                </div>
              </div>

              <div className="absolute top-2 right-2 opacity-0 group-hover:opacity-100 transition-opacity duration-300">
                <button
                  onClick={(e) => {
                    e.preventDefault()
                    toggleFavorite(movie.id)
                  }}
                  className={`p-2 rounded-full transition-colors ${
                    favorites.has(movie.id) ? "bg-red-600 text-white" : "bg-black/50 text-white hover:bg-red-600"
                  }`}
                >
                  <Heart className="w-4 h-4" />
                </button>
              </div>

              <div className="absolute top-2 left-2">
                <span className="bg-yellow-500 text-black px-2 py-1 rounded text-xs font-bold">⭐ {movie.rating}</span>
              </div>

              <div className="absolute bottom-2 left-2">
                <span className="bg-red-600 px-2 py-1 rounded text-xs">{movie.quality}</span>
              </div>
            </div>
          </Link>

          <div className="mt-2">
            <h3 className="font-semibold text-sm line-clamp-1 group-hover:text-red-400 transition-colors">
              {movie.title}
            </h3>
            <div className="flex items-center justify-between mt-1">
              <span className="text-gray-400 text-xs">{movie.releaseYear}</span>
              <div className="flex items-center gap-1">
                <Star className="w-3 h-3 text-yellow-500" />
                <span className="text-xs text-gray-400">{movie.rating}</span>
              </div>
            </div>
            <div className="flex flex-wrap gap-1 mt-1">
              {movie.genres.slice(0, 2).map((genre) => (
                <span key={genre} className="text-xs text-gray-500 bg-gray-800 px-1 py-0.5 rounded">
                  {genre}
                </span>
              ))}
            </div>
          </div>
        </div>
      ))}
    </div>
  )
}
