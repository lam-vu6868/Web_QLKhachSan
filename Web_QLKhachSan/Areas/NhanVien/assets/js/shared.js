/* ======== SHARED JAVASCRIPT FOR NHANVIEN DASHBOARD ======== */
/* Đây là file JS chung cho tất cả trang trong NhanVien dashboard */
/* Cần include trước các file JS theo trang */

// ======== KHỞI TẠO CHUNG ========
document.addEventListener('DOMContentLoaded', function() {
    // Khởi tạo các chức năng dùng chung
    initializeClock();
    initializeSidebarToggle();
    initializeLogout();
    initializeNotifications();
    
    // Cập nhật thời gian mỗi giây
    setInterval(updateClock, 1000);
    
    console.log('Shared dashboard functions loaded');
});

// ======== CHỨC NĂNG ĐỒNG HỒ VÀ NGÀY ========
function initializeClock() {
    updateClock();
    updateDate();
}

function updateClock() {
    const now = new Date();
    const timeString = now.toLocaleTimeString('vi-VN', {
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit'
    });
    
    const timeElement = document.getElementById('current-time');
    if (timeElement) {
        timeElement.textContent = timeString;
    }
}

function updateDate() {
    const now = new Date();
    const dateString = now.toLocaleDateString('vi-VN', {
        weekday: 'long',
        year: 'numeric',
        month: 'long',
        day: 'numeric'
    });
    
    const dateElement = document.getElementById('current-date');
    if (dateElement) {
        dateElement.textContent = dateString;
    }
}

// ======== CHỨC NĂNG SIDEBAR ========
function initializeSidebarToggle() {
    const sidebarToggle = document.querySelector('.sidebar-toggle');
    const sidebar = document.querySelector('.sidebar');
    const mainContent = document.querySelector('.main-content');
    
    if (sidebarToggle && sidebar) {
        sidebarToggle.addEventListener('click', function() {
            if (window.innerWidth <= 992) {
                sidebar.classList.toggle('mobile-open');
            } else {
                sidebar.classList.toggle('collapsed');
                if (mainContent) {
                    mainContent.classList.toggle('sidebar-collapsed');
                }
            }
        });
    }
    
    // Đóng sidebar khi click bên ngoài trên mobile
    document.addEventListener('click', function(e) {
        if (window.innerWidth <= 992 && sidebar) {
            if (!sidebar.contains(e.target) && !sidebarToggle.contains(e.target)) {
                sidebar.classList.remove('mobile-open');
            }
        }
    });
    
    // Handle responsive sidebar
    window.addEventListener('resize', function() {
        if (sidebar && window.innerWidth > 992) {
            sidebar.classList.remove('mobile-open');
        }
    });
}

// ======== CHỨC NĂNG NAVIGATION ========
function handleNavigation(section) {
    console.log(`Navigating to: ${section}`);
    
    // Ở đây bạn có thể thêm logic để hiển thị các section khác nhau
    switch(section) {
        case 'dashboard':
            navigateToPage('dashboard-nhanvien.html');
            break;
        case 'booking-management':
            navigateToPage('quanly-datphong.html');
            break;
        case 'guest-management':
            navigateToPage('quanly-khach.html');
            break;
        case 'room-management':
            navigateToPage('quanly-phong.html');
            break;
        case 'service-management':
            navigateToPage('quanly-dichvu.html');
            break;
        case 'billing':
            navigateToPage('thanhtoan-hoadon.html');
            break;
        case 'reports':
            navigateToPage('baocao-calam.html');
            break;
        default:
            console.log(`Unknown section: ${section}`);
    }
}

function navigateToPage(page) {
    window.location.href = page;
}

