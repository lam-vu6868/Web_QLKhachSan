// Gallery data
const galleryData = [
    {
        src: 'assets/imgs/gt1.jpg',
        title: 'Phòng Deluxe',
        desc: 'Phòng nghỉ cao cấp với view biển tuyệt đẹp',
        category: 'phong'
    },
    {
        src: 'assets/imgs/gt2.jpg',
        title: 'Suite Room',
        desc: 'Không gian sang trọng và rộng rãi',
        category: 'phong'
    },
    {
        src: 'assets/imgs/gt3.jpg',
        title: 'Spa & Massage',
        desc: 'Thư giãn với dịch vụ spa đẳng cấp 5 sao',
        category: 'dichvu'
    },
    {
        src: 'assets/imgs/gt4.jpg',
        title: 'Nhà Hàng',
        desc: 'Ẩm thực đỉnh cao từ bếp trưởng Michelin',
        category: 'dichvu'
    },
    {
        src: 'assets/imgs/gt5.jpg',
        title: 'Sảnh Chính',
        desc: 'Lối vào sang trọng và ấn tượng',
        category: 'khonggian'
    },
    {
        src: 'assets/imgs/gt6.jpg',
        title: 'Hồ Bơi Vô Cực',
        desc: 'View biển tuyệt đẹp từ hồ bơi rooftop',
        category: 'khonggian'
    },
    {
        src: 'assets/imgs/gt7.jpg',
        title: 'Phòng Hội Nghị',
        desc: 'Không gian tổ chức sự kiện chuyên nghiệp',
        category: 'sukien'
    },
    {
        src: 'assets/imgs/gt.jpg',
        title: 'View Tổng Thể',
        desc: 'Khách sạn nhìn từ trên cao',
        category: 'khonggian'
    }
];

// Current lightbox index
let currentLightboxIndex = 0;
// Array of indices (into galleryData) that are currently visible (after filtering)
let activeIndices = galleryData.map((_, i) => i);

// Filter functionality
document.addEventListener('DOMContentLoaded', function () {
    // Re-initialize AOS for this page
    if (typeof AOS !== 'undefined') {
        AOS.refresh();
    }
    
    setupFilters();
    setupLightbox();
    setupKeyboardNavigation();
    updateActiveIndices();
    updateLightboxCounter();
});

// Setup filters
function setupFilters() {
    const filterBtns = document.querySelectorAll('.filter-btn');
    const galleryItems = document.querySelectorAll('.gallery-item');

    filterBtns.forEach(btn => {
        btn.addEventListener('click', () => {
            // Remove active class from all buttons
            filterBtns.forEach(b => b.classList.remove('active'));

            // Add active class to clicked button
            btn.classList.add('active');

            // Get filter value
            const filterValue = btn.getAttribute('data-filter');

            // Filter items
            galleryItems.forEach(item => {
                const category = item.getAttribute('data-category');

                if (filterValue === 'all' || category === filterValue) {
                    item.classList.remove('hide');
                    // Re-trigger AOS animation
                    item.setAttribute('data-aos', 'zoom-in');
                    AOS.refresh();
                } else {
                    item.classList.add('hide');
                }
            });

            // After changing visible items, update the activeIndices used by the lightbox
            updateActiveIndices();
        });
    });
}

// Rebuild activeIndices array from DOM (visible .gallery-item elements)
function updateActiveIndices() {
    const visibleItems = Array.from(document.querySelectorAll('.gallery-item')).filter(i => !i.classList.contains('hide'));
    activeIndices = visibleItems.map(i => parseInt(i.getAttribute('data-index'), 10));
    // Reset currentLightboxIndex to the first visible item if it's no longer visible
    if (!activeIndices.includes(currentLightboxIndex)) {
        currentLightboxIndex = activeIndices.length ? activeIndices[0] : 0;
    }
}

// Setup lightbox
function setupLightbox() {
    const lightbox = document.getElementById('lightbox');

    // Close lightbox when clicking outside image
    lightbox.addEventListener('click', (e) => {
        if (e.target === lightbox) {
            closeLightbox();
        }
    });
}

// Open lightbox
function openLightbox(index) {
    // index passed is the galleryData index (from data-index). We set currentLightboxIndex to it.
    currentLightboxIndex = index;
    const lightbox = document.getElementById('lightbox');
    const lightboxImg = document.getElementById('lightbox-img');

    const data = galleryData[index];
    lightboxImg.src = data.src;
    lightboxImg.alt = data.title;

    updateLightboxCounter();
    preloadAdjacentImages(index);

    lightbox.classList.add('active');
    document.body.style.overflow = 'hidden';
}

// Close lightbox
function closeLightbox() {
    const lightbox = document.getElementById('lightbox');
    lightbox.classList.remove('active');
    document.body.style.overflow = 'auto';
}

// Change slide
function changeSlide(direction) {
    // When filtered, navigate within activeIndices (which contain galleryData indices)
    if (activeIndices.length === 0) return;

    // Find position of current index in activeIndices
    let pos = activeIndices.indexOf(currentLightboxIndex);
    if (pos === -1) pos = 0;

    pos = (pos + direction + activeIndices.length) % activeIndices.length;
    currentLightboxIndex = activeIndices[pos];

    const lightboxImg = document.getElementById('lightbox-img');
    const data = galleryData[currentLightboxIndex];
    lightboxImg.style.opacity = '0';
    setTimeout(() => {
        lightboxImg.src = data.src;
        lightboxImg.alt = data.title;
        lightboxImg.style.opacity = '1';
        updateLightboxCounter();
    }, 150);
}

