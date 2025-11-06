// JavaScript cho trang Quản lý phòng
document.addEventListener('DOMContentLoaded', function() {
    initializeRoomManagement();
    initializeFilters();
    initializeRoomActions();
    initializeHousekeepingTasks();
    initializeBulkActions();
});

// Dữ liệu mẫu cho các phòng
const roomsData = {
    501: { guest: 'Trần Minh Hoàng', checkin: '14:00', checkout: '11:00 (Ngày mai)', phone: '0901234567', type: 'Executive Suite', rate: '$250/đêm' },
    502: { guest: 'Nguyễn Thị Lan', checkin: '15:30', checkout: '12:00 (Ngày mai)', phone: '0907654321', type: 'Executive Suite', rate: '$250/đêm' },
    503: { status: 'Sẵn sàng', cleaned: '11:30', type: 'Family Bungalow', rate: '$320/đêm' },
    504: { guest: 'Gia đình Lê Văn Nam', checkin: '16:00', checkout: '11:00 (Ngày mai)', phone: '0912345678', type: 'Family Bungalow', rate: '$320/đêm' },
    // Thêm dữ liệu cho các phòng khác...
};

function initializeRoomManagement() {
    // Khởi tạo các chức năng quản lý phòng
    updateRoomStats();
    attachRoomClickEvents();
}

function updateRoomStats() {
    // Cập nhật số liệu thống kê phòng
    const rooms = document.querySelectorAll('.room-card');
    const stats = {
        available: 0,
        occupied: 0,
        cleaning: 0,
        maintenance: 0
    };
    
    rooms.forEach(room => {
        if (room.classList.contains('available')) stats.available++;
        else if (room.classList.contains('occupied')) stats.occupied++;
        else if (room.classList.contains('cleaning')) stats.cleaning++;
        else if (room.classList.contains('maintenance')) stats.maintenance++;
    });
    
    // Cập nhật header stats
    document.querySelector('.stat-badge.available .count').textContent = stats.available;
    document.querySelector('.stat-badge.occupied .count').textContent = stats.occupied;
    document.querySelector('.stat-badge.cleaning .count').textContent = stats.cleaning;
    document.querySelector('.stat-badge.maintenance .count').textContent = stats.maintenance;
    
    // Cập nhật floor stats
    updateFloorStats();
}

function updateFloorStats() {
    const floors = document.querySelectorAll('.floor-section');
    
    floors.forEach(floor => {
        const floorNum = floor.getAttribute('data-floor');
        const rooms = floor.querySelectorAll('.room-card');
        const floorStats = {
            available: 0,
            occupied: 0,
            cleaning: 0,
            maintenance: 0
        };
        
        rooms.forEach(room => {
            if (room.classList.contains('available')) floorStats.available++;
            else if (room.classList.contains('occupied')) floorStats.occupied++;
            else if (room.classList.contains('cleaning')) floorStats.cleaning++;
            else if (room.classList.contains('maintenance')) floorStats.maintenance++;
        });
        
        // Cập nhật floor stats display
        const statsDiv = floor.querySelector('.floor-stats');
        statsDiv.querySelector('.available').textContent = `${floorStats.available} trống`;
        statsDiv.querySelector('.occupied').textContent = `${floorStats.occupied} đang ở`;
        statsDiv.querySelector('.cleaning').textContent = `${floorStats.cleaning} dọn`;
        statsDiv.querySelector('.maintenance').textContent = `${floorStats.maintenance} bảo trì`;
    });
}

function attachRoomClickEvents() {
    const roomCards = document.querySelectorAll('.room-card');
    
    roomCards.forEach(card => {
        // Click vào room card để xem chi tiết
        card.addEventListener('click', function(e) {
            if (!e.target.closest('.room-actions')) {
                const roomNumber = this.getAttribute('data-room');
                showRoomDetail(roomNumber);
            }
        });
        
        // Prevent event bubbling for action buttons
        const actionButtons = card.querySelectorAll('.room-actions button');
        actionButtons.forEach(btn => {
            btn.addEventListener('click', function(e) {
                e.stopPropagation();
            });
        });
    });
}

