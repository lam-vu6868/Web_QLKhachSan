// ======== GIOI THIEU PAGE - IMAGE ZOOM FUNCTIONALITY ======== 
class ImageZoom {
    constructor() {
        this.currentImageIndex = 0;
        this.images = [];
        this.init();
    }

    init() {
        this.createModal();
        this.bindEvents();
        this.collectImages();
    }

    createModal() {
        // Tạo modal HTML
        const modalHTML = `
            <div id="imageZoomModal" class="image-zoom-modal">
                <div class="zoom-image-container">
                    <img id="zoomImage" class="zoom-image" src="" alt="">
                    <button class="zoom-close-btn" id="zoomCloseBtn">
                        <i class="fas fa-times"></i>
                    </button>
                    <div class="zoom-image-info" id="zoomImageInfo">
                        Hình ảnh 1 / 1
                    </div>
                </div>
            </div>
        `;
        
        // Thêm modal vào body
        document.body.insertAdjacentHTML('beforeend', modalHTML);
        
        // Lấy references
        this.modal = document.getElementById('imageZoomModal');
        this.zoomImage = document.getElementById('zoomImage');
        this.zoomInfo = document.getElementById('zoomImageInfo');
        this.closeBtn = document.getElementById('zoomCloseBtn');
    }

    bindEvents() {
        // Event cho nút đóng
        this.closeBtn.addEventListener('click', (e) => {
            e.stopPropagation();
            this.closeModal();
        });

        // Event cho click ra ngoài modal
        this.modal.addEventListener('click', (e) => {
            if (e.target === this.modal) {
                this.closeModal();
            }
        });

        // Event cho phím ESC
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape' && this.modal.classList.contains('show')) {
                this.closeModal();
            }
        });

        // Event cho các icon kính lúp trong timeline
        document.addEventListener('click', (e) => {
            // Kiểm tra nếu click vào icon zoom trong image-overlay
            if (e.target.closest('.image-overlay i')) {
                e.preventDefault();
                e.stopPropagation();
                
                const imageElement = e.target.closest('.timeline-image').querySelector('img');
                if (imageElement) {
                    this.openModal(imageElement.src, imageElement.alt);
                }
            }
        });

        // Event cho story images
        document.addEventListener('click', (e) => {
            if (e.target.closest('.story-image-v2')) {
                e.preventDefault();
                const imageElement = e.target.closest('.story-image-v2').querySelector('img');
                if (imageElement) {
                    this.openModal(imageElement.src, imageElement.alt);
                }
            }
        });

        // Event cho value card images (nếu có)
        document.addEventListener('click', (e) => {
            if (e.target.closest('.value-card img')) {
                e.preventDefault();
                const imageElement = e.target.closest('.value-card').querySelector('img');
                if (imageElement) {
                    this.openModal(imageElement.src, imageElement.alt);
                }
            }
        });
    }

    collectImages() {
        // Thu thập ảnh timeline riêng biệt
        const timelineImages = document.querySelectorAll('.timeline-image img');
        this.timelineImages = [...timelineImages].map(img => ({
            src: img.src,
            alt: img.alt || 'Hình ảnh timeline'
        }));
        
        // Thu thập ảnh story riêng biệt
        const storyImages = document.querySelectorAll('.story-image-v2 img');
        this.storyImages = [...storyImages].map(img => ({
            src: img.src,
            alt: img.alt || 'Hình ảnh câu chuyện'
        }));
        
        // Thu thập ảnh value card riêng biệt
        const valueImages = document.querySelectorAll('.value-card img');
        this.valueImages = [...valueImages].map(img => ({
            src: img.src,
            alt: img.alt || 'Hình ảnh giá trị'
        }));
        
        // Tổng hợp tất cả ảnh (để backward compatibility)
        this.images = [...this.timelineImages, ...this.storyImages, ...this.valueImages];
    }

    openModal(imageSrc, imageAlt = 'Hình ảnh giới thiệu') {
        // Xác định loại ảnh và tìm index trong collection tương ứng
        let currentCollection = this.images;
        let collectionName = 'Hình ảnh';
        
        // Kiểm tra nếu là timeline image
        const timelineIndex = this.timelineImages.findIndex(img => img.src === imageSrc);
        if (timelineIndex !== -1) {
            this.currentImageIndex = timelineIndex;
            currentCollection = this.timelineImages;
            collectionName = 'Timeline';
        } else {
            // Kiểm tra nếu là story image
            const storyIndex = this.storyImages.findIndex(img => img.src === imageSrc);
            if (storyIndex !== -1) {
                this.currentImageIndex = storyIndex;
                currentCollection = this.storyImages;
                collectionName = 'Câu chuyện';
            } else {
                // Kiểm tra nếu là value image
                const valueIndex = this.valueImages.findIndex(img => img.src === imageSrc);
                if (valueIndex !== -1) {
                    this.currentImageIndex = valueIndex;
                    currentCollection = this.valueImages;
                    collectionName = 'Giá trị';
                } else {
                    this.currentImageIndex = 0;
                }
            }
        }

        // Lưu collection hiện tại để sử dụng trong updateImageInfo
        this.currentCollection = currentCollection;
        this.currentCollectionName = collectionName;

        // Hiển thị modal
        this.zoomImage.src = imageSrc;
        this.zoomImage.alt = imageAlt;
        this.updateImageInfo();
        
        this.modal.classList.add('show');
        document.body.style.overflow = 'hidden'; // Ngăn scroll trang chính
        
        // Log cho debug
        console.log('Opening image zoom:', imageSrc, 'Collection:', collectionName);
    }

    closeModal() {
        this.modal.classList.remove('show');
        document.body.style.overflow = ''; // Khôi phục scroll
        
        // Reset ảnh sau khi đóng
        setTimeout(() => {
            this.zoomImage.src = '';
        }, 300);
        
        console.log('Closing image zoom modal');
    }

    updateImageInfo() {
        const currentIndex = this.currentImageIndex + 1;
        const totalImages = this.currentCollection ? this.currentCollection.length : this.images.length;
        const collectionName = this.currentCollectionName || 'Hình ảnh';
        
        this.zoomInfo.textContent = `${collectionName} ${currentIndex} / ${totalImages}`;
    }

    // Phương thức công khai để mở modal từ bên ngoài
    static openImage(imageSrc, imageAlt) {
        if (window.gioiThieuImageZoom) {
            window.gioiThieuImageZoom.openModal(imageSrc, imageAlt);
        }
    }
}

