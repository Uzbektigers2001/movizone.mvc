import { notFound } from "next/navigation"
import Header from "@/components/layout/Header"
import Footer from "@/components/layout/Footer"
import VideoPlayer from "@/components/movie/VideoPlayer"
import MovieInfo from "@/components/movie/MovieInfo"
import CommentSection from "@/components/movie/CommentSection"
import RelatedMovies from "@/components/movie/RelatedMovies"
import { getMovieById, getRelatedMovies } from "@/lib/api/movies"

interface MoviePageProps {
  params: {
    id: string
  }
}

export default async function MoviePage({ params }: MoviePageProps) {
  const movie = await getMovieById(params.id)

  if (!movie) {
    notFound()
  }

  const relatedMovies = await getRelatedMovies(params.id)

  return (
    <div className="min-h-screen bg-gray-900 text-white">
      <Header />

      <main className="pt-20">
        {/* Video Player Section */}
        <section className="relative">
          <VideoPlayer src={movie.videoUrl} poster={movie.posterUrl} title={movie.title} />
        </section>

        {/* Movie Information */}
        <section className="py-8">
          <div className="container mx-auto px-4">
            <MovieInfo movie={movie} />
          </div>
        </section>

        {/* Comments Section */}
        <section className="py-8 border-t border-gray-800">
          <div className="container mx-auto px-4">
            <CommentSection movieId={params.id} />
          </div>
        </section>

        {/* Related Movies */}
        <section className="py-8 border-t border-gray-800">
          <div className="container mx-auto px-4">
            <h3 className="text-2xl font-bold mb-6">Related Movies</h3>
            <RelatedMovies movies={relatedMovies} />
          </div>
        </section>
      </main>

      <Footer />
    </div>
  )
}