// Update lightbox counter
function updateLightboxCounter() {
    const currentSpan = document.getElementById('lightbox-current');
    const totalSpan = document.getElementById('lightbox-total');

    // Determine position within activeIndices for current
    const pos = activeIndices.indexOf(currentLightboxIndex);
    currentSpan.textContent = (pos === -1 ? 0 : pos + 1);
    totalSpan.textContent = activeIndices.length || galleryData.length;
}

// Keyboard navigation
function setupKeyboardNavigation() {
    document.addEventListener('keydown', (e) => {
        const lightbox = document.getElementById('lightbox');

        if (lightbox.classList.contains('active')) {
            if (e.key === 'Escape') {
                closeLightbox();
            } else if (e.key === 'ArrowLeft') {
                changeSlide(-1);
            } else if (e.key === 'ArrowRight') {
                changeSlide(1);
            }
        }
    });
}

// Touch swipe support for mobile
let touchStartX = 0;
let touchEndX = 0;

document.addEventListener('DOMContentLoaded', function () {
    const lightbox = document.getElementById('lightbox');

    lightbox.addEventListener('touchstart', (e) => {
        touchStartX = e.changedTouches[0].screenX;
    }, { passive: true });

    lightbox.addEventListener('touchend', (e) => {
        touchEndX = e.changedTouches[0].screenX;
        handleSwipe();
    }, { passive: true });
});

function handleSwipe() {
    const swipeThreshold = 50;
    const diff = touchStartX - touchEndX;

    if (Math.abs(diff) > swipeThreshold) {
        if (diff > 0) {
            // Swipe left - next image
            changeSlide(1);
        } else {
            // Swipe right - previous image
            changeSlide(-1);
        }
    }
}

// Lazy loading for images
document.addEventListener('DOMContentLoaded', function () {
    const images = document.querySelectorAll('.gallery-card img');

    const imageObserver = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const img = entry.target;
                img.style.opacity = '0';

                // Simulate loading
                setTimeout(() => {
                    img.style.transition = 'opacity 0.5s ease';
                    img.style.opacity = '1';
                }, 100);

                observer.unobserve(img);
            }
        });
    }, {
        threshold: 0.1,
        rootMargin: '50px'
    });

    images.forEach(img => imageObserver.observe(img));
});

// Animate stats on scroll
document.addEventListener('DOMContentLoaded', function () {
    const statNumbers = document.querySelectorAll('.stat-number');
    let animated = false;

    const statsObserver = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting && !animated) {
                animateStats();
                animated = true;
            }
        });
    }, {
        threshold: 0.5
    });

    const statsSection = document.querySelector('.stats-section');
    if (statsSection) {
        statsObserver.observe(statsSection);
    }
});

function animateStats() {
    const statNumbers = document.querySelectorAll('.stat-number');

    statNumbers.forEach(stat => {
        const target = parseInt(stat.textContent);
        const duration = 2000; // 2 seconds
        const step = target / (duration / 16); // 60fps
        let current = 0;

        const timer = setInterval(() => {
            current += step;
            if (current >= target) {
                stat.textContent = target + (stat.textContent.includes('+') ? '+' : '');
                clearInterval(timer);
            } else {
                stat.textContent = Math.floor(current) + (stat.textContent.includes('+') ? '+' : '');
            }
        }, 16);
    });
}

// Smooth scroll to top button (optional)
window.addEventListener('scroll', function () {
    const scrollTop = window.pageYOffset || document.documentElement.scrollTop;

    // You can add a scroll to top button here if needed
    if (scrollTop > 500) {
        // Show scroll to top button
    } else {
        // Hide scroll to top button
    }
});

// Preload next/previous images in lightbox for smoother transition
function preloadAdjacentImages(index) {
    // Preload adjacent images based on activeIndices if filtered
    if (activeIndices.length > 1) {
        const pos = activeIndices.indexOf(index);
        const prevPos = (pos - 1 + activeIndices.length) % activeIndices.length;
        const nextPos = (pos + 1) % activeIndices.length;
        const prevIndex = activeIndices[prevPos];
        const nextIndex = activeIndices[nextPos];

        const prevImg = new Image();
        const nextImg = new Image();

        prevImg.src = galleryData[prevIndex].src;
        nextImg.src = galleryData[nextIndex].src;
    } else if (galleryData.length) {
        const prevIndex = (index - 1 + galleryData.length) % galleryData.length;
        const nextIndex = (index + 1) % galleryData.length;

        const prevImg = new Image();
        const nextImg = new Image();

        prevImg.src = galleryData[prevIndex].src;
        nextImg.src = galleryData[nextIndex].src;
    }
}

// Log gallery info for debugging
console.log('Gallery initialized with ' + galleryData.length + ' images');
console.log('Categories available:', [...new Set(galleryData.map(item => item.category))]);

// Lightweight parallax for hero and CTA backgrounds (desktop only)
(function () {
    const isMobile = window.matchMedia('(max-width: 768px)').matches;
    if (isMobile) return; // disable on mobile for performance

    const hero = document.querySelector('.gallery-hero');
    const cta = document.querySelector('.cta-section');

    let lastScrollY = window.scrollY;
    let ticking = false;

    function update() {
        const y = window.scrollY;
        if (hero) {
            // subtle shift: move background-position by a fraction of scroll
            const offset = Math.round((y - lastScrollY) * 0.15 + (y * 0.06));
            hero.style.backgroundPosition = `center ${50 + offset}px`;
        }
        if (cta) {
            const offset2 = Math.round((y - lastScrollY) * 0.12 + (y * 0.03));
            cta.style.backgroundPosition = `center ${50 + offset2}px`;
        }
        lastScrollY = y;
        ticking = false;
    }

    window.addEventListener('scroll', () => {
        if (!ticking) {
            window.requestAnimationFrame(update);
            ticking = true;
        }
    }, { passive: true });
})();
