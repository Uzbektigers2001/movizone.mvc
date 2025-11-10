// Favorites Management using localStorage
(function() {
    'use strict';

    // Get favorites from localStorage
    function getFavorites() {
        const favorites = localStorage.getItem('movizone_favorites');
        return favorites ? JSON.parse(favorites) : [];
    }

    // Save favorites to localStorage
    function saveFavorites(favorites) {
        localStorage.setItem('movizone_favorites', JSON.stringify(favorites));
    }

    // Check if item is favorite
    function isFavorite(id, type) {
        const favorites = getFavorites();
        return favorites.some(fav => fav.id === id && fav.type === type);
    }

    // Add to favorites
    function addToFavorites(id, type, title, coverImage, rating, year, genre) {
        const favorites = getFavorites();
        if (!isFavorite(id, type)) {
            favorites.push({
                id,
                type,
                title,
                coverImage: coverImage || '',
                rating: rating ?? 'N/A', // Use nullish coalescing to preserve 0
                year: year || '',
                genre: genre || '',
                addedAt: new Date().toISOString()
            });
            saveFavorites(favorites);

            return true;
        }
        return false;
    }

    // Remove from favorites
    function removeFromFavorites(id, type) {
        const favorites = getFavorites();
        const filtered = favorites.filter(fav => !(fav.id === id && fav.type === type));
        saveFavorites(filtered);
        return true;
    }

    // Toggle favorite
    function toggleFavorite(id, type, title, coverImage, rating, year, genre) {
        if (isFavorite(id, type)) {
            removeFromFavorites(id, type);
            return false;
        } else {
            addToFavorites(id, type, title, coverImage, rating, year, genre);
            return true;
        }
    }

    // Update button UI
    function updateButtonUI(button, isActive) {
        if (isActive) {
            button.classList.add('item__favorite--active');
            button.querySelector('i').classList.remove('ti-bookmark');
            button.querySelector('i').classList.add('ti-bookmark-filled');
        } else {
            button.classList.remove('item__favorite--active');
            button.querySelector('i').classList.remove('ti-bookmark-filled');
            button.querySelector('i').classList.add('ti-bookmark');
        }
    }

    // Initialize favorites on page load
    function initFavorites() {
        // This function only updates button states for items already in favorites
        document.querySelectorAll('.item__favorite').forEach(button => {
            const id = parseInt(button.dataset.id);
            const type = button.dataset.type;

            // Only update UI state, don't add click handlers
            if (id && !isNaN(id) && isFavorite(id, type)) {
                updateButtonUI(button, true);
            }
        });
    }

    // Simple toast notification
    function showToast(message) {
        // Remove existing toast if any
        const existingToast = document.querySelector('.favorite-toast');
        if (existingToast) {
            existingToast.remove();
        }

        // Create toast
        const toast = document.createElement('div');
        toast.className = 'favorite-toast';
        toast.textContent = message;
        toast.style.cssText = `
            position: fixed;
            bottom: 30px;
            right: 30px;
            background: rgba(255, 107, 0, 0.95);
            color: white;
            padding: 15px 25px;
            border-radius: 8px;
            font-size: 14px;
            font-weight: 500;
            z-index: 10000;
            animation: slideInRight 0.3s ease-out;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
        `;

        document.body.appendChild(toast);

        // Remove after 3 seconds
        setTimeout(() => {
            toast.style.animation = 'slideOutRight 0.3s ease-out';
            setTimeout(() => toast.remove(), 300);
        }, 3000);
    }

    // Add CSS animations
    const style = document.createElement('style');
    style.textContent = `
        @keyframes slideInRight {
            from {
                transform: translateX(400px);
                opacity: 0;
            }
            to {
                transform: translateX(0);
                opacity: 1;
            }
        }
        @keyframes slideOutRight {
            from {
                transform: translateX(0);
                opacity: 1;
            }
            to {
                transform: translateX(400px);
                opacity: 0;
            }
        }
        .item__favorite--active {
            background-color: var(--main-color) !important;
            color: #fff !important;
        }
    `;
    document.head.appendChild(style);

    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initFavorites);
    } else {
        initFavorites();
    }

    // Expose for debugging
    window.MovizoneFavorites = {
        get: getFavorites,
        add: addToFavorites,
        remove: removeFromFavorites,
        toggle: toggleFavorite,
        isFavorite: isFavorite
    };
})();