function showRoomDetail(roomNumber) {
    const roomData = roomsData[roomNumber];
    const roomCard = document.querySelector(`[data-room="${roomNumber}"]`);
    const roomType = roomCard.getAttribute('data-type');
    const roomStatus = getRoomStatus(roomCard);
    
    let modalContent = `
        <div class="room-detail-info">
            <div class="row">
                <div class="col-md-6">
                    <div class="detail-section">
                        <h4><i class="fas fa-bed"></i> Thông tin phòng</h4>
                        <div class="detail-row">
                            <span class="label">Số phòng:</span>
                            <span class="value">${roomNumber}</span>
                        </div>
                        <div class="detail-row">
                            <span class="label">Loại phòng:</span>
                            <span class="value">${getFullRoomType(roomType)}</span>
                        </div>
                        <div class="detail-row">
                            <span class="label">Giá phòng:</span>
                            <span class="value">${roomData?.rate || 'Liên hệ'}</span>
                        </div>
                        <div class="detail-row">
                            <span class="label">Trạng thái:</span>
                            <span class="value status-${roomStatus.toLowerCase()}">${roomStatus}</span>
                        </div>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="detail-section">
                        <h4><i class="fas fa-user"></i> Thông tin khách hàng</h4>
                        ${roomData?.guest ? `
                            <div class="detail-row">
                                <span class="label">Tên khách:</span>
                                <span class="value">${roomData.guest}</span>
                            </div>
                            <div class="detail-row">
                                <span class="label">Điện thoại:</span>
                                <span class="value">${roomData.phone || 'Chưa có'}</span>
                            </div>
                            <div class="detail-row">
                                <span class="label">Check-in:</span>
                                <span class="value">${roomData.checkin}</span>
                            </div>
                            <div class="detail-row">
                                <span class="label">Check-out:</span>
                                <span class="value">${roomData.checkout}</span>
                            </div>
                        ` : `
                            <p class="text-muted">Phòng hiện tại không có khách</p>
                            ${roomData?.cleaned ? `
                                <div class="detail-row">
                                    <span class="label">Dọn phòng xong:</span>
                                    <span class="value">${roomData.cleaned}</span>
                                </div>
                            ` : ''}
                        `}
                    </div>
                </div>
            </div>
            
            <div class="room-actions-detail">
                <h4><i class="fas fa-tools"></i> Thao tác</h4>
                <div class="action-buttons">
                    <button class="btn btn-primary" onclick="updateRoomStatus('${roomNumber}', 'available')">
                        <i class="fas fa-check"></i> Đánh dấu sẵn sàng
                    </button>
                    <button class="btn btn-warning" onclick="updateRoomStatus('${roomNumber}', 'cleaning')">
                        <i class="fas fa-broom"></i> Cần dọn phòng
                    </button>
                    <button class="btn btn-danger" onclick="updateRoomStatus('${roomNumber}', 'maintenance')">
                        <i class="fas fa-tools"></i> Báo cáo sự cố
                    </button>
                    <button class="btn btn-info" onclick="assignHousekeeper('${roomNumber}')">
                        <i class="fas fa-user-plus"></i> Giao việc
                    </button>
                </div>
            </div>
        </div>
    `;
    
    // Thêm CSS cho modal content
    modalContent += `
        <style>
            .detail-section {
                background: #f8f9fa;
                padding: 1.5rem;
                border-radius: 8px;
                margin-bottom: 1rem;
            }
            .detail-section h4 {
                color: var(--deep-navy);
                margin-bottom: 1rem;
                display: flex;
                align-items: center;
                gap: 8px;
            }
            .detail-row {
                display: flex;
                justify-content: space-between;
                padding: 8px 0;
                border-bottom: 1px solid #dee2e6;
            }
            .detail-row:last-child {
                border-bottom: none;
            }
            .label {
                font-weight: 600;
                color: var(--charcoal-grey);
            }
            .value {
                color: var(--deep-navy);
            }
            .status-available { color: var(--success-color); }
            .status-occupied { color: var(--danger-color); }
            .status-cleaning { color: var(--warning-color); }
            .status-maintenance { color: var(--charcoal-grey); }
            .room-actions-detail {
                margin-top: 2rem;
                padding-top: 1.5rem;
                border-top: 2px solid #dee2e6;
            }
            .action-buttons {
                display: flex;
                gap: 0.5rem;
                flex-wrap: wrap;
            }
            .action-buttons .btn {
                flex: 1;
                min-width: 140px;
            }
        </style>
    `;
    
    document.getElementById('roomDetailContent').innerHTML = modalContent;
    
    // Show modal
    const modal = new bootstrap.Modal(document.getElementById('roomDetailModal'));
    modal.show();
}

function getRoomStatus(roomCard) {
    if (roomCard.classList.contains('available')) return 'Trống';
    if (roomCard.classList.contains('occupied')) return 'Đang ở';
    if (roomCard.classList.contains('cleaning')) return 'Đang dọn';
    if (roomCard.classList.contains('maintenance')) return 'Bảo trì';
    return 'Không xác định';
}

function getFullRoomType(type) {
    switch(type) {
        case 'deluxe': return 'Deluxe Ocean View';
        case 'executive': return 'Executive Suite';
        case 'family': return 'Family Bungalow';
        default: return 'Standard Room';
    }
}

