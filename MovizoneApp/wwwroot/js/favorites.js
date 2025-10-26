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
    function addToFavorites(id, type, title) {
        const favorites = getFavorites();
        if (!isFavorite(id, type)) {
            favorites.push({ id, type, title, addedAt: new Date().toISOString() });
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
    function toggleFavorite(id, type, title) {
        if (isFavorite(id, type)) {
            removeFromFavorites(id, type);
            return false;
        } else {
            addToFavorites(id, type, title);
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
        document.querySelectorAll('.item__favorite').forEach(button => {
            const id = parseInt(button.dataset.id);
            const type = button.dataset.type; // 'movie' or 'series'
            const title = button.dataset.title;

            // Set initial state
            if (isFavorite(id, type)) {
                updateButtonUI(button, true);
            }

            // Add click handler
            button.addEventListener('click', function(e) {
                e.preventDefault();
                e.stopPropagation();

                const isNowFavorite = toggleFavorite(id, type, title);
                updateButtonUI(button, isNowFavorite);

                // Show toast notification (optional)
                const message = isNowFavorite
                    ? `Added "${title}" to favorites`
                    : `Removed "${title}" from favorites`;

                showToast(message);
            });
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
