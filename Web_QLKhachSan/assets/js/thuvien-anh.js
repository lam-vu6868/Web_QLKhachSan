// Gallery data - will be loaded from DOM
let galleryData = [];

// Current lightbox index
let currentLightboxIndex = 0;
// Array of indices (into galleryData) that are currently visible (after filtering)
let activeIndices = [];

// Load gallery data from DOM
function loadGalleryDataFromDOM() {
    const galleryItems = document.querySelectorAll('.gallery-item');
    galleryData = [];
    
    galleryItems.forEach(item => {
        const img = item.querySelector('img');
        const categorySpan = item.querySelector('.gallery-category');
        
        if (img) {
            galleryData.push({
                src: img.getAttribute('src'),
                title: img.getAttribute('alt') || 'Hình ảnh khách sạn',
                desc: categorySpan ? categorySpan.textContent.trim() : '',
                category: 'phong' // Default category since we removed filters
            });
        }
    });
    
    activeIndices = galleryData.map((_, i) => i);
    return galleryData;
}

// Filter functionality
document.addEventListener('DOMContentLoaded', function () {
    // Load gallery data from DOM first
    loadGalleryDataFromDOM();
    
    // Re-initialize AOS for this page
    if (typeof AOS !== 'undefined') {
        AOS.refresh();
    }
    
    setupLightbox();
    setupKeyboardNavigation();
    updateActiveIndices();
    updateLightboxCounter();
});

// Setup filters - REMOVED since we don't have filter buttons anymore
// function setupFilters() { ... }

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

// Animate stats on scroll - REMOVED since we don't have stats section anymore
// Stats section removed from HTML

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