function initializeFilters() {
    const searchInput = document.getElementById('room-search');
    const floorFilter = document.getElementById('floor-filter');
    const statusFilter = document.getElementById('status-filter');
    const typeFilter = document.getElementById('type-filter');
    
    // Search functionality
    searchInput.addEventListener('input', function() {
        filterRooms();
    });
    
    // Filter change events
    [floorFilter, statusFilter, typeFilter].forEach(filter => {
        filter.addEventListener('change', function() {
            filterRooms();
        });
    });
}

function filterRooms() {
    const searchTerm = document.getElementById('room-search').value.toLowerCase();
    const selectedFloor = document.getElementById('floor-filter').value;
    const selectedStatus = document.getElementById('status-filter').value;
    const selectedType = document.getElementById('type-filter').value;
    
    const floors = document.querySelectorAll('.floor-section');
    
    floors.forEach(floor => {
        const floorNum = floor.getAttribute('data-floor');
        const rooms = floor.querySelectorAll('.room-card');
        let visibleRooms = 0;
        
        // Hide/show floor based on filter
        if (selectedFloor && selectedFloor !== floorNum) {
            floor.style.display = 'none';
            return;
        } else {
            floor.style.display = 'block';
        }
        
        rooms.forEach(room => {
            const roomNumber = room.getAttribute('data-room');
            const roomType = room.getAttribute('data-type');
            let shouldShow = true;
            
            // Search filter
            if (searchTerm && !roomNumber.includes(searchTerm)) {
                shouldShow = false;
            }
            
            // Status filter
            if (selectedStatus && !room.classList.contains(selectedStatus)) {
                shouldShow = false;
            }
            
            // Type filter
            if (selectedType && roomType !== selectedType) {
                shouldShow = false;
            }
            
            room.style.display = shouldShow ? 'block' : 'none';
            if (shouldShow) visibleRooms++;
        });
        
        // Hide floor if no visible rooms
        if (visibleRooms === 0) {
            floor.style.display = 'none';
        }
    });
}

function initializeRoomActions() {
    // Status change buttons
    document.addEventListener('click', function(e) {
        if (e.target.closest('.status-btn')) {
            const btn = e.target.closest('.status-btn');
            const roomCard = btn.closest('.room-card');
            const roomNumber = roomCard.getAttribute('data-room');
            
            if (btn.classList.contains('available')) {
                updateRoomStatus(roomNumber, 'available');
            } else if (btn.classList.contains('occupied')) {
                updateRoomStatus(roomNumber, 'occupied');
            } else if (btn.classList.contains('cleaning')) {
                updateRoomStatus(roomNumber, 'cleaning');
            } else if (btn.classList.contains('maintenance')) {
                updateRoomStatus(roomNumber, 'maintenance');
            }
        }
        
        if (e.target.closest('.info-btn')) {
            const roomCard = e.target.closest('.room-card');
            const roomNumber = roomCard.getAttribute('data-room');
            showRoomDetail(roomNumber);
        }
    });
}

function updateRoomStatus(roomNumber, newStatus) {
    const roomCard = document.querySelector(`[data-room="${roomNumber}"]`);
    
    if (!roomCard) return;
    
    // Remove all status classes
    roomCard.classList.remove('available', 'occupied', 'cleaning', 'maintenance');
    
    // Add new status class
    roomCard.classList.add(newStatus);
    
    // Update room content based on status
    updateRoomContent(roomCard, newStatus);
    
    // Update statistics
    updateRoomStats();
    
    // Show notification
    showNotification(`Phòng ${roomNumber} đã được cập nhật thành ${getStatusText(newStatus)}`, 'success');
    
    // Close modal if open
    const modal = bootstrap.Modal.getInstance(document.getElementById('roomDetailModal'));
    if (modal) {
        modal.hide();
    }
}

function updateRoomContent(roomCard, status) {
    const roomNumber = roomCard.getAttribute('data-room');
    let content = '';
    
    switch(status) {
        case 'available':
            content = `
                <div class="room-status">Sẵn sàng nhận khách</div>
                <div class="room-cleaned">Dọn xong: ${getCurrentTime()}</div>
            `;
            break;
        case 'cleaning':
            content = `
                <div class="room-status">Đang dọn phòng</div>
                <div class="room-housekeeper">Nhân viên: Chờ phân công</div>
            `;
            break;
        case 'maintenance':
            content = `
                <div class="room-status">Đang bảo trì</div>
                <div class="room-issue">Sự cố: Cần kiểm tra</div>
            `;
            break;
        case 'occupied':
            content = `
                <div class="room-guest">Khách mới</div>
                <div class="room-checkin">Check-in: ${getCurrentTime()}</div>
            `;
            break;
    }
    
    // Update content
    const existingContent = roomCard.querySelector('.room-guest, .room-status, .room-housekeeper, .room-issue');
    if (existingContent) {
        existingContent.nextElementSibling?.remove();
        existingContent.remove();
    }
    
    const roomActions = roomCard.querySelector('.room-actions');
    roomActions.insertAdjacentHTML('beforebegin', content);
}

