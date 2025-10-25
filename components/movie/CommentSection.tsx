"use client"

import type React from "react"

import { useState, useEffect } from "react"
import { Star, Send, ThumbsUp, ThumbsDown } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Textarea } from "@/components/ui/textarea"
import { useAuth } from "@/contexts/AuthContext"
import { useToast } from "@/hooks/use-toast"
import type { Comment } from "@/types/movie"

interface CommentSectionProps {
  movieId: string
}

export default function CommentSection({ movieId }: CommentSectionProps) {
  const [comments, setComments] = useState<Comment[]>([])
  const [newComment, setNewComment] = useState("")
  const [rating, setRating] = useState(0)
  const [isLoading, setIsLoading] = useState(true)
  const [isSubmitting, setIsSubmitting] = useState(false)
  const { user } = useAuth()
  const { toast } = useToast()

  useEffect(() => {
    fetchComments()
  }, [movieId])

  const fetchComments = async () => {
    try {
      const response = await fetch(`/api/movies/${movieId}/comments`)
      const data = await response.json()
      setComments(data)
    } catch (error) {
      console.error("Failed to fetch comments:", error)
    } finally {
      setIsLoading(false)
    }
  }

  const handleSubmitComment = async (e: React.FormEvent) => {
    e.preventDefault()

    if (!user) {
      toast({
        title: "Tizimga kiring",
        description: "Izoh qoldirish uchun tizimga kiring",
        variant: "destructive",
      })
      return
    }

    if (!newComment.trim()) {
      toast({
        title: "Xatolik",
        description: "Izoh matnini kiriting",
        variant: "destructive",
      })
      return
    }

    setIsSubmitting(true)

    try {
      const response = await fetch(`/api/movies/${movieId}/comments`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
        body: JSON.stringify({
          content: newComment,
          rating: rating > 0 ? rating : undefined,
        }),
      })

      if (response.ok) {
        const newCommentData = await response.json()
        setComments([newCommentData, ...comments])
        setNewComment("")
        setRating(0)
        toast({
          title: "Muvaffaqiyat",
          description: "Izohingiz qo'shildi",
        })
      } else {
        throw new Error("Failed to submit comment")
      }
    } catch (error) {
      toast({
        title: "Xatolik",
        description: "Izohni qo'shishda xatolik yuz berdi",
        variant: "destructive",
      })
    } finally {
      setIsSubmitting(false)
    }
  }

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString("uz-UZ", {
      year: "numeric",
      month: "long",
      day: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    })
  }

  if (isLoading) {
    return (
      <div className="flex justify-center py-8">
        <div className="loading-spinner" />
      </div>
    )
  }

  return (
    <div className="space-y-8">
      <h2 className="text-2xl font-bold text-white">Izohlar ({comments.length})</h2>

      {/* Comment Form */}
      {user ? (
        <form onSubmit={handleSubmitComment} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-300 mb-2">Baho bering (ixtiyoriy)</label>
            <div className="flex space-x-1">
              {[1, 2, 3, 4, 5].map((star) => (
                <button
                  key={star}
                  type="button"
                  onClick={() => setRating(star)}
                  className={`text-2xl transition-colors ${star <= rating ? "text-yellow-400" : "text-gray-600"}`}
                >
                  <Star className={`w-6 h-6 ${star <= rating ? "fill-current" : ""}`} />
                </button>
              ))}
            </div>
          </div>

          <div>
            <Textarea
              value={newComment}
              onChange={(e) => setNewComment(e.target.value)}
              placeholder="Izohingizni yozing..."
              className="bg-gray-800 border-gray-700 text-white min-h-[100px]"
              maxLength={1000}
            />
            <div className="text-right text-sm text-gray-400 mt-1">{newComment.length}/1000</div>
          </div>

          <Button type="submit" disabled={isSubmitting || !newComment.trim()} className="bg-red-600 hover:bg-red-700">
            <Send className="w-4 h-4 mr-2" />
            {isSubmitting ? "Yuborilmoqda..." : "Izoh qoldirish"}
          </Button>
        </form>
      ) : (
        <div className="bg-gray-800 rounded-lg p-6 text-center">
          <p className="text-gray-300 mb-4">Izoh qoldirish uchun tizimga kiring</p>
          <Button asChild className="bg-red-600 hover:bg-red-700">
            <a href="/auth/login">Tizimga kirish</a>
          </Button>
        </div>
      )}

      {/* Comments List */}
      <div className="space-y-6">
        {comments.length === 0 ? (
          <div className="text-center py-8">
            <p className="text-gray-400">Hali izohlar yo'q. Birinchi bo'lib izoh qoldiring!</p>
          </div>
        ) : (
          comments.map((comment) => (
            <div key={comment.id} className="bg-gray-800 rounded-lg p-6">
              <div className="flex items-start justify-between mb-4">
                <div className="flex items-center space-x-3">
                  <div className="w-10 h-10 bg-red-600 rounded-full flex items-center justify-center">
                    <span className="text-white font-semibold">{comment.userName.charAt(0).toUpperCase()}</span>
                  </div>
                  <div>
                    <h4 className="text-white font-semibold">{comment.userName}</h4>
                    <p className="text-gray-400 text-sm">{formatDate(comment.createdAt)}</p>
                  </div>
                </div>

                {comment.rating && (
                  <div className="flex items-center space-x-1">
                    <Star className="w-4 h-4 text-yellow-400 fill-current" />
                    <span className="text-yellow-400 font-semibold">{comment.rating}</span>
                  </div>
                )}
              </div>

              <p className="text-gray-300 leading-relaxed mb-4">{comment.content}</p>

              <div className="flex items-center space-x-4">
                <Button variant="ghost" size="sm" className="text-gray-400 hover:text-green-400">
                  <ThumbsUp className="w-4 h-4 mr-1" />
                  Foydali
                </Button>
                <Button variant="ghost" size="sm" className="text-gray-400 hover:text-red-400">
                  <ThumbsDown className="w-4 h-4 mr-1" />
                  Foydasiz
                </Button>
              </div>
            </div>
          ))
        )}
      </div>
    </div>
  )
}
