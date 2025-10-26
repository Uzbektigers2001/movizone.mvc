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
        localStorage.setItem('movizone_watchlist', JSON.stringify(watchlist));
    }

    // Check if item is in watchlist
    function isInWatchlist(id, type) {
        const watchlist = getWatchlist();
        return watchlist.some(item => item.id === id && item.type === type);
    }

    // Add to watchlist
    function addToWatchlist(id, type, title, coverImage, rating, year, genre) {
        const watchlist = getWatchlist();
        if (!isInWatchlist(id, type)) {
            watchlist.push({
                id,
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
                favorites.some(fav => fav.id === item.id && fav.type === item.type)
            );
        } else {
            items = getWatchlist();
        }

        if (items.length === 0) {
            const iconClass = isFavoritesPage ? 'ti-heart' : 'ti-bookmark';
            const title = isFavoritesPage ? 'favorites' : 'watchlist';
            const browseBtns = `
                <div class="text-center mt-4">
                    <a href="/Movie/Catalog" class="btn btn-primary me-2">
                        <i class="ti ti-movie"></i> Browse Movies
                    </a>
                    <a href="/TVSeries/Catalog" class="btn btn-primary">
                        <i class="ti ti-device-tv"></i> Browse TV Series
                    </a>
                </div>
            `;
            container.innerHTML = `
                <div class="col-12">
                    <div class="alert alert-info">
                        <i class="ti ${iconClass}"></i> You haven't added any ${title} yet. Start adding movies and TV series you love!
                    </div>
                    ${browseBtns}
                </div>
            `;
            return;
        }

        container.innerHTML = items.map(item => {
            const detailsUrl = item.type === 'movie'
                ? `/Movie/Details/${item.id}`
                : `/TVSeries/Details/${item.id}`;

            const badgeIcon = isFavoritesPage ? 'ti-heart-filled' : 'ti-bookmark-filled';
            const badgeText = isFavoritesPage ? 'Favorite' : 'Watchlist';
            const removeIcon = isFavoritesPage ? 'ti-heart-minus' : 'ti-bookmark-minus';
            const removeText = isFavoritesPage ? 'Remove from Favorites' : 'Remove from Watchlist';

            const addedDate = new Date(item.addedAt).toLocaleDateString('en-US', {
                month: 'short',
                day: 'numeric',
                year: 'numeric'
            });

            return `
                <div class="col-12 col-sm-6 col-md-4 col-lg-3">
                    <div class="card bg-dark h-100 border-secondary">
                        <div class="position-relative">
                            <img src="${item.coverImage || '/images/placeholder.jpg'}"
                                 class="card-img-top"
                                 alt="${item.title}"
                                 style="height: 350px; object-fit: cover;">
                            <span class="position-absolute top-0 end-0 m-2 badge bg-danger">
                                <i class="ti ${badgeIcon}"></i> ${badgeText}
                            </span>
                        </div>
                        <div class="card-body d-flex flex-column">
                            <h5 class="card-title">${item.title}</h5>
                            <div class="mb-2">
                                <span class="item__rate--green me-2">${item.rating || 'N/A'}</span>
                                <span class="text-muted">${item.year || ''}</span>
                                <span class="badge bg-info ms-2">${item.type === 'movie' ? 'Movie' : 'TV Series'}</span>
                            </div>
                            <p class="text-muted mb-2">
                                <small><i class="ti ti-clock"></i> Added: ${addedDate}</small>
                            </p>
                            <p class="text-muted mb-3">${item.genre || ''}</p>
                            <div class="mt-auto">
                                <a href="${detailsUrl}" class="btn btn-primary btn-sm w-100 mb-2">
                                    <i class="ti ti-eye"></i> View Details
                                </a>
                                <button type="button"
                                        class="btn btn-outline-danger btn-sm w-100 remove-item-btn"
                                        data-id="${item.id}"
                                        data-type="${item.type}"
                                        data-is-favorites="${isFavoritesPage}">
                                    <i class="ti ${removeIcon}"></i> ${removeText}
                                </button>
                            </div>
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
        render: renderWatchlistItems
    };
})();
