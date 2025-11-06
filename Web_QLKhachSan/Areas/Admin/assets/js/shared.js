// Dashboard JavaScript cho nhân viên khách sạn
document.addEventListener('DOMContentLoaded', function() {
    // Khởi tạo các chức năng
    initializeClock();
    initializeQuickActions();
    initializeSidebarToggle();
    initializeRoomHover();
    initializeActivityTimestamps();
    
    // Cập nhật thời gian mỗi giây
    setInterval(updateClock, 1000);
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
                mainContent.classList.toggle('sidebar-collapsed');
            }
        });
    }
    
    // Đóng sidebar khi click bên ngoài trên mobile
    document.addEventListener('click', function(e) {
        if (window.innerWidth <= 992) {
            if (!sidebar.contains(e.target) && !sidebarToggle.contains(e.target)) {
                sidebar.classList.remove('mobile-open');
            }
        }
    });
}

function handleNavigation(section) {
    console.log(`Navigating to: ${section}`);
    
    // Ở đây bạn có thể thêm logic để hiển thị các section khác nhau
    switch(section) {
        case 'dashboard':
            showDashboard();
            break;
        case 'booking-management':
            showBookingManagement();
            break;
        case 'guest-management':
            showGuestManagement();
            break;
        case 'room-management':
            showRoomManagement();
            break;
        case 'service-management':
            showServiceManagement();
            break;
        case 'billing':
            showBilling();
            break;
        case 'reports':
            showReports();
            break;
        default:
            showDashboard();
    }
}

// Placeholder functions for different sections
function showDashboard() {
    // Hiển thị dashboard chính
    console.log('Showing dashboard');
}

function showBookingManagement() {
    // Hiển thị quản lý đặt phòng
    console.log('Showing booking management');
}

function showGuestManagement() {
    // Hiển thị quản lý khách hàng
    console.log('Showing guest management');
}

function showRoomManagement() {
    // Hiển thị quản lý phòng
    console.log('Showing room management');
}

function showServiceManagement() {
    // Hiển thị quản lý dịch vụ
    console.log('Showing service management');
}

function showBilling() {
    // Hiển thị thanh toán và hóa đơn
    console.log('Showing billing');
}

function showReports() {
    // Hiển thị báo cáo
    console.log('Showing reports');
}

// ======== CHỨC NĂNG QUICK ACTIONS ========
function initializeQuickActions() {
    const quickActionCards = document.querySelectorAll('.quick-action-card');
    
    quickActionCards.forEach(card => {
        card.addEventListener('click', function() {
            const action = this.getAttribute('data-action');
            handleQuickAction(action);
        });
    });
}

function handleQuickAction(action) {
    switch(action) {
        case 'new-booking':
            openNewBookingModal();
            break;
        case 'check-in':
            openCheckInModal();
            break;
        case 'check-out':
            openCheckOutModal();
            break;
        case 'room-search':
            openRoomSearchModal();
            break;
        case 'invoice':
            openInvoiceModal();
            break;
        case 'room-status':
            openRoomStatusModal();
            break;
        default:
            console.log(`Unknown action: ${action}`);
    }
}

// Quick Action Modal Functions
function openNewBookingModal() {
    // Mở form đặt phòng mới
    console.log('Mở form đặt phòng mới');
}

function openCheckInModal() {
    // Mở modal check-in
    console.log('Mở form check-in');
}

function openCheckOutModal() {
    // Mở modal check-out
    console.log('Mở form check-out');
}

function openRoomSearchModal() {
    // Mở modal tìm kiếm phòng
    console.log('Mở tìm kiếm phòng trống');
}

function openInvoiceModal() {
    // Mở modal lập hóa đơn
    console.log('Mở form lập hóa đơn');
}

function openRoomStatusModal() {
    // Mở modal cập nhật trạng thái phòng
    console.log('Mở cập nhật trạng thái phòng');
}

// ======== CHỨC NĂNG ROOM HOVER ========
function initializeRoomHover() {
    const roomItems = document.querySelectorAll('.room-item');
    
    roomItems.forEach(room => {
        room.addEventListener('click', function() {
            const roomNumber = this.getAttribute('data-room');
            const roomStatus = this.classList.contains('available') ? 'Trống' :
                              this.classList.contains('occupied') ? 'Đang ở' :
                              this.classList.contains('cleaning') ? 'Đang dọn' :
                              this.classList.contains('maintenance') ? 'Bảo trì' : 'Không xác định';
            
            showRoomDetails(roomNumber, roomStatus);
        });
        
        // Thêm tooltip hover effect
        room.addEventListener('mouseenter', function() {
            const roomNumber = this.getAttribute('data-room');
            const roomStatus = this.classList.contains('available') ? 'Trống' :
                              this.classList.contains('occupied') ? 'Đang ở' :
                              this.classList.contains('cleaning') ? 'Đang dọn' :
                              this.classList.contains('maintenance') ? 'Bảo trì' : 'Không xác định';
                              
            this.setAttribute('title', `Phòng ${roomNumber} - ${roomStatus}`);
        });
    });
}

