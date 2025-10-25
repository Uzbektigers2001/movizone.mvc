"use client"

import { useState } from "react"
import Link from "next/link"
import { Search, Menu, X, User, Heart, List, Settings, LogOut } from "lucide-react"

export default function Header() {
  const [isMenuOpen, setIsMenuOpen] = useState(false)
  const [isUserMenuOpen, setIsUserMenuOpen] = useState(false)
  const [searchQuery, setSearchQuery] = useState("")

  const toggleMenu = () => setIsMenuOpen(!isMenuOpen)
  const toggleUserMenu = () => setIsUserMenuOpen(!isUserMenuOpen)

  return (
    <header className="bg-gray-900/95 backdrop-blur-sm border-b border-gray-800 sticky top-0 z-50">
      <div className="container mx-auto px-4">
        <div className="flex items-center justify-between h-16">
          {/* Logo */}
          <Link href="/" className="flex items-center space-x-2">
            <div className="w-8 h-8 bg-red-600 rounded flex items-center justify-center">
              <span className="text-white font-bold text-lg">M</span>
            </div>
            <span className="text-xl font-bold text-white">Movizone.uz</span>
          </Link>

          {/* Desktop Navigation */}
          <nav className="hidden md:flex items-center space-x-8">
            <Link href="/" className="text-white hover:text-red-400 transition-colors">
              Home
            </Link>
            <Link href="/catalog" className="text-gray-300 hover:text-red-400 transition-colors">
              Movies
            </Link>
            <Link href="/tv-shows" className="text-gray-300 hover:text-red-400 transition-colors">
              TV Shows
            </Link>
            <Link href="/genres" className="text-gray-300 hover:text-red-400 transition-colors">
              Genres
            </Link>
            <Link href="/top-rated" className="text-gray-300 hover:text-red-400 transition-colors">
              Top Rated
            </Link>
          </nav>

          {/* Search Bar */}
          <div className="hidden md:flex items-center flex-1 max-w-md mx-8">
            <div className="relative w-full">
              <input
                type="text"
                placeholder="Search movies, TV shows..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className="w-full bg-gray-800 text-white placeholder-gray-400 rounded-lg pl-10 pr-4 py-2 focus:outline-none focus:ring-2 focus:ring-red-500"
              />
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-4 h-4" />
            </div>
          </div>

          {/* User Menu */}
          <div className="flex items-center space-x-4">
            {/* Mobile Search Button */}
            <button className="md:hidden text-gray-300 hover:text-white">
              <Search className="w-5 h-5" />
            </button>

            {/* User Profile Dropdown */}
            <div className="relative">
              <button
                onClick={toggleUserMenu}
                className="flex items-center space-x-2 text-gray-300 hover:text-white transition-colors"
              >
                <div className="w-8 h-8 bg-gray-700 rounded-full flex items-center justify-center">
                  <User className="w-4 h-4" />
                </div>
                <span className="hidden md:inline">Profile</span>
              </button>

              {isUserMenuOpen && (
                <div className="absolute right-0 mt-2 w-48 bg-gray-800 rounded-lg shadow-lg border border-gray-700 py-2">
                  <Link
                    href="/profile"
                    className="flex items-center px-4 py-2 text-gray-300 hover:text-white hover:bg-gray-700 transition-colors"
                  >
                    <User className="w-4 h-4 mr-3" />
                    My Profile
                  </Link>
                  <Link
                    href="/favorites"
                    className="flex items-center px-4 py-2 text-gray-300 hover:text-white hover:bg-gray-700 transition-colors"
                  >
                    <Heart className="w-4 h-4 mr-3" />
                    Favorites
                  </Link>
                  <Link
                    href="/watchlist"
                    className="flex items-center px-4 py-2 text-gray-300 hover:text-white hover:bg-gray-700 transition-colors"
                  >
                    <List className="w-4 h-4 mr-3" />
                    Watchlist
                  </Link>
                  <Link
                    href="/settings"
                    className="flex items-center px-4 py-2 text-gray-300 hover:text-white hover:bg-gray-700 transition-colors"
                  >
                    <Settings className="w-4 h-4 mr-3" />
                    Settings
                  </Link>
                  <hr className="border-gray-700 my-2" />
                  <button className="flex items-center w-full px-4 py-2 text-gray-300 hover:text-white hover:bg-gray-700 transition-colors">
                    <LogOut className="w-4 h-4 mr-3" />
                    Sign Out
                  </button>
                </div>
              )}
            </div>

            {/* Mobile Menu Button */}
            <button onClick={toggleMenu} className="md:hidden text-gray-300 hover:text-white">
              {isMenuOpen ? <X className="w-6 h-6" /> : <Menu className="w-6 h-6" />}
            </button>
          </div>
        </div>

        {/* Mobile Menu */}
        {isMenuOpen && (
          <div className="md:hidden border-t border-gray-800 py-4">
            <div className="flex flex-col space-y-4">
              <div className="relative">
                <input
                  type="text"
                  placeholder="Search movies, TV shows..."
                  value={searchQuery}
                  onChange={(e) => setSearchQuery(e.target.value)}
                  className="w-full bg-gray-800 text-white placeholder-gray-400 rounded-lg pl-10 pr-4 py-2 focus:outline-none focus:ring-2 focus:ring-red-500"
                />
                <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-4 h-4" />
              </div>

              <nav className="flex flex-col space-y-2">
                <Link href="/" className="text-white hover:text-red-400 transition-colors py-2">
                  Home
                </Link>
                <Link href="/catalog" className="text-gray-300 hover:text-red-400 transition-colors py-2">
                  Movies
                </Link>
                <Link href="/tv-shows" className="text-gray-300 hover:text-red-400 transition-colors py-2">
                  TV Shows
                </Link>
                <Link href="/genres" className="text-gray-300 hover:text-red-400 transition-colors py-2">
                  Genres
                </Link>
                <Link href="/top-rated" className="text-gray-300 hover:text-red-400 transition-colors py-2">
                  Top Rated
                </Link>
              </nav>
            </div>
          </div>
        )}
      </div>
    </header>
  )
}