function getCurrentTime() {
    return new Date().toLocaleTimeString('vi-VN', {
        hour: '2-digit',
        minute: '2-digit'
    });
}

function getStatusText(status) {
    switch(status) {
        case 'available': return 'Sẵn sàng';
        case 'occupied': return 'Có khách';
        case 'cleaning': return 'Đang dọn';
        case 'maintenance': return 'Bảo trì';
        default: return 'Không xác định';
    }
}

function initializeHousekeepingTasks() {
    // Assign task buttons
    document.addEventListener('click', function(e) {
        if (e.target.closest('.assign-btn')) {
            const taskItem = e.target.closest('.task-item');
            const roomNumber = taskItem.querySelector('.task-room').textContent.replace('Phòng ', '');
            assignHousekeeper(roomNumber);
        }
        
        if (e.target.closest('.complete-btn')) {
            const taskItem = e.target.closest('.task-item');
            const roomNumber = taskItem.querySelector('.task-room').textContent.replace('Phòng ', '');
            completeTask(roomNumber);
        }
    });
}

function assignHousekeeper(roomNumber) {
    // Mock data cho nhân viên
    const housekeepers = [
        'Mai Thị Hoa',
        'Lê Thị Lan', 
        'Trần Thị Mai',
        'Nguyễn Thị Hương',
        'Phạm Thị Linh'
    ];
    
    const selectedHousekeeper = housekeepers[Math.floor(Math.random() * housekeepers.length)];
    
    // Update room status to cleaning
    updateRoomStatus(roomNumber, 'cleaning');
    
    // Update room content with assigned housekeeper
    const roomCard = document.querySelector(`[data-room="${roomNumber}"]`);
    const housekeeperDiv = roomCard.querySelector('.room-housekeeper');
    if (housekeeperDiv) {
        housekeeperDiv.textContent = `Nhân viên: ${selectedHousekeeper}`;
    }
    
    showNotification(`Đã giao việc dọn phòng ${roomNumber} cho ${selectedHousekeeper}`, 'success');
}

function completeTask(roomNumber) {
    // Update room status to available
    updateRoomStatus(roomNumber, 'available');
    
    showNotification(`Hoàn thành dọn phòng ${roomNumber}`, 'success');
}

function initializeBulkActions() {
    document.getElementById('bulk-cleaning').addEventListener('click', function() {
        // Get selected rooms (this would be implemented with checkboxes)
        showNotification('Chức năng bulk actions đang được phát triển', 'info');
    });
    
    document.getElementById('bulk-available').addEventListener('click', function() {
        showNotification('Chức năng bulk actions đang được phát triển', 'info');
    });
    
    document.getElementById('bulk-maintenance').addEventListener('click', function() {
        showNotification('Chức năng bulk actions đang được phát triển', 'info');
    });
}

// Notification function
function showNotification(message, type = 'info') {
    // Tái sử dụng function từ dashboard.js
    const notification = document.createElement('div');
    notification.className = `notification notification-${type}`;
    notification.innerHTML = `
        <div class="notification-content">
            <i class="fas fa-${getNotificationIcon(type)}"></i>
            <span>${message}</span>
        </div>
        <button class="notification-close">&times;</button>
    `;
    
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: ${getNotificationColor(type)};
        color: white;
        padding: 1rem 1.5rem;
        border-radius: 8px;
        box-shadow: 0 4px 20px rgba(0,0,0,0.15);
        z-index: 10000;
        display: flex;
        align-items: center;
        justify-content: space-between;
        min-width: 300px;
        transform: translateX(100%);
        transition: transform 0.3s ease;
    `;
    
    document.body.appendChild(notification);
    
    setTimeout(() => {
        notification.style.transform = 'translateX(0)';
    }, 100);
    
    const closeBtn = notification.querySelector('.notification-close');
    closeBtn.addEventListener('click', () => {
        closeNotification(notification);
    });
    
    setTimeout(() => {
        closeNotification(notification);
    }, 5000);
}

function closeNotification(notification) {
    notification.style.transform = 'translateX(100%)';
    setTimeout(() => {
        if (notification.parentNode) {
            notification.parentNode.removeChild(notification);
        }
    }, 300);
}

function getNotificationIcon(type) {
    switch(type) {
        case 'success': return 'check-circle';
        case 'error': return 'exclamation-triangle';
        case 'warning': return 'exclamation-circle';
        default: return 'info-circle';
    }
}

function getNotificationColor(type) {
    switch(type) {
        case 'success': return '#27ae60';
        case 'error': return '#e74c3c';
        case 'warning': return '#f39c12';
        default: return '#3498db';
    }
}