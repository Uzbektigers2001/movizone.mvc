// Watchlist Management using localStorage
(function() {
    'use strict';

    // Get watchlist from localStorage
    function getWatchlist() {
        const watchlist = localStorage.getItem('movizone_watchlist');
        return watchlist ? JSON.parse(watchlist) : [];
    }

    // Get favorites from localStorage
    function getFavorites() {
        const favorites = localStorage.getItem('movizone_favorites');
        return favorites ? JSON.parse(favorites) : [];
    }

    // Save watchlist to localStorage
    function saveWatchlist(watchlist) {
        // Filter out invalid entries before saving
        const validWatchlist = watchlist.filter(item => item.id && !isNaN(item.id));
        localStorage.setItem('movizone_watchlist', JSON.stringify(validWatchlist));
    }

    // Clean watchlist - remove invalid entries
    function cleanWatchlist() {
        const watchlist = getWatchlist();
        const validWatchlist = watchlist.filter(item => item.id && !isNaN(item.id));
        if (validWatchlist.length !== watchlist.length) {
            saveWatchlist(validWatchlist);
            console.log(`Cleaned watchlist: removed ${watchlist.length - validWatchlist.length} invalid items`);
        }
        return validWatchlist;
    }

    // Check if item is in watchlist
    function isInWatchlist(id, type) {
        const watchlist = getWatchlist();
        return watchlist.some(item => item.id === id && item.type === type);
    }

    // Add to watchlist
    function addToWatchlist(id, type, title, coverImage, rating, year, genre) {
        // Validate ID
        if (!id || id === 'null' || id === 'undefined' || isNaN(id)) {
            console.warn('Invalid ID for watchlist:', id);
            return false;
        }

        const watchlist = getWatchlist();
        if (!isInWatchlist(id, type)) {
            watchlist.push({
                id: parseInt(id), // Ensure ID is a number
                type,
                title,
                coverImage,
                rating,
                year,
                genre,
                addedAt: new Date().toISOString()
            });
            saveWatchlist(watchlist);
            return true;
        }
        return false;
    }

    // Remove from watchlist
    function removeFromWatchlist(id, type) {
        const watchlist = getWatchlist();
        const filtered = watchlist.filter(item => !(item.id === id && item.type === type));
        saveWatchlist(filtered);
        return true;
    }

    // Toggle watchlist
    function toggleWatchlist(id, type, title, coverImage, rating, year, genre) {
        if (isInWatchlist(id, type)) {
            removeFromWatchlist(id, type);
            return false;
        } else {
            addToWatchlist(id, type, title, coverImage, rating, year, genre);
            return true;
        }
    }

    // Render watchlist items
    function renderWatchlistItems(containerSelector, isFavoritesPage = false) {
        const container = document.querySelector(containerSelector);
        if (!container) return;

        let items = [];
        if (isFavoritesPage) {
            const favorites = getFavorites();
            const watchlist = getWatchlist();
            // Get full details from watchlist for favorite items
            items = watchlist.filter(item =>
                item.id && !isNaN(item.id) && // Filter out null/invalid IDs
                favorites.some(fav => fav.id === item.id && fav.type === item.type)
            );
        } else {
            items = getWatchlist().filter(item => item.id && !isNaN(item.id)); // Filter out null/invalid IDs
        }

        if (items.length === 0) {
            const iconClass = isFavoritesPage ? 'ti-heart-off' : 'ti-bookmark-off';
            const title = isFavoritesPage ? 'favorites' : 'watchlist';
            container.innerHTML = `
                <div class="col-12">
                    <div class="reviews__empty">
                        <i class="${iconClass}"></i>
                        <p>You haven't added any ${title} yet. Start adding movies and TV series you love!</p>
                        <div class="mt-4">
                            <a href="/Movie/Catalog" class="btn btn-primary me-2">
                                <i class="ti ti-movie"></i> Browse Movies
                            </a>
                            <a href="/TVSeries/Catalog" class="btn btn-primary">
                                <i class="ti ti-device-tv"></i> Browse TV Series
                            </a>
                        </div>
                    </div>
                </div>
            `;
            return;
        }

        container.innerHTML = items.map(item => {
            const detailsUrl = item.type === 'movie'
                ? `/Movie/Details/${item.id}`
                : `/TVSeries/Details/${item.id}`;

            const ratingClass = item.rating >= 8 ? 'item__rate--green' : item.rating >= 6 ? 'item__rate--yellow' : 'item__rate--red';
            const removeIcon = isFavoritesPage ? 'ti-heart-minus' : 'ti-bookmark-minus';
            const removeText = isFavoritesPage ? 'Remove from Favorites' : 'Remove from Watchlist';

            return `
                <div class="col-6 col-sm-4 col-lg-3 col-xl-2">
                    <div class="item">
                        <div class="item__cover">
                            <img src="${item.coverImage || '/images/placeholder.jpg'}" alt="${item.title}">
                            <a href="${detailsUrl}" class="item__play">
                                <i class="ti ti-player-play-filled"></i>
                            </a>
                            <span class="item__rate ${ratingClass}">${item.rating || 'N/A'}</span>
                            <button type="button"
                                    class="item__favorite item__favorite--active remove-item-btn"
                                    data-id="${item.id}"
                                    data-type="${item.type}"
                                    data-is-favorites="${isFavoritesPage}"
                                    title="${removeText}">
                                <i class="ti ti-bookmark-filled"></i>
                            </button>
                        </div>
                        <div class="item__content">
                            <h3 class="item__title">
                                <a href="${detailsUrl}">${item.title}</a>
                            </h3>
                            <span class="item__category">
                                <a href="#">${item.genre || 'N/A'}</a>
                            </span>
                        </div>
                    </div>
                </div>
            `;
        }).join('');

        // Add event listeners to remove buttons
        container.querySelectorAll('.remove-item-btn').forEach(btn => {
            btn.addEventListener('click', function() {
                const id = parseInt(this.dataset.id);
                const type = this.dataset.type;
                const isFavPage = this.dataset.isFavorites === 'true';

                if (isFavPage) {
                    // Remove from favorites
                    window.MovizoneFavorites?.remove(id, type);
                } else {
                    // Remove from watchlist
                    removeFromWatchlist(id, type);
                }

                // Re-render
                renderWatchlistItems(containerSelector, isFavPage);

                const message = isFavPage
                    ? 'Removed from favorites'
                    : 'Removed from watchlist';
                showToast(message);
            });
        });
    }

    // Simple toast notification
    function showToast(message) {
        const existingToast = document.querySelector('.watchlist-toast');
        if (existingToast) {
            existingToast.remove();
        }

        const toast = document.createElement('div');
        toast.className = 'watchlist-toast';
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

        setTimeout(() => {
            toast.style.animation = 'slideOutRight 0.3s ease-out';
            setTimeout(() => toast.remove(), 300);
        }, 3000);
    }

    // Update button UI
    function updateWatchlistButtonUI(button, isInWatchlist) {
        const icon = button.querySelector('i');
        if (isInWatchlist) {
            button.classList.add('item__favorite--active');
            if (icon) {
                icon.classList.remove('ti-bookmark');
                icon.classList.add('ti-bookmark-filled');
            }
            button.setAttribute('title', 'Remove from watchlist');
        } else {
            button.classList.remove('item__favorite--active');
            if (icon) {
                icon.classList.remove('ti-bookmark-filled');
                icon.classList.add('ti-bookmark');
            }
            button.setAttribute('title', 'Add to watchlist');
        }
    }

    // Initialize watchlist buttons on catalog and details pages
    function initWatchlistButtons() {
        document.querySelectorAll('.item__favorite').forEach(button => {
            const id = parseInt(button.dataset.id);
            const type = button.dataset.type || 'movie'; // 'movie' or 'series'
            const title = button.dataset.title || '';
            const coverImage = button.dataset.coverImage || button.dataset.cover || '';
            const rating = button.dataset.rating || 'N/A';
            const year = button.dataset.year || '';
            const genre = button.dataset.genre || '';

            if (!id) return; // Skip if no ID

            // Set initial state
            if (isInWatchlist(id, type)) {
                updateWatchlistButtonUI(button, true);
            }

            // Add click handler
            button.addEventListener('click', function(e) {
                e.preventDefault();
                e.stopPropagation();

                const isNowInWatchlist = toggleWatchlist(id, type, title, coverImage, rating, year, genre);
                updateWatchlistButtonUI(button, isNowInWatchlist);

                // Show toast notification
                const message = isNowInWatchlist
                    ? `Added "${title}" to watchlist`
                    : `Removed "${title}" from watchlist`;

                showToast(message);
            });
        });
    }

    // Initialize watchlist page
    function initWatchlistPage() {
        // Clean up watchlist on initialization
        cleanWatchlist();

        const watchlistContainer = document.querySelector('#watchlist-items-container');
        const favoritesContainer = document.querySelector('#favorites-items-container');

        if (watchlistContainer) {
            renderWatchlistItems('#watchlist-items-container', false);
        }

        if (favoritesContainer) {
            renderWatchlistItems('#favorites-items-container', true);
        }

        // Initialize watchlist buttons on any page
        initWatchlistButtons();
    }

    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initWatchlistPage);
    } else {
        initWatchlistPage();
    }

    // Expose for debugging and external use
    window.MovizoneWatchlist = {
        get: getWatchlist,
        add: addToWatchlist,
        remove: removeFromWatchlist,
        toggle: toggleWatchlist,
        isInWatchlist: isInWatchlist,
        render: renderWatchlistItems,
        clean: cleanWatchlist
    };
})();
