"use client"

import { useState, useEffect } from "react"
import { Play, Info, ChevronLeft, ChevronRight } from "lucide-react"
import { Button } from "@/components/ui/button"
import type { Movie } from "@/types/movie"

interface HeroSectionProps {
  featuredMovies: Movie[]
}

export default function HeroSection({ featuredMovies }: HeroSectionProps) {
  const [currentSlide, setCurrentSlide] = useState(0)

  useEffect(() => {
    if (featuredMovies.length === 0) return

    const interval = setInterval(() => {
      setCurrentSlide((prev) => (prev + 1) % featuredMovies.length)
    }, 5000)

    return () => clearInterval(interval)
  }, [featuredMovies.length])

  if (featuredMovies.length === 0) {
    return (
      <section className="relative h-[70vh] bg-gradient-to-r from-gray-800 to-gray-900 flex items-center">
        <div className="container mx-auto px-4">
          <div className="max-w-2xl">
            <h1 className="text-5xl font-bold mb-4">Welcome to Movizone.uz</h1>
            <p className="text-xl text-gray-300 mb-8">Discover amazing movies and TV shows</p>
            <div className="flex gap-4">
              <Button size="lg" className="bg-red-600 hover:bg-red-700">
                <Play className="w-5 h-5 mr-2" />
                Explore Movies
              </Button>
            </div>
          </div>
        </div>
      </section>
    )
  }

  const currentMovie = featuredMovies[currentSlide]

  const nextSlide = () => {
    setCurrentSlide((prev) => (prev + 1) % featuredMovies.length)
  }

  const prevSlide = () => {
    setCurrentSlide((prev) => (prev - 1 + featuredMovies.length) % featuredMovies.length)
  }

  return (
    <section className="relative h-[70vh] overflow-hidden">
      <div
        className="absolute inset-0 bg-cover bg-center transition-all duration-1000"
        style={{
          backgroundImage: `url(${currentMovie.backdrop})`,
        }}
      >
        <div className="absolute inset-0 bg-black/50" />
      </div>

      <div className="relative z-10 container mx-auto px-4 h-full flex items-center">
        <div className="max-w-2xl">
          <h1 className="text-5xl font-bold mb-4">{currentMovie.title}</h1>
          <p className="text-xl text-gray-300 mb-6 line-clamp-3">{currentMovie.description}</p>

          <div className="flex items-center gap-4 mb-8">
            <span className="bg-yellow-500 text-black px-2 py-1 rounded font-bold">⭐ {currentMovie.rating}</span>
            <span className="text-gray-300">{currentMovie.releaseYear}</span>
            <span className="text-gray-300">{currentMovie.duration} min</span>
            <span className="bg-red-600 px-2 py-1 rounded text-sm">{currentMovie.quality}</span>
          </div>

          <div className="flex gap-4">
            <Button size="lg" className="bg-red-600 hover:bg-red-700">
              <Play className="w-5 h-5 mr-2" />
              Watch Now
            </Button>
            <Button
              size="lg"
              variant="outline"
              className="border-white text-white hover:bg-white hover:text-black bg-transparent"
            >
              <Info className="w-5 h-5 mr-2" />
              More Info
            </Button>
          </div>
        </div>
      </div>

      {featuredMovies.length > 1 && (
        <>
          <button
            onClick={prevSlide}
            className="absolute left-4 top-1/2 -translate-y-1/2 z-20 bg-black/50 hover:bg-black/70 text-white p-2 rounded-full transition-colors"
          >
            <ChevronLeft className="w-6 h-6" />
          </button>
          <button
            onClick={nextSlide}
            className="absolute right-4 top-1/2 -translate-y-1/2 z-20 bg-black/50 hover:bg-black/70 text-white p-2 rounded-full transition-colors"
          >
            <ChevronRight className="w-6 h-6" />
          </button>

          <div className="absolute bottom-8 left-1/2 -translate-x-1/2 z-20 flex gap-2">
            {featuredMovies.map((_, index) => (
              <button
                key={index}
                onClick={() => setCurrentSlide(index)}
                className={`w-3 h-3 rounded-full transition-colors ${
                  index === currentSlide ? "bg-red-600" : "bg-white/50"
                }`}
              />
            ))}
          </div>
        </>
      )}
    </section>
  )
}
