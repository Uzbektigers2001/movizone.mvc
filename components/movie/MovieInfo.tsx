"use client"

import { useState } from "react"
import { Star, Heart, Plus, Share2, Clock, Calendar } from "lucide-react"
import { Button } from "@/components/ui/button"
import { useAuth } from "@/contexts/AuthContext"
import { useToast } from "@/hooks/use-toast"
import type { Movie } from "@/types/movie"

interface MovieInfoProps {
  movie: Movie
}

export default function MovieInfo({ movie }: MovieInfoProps) {
  const [isFavorite, setIsFavorite] = useState(false)
  const [isInWatchlist, setIsInWatchlist] = useState(false)
  const { user } = useAuth()
  const { toast } = useToast()

  const handleFavorite = async () => {
    if (!user) {
      toast({
        title: "Tizimga kiring",
        description: "Sevimlilar ro'yxatiga qo'shish uchun tizimga kiring",
        variant: "destructive",
      })
      return
    }

    try {
      // API call to toggle favorite
      setIsFavorite(!isFavorite)
      toast({
        title: isFavorite ? "Sevimlilardan olib tashlandi" : "Sevimlilarga qo'shildi",
        description: movie.title,
      })
    } catch (error) {
      toast({
        title: "Xatolik",
        description: "Amalni bajarishda xatolik yuz berdi",
        variant: "destructive",
      })
    }
  }

  const handleWatchlist = async () => {
    if (!user) {
      toast({
        title: "Tizimga kiring",
        description: "Keyinroq ko'rish ro'yxatiga qo'shish uchun tizimga kiring",
        variant: "destructive",
      })
      return
    }

    try {
      // API call to toggle watchlist
      setIsInWatchlist(!isInWatchlist)
      toast({
        title: isInWatchlist ? "Ro'yxatdan olib tashlandi" : "Ro'yxatga qo'shildi",
        description: movie.title,
      })
    } catch (error) {
      toast({
        title: "Xatolik",
        description: "Amalni bajarishda xatolik yuz berdi",
        variant: "destructive",
      })
    }
  }

  const handleShare = async () => {
    if (navigator.share) {
      try {
        await navigator.share({
          title: movie.title,
          text: movie.description,
          url: window.location.href,
        })
      } catch (error) {
        // User cancelled sharing
      }
    } else {
      // Fallback to clipboard
      navigator.clipboard.writeText(window.location.href)
      toast({
        title: "Havola nusxalandi",
        description: "Film havolasi clipboardga nusxalandi",
      })
    }
  }

  return (
    <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
      {/* Movie Poster */}
      <div className="lg:col-span-1">
        <div className="sticky top-24">
          <img src={movie.posterUrl || "/placeholder.svg"} alt={movie.title} className="w-full rounded-lg shadow-2xl" />

          {/* Action Buttons */}
          <div className="flex flex-col space-y-3 mt-6">
            <Button onClick={handleFavorite} variant={isFavorite ? "default" : "outline"} className="w-full">
              <Heart className={`w-4 h-4 mr-2 ${isFavorite ? "fill-current" : ""}`} />
              {isFavorite ? "Sevimlilardan olib tashlash" : "Sevimlilarga qo'shish"}
            </Button>

            <Button onClick={handleWatchlist} variant={isInWatchlist ? "default" : "outline"} className="w-full">
              <Plus className="w-4 h-4 mr-2" />
              {isInWatchlist ? "Ro'yxatdan olib tashlash" : "Keyinroq ko'rish"}
            </Button>

            <Button onClick={handleShare} variant="outline" className="w-full bg-transparent">
              <Share2 className="w-4 h-4 mr-2" />
              Ulashish
            </Button>
          </div>
        </div>
      </div>

      {/* Movie Details */}
      <div className="lg:col-span-2">
        <div className="space-y-6">
          {/* Title and Rating */}
          <div>
            <h1 className="text-4xl font-bold text-white mb-4">{movie.title}</h1>
            <div className="flex items-center space-x-6 text-gray-300">
              <div className="flex items-center">
                <Star className="w-5 h-5 text-yellow-400 fill-current mr-1" />
                <span className="font-semibold">{movie.rating.toFixed(1)}</span>
                <span className="text-gray-500 ml-1">/10</span>
              </div>
              <div className="flex items-center">
                <Calendar className="w-4 h-4 mr-1" />
                <span>{movie.releaseYear}</span>
              </div>
              <div className="flex items-center">
                <Clock className="w-4 h-4 mr-1" />
                <span>{movie.duration} min</span>
              </div>
              <div className="bg-red-600 text-white px-2 py-1 rounded text-sm font-semibold">{movie.quality}</div>
            </div>
          </div>

          {/* Genres */}
          <div>
            <h3 className="text-lg font-semibold text-white mb-2">Janrlar</h3>
            <div className="flex flex-wrap gap-2">
              {movie.genres.map((genre) => (
                <span key={genre} className="bg-gray-700 text-gray-300 px-3 py-1 rounded-full text-sm">
                  {genre}
                </span>
              ))}
            </div>
          </div>

          {/* Description */}
          <div>
            <h3 className="text-lg font-semibold text-white mb-2">Tavsif</h3>
            <p className="text-gray-300 leading-relaxed">{movie.description}</p>
          </div>

          {/* Cast and Crew */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <h3 className="text-lg font-semibold text-white mb-2">Rejissyor</h3>
              <p className="text-gray-300">{movie.director}</p>
            </div>
            <div>
              <h3 className="text-lg font-semibold text-white mb-2">Mamlakat</h3>
              <p className="text-gray-300">{movie.country}</p>
            </div>
          </div>

          {/* Cast */}
          <div>
            <h3 className="text-lg font-semibold text-white mb-2">Aktyorlar</h3>
            <div className="flex flex-wrap gap-2">
              {movie.cast.map((actor, index) => (
                <span key={index} className="text-gray-300 hover:text-white cursor-pointer transition-colors">
                  {actor}
                  {index < movie.cast.length - 1 && ","}
                </span>
              ))}
            </div>
          </div>

          {/* Additional Info */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6 pt-4 border-t border-gray-700">
            <div>
              <h3 className="text-lg font-semibold text-white mb-2">Til</h3>
              <p className="text-gray-300">{movie.language}</p>
            </div>
            <div>
              <h3 className="text-lg font-semibold text-white mb-2">Chiqarilgan sana</h3>
              <p className="text-gray-300">{new Date(movie.createdAt).toLocaleDateString("uz-UZ")}</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