// ======== MODAL HELPER FUNCTIONS ========
function showModal(title, content, options = {}) {
    // Tạo modal backdrop
    const backdrop = document.createElement('div');
    backdrop.className = 'modal-backdrop fade';
    backdrop.style.cssText = `
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background: rgba(0,0,0,0.5);
        z-index: 10000;
        display: flex;
        align-items: center;
        justify-content: center;
        opacity: 0;
        transition: opacity 0.3s ease;
    `;
    
    // Tạo modal content
    const modal = document.createElement('div');
    modal.className = 'modal-dialog';
    const maxWidth = options.size === 'large' ? '800px' : options.size === 'small' ? '400px' : '500px';
    modal.style.cssText = `
        background: var(--white);
        border-radius: 12px;
        max-width: ${maxWidth};
        width: 90%;
        max-height: 90vh;
        overflow: hidden;
        transform: scale(0.8);
        transition: transform 0.3s ease;
        box-shadow: var(--shadow-medium);
    `;
    
    modal.innerHTML = `
        <div class="modal-header" style="background: linear-gradient(135deg, var(--primary-navy), #1e3a5f); color: var(--white); padding: 1.5rem 2rem; position: relative;">
            <h3 style="margin: 0; font-weight: 600;">${title}</h3>
            <button class="modal-close" style="position: absolute; top: 50%; right: 2rem; transform: translateY(-50%); background: none; border: none; color: var(--white); font-size: 1.5rem; cursor: pointer; width: 30px; height: 30px; display: flex; align-items: center; justify-content: center; border-radius: 50%; transition: background-color 0.2s;">&times;</button>
        </div>
        <div class="modal-body" style="padding: 2rem; max-height: 70vh; overflow-y: auto;">
            ${content}
        </div>
    `;
    
    backdrop.appendChild(modal);
    document.body.appendChild(backdrop);
    
    // Animate in
    setTimeout(() => {
        backdrop.style.opacity = '1';
        modal.style.transform = 'scale(1)';
    }, 100);
    
    // Close functionality
    const closeBtn = modal.querySelector('.modal-close');
    closeBtn.addEventListener('click', () => {
        closeModal(backdrop);
    });
    
    backdrop.addEventListener('click', (e) => {
        if (e.target === backdrop) {
            closeModal(backdrop);
        }
    });
    
    // ESC key to close
    const handleEscape = (e) => {
        if (e.key === 'Escape') {
            closeModal(backdrop);
            document.removeEventListener('keydown', handleEscape);
        }
    };
    document.addEventListener('keydown', handleEscape);
    
    return backdrop;
}

function closeModal(backdrop) {
    const modal = backdrop.querySelector('.modal-dialog');
    backdrop.style.opacity = '0';
    modal.style.transform = 'scale(0.8)';
    
    setTimeout(() => {
        if (backdrop.parentNode) {
            backdrop.parentNode.removeChild(backdrop);
        }
    }, 300);
}

