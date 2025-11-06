// ===== CHI TIẾT PHÒNG - JAVASCRIPT FUNCTIONALITY =====

document.addEventListener('DOMContentLoaded', function() {
    initializeGallery();
    initializeBookingWidget();
    initializeDateInputs();
    initializeAnimations();
    initializeScrollEffects();
});

// ===== GALLERY FUNCTIONALITY =====
function initializeGallery() {
    const mainImage = document.getElementById('main-image');
    const thumbnails = document.querySelectorAll('.thumbnail');
    const prevBtn = document.getElementById('prev-btn');
    const nextBtn = document.getElementById('next-btn');
    const indicatorsContainer = document.getElementById('gallery-indicators');
    
    let currentIndex = 0;
    const images = Array.from(thumbnails).map(thumb => thumb.dataset.src);
    
    // Create indicators
    createIndicators();
    
    // Thumbnail click events
    thumbnails.forEach((thumbnail, index) => {
        thumbnail.addEventListener('click', () => {
            setActiveImage(index);
        });
    });
    
    // Navigation buttons
    if (prevBtn && nextBtn) {
        prevBtn.addEventListener('click', () => {
            currentIndex = currentIndex > 0 ? currentIndex - 1 : images.length - 1;
            setActiveImage(currentIndex);
        });
        
        nextBtn.addEventListener('click', () => {
            currentIndex = currentIndex < images.length - 1 ? currentIndex + 1 : 0;
            setActiveImage(currentIndex);
        });
    }
    
    // Keyboard navigation
    document.addEventListener('keydown', (e) => {
        if (e.key === 'ArrowLeft') {
            prevBtn?.click();
        } else if (e.key === 'ArrowRight') {
            nextBtn?.click();
        }
    });
    
    // Auto-play functionality (optional)
    let autoPlayInterval;
    
    function startAutoPlay() {
        autoPlayInterval = setInterval(() => {
            nextBtn?.click();
        }, 5000);
    }
    
    function stopAutoPlay() {
        clearInterval(autoPlayInterval);
    }
    
    // Start auto-play and pause on hover
    const galleryMain = document.querySelector('.gallery-main');
    if (galleryMain) {
        galleryMain.addEventListener('mouseenter', stopAutoPlay);
        galleryMain.addEventListener('mouseleave', startAutoPlay);
        startAutoPlay();
    }
    
    function createIndicators() {
        if (!indicatorsContainer) return;
        
        images.forEach((_, index) => {
            const indicator = document.createElement('div');
            indicator.className = `indicator ${index === 0 ? 'active' : ''}`;
            indicator.addEventListener('click', () => setActiveImage(index));
            indicatorsContainer.appendChild(indicator);
        });
    }
    
    function setActiveImage(index) {
        currentIndex = index;
        
        // Update main image with fade effect
        if (mainImage) {
            mainImage.style.opacity = '0';
            setTimeout(() => {
                mainImage.src = images[index];
                mainImage.style.opacity = '1';
            }, 200);
        }
        
        // Update thumbnails
        thumbnails.forEach((thumb, i) => {
            thumb.classList.toggle('active', i === index);
        });
        
        // Update indicators
        const indicators = document.querySelectorAll('.indicator');
        indicators.forEach((indicator, i) => {
            indicator.classList.toggle('active', i === index);
        });
    }
}

// ===== BOOKING WIDGET FUNCTIONALITY =====
function initializeBookingWidget() {
    const guestButtons = document.querySelectorAll('.guest-btn');
    const guestCount = document.querySelector('.guest-count');
    const bookNowBtn = document.querySelector('.btn-book-now');
    
    let guests = 2;
    const maxGuests = 4;
    const minGuests = 1;
    
    // Guest counter functionality
    guestButtons.forEach(btn => {
        btn.addEventListener('click', () => {
            if (btn.classList.contains('plus') && guests < maxGuests) {
                guests++;
            } else if (btn.classList.contains('minus') && guests > minGuests) {
                guests--;
            }
            
            if (guestCount) {
                guestCount.textContent = guests;
                updateBookingSummary();
            }
        });
    });
    
    // Book now button functionality
    if (bookNowBtn) {
        bookNowBtn.addEventListener('click', (e) => {
            e.preventDefault();
            
            // Add loading state
            bookNowBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Đang xử lý...';
            bookNowBtn.disabled = true;
            
            // Simulate booking process
            setTimeout(() => {
                // Reset button
                bookNowBtn.innerHTML = '<i class="fas fa-calendar-check"></i> Đặt Phòng Ngay';
                bookNowBtn.disabled = false;
                
                // Redirect to ThongTin page
                window.location.href = '/ThongTin';
            }, 2000);
        });
    }
    
    function updateBookingSummary() {
        // Update pricing based on guest count (example logic)
        const basePrice = 150;
        const extraGuestFee = guests > 2 ? (guests - 2) * 25 : 0;
        const serviceFee = 15;
        const total = basePrice + extraGuestFee + serviceFee;
        
        // Update summary display (if elements exist)
        const summaryRows = document.querySelectorAll('.summary-row');
        if (summaryRows.length >= 3) {
            summaryRows[0].querySelector('span:last-child').textContent = `$${basePrice + extraGuestFee}`;
            summaryRows[2].querySelector('span:last-child').textContent = `$${total}`;
        }
    }
}