// ======== GIOI THIEU PAGE ENHANCEMENTS ========
class GioiThieuEnhancements {
    constructor() {
        this.init();
    }

    init() {
        this.addImageHoverEffects();
        this.addScrollAnimations();
        this.addTimelineInteractions();
    }

    addImageHoverEffects() {
        // Thêm style cho cursor khi hover vào ảnh có thể zoom
        const style = document.createElement('style');
        style.textContent = `
            .timeline-image:hover,
            .story-image-v2:hover {
                cursor: zoom-in;
            }
            
            .image-overlay {
                opacity: 0;
                transition: opacity 0.3s ease;
            }
            
            .timeline-content:hover .image-overlay {
                opacity: 1;
            }
            
            .image-overlay i {
                cursor: pointer;
                transition: all 0.3s ease;
                transform: scale(0.8);
            }
            
            .image-overlay i:hover {
                transform: scale(1.2);
                color: var(--gold) !important;
                text-shadow: 0 0 10px rgba(212, 175, 55, 0.8);
            }

            .timeline-image::after {
                content: '';
                position: absolute;
                top: 50%;
                left: 50%;
                transform: translate(-50%, -50%);
                width: 60px;
                height: 60px;
                background: rgba(255, 255, 255, 0.9);
                border-radius: 50%;
                display: flex;
                align-items: center;
                justify-content: center;
                opacity: 0;
                transition: all 0.3s ease;
                pointer-events: none;
                z-index: 10;
            }

            .timeline-content:hover .timeline-image::after {
                opacity: 1;
            }
        `;
        document.head.appendChild(style);
    }

    addScrollAnimations() {
        // Observer cho animations khi scroll
        const observerOptions = {
            threshold: 0.1,
            rootMargin: '0px 0px -50px 0px'
        };

        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.style.opacity = '1';
                    entry.target.style.transform = 'translateY(0)';
                }
            });
        }, observerOptions);

        // Observe timeline items
        document.querySelectorAll('.timeline-item').forEach(item => {
            item.style.opacity = '0';
            item.style.transform = 'translateY(30px)';
            item.style.transition = 'all 0.6s ease';
            observer.observe(item);
        });

        // Observe value cards
        document.querySelectorAll('.value-card').forEach(card => {
            card.style.opacity = '0';
            card.style.transform = 'translateY(30px)';
            card.style.transition = 'all 0.6s ease';
            observer.observe(card);
        });
    }

    addTimelineInteractions() {
        // Thêm interactive effects cho timeline
        document.querySelectorAll('.timeline-content').forEach(content => {
            content.addEventListener('mouseenter', () => {
                // Highlight timeline line
                const timelineItem = content.closest('.timeline-item');
                if (timelineItem) {
                    timelineItem.style.zIndex = '10';
                }
            });

            content.addEventListener('mouseleave', () => {
                const timelineItem = content.closest('.timeline-item');
                if (timelineItem) {
                    timelineItem.style.zIndex = '1';
                }
            });
        });
    }
}

// Khởi tạo khi DOM loaded
document.addEventListener('DOMContentLoaded', () => {
    console.log('Initializing Gioi Thieu page enhancements...');
    
    // Khởi tạo image zoom
    window.gioiThieuImageZoom = new ImageZoom();
    
    // Khởi tạo các enhancements khác
    window.gioiThieuEnhancements = new GioiThieuEnhancements();
    
    console.log('Gioi Thieu page ready!');
});

// Export cho sử dụng global
window.GioiThieuImageZoom = ImageZoom;