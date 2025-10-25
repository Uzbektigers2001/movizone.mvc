/*==============================
    Main JS
==============================*/

document.addEventListener('DOMContentLoaded', function() {

    /*==============================
        Mobile menu toggle
    ==============================*/
    const headerBtn = document.querySelector('.header__btn');
    const headerNav = document.querySelector('.header__nav');

    if (headerBtn) {
        headerBtn.addEventListener('click', function() {
            if (headerNav) {
                headerNav.classList.toggle('active');
                this.classList.toggle('active');

                // Animate hamburger icon
                const spans = this.querySelectorAll('span');
                if (this.classList.contains('active')) {
                    spans[0].style.transform = 'rotate(45deg) translateY(10px)';
                    spans[1].style.opacity = '0';
                    spans[2].style.transform = 'rotate(-45deg) translateY(-10px)';
                } else {
                    spans[0].style.transform = '';
                    spans[1].style.opacity = '';
                    spans[2].style.transform = '';
                }
            }
        });
    }

    /*==============================
        Search toggle
    ==============================*/
    const searchBtn = document.querySelector('.header__search-btn');
    const searchForm = document.querySelector('.header__search');
    const searchClose = document.querySelector('.header__search-close');
    const searchInput = document.querySelector('.header__search-input');

    if (searchBtn) {
        searchBtn.addEventListener('click', function() {
            if (searchForm) {
                searchForm.classList.add('active');
                if (searchInput) {
                    setTimeout(() => searchInput.focus(), 100);
                }
            }
        });
    }

    if (searchClose) {
        searchClose.addEventListener('click', function() {
            if (searchForm) {
                searchForm.classList.remove('active');
                if (searchInput) {
                    searchInput.value = '';
                }
            }
        });
    }

    /*==============================
        Back to top button
    ==============================*/
    const backBtn = document.querySelector('.footer__back');

    if (backBtn) {
        backBtn.addEventListener('click', function() {
            window.scrollTo({
                top: 0,
                behavior: 'smooth'
            });
        });
    }

    /*==============================
        Close mobile menu on link click
    ==============================*/
    const navLinks = document.querySelectorAll('.header__nav a');

    navLinks.forEach(link => {
        link.addEventListener('click', function() {
            if (window.innerWidth < 1200) {
                if (headerNav && headerNav.classList.contains('active')) {
                    headerNav.classList.remove('active');
                    if (headerBtn) {
                        headerBtn.classList.remove('active');
                        const spans = headerBtn.querySelectorAll('span');
                        spans[0].style.transform = '';
                        spans[1].style.opacity = '';
                        spans[2].style.transform = '';
                    }
                }
            }
        });
    });

    /*==============================
        Close search on ESC
    ==============================*/
    document.addEventListener('keydown', function(e) {
        if (e.key === 'Escape' && searchForm && searchForm.classList.contains('active')) {
            searchForm.classList.remove('active');
            if (searchInput) {
                searchInput.value = '';
            }
        }
    });

    /*==============================
        Add active class to current page
    ==============================*/
    const currentPath = window.location.pathname;
    const navItems = document.querySelectorAll('.header__nav-link');

    navItems.forEach(item => {
        const href = item.getAttribute('href');
        if (href && currentPath.includes(href) && href !== '#') {
            item.classList.add('active');
        }
    });

    /*==============================
        Smooth scroll for anchor links
    ==============================*/
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            const href = this.getAttribute('href');
            if (href !== '#' && href.length > 1) {
                e.preventDefault();
                const target = document.querySelector(href);
                if (target) {
                    target.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }
            }
        });
    });

    /*==============================
        Dropdown hover effect (desktop only)
    ==============================*/
    if (window.innerWidth >= 1200) {
        const dropdownItems = document.querySelectorAll('.header__nav-item');

        dropdownItems.forEach(item => {
            const link = item.querySelector('.header__nav-link');
            const menu = item.querySelector('.header__dropdown-menu');

            if (link && menu) {
                item.addEventListener('mouseenter', function() {
                    // Close other dropdowns
                    document.querySelectorAll('.header__dropdown-menu').forEach(m => {
                        if (m !== menu) {
                            m.style.display = 'none';
                        }
                    });
                    menu.style.display = 'block';
                });

                item.addEventListener('mouseleave', function() {
                    menu.style.display = 'none';
                });
            }
        });
    }

    /*==============================
        Add fade-in animation to sections
    ==============================*/
    const observer = new IntersectionObserver(entries => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('fade-in');
            }
        });
    }, {
        threshold: 0.1
    });

    document.querySelectorAll('.section').forEach(section => {
        observer.observe(section);
    });

    /*==============================
        Show/hide header on scroll
    ==============================*/
    let lastScroll = 0;
    const header = document.querySelector('.header');

    window.addEventListener('scroll', function() {
        const currentScroll = window.pageYOffset;

        if (currentScroll <= 0) {
            header.classList.remove('scroll-up');
            return;
        }

        if (currentScroll > lastScroll && !header.classList.contains('scroll-down')) {
            // Scrolling down
            header.classList.remove('scroll-up');
            header.classList.add('scroll-down');
        } else if (currentScroll < lastScroll && header.classList.contains('scroll-down')) {
            // Scrolling up
            header.classList.remove('scroll-down');
            header.classList.add('scroll-up');
        }

        lastScroll = currentScroll;
    });
});