// ===== DATE INPUT FUNCTIONALITY =====
function initializeDateInputs() {
    const checkinInput = document.getElementById('checkin-date');
    const checkoutInput = document.getElementById('checkout-date');
    
    // Set minimum date to today
    const today = new Date().toISOString().split('T')[0];
    if (checkinInput) {
        checkinInput.min = today;
        checkinInput.value = today;
    }
    
    // Set checkout minimum to checkin + 1 day
    if (checkinInput && checkoutInput) {
        checkinInput.addEventListener('change', () => {
            const checkinDate = new Date(checkinInput.value);
            const nextDay = new Date(checkinDate);
            nextDay.setDate(nextDay.getDate() + 1);
            
            checkoutInput.min = nextDay.toISOString().split('T')[0];
            if (!checkoutInput.value || new Date(checkoutInput.value) <= checkinDate) {
                checkoutInput.value = nextDay.toISOString().split('T')[0];
            }
        });
        
        // Trigger initial setup
        checkinInput.dispatchEvent(new Event('change'));
    }
}

// ===== ANIMATIONS AND SCROLL EFFECTS =====
function initializeAnimations() {
    // Add fade-in animation to elements when they come into view
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };
    
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('fade-in-up');
                observer.unobserve(entry.target);
            }
        });
    }, observerOptions);
    
    // Observe elements for animation
    const animatedElements = document.querySelectorAll('.amenity-category, .room-card, .feature-item');
    animatedElements.forEach(el => observer.observe(el));
}

function initializeScrollEffects() {
    // Parallax effect for header
    const header = document.querySelector('.page-header');
    
    window.addEventListener('scroll', () => {
        const scrolled = window.pageYOffset;
        const rate = scrolled * -0.5;
        
        if (header) {
            header.style.transform = `translateY(${rate}px)`;
        }
    });
    
    // Sticky elements enhancement
    const stickyElements = document.querySelectorAll('.booking-widget, .gallery-section');
    
    window.addEventListener('scroll', () => {
        const scrolled = window.pageYOffset;
        
        stickyElements.forEach(element => {
            if (scrolled > 100) {
                element.classList.add('scrolled');
            } else {
                element.classList.remove('scrolled');
            }
        });
    });
}

// ===== UTILITY FUNCTIONS =====
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

function throttle(func, limit) {
    let inThrottle;
    return function() {
        const args = arguments;
        const context = this;
        if (!inThrottle) {
            func.apply(context, args);
            inThrottle = true;
            setTimeout(() => inThrottle = false, limit);
        }
    }
}

// ===== ROOM CARD INTERACTIONS =====
document.addEventListener('DOMContentLoaded', function() {
    const roomCards = document.querySelectorAll('.room-card');
    
    roomCards.forEach(card => {
        card.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-10px) scale(1.02)';
        });
        
        card.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(0) scale(1)';
        });
    });
});

// ===== RESPONSIVE ENHANCEMENTS =====
function handleResponsiveChanges() {
    const isMobile = window.innerWidth <= 768;
    const galleryThumbnails = document.querySelector('.gallery-thumbnails');
    
    if (isMobile && galleryThumbnails) {
        // Make thumbnails swipeable on mobile
        let isDown = false;
        let startX;
        let scrollLeft;
        
        galleryThumbnails.addEventListener('mousedown', (e) => {
            isDown = true;
            startX = e.pageX - galleryThumbnails.offsetLeft;
            scrollLeft = galleryThumbnails.scrollLeft;
        });
        
        galleryThumbnails.addEventListener('mouseleave', () => {
            isDown = false;
        });
        
        galleryThumbnails.addEventListener('mouseup', () => {
            isDown = false;
        });
        
        galleryThumbnails.addEventListener('mousemove', (e) => {
            if (!isDown) return;
            e.preventDefault();
            const x = e.pageX - galleryThumbnails.offsetLeft;
            const walk = (x - startX) * 2;
            galleryThumbnails.scrollLeft = scrollLeft - walk;
        });
    }
}

// Initialize responsive changes
window.addEventListener('resize', debounce(handleResponsiveChanges, 250));
handleResponsiveChanges();

// ===== ERROR HANDLING =====
window.addEventListener('error', function(e) {
    console.error('Chi Tiết Phòng JS Error:', e.error);
});

// ===== PERFORMANCE OPTIMIZATION =====
// Lazy load images in similar rooms section
function initializeLazyLoading() {
    const lazyImages = document.querySelectorAll('img[data-src]');
    
    const imageObserver = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const img = entry.target;
                img.src = img.dataset.src;
                img.classList.remove('lazy');
                observer.unobserve(img);
            }
        });
    });
    
    lazyImages.forEach(img => imageObserver.observe(img));
}

// Initialize when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeLazyLoading);
} else {
    initializeLazyLoading();
}