import { Suspense } from "react"
import Header from "@/components/layout/Header"
import Footer from "@/components/layout/Footer"
import HeroSection from "@/components/home/HeroSection"
import MovieGrid from "@/components/home/MovieGrid"
import { getMovies, getFeaturedMovies } from "@/lib/api/movies"

export default async function HomePage() {
  try {
    const [moviesResponse, featuredMovies] = await Promise.all([getMovies({ page: 1, limit: 18 }), getFeaturedMovies()])

    return (
      <div className="min-h-screen bg-gray-900 text-white">
        <Header />

        <main>
          <HeroSection featuredMovies={featuredMovies} />

          <section className="py-12">
            <div className="container mx-auto px-4">
              <div className="flex items-center justify-between mb-8">
                <h2 className="text-3xl font-bold">Popular Movies</h2>
                <a href="/catalog" className="text-red-500 hover:text-red-400 transition-colors">
                  View All
                </a>
              </div>

              <Suspense
                fallback={
                  <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-6 gap-4">
                    {Array.from({ length: 18 }).map((_, i) => (
                      <div key={i} className="animate-pulse">
                        <div className="bg-gray-700 aspect-[2/3] rounded-lg mb-2"></div>
                        <div className="bg-gray-700 h-4 rounded mb-1"></div>
                        <div className="bg-gray-700 h-3 rounded w-2/3"></div>
                      </div>
                    ))}
                  </div>
                }
              >
                <MovieGrid movies={moviesResponse.data} />
              </Suspense>
            </div>
          </section>
        </main>

        <Footer />
      </div>
    )
  } catch (error) {
    console.error("Error loading homepage:", error)

    return (
      <div className="min-h-screen bg-gray-900 text-white">
        <Header />

        <main className="container mx-auto px-4 py-12">
          <div className="text-center">
            <h1 className="text-4xl font-bold mb-4">Welcome to Movizone.uz</h1>
            <p className="text-gray-400 mb-8">Your favorite movies and TV shows in one place</p>
            <div className="bg-red-500/10 border border-red-500/20 rounded-lg p-6 max-w-md mx-auto">
              <p className="text-red-400">Unable to load content. Please check your connection and try again.</p>
            </div>
          </div>
        </main>

        <Footer />
      </div>
    )
  }
}
