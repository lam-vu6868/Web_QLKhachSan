// ========== DANH GIA JS - MODERN VERSION ==========

class ReviewSystem {
    constructor() {
        this.reviews = this.generateSampleReviews();
        this.displayedReviews = 0;
        this.reviewsPerLoad = 6;
        this.isExpanded = false;
        this.currentFilter = 'all'; // 'all' or 'mine'
        this.currentUserEmail = '';
        this.currentUserName = '';
        this.ratings = {
            service: 0,
            cleanliness: 0,
            amenities: 0,
            location: 0
        };
        
        this.init();
    }

    init() {
        this.initCountAnimation();
        this.loadInitialReviews();
        this.setupEventListeners();
        this.setupStarRatings();
        this.setupPhotoUpload();
        this.setupFormValidation();
        this.setupFilters();
    }

    // ========== COUNT ANIMATION ==========
    initCountAnimation() {
        const observerOptions = {
            threshold: 0.5,
            rootMargin: '0px'
        };

        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    this.animateNumbers();
                    observer.disconnect();
                }
            });
        }, observerOptions);

        const statsSection = document.querySelector('.hero-stats');
        if (statsSection) {
            observer.observe(statsSection);
        }
    }

    animateNumbers() {
        const counters = document.querySelectorAll('.stat-number');
        
        counters.forEach(counter => {
            const target = parseInt(counter.getAttribute('data-count'));
            const increment = target / 60; // 1 second animation at 60fps
            let current = 0;
            
            const timer = setInterval(() => {
                current += increment;
                if (current >= target) {
                    counter.textContent = target;
                    clearInterval(timer);
                } else {
                    if (target === 4.7) {
                        counter.textContent = current.toFixed(1);
                    } else {
                        counter.textContent = Math.floor(current);
                    }
                }
            }, 16);
        });
    }







    // ========== SAMPLE REVIEWS DATA ==========
    generateSampleReviews() {
        return [
            {
                id: 1,
                name: "Nguyễn Minh Anh",
                avatar: "N",
                rating: 5,
                date: "2 ngày trước",
                service: "Phòng Suite",
                content: "Trải nghiệm tuyệt vời! Dịch vụ chuyên nghiệp, phòng sạch sẽ và view biển tuyệt đẹp. Nhân viên nhiệt tình, chu đáo. Chắc chắn sẽ quay lại lần sau.",
                verified: true
            },
            {
                id: 2,
                name: "Trần Văn Hùng",
                avatar: "T",
                rating: 5,
                date: "1 tuần trước",
                service: "Phòng Deluxe",
                content: "Khách sạn đẳng cấp với không gian sang trọng. Bữa sáng buffet phong phú, hồ bơi đẹp. Đặc biệt ấn tượng với dịch vụ spa thư giãn tuyệt vời.",
                verified: true
            },
            {
                id: 3,
                name: "Lê Thị Mai",
                avatar: "L",
                rating: 4,
                date: "2 tuần trước",
                service: "Dịch vụ Spa",
                content: "Dịch vụ spa rất chuyên nghiệp, không gian yên tĩnh và thư giãn. Các liệu pháp massage giúp tôi cảm thấy thoải mái và tái tạo năng lượng hoàn toàn.",
                verified: true
            },
            {
                id: 4,
                name: "Phạm Đức Thắng",
                avatar: "P",
                rating: 5,
                date: "3 tuần trước",
                service: "Nhà hàng",
                content: "Nhà hàng của khách sạn có món ăn ngon tuyệt, đa dạng từ món Việt đến món Âu. Đầu bếp rất khéo léo, trình bày đẹp mắt. Giá cả hợp lý.",
                verified: true
            },
            {
                id: 5,
                name: "Hoàng Thị Lan",
                avatar: "H",
                rating: 5,
                date: "1 tháng trước",
                service: "Phòng Presidential",
                content: "Phòng Presidential thật sự đẳng cấp! Không gian rộng rãi, nội thất cao cấp, ban công nhìn ra biển tuyệt đẹp. Dịch vụ butler tận tâm 24/7.",
                verified: true
            },
            {
                id: 6,
                name: "Vũ Minh Tuấn",
                avatar: "V",
                rating: 4,
                date: "1 tháng trước",
                service: "Tổ chức sự kiện",
                content: "Tổ chức đám cưới tại đây rất thành công. Đội ngũ event chuyên nghiệp, tư vấn tận tình. Không gian tiệc sang trọng, âm thanh ánh sáng hoàn hảo.",
                verified: true
            },
            {
                id: 7,
                name: "Đỗ Thị Hương",
                avatar: "Đ",
                rating: 5,
                date: "1 tháng trước",
                service: "Phòng Deluxe",
                content: "Kỳ nghỉ tuyệt vời bên gia đình! Trẻ em rất thích hồ bơi và khu vui chơi. Dịch vụ baby-sitting chu đáo giúp vợ chồng tôi có thời gian riêng tư.",
                verified: true
            },
            {
                id: 8,
                name: "Ngô Văn Đức",
                avatar: "N",
                rating: 5,
                date: "2 tháng trước",
                service: "Phòng Suite",
                content: "Chuyến honeymoon không thể hoàn hảo hơn! Phòng được trang trí lãng mạn, dịch vụ couples spa tuyệt vời. Bữa tối candlelight dinner rất ấn tượng.",
                verified: true
            }
        ];
    }

    // ========== LOAD REVIEWS ==========
    loadInitialReviews() {
        this.renderReviews(this.reviewsPerLoad);
    }

    // Render an explicit list (clears existing grid)
    renderReviewList(list) {
        const grid = document.querySelector('.reviews-grid');
        if (!grid) return;
        grid.innerHTML = '';
        this.displayedReviews = 0;

        list.forEach((review, idx) => {
            const card = this.createReviewCard(review);
            grid.appendChild(card);
        });
    }

    renderReviews(count) {
        const grid = document.querySelector('.reviews-grid');
        if (!grid) return;

        const reviewsToShow = this.reviews.slice(this.displayedReviews, this.displayedReviews + count);
        
        reviewsToShow.forEach((review, index) => {
            setTimeout(() => {
                const reviewCard = this.createReviewCard(review);
                grid.appendChild(reviewCard);
            }, index * 100);
        });

        this.displayedReviews += reviewsToShow.length;
        this.updateLoadMoreButton();
    }

    // ========== TOGGLE REVIEWS ==========
    toggleReviews() {
        const grid = document.querySelector('.reviews-grid');
        const loadMoreBtn = document.getElementById('loadMoreReviews');
        const btnText = loadMoreBtn.querySelector('.btn-text');
        const btnIcon = loadMoreBtn.querySelector('.btn-icon');

        // Add loading state
        loadMoreBtn.disabled = true;
        btnText.textContent = 'Đang tải...';

        if (!this.isExpanded) {
            // Show more reviews
            setTimeout(() => {
                this.renderReviews(this.reviewsPerLoad);
                loadMoreBtn.disabled = false;
                
                // If we've shown all reviews, change to collapse mode
                if (this.displayedReviews >= this.reviews.length) {
                    this.isExpanded = true;
                    btnText.textContent = 'Thu gọn';
                    btnIcon.className = 'fas fa-chevron-up btn-icon';
                    loadMoreBtn.classList.add('expanded');
                }
            }, 300);
        } else {
            // Collapse - show only initial reviews
            const reviewCards = grid.querySelectorAll('.review-card');
            const cardsToRemove = Array.from(reviewCards).slice(this.reviewsPerLoad);
            
            cardsToRemove.forEach((card, index) => {
                setTimeout(() => {
                    card.style.transition = 'all 0.5s ease';
                    card.style.opacity = '0';
                    card.style.transform = 'translateY(-20px) scale(0.9)';
                    
                    setTimeout(() => {
                        if (card.parentNode) {
                            card.parentNode.removeChild(card);
                        }
                    }, 500);
                }, index * 30);
            });

            // Reset state
            setTimeout(() => {
                this.displayedReviews = this.reviewsPerLoad;
                this.isExpanded = false;
                btnText.textContent = 'Xem thêm đánh giá';
                btnIcon.className = 'fas fa-chevron-down btn-icon';
                loadMoreBtn.classList.remove('expanded');
                loadMoreBtn.disabled = false;
            }, cardsToRemove.length * 30 + 500);
        }
    }

    updateLoadMoreButton() {
        const loadMoreBtn = document.getElementById('loadMoreReviews');
        const btnText = loadMoreBtn.querySelector('.btn-text');
        const btnIcon = loadMoreBtn.querySelector('.btn-icon');

        if (this.displayedReviews >= this.reviews.length && !this.isExpanded) {
            this.isExpanded = true;
            btnText.textContent = 'Thu gọn';
            btnIcon.className = 'fas fa-chevron-up btn-icon';
            loadMoreBtn.classList.add('expanded');
        }
    }

    createReviewCard(review) {
        const card = document.createElement('div');
        card.className = 'review-card';
        card.style.opacity = '0';
        card.style.transform = 'translateY(30px)';
        // attach metadata for filtering
        if (review.email) card.dataset.email = review.email.toLowerCase();
        if (review.id) card.dataset.id = review.id;

        const starsHtml = Array.from({length: 5}, (_, i) => {
            return `<i class="fas fa-star" style="color: ${i < review.rating ? 'var(--gold)' : '#e2e8f0'}"></i>`;
        }).join('');

        card.innerHTML = `
            <div class="review-header">
                <div class="reviewer-avatar">${review.avatar}</div>
                <div class="reviewer-info">
                    <h4>${review.name} ${review.verified ? '<i class="fas fa-check-circle" style="color: var(--gold); font-size: 0.8rem; margin-left: 5px;"></i>' : ''}</h4>
                    <div class="review-date">${review.date}</div>
                </div>
            </div>
            <div class="review-rating">
                <div class="stars">${starsHtml}</div>
                <span class="rating-score">${review.rating}/5</span>
            </div>
            <div class="review-content">${review.content}</div>
            <div class="review-service">${review.service}</div>
        `;

        // Animate in
        setTimeout(() => {
            card.style.transition = 'all 0.6s ease';
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
        }, 50);

        return card;
    }

    // ========= FILTERS (Tất cả / Của tôi) =========
    setupFilters() {
        const section = document.querySelector('.reviews-section');
        if (section) {
            this.currentUserEmail = (section.dataset.currentUserEmail || '').toLowerCase();
            this.currentUserName = section.dataset.currentUserName || '';
        }

        const btnAll = document.getElementById('filterAll');
        const btnMine = document.getElementById('filterMine');
        const loadMoreBtn = document.getElementById('loadMoreReviews');

        if (btnAll) btnAll.addEventListener('click', () => {
            this.currentFilter = 'all';
            btnAll.classList.add('active');
            if (btnMine) btnMine.classList.remove('active');
            // restore paginated view
            if (loadMoreBtn) loadMoreBtn.style.display = '';
            // show initial chunk
            const grid = document.querySelector('.reviews-grid');
            if (grid) grid.innerHTML = '';
            this.displayedReviews = 0;
            this.isExpanded = false;
            this.renderReviews(this.reviewsPerLoad);
        });

        if (btnMine) btnMine.addEventListener('click', () => {
            this.currentFilter = 'mine';
            btnMine.classList.add('active');
            if (btnAll) btnAll.classList.remove('active');
            // hide load more (show all mine)
            if (loadMoreBtn) loadMoreBtn.style.display = 'none';
            this.showMyReviews();
        });
    }

    showMyReviews() {
        if (!this.currentUserEmail) {
            // No user email known — render empty placeholder
            this.renderReviewList([]);
            this.showEmptyMine();
            return;
        }

        const mine = this.reviews.filter(r => (r.email || '').toLowerCase() === this.currentUserEmail);
        this.renderReviewList(mine);
        if (mine.length === 0) this.showEmptyMine();
    }

    showEmptyMine() {
        const grid = document.querySelector('.reviews-grid');
        if (!grid) return;
        const placeholder = document.createElement('div');
        placeholder.className = 'no-reviews-placeholder';
        placeholder.innerHTML = `
            <div style="padding:30px;text-align:center;color:#475569;">
                <h4>Bạn chưa có đánh giá nào</h4>
                <p>Hãy chia sẻ trải nghiệm của bạn để mọi người cùng biết nhé.</p>
            </div>
        `;
        grid.appendChild(placeholder);
    }

    // ========== EVENT LISTENERS ==========
    setupEventListeners() {
        // Load more reviews
        const loadMoreBtn = document.getElementById('loadMoreReviews');
        if (loadMoreBtn) {
            loadMoreBtn.addEventListener('click', () => {
                this.toggleReviews();
            });
        }

        // Form submission
        const form = document.getElementById('reviewForm');
        if (form) {
            form.addEventListener('submit', (e) => {
                e.preventDefault();
                this.handleFormSubmission();
            });
        }

        // Reset form
        const resetBtn = document.getElementById('resetForm');
        if (resetBtn) {
            resetBtn.addEventListener('click', () => {
                this.resetForm();
            });
        }

        // Character counter
        const textarea = document.getElementById('reviewContent');
        const charCount = document.getElementById('charCount');
        if (textarea && charCount) {
            textarea.addEventListener('input', () => {
                const count = textarea.value.length;
                charCount.textContent = count;
                charCount.style.color = count > 500 ? '#ef4444' : '#64748b';
            });
        }
    }

    // ========== STAR RATINGS ==========
    setupStarRatings() {
        const ratingContainers = document.querySelectorAll('.star-rating');
        
        ratingContainers.forEach(container => {
            const category = container.getAttribute('data-category');
            const stars = container.querySelectorAll('i');
            const ratingText = container.parentNode.querySelector('.rating-text');
            
            stars.forEach((star, index) => {
                star.addEventListener('click', () => {
                    const value = index + 1;
                    this.ratings[category] = value;
                    
                    // Update visual state
                    stars.forEach((s, i) => {
                        s.classList.toggle('active', i <= index);
                    });
                    
                    // Update text
                    const ratingTexts = ['Rất tệ', 'Tệ', 'Bình thường', 'Tốt', 'Xuất sắc'];
                    ratingText.textContent = ratingTexts[index];
                    ratingText.style.color = 'var(--gold)';
                    ratingText.style.fontWeight = '600';
                });
                
                star.addEventListener('mouseenter', () => {
                    stars.forEach((s, i) => {
                        s.style.color = i <= index ? 'var(--gold)' : '#e2e8f0';
                    });
                });
            });
            
            container.addEventListener('mouseleave', () => {
                stars.forEach((star, i) => {
                    star.style.color = i < this.ratings[category] ? 'var(--gold)' : '#e2e8f0';
                });
            });
        });
    }

    // ========== PHOTO UPLOAD ==========
    setupPhotoUpload() {
        const uploadArea = document.getElementById('uploadArea');
        const photoInput = document.getElementById('photoInput');
        const preview = document.getElementById('photoPreview');
        let uploadedFiles = [];

        if (!uploadArea || !photoInput || !preview) return;

        // Click to upload
        uploadArea.addEventListener('click', () => {
            photoInput.click();
        });

        // Drag and drop
        uploadArea.addEventListener('dragover', (e) => {
            e.preventDefault();
            uploadArea.style.borderColor = '#f39c12';
            uploadArea.style.background = 'linear-gradient(135deg, rgba(212, 175, 55, 0.15) 0%, rgba(212, 175, 55, 0.1) 100%)';
        });

        uploadArea.addEventListener('dragleave', (e) => {
            e.preventDefault();
            uploadArea.style.borderColor = 'var(--gold)';
            uploadArea.style.background = 'linear-gradient(135deg, rgba(212, 175, 55, 0.05) 0%, rgba(212, 175, 55, 0.02) 100%)';
        });

        uploadArea.addEventListener('drop', (e) => {
            e.preventDefault();
            uploadArea.style.borderColor = 'var(--gold)';
            uploadArea.style.background = 'linear-gradient(135deg, rgba(212, 175, 55, 0.05) 0%, rgba(212, 175, 55, 0.02) 100%)';
            
            const files = Array.from(e.dataTransfer.files);
            this.handleFileUpload(files, uploadedFiles, preview);
        });

        // File input change
        photoInput.addEventListener('change', (e) => {
            const files = Array.from(e.target.files);
            this.handleFileUpload(files, uploadedFiles, preview);
        });
    }

    handleFileUpload(files, uploadedFiles, preview) {
        files.forEach(file => {
            if (file.type.startsWith('image/') && uploadedFiles.length < 5) {
                if (file.size <= 5 * 1024 * 1024) { // 5MB limit
                    uploadedFiles.push(file);
                    this.createPhotoPreview(file, uploadedFiles, preview);
                } else {
                    this.showToast('Kích thước file quá lớn (tối đa 5MB)', 'error');
                }
            }
        });

        if (uploadedFiles.length >= 5) {
            document.getElementById('uploadArea').style.opacity = '0.5';
            document.getElementById('uploadArea').style.pointerEvents = 'none';
        }
    }

    createPhotoPreview(file, uploadedFiles, preview) {
        const reader = new FileReader();
        reader.onload = (e) => {
            const previewItem = document.createElement('div');
            previewItem.className = 'preview-item';
            previewItem.innerHTML = `
                <img src="${e.target.result}" alt="Preview">
                <button type="button" class="remove-photo">
                    <i class="fas fa-times"></i>
                </button>
            `;

            const removeBtn = previewItem.querySelector('.remove-photo');
            removeBtn.addEventListener('click', () => {
                const index = uploadedFiles.indexOf(file);
                if (index > -1) {
                    uploadedFiles.splice(index, 1);
                    previewItem.remove();
                    
                    if (uploadedFiles.length < 5) {
                        document.getElementById('uploadArea').style.opacity = '1';
                        document.getElementById('uploadArea').style.pointerEvents = 'auto';
                    }
                }
            });

            preview.appendChild(previewItem);
        };
        reader.readAsDataURL(file);
    }

    // ========== FORM VALIDATION ==========
    setupFormValidation() {
        const form = document.getElementById('reviewForm');
        const inputs = form.querySelectorAll('input[required], select[required], textarea[required]');
        
        inputs.forEach(input => {
            input.addEventListener('blur', () => {
                this.validateField(input);
            });
        });
    }

    validateField(field) {
        const isValid = field.checkValidity();
        
        if (isValid) {
            field.style.borderColor = '#10b981';
            this.removeErrorMessage(field);
        } else {
            field.style.borderColor = '#ef4444';
            this.showErrorMessage(field);
        }
        
        return isValid;
    }

    showErrorMessage(field) {
        this.removeErrorMessage(field);
        
        const errorMsg = document.createElement('div');
        errorMsg.className = 'error-message';
        errorMsg.style.color = '#ef4444';
        errorMsg.style.fontSize = '0.9rem';
        errorMsg.style.marginTop = '5px';
        
        if (field.type === 'email') {
            errorMsg.textContent = 'Vui lòng nhập email hợp lệ';
        } else if (field.tagName === 'SELECT') {
            errorMsg.textContent = 'Vui lòng chọn một tùy chọn';
        } else {
            errorMsg.textContent = 'Trường này là bắt buộc';
        }
        
        field.parentNode.appendChild(errorMsg);
    }

    removeErrorMessage(field) {
        const errorMsg = field.parentNode.querySelector('.error-message');
        if (errorMsg) {
            errorMsg.remove();
        }
    }

    // ========== FORM SUBMISSION ==========
    handleFormSubmission() {
        const form = document.getElementById('reviewForm');
        const submitBtn = document.getElementById('submitReview');
        
        // Validate all ratings
        const ratingValid = Object.values(this.ratings).every(rating => rating > 0);
        if (!ratingValid) {
            this.showToast('Vui lòng đánh giá tất cả các tiêu chí', 'error');
            return;
        }

        // Validate form fields
        const requiredFields = form.querySelectorAll('input[required], select[required], textarea[required]');
        let allValid = true;
        
        requiredFields.forEach(field => {
            if (!this.validateField(field)) {
                allValid = false;
            }
        });

        if (!allValid) {
            this.showToast('Vui lòng điền đầy đủ thông tin bắt buộc', 'error');
            return;
        }

        // Show loading state
        submitBtn.disabled = true;
        submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Đang gửi...';

        // Simulate API call
        setTimeout(() => {
            // create a new review object and add to local list (frontend-only)
            const nameInput = document.getElementById('reviewerName');
            const emailInput = document.getElementById('reviewerEmail');
            const serviceSelect = document.getElementById('serviceType');
            const visitDate = document.getElementById('visitDate');
            const content = document.getElementById('reviewContent');

            const newReview = {
                id: Date.now(),
                name: (nameInput && nameInput.value) ? nameInput.value : this.currentUserName || 'Khách',
                email: (emailInput && emailInput.value) ? emailInput.value : this.currentUserEmail || '',
                avatar: (nameInput && nameInput.value) ? nameInput.value.charAt(0).toUpperCase() : (this.currentUserName ? this.currentUserName.charAt(0).toUpperCase() : 'K'),
                rating: Math.round((this.ratings.service + this.ratings.cleanliness + this.ratings.amenities + this.ratings.location) / 4) || 5,
                date: 'Vừa xong',
                service: serviceSelect ? serviceSelect.options[serviceSelect.selectedIndex].text : '',
                content: content ? content.value : '',
                verified: false
            };

            // add to front of reviews so it appears first
            this.reviews.unshift(newReview);

            // If currently viewing 'mine', refresh mine list; otherwise prepend to grid
            if (this.currentFilter === 'mine') {
                this.showMyReviews();
            } else {
                const grid = document.querySelector('.reviews-grid');
                if (grid) {
                    const card = this.createReviewCard(newReview);
                    grid.insertBefore(card, grid.firstChild);
                    this.displayedReviews += 1;
                }
            }

            this.showToast('Cảm ơn bạn đã gửi đánh giá! Chúng tôi sẽ xem xét và phản hồi sớm nhất.', 'success');
            this.resetForm();
            
            submitBtn.disabled = false;
            submitBtn.innerHTML = '<i class="fas fa-paper-plane"></i> Gửi đánh giá';
        }, 2000);
    }

    resetForm() {
        const form = document.getElementById('reviewForm');
        form.reset();
        
        // Reset ratings
        this.ratings = { service: 0, cleanliness: 0, amenities: 0, location: 0 };
        
        // Reset star ratings
        document.querySelectorAll('.star-rating i').forEach(star => {
            star.classList.remove('active');
            star.style.color = '#e2e8f0';
        });
        
        // Reset rating texts
        document.querySelectorAll('.rating-text').forEach(text => {
            text.textContent = 'Chọn số sao';
            text.style.color = '#64748b';
            text.style.fontWeight = 'normal';
        });
        
        // Reset photo preview
        document.getElementById('photoPreview').innerHTML = '';
        document.getElementById('charCount').textContent = '0';
        
        // Reset upload area
        document.getElementById('uploadArea').style.opacity = '1';
        document.getElementById('uploadArea').style.pointerEvents = 'auto';
        
        // Remove error messages
        document.querySelectorAll('.error-message').forEach(msg => msg.remove());
        
        // Reset field styles
        document.querySelectorAll('input, select, textarea').forEach(field => {
            field.style.borderColor = '#e2e8f0';
        });
    }

    // ========== TOAST NOTIFICATIONS ==========
    showToast(message, type = 'success') {
        const container = document.getElementById('toastContainer');
        if (!container) return;

        const toast = document.createElement('div');
        toast.className = `toast ${type}`;
        
        const icon = type === 'success' ? 'fa-check-circle' : 'fa-exclamation-circle';
        const title = type === 'success' ? 'Thành công!' : 'Lỗi!';
        
        toast.innerHTML = `
            <i class="fas ${icon}"></i>
            <div class="toast-content">
                <h4>${title}</h4>
                <p>${message}</p>
            </div>
        `;

        container.appendChild(toast);

        // Show toast
        setTimeout(() => {
            toast.classList.add('show');
        }, 100);

        // Hide toast after 5 seconds
        setTimeout(() => {
            toast.classList.remove('show');
            setTimeout(() => {
                if (container.contains(toast)) {
                    container.removeChild(toast);
                }
            }, 300);
        }, 5000);
    }
}

// ========== INITIALIZE ==========
document.addEventListener('DOMContentLoaded', () => {
    new ReviewSystem();
});

// ========== UTILITY FUNCTIONS ==========
// Smooth scroll for anchor links
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        e.preventDefault();
        const target = document.querySelector(this.getAttribute('href'));
        if (target) {
            target.scrollIntoView({
                behavior: 'smooth',
                block: 'start'
            });
        }
    });
});

// Lazy loading for images
if ('IntersectionObserver' in window) {
    const imageObserver = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const img = entry.target;
                img.src = img.dataset.src;
                img.classList.remove('lazy');
                imageObserver.unobserve(img);
            }
        });
    });

    document.querySelectorAll('img[data-src]').forEach(img => {
        imageObserver.observe(img);
    });
}