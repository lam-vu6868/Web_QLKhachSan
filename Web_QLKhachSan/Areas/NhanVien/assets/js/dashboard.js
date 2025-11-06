// Dashboard JavaScript riêng cho trang dashboard-nhanvien.html
document.addEventListener('DOMContentLoaded', function() {
    // Khởi tạo các chức năng riêng cho dashboard
    initializeQuickActions();
    initializeRoomHover();
    initializeActivityTimestamps();
    initializeDashboardStats();
    
    console.log('Dashboard page loaded successfully');
});

// ======== CHỨC NĂNG RIÊNG CHO DASHBOARD ========
function initializeDashboardStats() {
    // Khởi tạo thống kê dashboard
    updateStatCards();
    initializeStatsAnimation();
}

function updateStatCards() {
    // Cập nhật các thẻ thống kê
    const stats = {
        totalRooms: 42,
        availableRooms: 18,
        checkedIn: 22,
        checkedOut: 8
    };
    
    // Cập nhật UI với dữ liệu thống kê
    if (document.getElementById('total-rooms')) {
        document.getElementById('total-rooms').textContent = stats.totalRooms;
    }
    if (document.getElementById('available-rooms')) {
        document.getElementById('available-rooms').textContent = stats.availableRooms;
    }
    if (document.getElementById('checked-in')) {
        document.getElementById('checked-in').textContent = stats.checkedIn;
    }
    if (document.getElementById('checked-out')) {
        document.getElementById('checked-out').textContent = stats.checkedOut;
    }
}

function initializeStatsAnimation() {
    // Animation cho stat cards
    const statCards = document.querySelectorAll('.stat-card');
    statCards.forEach((card, index) => {
        card.style.animationDelay = `${index * 0.1}s`;
    });
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
    
    window.dashboardShared.showModal('Chi tiết phòng', modalContent);
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

// ======== DASHBOARD HELPER FUNCTIONS ========
// Sử dụng showModal từ shared.js thông qua window.dashboardShared.showModal

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

// Logout functionality được xử lý trong shared.js