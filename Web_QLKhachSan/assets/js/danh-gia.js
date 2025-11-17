// ========== DANH GIA JS - SIMPLIFIED VERSION ==========

class ReviewSystem {
    constructor() {
        this.currentFilter = 'all';
        this.currentUserEmail = '';
        this.currentUserName = '';
        
        this.init();
    }

    init() {
        this.initCountAnimation();
        this.updateScoreBadge();
        this.setupFilters();
        this.updateImageOverlays();
        this.setupImageViewer();
        this.setupPhotoUpload();
        this.setupStarRatings();
        this.setupFormReset();
    }

    // ========== UPDATE SCORE BADGE ==========
    updateScoreBadge() {
        const scoreBadge = document.querySelector('.score-badge-large');
        if (!scoreBadge) return;

        const scoreText = scoreBadge.querySelector('span');
        if (!scoreText) return;

        const scoreValue = parseFloat(scoreText.textContent);
        if (isNaN(scoreValue)) return;

        // Tính phần trăm cho vòng tròn (từ 0-5 sao thành 0-100%)
        const percentage = (scoreValue / 5) * 100;
        
        // Cập nhật CSS variable --score
        scoreBadge.style.setProperty('--score', percentage);
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
            const target = parseFloat(counter.textContent) || parseInt(counter.getAttribute('data-count')) || 0;
            const increment = target / 60;
            let current = 0;
            
            const timer = setInterval(() => {
                current += increment;
                if (current >= target) {
                    counter.textContent = target % 1 === 0 ? target : target.toFixed(1);
                    clearInterval(timer);
                } else {
                    counter.textContent = target % 1 === 0 ? Math.floor(current) : current.toFixed(1);
                }
            }, 16);
        });
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
        const grid = document.querySelector('.reviews-grid');

        if (btnAll) btnAll.addEventListener('click', () => {
            this.currentFilter = 'all';
            btnAll.classList.add('active');
            if (btnMine) btnMine.classList.remove('active');
            
            // Hiển thị tất cả review cards
            if (grid) {
                const allCards = grid.querySelectorAll('.review-card');
                allCards.forEach(card => card.style.display = '');
            }
        });

        if (btnMine) btnMine.addEventListener('click', () => {
            this.currentFilter = 'mine';
            btnMine.classList.add('active');
            if (btnAll) btnAll.classList.remove('active');
            
            // Chỉ hiển thị review của user hiện tại
            if (grid && this.currentUserEmail) {
                const allCards = grid.querySelectorAll('.review-card');
                let hasMyReview = false;
                allCards.forEach(card => {
                    const cardEmail = (card.dataset.email || '').toLowerCase();
                    if (cardEmail === this.currentUserEmail) {
                        card.style.display = '';
                        hasMyReview = true;
                    } else {
                        card.style.display = 'none';
                    }
                });
                
                if (!hasMyReview) {
                    this.showEmptyMine();
                }
            }
        });
    }

    showEmptyMine() {
        const grid = document.querySelector('.reviews-grid');
        if (!grid) return;
        
        // Xóa placeholder cũ nếu có
        const oldPlaceholder = grid.querySelector('.no-reviews-placeholder');
        if (oldPlaceholder) oldPlaceholder.remove();
        
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

    // ========== UPDATE IMAGE OVERLAYS ==========
    updateImageOverlays() {
        const reviewCards = document.querySelectorAll('.review-card');
        
        reviewCards.forEach(card => {
            const reviewImages = card.querySelector('.review-images');
            if (!reviewImages) return;
            
            const allImages = reviewImages.querySelectorAll('img');
            const totalImages = allImages.length;
            
            // Nếu có hơn 2 ảnh, thêm overlay vào ảnh thứ 2
            if (totalImages > 2) {
                const imageItems = reviewImages.querySelectorAll('.review-image-item');
                if (imageItems.length >= 2) {
                    const secondItem = imageItems[1];
                    
                    // Xóa overlay cũ nếu có
                    const oldOverlay = secondItem.querySelector('.review-image-overlay');
                    if (oldOverlay) {
                        oldOverlay.remove();
                    }
                    
                    // Thêm overlay mới với số ảnh còn lại
                    const overlay = document.createElement('div');
                    overlay.className = 'review-image-overlay';
                    overlay.textContent = `+${totalImages - 2}`;
                    secondItem.appendChild(overlay);
                }
                
                // Ẩn các ảnh từ thứ 3 trở đi
                for (let i = 2; i < imageItems.length; i++) {
                    imageItems[i].style.display = 'none';
                }
            }
        });
    }

    // ========== IMAGE VIEWER ==========
    setupImageViewer() {
        const reviewsGrid = document.querySelector('.reviews-grid');
        if (!reviewsGrid) return;

        // Tạo modal
        const modal = document.createElement('div');
        modal.className = 'image-modal';
        modal.innerHTML = `
            <div class="image-modal-content">
                <button class="image-modal-close">&times;</button>
                <div class="image-modal-images"></div>
            </div>
        `;
        document.body.appendChild(modal);

        // Xử lý click vào ảnh có overlay
        reviewsGrid.addEventListener('click', (e) => {
            const imageItem = e.target.closest('.review-image-item');
            if (!imageItem) return;

            const overlay = imageItem.querySelector('.review-image-overlay');
            if (!overlay) return; // Chỉ xử lý khi có overlay

            // Lấy tất cả ảnh từ review-images (kể cả ảnh ẩn)
            const reviewImages = imageItem.closest('.review-images');
            const allImages = reviewImages.querySelectorAll('img');
            
            // Hiển thị modal với tất cả ảnh
            const modalImages = modal.querySelector('.image-modal-images');
            modalImages.innerHTML = '';
            allImages.forEach(img => {
                const imgElement = document.createElement('img');
                imgElement.src = img.src;
                modalImages.appendChild(imgElement);
            });

            modal.classList.add('show');
            document.body.style.overflow = 'hidden';
        });

        // Đóng modal
        const closeBtn = modal.querySelector('.image-modal-close');
        closeBtn.addEventListener('click', () => {
            modal.classList.remove('show');
            document.body.style.overflow = '';
        });

        modal.addEventListener('click', (e) => {
            if (e.target === modal) {
                modal.classList.remove('show');
                document.body.style.overflow = '';
            }
        });
    }



    // ========== STAR RATINGS ==========
    setupStarRatings() {
        const container = document.querySelector('.star-rating[data-category="overall"]');
        if (!container) return;
        
        const stars = container.querySelectorAll('i');
        const ratingText = container.parentNode.querySelector('.rating-text');
        const diemInput = document.getElementById('Diem');
        let currentRating = 0;
        
        stars.forEach((star, index) => {
            star.addEventListener('click', () => {
                currentRating = index + 1;
                
                // Update hidden input value
                if (diemInput) {
                    diemInput.value = currentRating;
                }
                
                // Update visual state
                stars.forEach((s, i) => {
                    s.classList.toggle('active', i <= index);
                });
                
                // Update text
                const ratingTexts = ['Rất tệ', 'Tệ', 'Bình thường', 'Tốt', 'Xuất sắc'];
                if (ratingText) {
                    ratingText.textContent = ratingTexts[index];
                    ratingText.style.color = 'var(--gold)';
                    ratingText.style.fontWeight = '600';
                }
            });
            
            star.addEventListener('mouseenter', () => {
                stars.forEach((s, i) => {
                    s.style.color = i <= index ? 'var(--gold)' : '#e2e8f0';
                });
            });
        });
        
        container.addEventListener('mouseleave', () => {
            stars.forEach((star, i) => {
                star.style.color = i < currentRating ? 'var(--gold)' : '#e2e8f0';
            });
        });
    }

    // ========== FORM RESET ==========
    setupFormReset() {
        const resetBtn = document.getElementById('resetForm');
        if (!resetBtn) return;

        const diemInput = document.getElementById('Diem');
        const phongSelect = document.getElementById('PhongId');
        const binhLuanTextarea = document.getElementById('BinhLuan');

        resetBtn.addEventListener('click', () => {
            // Reset các field ngoại trừ tên khách hàng
            if (phongSelect) phongSelect.value = '';
            if (binhLuanTextarea) binhLuanTextarea.value = '';
            if (diemInput) diemInput.value = '0';
            
            const photoPreview = document.getElementById('photoPreview');
            if (photoPreview) photoPreview.innerHTML = '';

            // Reset star rating
            const stars = document.querySelectorAll('.star-rating[data-category="overall"] i');
            stars.forEach(star => {
                star.classList.remove('active');
                star.style.color = '#e2e8f0';
            });

            const ratingText = document.querySelector('.rating-text');
            if (ratingText) {
                ratingText.textContent = 'Chọn số sao';
                ratingText.style.color = '#64748b';
                ratingText.style.fontWeight = 'normal';
            }

            // Reset upload area
            const uploadArea = document.getElementById('uploadArea');
            if (uploadArea) {
                uploadArea.style.opacity = '1';
                uploadArea.style.pointerEvents = 'auto';
            }
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
                uploadedFiles.push(file);
                this.createPhotoPreview(file, uploadedFiles, preview);
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

}

// ========== INITIALIZE ==========
document.addEventListener('DOMContentLoaded', () => {
    new ReviewSystem();
});