function showRoomDetails(roomNumber, status) {
    const modalContent = `
        <div class="room-detail-modal">
            <h3>Chi tiết phòng ${roomNumber}</h3>
            <p><strong>Trạng thái:</strong> ${status}</p>
            <p><strong>Loại phòng:</strong> ${getRoomType(roomNumber)}</p>
            <p><strong>Giá:</strong> ${getRoomPrice(roomNumber)}</p>
            ${status === 'Đang ở' ? `<p><strong>Khách:</strong> ${getGuestInfo(roomNumber)}</p>` : ''}
            <div class="modal-actions">
                <button onclick="updateRoomStatus('${roomNumber}')">Cập nhật trạng thái</button>
                <button onclick="viewRoomHistory('${roomNumber}')">Xem lịch sử</button>
            </div>
        </div>
    `;
    
    showModal('Chi tiết phòng', modalContent);
}

function getRoomType(roomNumber) {
    // Logic để xác định loại phòng dựa trên số phòng
    const floor = Math.floor(roomNumber / 100);
    const roomNum = roomNumber % 100;
    
    if (roomNum <= 4) return 'Deluxe Ocean View';
    if (roomNum <= 6) return 'Executive Suite';
    return 'Family Bungalow';
}

function getRoomPrice(roomNumber) {
    const type = getRoomType(roomNumber);
    switch(type) {
        case 'Deluxe Ocean View': return '$150/đêm';
        case 'Executive Suite': return '$250/đêm';
        case 'Family Bungalow': return '$320/đêm';
        default: return 'Liên hệ';
    }
}

function getGuestInfo(roomNumber) {
    // Mock data - trong thực tế sẽ lấy từ database
    const guests = [
        'Trần Văn Hùng',
        'Nguyễn Thị Lan', 
        'Lê Minh Đức',
        'Phạm Thị Hoa',
        'Võ Thanh Nam'
    ];
    return guests[Math.floor(Math.random() * guests.length)];
}

// ======== CHỨC NĂNG ACTIVITIES ========
function initializeActivityTimestamps() {
    // Cập nhật timestamps của activities theo thời gian thực
    updateActivityTimestamps();
    setInterval(updateActivityTimestamps, 60000); // Cập nhật mỗi phút
}

function updateActivityTimestamps() {
    const timeElements = document.querySelectorAll('.activity-content .time');
    // Ở đây có thể tính toán lại thời gian từ timestamps thực tế
}

// ======== HELPER FUNCTIONS ========

function showModal(title, content) {
    // Tạo modal backdrop
    const backdrop = document.createElement('div');
    backdrop.className = 'modal-backdrop';
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
    modal.className = 'modal-content';
    modal.style.cssText = `
        background: white;
        border-radius: 12px;
        padding: 2rem;
        max-width: 500px;
        width: 90%;
        max-height: 80vh;
        overflow-y: auto;
        transform: scale(0.8);
        transition: transform 0.3s ease;
    `;
    
    modal.innerHTML = `
        <div class="modal-header">
            <h3 style="color: var(--deep-navy); margin-bottom: 1rem;">${title}</h3>
            <button class="modal-close" style="position: absolute; top: 1rem; right: 1rem; background: none; border: none; font-size: 1.5rem; cursor: pointer;">&times;</button>
        </div>
        <div class="modal-body">
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
}

function closeModal(backdrop) {
    const modal = backdrop.querySelector('.modal-content');
    backdrop.style.opacity = '0';
    modal.style.transform = 'scale(0.8)';
    
    setTimeout(() => {
        if (backdrop.parentNode) {
            backdrop.parentNode.removeChild(backdrop);
        }
    }, 300);
}

// Room status update function
function updateRoomStatus(roomNumber) {
    console.log(`Cập nhật trạng thái phòng ${roomNumber}`);
    // Logic cập nhật trạng thái phòng
}

function viewRoomHistory(roomNumber) {
    console.log(`Xem lịch sử phòng ${roomNumber}`);
    // Logic xem lịch sử phòng
}

// ======== RESPONSIVE HANDLING ========
window.addEventListener('resize', function() {
    const sidebar = document.querySelector('.sidebar');
    if (window.innerWidth > 992) {
        sidebar.classList.remove('mobile-open');
    }
});

// ======== LOGOUT FUNCTIONALITY ========
document.addEventListener('click', function(e) {
    if (e.target.closest('.logout-btn')) {
        if (confirm('Bạn có chắc chắn muốn đăng xuất?')) {
            // Redirect to login page
            window.location.href = '../dang-nhap.html';
        }
    }
});