// ======== NOTIFICATION SYSTEM ========
function initializeNotifications() {
    // Tạo container cho notifications nếu chưa có
    if (!document.getElementById('notification-container')) {
        const container = document.createElement('div');
        container.id = 'notification-container';
        container.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            z-index: 20000;
            max-width: 400px;
        `;
        document.body.appendChild(container);
    }
}

function showNotification(message, type = 'info', duration = 5000) {
    const container = document.getElementById('notification-container');
    if (!container) return;
    
    const notification = document.createElement('div');
    const colors = {
        success: { bg: 'var(--success-color)', icon: 'fa-check-circle' },
        error: { bg: 'var(--danger-color)', icon: 'fa-exclamation-circle' },
        warning: { bg: 'var(--warning-color)', icon: 'fa-exclamation-triangle' },
        info: { bg: 'var(--info-color)', icon: 'fa-info-circle' }
    };
    
    const color = colors[type] || colors.info;
    
    notification.className = 'notification';
    notification.style.cssText = `
        background: var(--white);
        border-left: 4px solid ${color.bg};
        border-radius: 8px;
        padding: 1rem;
        margin-bottom: 10px;
        box-shadow: var(--shadow-medium);
        transform: translateX(100%);
        transition: transform 0.3s ease;
        display: flex;
        align-items: center;
        gap: 12px;
    `;
    
    notification.innerHTML = `
        <i class="fas ${color.icon}" style="color: ${color.bg}; font-size: 1.2rem;"></i>
        <span style="flex: 1; color: var(--text-dark);">${message}</span>
        <button onclick="this.parentElement.remove()" style="background: none; border: none; color: var(--text-dark); cursor: pointer; opacity: 0.7;">&times;</button>
    `;
    
    container.appendChild(notification);
    
    // Animate in
    setTimeout(() => {
        notification.style.transform = 'translateX(0)';
    }, 100);
    
    // Auto remove
    if (duration > 0) {
        setTimeout(() => {
            notification.style.transform = 'translateX(100%)';
            setTimeout(() => {
                if (notification.parentNode) {
                    notification.parentNode.removeChild(notification);
                }
            }, 300);
        }, duration);
    }
}

// ======== LOGOUT FUNCTIONALITY ========
function initializeLogout() {
    document.addEventListener('click', function(e) {
        if (e.target.closest('.logout-btn')) {
            e.preventDefault();
            showConfirm(
                'Xác nhận đăng xuất',
                'Bạn có chắc chắn muốn đăng xuất khỏi hệ thống?',
                () => {
                    // Thêm loading state
                    showNotification('Đang đăng xuất...', 'info', 2000);
                    
                    setTimeout(() => {
                        window.location.href = '../dang-nhap.html';
                    }, 1000);
                }
            );
        }
    });
}

// ======== CONFIRM DIALOG ========
function showConfirm(title, message, onConfirm, onCancel = null) {
    const content = `
        <div class="confirm-dialog">
            <p style="margin-bottom: 2rem; font-size: 1.1rem; line-height: 1.5; color: var(--text-dark);">${message}</p>
            <div style="display: flex; gap: 1rem; justify-content: flex-end;">
                <button class="btn-cancel" style="padding: 0.75rem 1.5rem; border: 2px solid var(--border-color); background: var(--white); color: var(--text-dark); border-radius: 8px; cursor: pointer; font-weight: 500; transition: all 0.2s;">Hủy</button>
                <button class="btn-confirm" style="padding: 0.75rem 1.5rem; border: none; background: var(--danger-color); color: var(--white); border-radius: 8px; cursor: pointer; font-weight: 500; transition: all 0.2s;">Xác nhận</button>
            </div>
        </div>
    `;
    
    const modal = showModal(title, content, { size: 'small' });
    
    const confirmBtn = modal.querySelector('.btn-confirm');
    const cancelBtn = modal.querySelector('.btn-cancel');
    
    confirmBtn.addEventListener('click', () => {
        closeModal(modal);
        if (onConfirm) onConfirm();
    });
    
    cancelBtn.addEventListener('click', () => {
        closeModal(modal);
        if (onCancel) onCancel();
    });
}

// ======== UTILITY FUNCTIONS ========

// Format số tiền
function formatCurrency(amount, currency = 'VND') {
    if (currency === 'USD') {
        return '$' + amount.toLocaleString('en-US', { minimumFractionDigits: 0 });
    }
    return amount.toLocaleString('vi-VN') + ' ₫';
}

// Format ngày tháng
function formatDate(date, format = 'dd/mm/yyyy') {
    const d = new Date(date);
    const day = String(d.getDate()).padStart(2, '0');
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const year = d.getFullYear();
    
    switch (format) {
        case 'dd/mm/yyyy':
            return `${day}/${month}/${year}`;
        case 'mm/dd/yyyy':
            return `${month}/${day}/${year}`;
        case 'yyyy-mm-dd':
            return `${year}-${month}-${day}`;
        default:
            return d.toLocaleDateString('vi-VN');
    }
}

// Format thời gian
function formatTime(date) {
    const d = new Date(date);
    return d.toLocaleTimeString('vi-VN', {
        hour: '2-digit',
        minute: '2-digit'
    });
}

// Tính khoảng thời gian
function timeAgo(date) {
    const now = new Date();
    const diffTime = Math.abs(now - new Date(date));
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    const diffHours = Math.ceil(diffTime / (1000 * 60 * 60));
    const diffMinutes = Math.ceil(diffTime / (1000 * 60));
    
    if (diffDays > 1) {
        return `${diffDays} ngày trước`;
    } else if (diffHours > 1) {
        return `${diffHours} giờ trước`;
    } else {
        return `${diffMinutes} phút trước`;
    }
}

// Debounce function cho search
function debounce(func, delay) {
    let timeoutId;
    return function (...args) {
        clearTimeout(timeoutId);
        timeoutId = setTimeout(() => func.apply(this, args), delay);
    };
}

// Loading state cho buttons
function setLoadingState(button, isLoading, text = 'Đang xử lý...') {
    if (isLoading) {
        button.disabled = true;
        button.dataset.originalText = button.textContent;
        button.innerHTML = `<i class="fas fa-spinner fa-spin"></i> ${text}`;
    } else {
        button.disabled = false;
        button.textContent = button.dataset.originalText || 'Xác nhận';
    }
}

// Copy to clipboard
function copyToClipboard(text) {
    navigator.clipboard.writeText(text).then(() => {
        showNotification('Đã sao chép vào clipboard', 'success', 2000);
    }).catch(() => {
        showNotification('Không thể sao chép', 'error', 3000);
    });
}

// Export functions to global scope for use in other files
window.dashboardShared = {
    showModal,
    closeModal,
    showNotification,
    showConfirm,
    handleNavigation,
    formatCurrency,
    formatDate,
    formatTime,
    timeAgo,
    debounce,
    setLoadingState,
    copyToClipboard
};