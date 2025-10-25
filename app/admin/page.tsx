"use client"

import { useEffect } from "react"
import { useRouter } from "next/navigation"
import { useAuth } from "@/contexts/AuthContext"
import AdminLayout from "@/components/admin/AdminLayout"
import DashboardStats from "@/components/admin/DashboardStats"
import RecentMovies from "@/components/admin/RecentMovies"
import RecentUsers from "@/components/admin/RecentUsers"

export default function AdminDashboard() {
  const { user, isLoading } = useAuth()
  const router = useRouter()

  useEffect(() => {
    if (!isLoading && (!user || user.role !== "admin")) {
      router.push("/auth/login")
    }
  }, [user, isLoading, router])

  if (isLoading) {
    return <div className="loading-spinner mx-auto mt-20" />
  }

  if (!user || user.role !== "admin") {
    return null
  }

  return (
    <AdminLayout>
      <div className="space-y-8">
        <div>
          <h1 className="text-3xl font-bold text-white">Dashboard</h1>
          <p className="text-gray-400 mt-2">Welcome back, {user.name}</p>
        </div>

        <DashboardStats />

        <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
          <RecentMovies />
          <RecentUsers />
        </div>
      </div>
    </AdminLayout>
  )
}
