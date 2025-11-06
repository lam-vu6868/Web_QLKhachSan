// Quản Lý Đặt Phòng - JavaScript (Cleaned Version)
document.addEventListener('DOMContentLoaded', function() {
    console.log('Booking Management System Loaded');
    
    // Initialize the booking management system
    initBookingManagement();
});

function initBookingManagement() {
    initSearch();
    initFilters();
    initModals();
    initCalendar();
    initBookingActions();
    initTableActions();
}

// Search functionality
function initSearch() {
    const searchInput = document.getElementById('booking-search');
    if (searchInput) {
        let searchTimeout;
        searchInput.addEventListener('input', function() {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(() => {
                const searchTerm = this.value.toLowerCase();
                filterBookings();
            }, 300);
        });
    }
}

// Filter functionality
function initFilters() {
    const filters = ['status-filter', 'date-from', 'date-to', 'room-type-filter'];
    
    filters.forEach(filterId => {
        const filterElement = document.getElementById(filterId);
        if (filterElement) {
            filterElement.addEventListener('change', filterBookings);
        }
    });

    // Clear filters button
    const clearFiltersBtn = document.getElementById('clear-filters');
    if (clearFiltersBtn) {
        clearFiltersBtn.addEventListener('click', clearAllFilters);
    }
}

function filterBookings() {
    const searchTerm = document.getElementById('booking-search').value.toLowerCase();
    const statusFilter = document.getElementById('status-filter').value;
    const dateFrom = document.getElementById('date-from').value;
    const dateTo = document.getElementById('date-to').value;
    const roomTypeFilter = document.getElementById('room-type-filter').value;
    
    const tableRows = document.querySelectorAll('#bookings-table tbody tr');
    
    tableRows.forEach(row => {
        let showRow = true;
        
        // Search filter
        if (searchTerm) {
            const bookingCode = row.querySelector('.booking-code').textContent.toLowerCase();
            const guestName = row.querySelector('.guest-info strong').textContent.toLowerCase();
            const guestPhone = row.querySelector('.guest-info small').textContent.toLowerCase();
            
            if (!bookingCode.includes(searchTerm) && 
                !guestName.includes(searchTerm) && 
                !guestPhone.includes(searchTerm)) {
                showRow = false;
            }
        }
        
        // Status filter
        if (statusFilter && showRow) {
            const statusBadge = row.querySelector('.status-badge');
            if (!statusBadge.classList.contains(statusFilter)) {
                showRow = false;
            }
        }
        
        // Date filters (simplified - would need proper date parsing in real app)
        // Room type filter would also need proper implementation
        
        row.style.display = showRow ? '' : 'none';
    });
    
    updateTableStats();
}

function clearAllFilters() {
    document.getElementById('booking-search').value = '';
    document.getElementById('status-filter').value = '';
    document.getElementById('date-from').value = '';
    document.getElementById('date-to').value = '';
    document.getElementById('room-type-filter').value = '';
    
    filterBookings();
}

function updateTableStats() {
    const visibleRows = document.querySelectorAll('#bookings-table tbody tr[style=""]').length;
    const totalRows = document.querySelectorAll('#bookings-table tbody tr').length;
    
    const paginationInfo = document.querySelector('.pagination-info');
    if (paginationInfo) {
        paginationInfo.textContent = `Hiển thị ${visibleRows} của ${totalRows} kết quả`;
    }
}

// Modal functionality
function initModals() {
    const newBookingBtn = document.getElementById('new-booking-btn');
    const newBookingModal = document.getElementById('newBookingModal');
    const createBookingBtn = document.getElementById('create-booking-btn');
    
    if (newBookingBtn && newBookingModal) {
        newBookingBtn.addEventListener('click', function() {
            const modal = new bootstrap.Modal(newBookingModal);
            modal.show();
            initBookingForm();
        });
    }
    
    if (createBookingBtn) {
        createBookingBtn.addEventListener('click', createNewBooking);
    }
}

function initBookingForm() {
    const form = document.getElementById('new-booking-form');
    const checkinInput = form.querySelector('[name="checkin_date"]');
    const checkoutInput = form.querySelector('[name="checkout_date"]');
    const roomTypeSelect = form.querySelector('[name="room_type"]');
    const totalAmountElement = document.getElementById('total-amount');
    
    // Set minimum dates
    const today = new Date().toISOString().split('T')[0];
    checkinInput.min = today;
    
    checkinInput.addEventListener('change', function() {
        const checkinDate = new Date(this.value);
        const minCheckout = new Date(checkinDate);
        minCheckout.setDate(minCheckout.getDate() + 1);
        checkoutInput.min = minCheckout.toISOString().split('T')[0];
        calculateTotal();
    });
    
    checkoutInput.addEventListener('change', calculateTotal);
    roomTypeSelect.addEventListener('change', calculateTotal);
    
    function calculateTotal() {
        const checkinDate = new Date(checkinInput.value);
        const checkoutDate = new Date(checkoutInput.value);
        const roomType = roomTypeSelect.value;
        
        if (checkinDate && checkoutDate && roomType && checkinDate < checkoutDate) {
            const nights = Math.ceil((checkoutDate - checkinDate) / (1000 * 60 * 60 * 24));
            
            const roomPrices = {
                'deluxe': 150,
                'executive': 250,
                'family': 320
            };
            
            const pricePerNight = roomPrices[roomType] || 0;
            const total = nights * pricePerNight;
            
            totalAmountElement.textContent = `$${total}`;
        } else {
            totalAmountElement.textContent = '$0';
        }
    }
}

function createNewBooking() {
    const form = document.getElementById('new-booking-form');
    const formData = new FormData(form);
    
    // Validate required fields
    const requiredFields = ['guest_name', 'guest_phone', 'checkin_date', 'checkout_date', 'room_type'];
    let isValid = true;
    
    requiredFields.forEach(field => {
        const input = form.querySelector(`[name="${field}"]`);
        if (!input.value.trim()) {
            input.classList.add('is-invalid');
            isValid = false;
        } else {
            input.classList.remove('is-invalid');
        }
    });
    
    if (!isValid) {
        return;
    }
    
    // Simulate booking creation
    showLoadingState(true);
    
    setTimeout(() => {
        const bookingCode = generateBookingCode();
        
        // Add new booking to table (in real app, this would be an API call)
        addBookingToTable({
            code: bookingCode,
            guestName: formData.get('guest_name'),
            guestPhone: formData.get('guest_phone'),
            checkinDate: formData.get('checkin_date'),
            checkoutDate: formData.get('checkout_date'),
            roomType: formData.get('room_type'),
            guests: formData.get('guests'),
            totalAmount: document.getElementById('total-amount').textContent
        });
        
        showLoadingState(false);
        
        // Close modal
        const modal = bootstrap.Modal.getInstance(document.getElementById('newBookingModal'));
        modal.hide();
        
        // Reset form
        form.reset();
        document.getElementById('total-amount').textContent = '$0';
        
        // Update stats
        updateBookingStats();
        
    }, 2000);
}

function generateBookingCode() {
    const year = new Date().getFullYear();
    const random = Math.floor(Math.random() * 10000).toString().padStart(4, '0');
    return `#SH${year}${random}`;
}

function addBookingToTable(bookingData) {
    const tableBody = document.querySelector('#bookings-table tbody');
    const roomTypeNames = {
        'deluxe': 'Deluxe Ocean View',
        'executive': 'Executive Suite',
        'family': 'Family Bungalow'
    };
    
    const row = document.createElement('tr');
    row.innerHTML = `
        <td><span class="booking-code">${bookingData.code}</span></td>
        <td>
            <div class="guest-info">
                <strong>${bookingData.guestName}</strong>
                <small>${bookingData.guestPhone}</small>
            </div>
        </td>
        <td>
            <div class="room-info">
                <strong>---</strong>
                <small>${roomTypeNames[bookingData.roomType]}</small>
            </div>
        </td>
        <td>${formatDate(bookingData.checkinDate)}</td>
        <td>${formatDate(bookingData.checkoutDate)}</td>
        <td>${calculateNights(bookingData.checkinDate, bookingData.checkoutDate)}</td>
        <td><strong>${bookingData.totalAmount}</strong></td>
        <td><span class="status-badge confirmed">Đã xác nhận</span></td>
        <td>
            <div class="action-buttons">
                <button class="btn btn-sm btn-outline-primary" title="Xem chi tiết">
                    <i class="fas fa-eye"></i>
                </button>
                <button class="btn btn-sm btn-outline-warning" title="Chỉnh sửa">
                    <i class="fas fa-edit"></i>
                </button>
                <button class="btn btn-sm btn-outline-success" title="Check-in">
                    <i class="fas fa-sign-in-alt"></i>
                </button>
            </div>
        </td>
    `;
    
    // Insert at the top of the table
    tableBody.insertBefore(row, tableBody.firstChild);
    
    // Add event listeners to new buttons
    initRowActions(row);
}

function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('vi-VN');
}

function calculateNights(checkin, checkout) {
    const checkinDate = new Date(checkin);
    const checkoutDate = new Date(checkout);
    return Math.ceil((checkoutDate - checkinDate) / (1000 * 60 * 60 * 24));
}

// Booking actions
function initBookingActions() {
    // Initialize actions for existing rows
    const tableRows = document.querySelectorAll('#bookings-table tbody tr');
    tableRows.forEach(row => initRowActions(row));
}

function initRowActions(row) {
    const viewBtn = row.querySelector('.btn-outline-primary');
    const editBtn = row.querySelector('.btn-outline-warning');
    const checkinBtn = row.querySelector('.btn-outline-success');
    const checkoutBtn = row.querySelector('.btn-outline-danger');
    const cancelBtn = row.querySelector('.btn-outline-secondary');
    
    if (viewBtn) {
        viewBtn.addEventListener('click', () => viewBookingDetails(row));
    }
    
    if (editBtn) {
        editBtn.addEventListener('click', () => editBooking(row));
    }
    
    if (checkinBtn) {
        checkinBtn.addEventListener('click', () => checkInBooking(row));
    }
    
    if (checkoutBtn) {
        checkoutBtn.addEventListener('click', () => checkOutBooking(row));
    }
}

function viewBookingDetails(row) {
    const bookingCode = row.querySelector('.booking-code').textContent;
    const guestName = row.querySelector('.guest-info strong').textContent;
    const guestPhone = row.querySelector('.guest-info small').textContent;
    const roomInfo = row.querySelector('.room-info strong').textContent;
    const roomType = row.querySelector('.room-info small').textContent;
    const checkinDate = row.cells[3].textContent;
    const checkoutDate = row.cells[4].textContent;
    const nights = row.cells[5].textContent;
    const totalAmount = row.cells[6].textContent;
    const status = row.querySelector('.status-badge').textContent;
    
    const modalContent = `
        <div class="booking-detail-section">
            <h6>Thông tin đặt phòng</h6>
            <div class="detail-row">
                <span class="detail-label">Mã đặt phòng:</span>
                <span class="detail-value">${bookingCode}</span>
            </div>
            <div class="detail-row">
                <span class="detail-label">Trạng thái:</span>
                <span class="detail-value">${status}</span>
            </div>
            <div class="detail-row">
                <span class="detail-label">Ngày nhận phòng:</span>
                <span class="detail-value">${checkinDate}</span>
            </div>
            <div class="detail-row">
                <span class="detail-label">Ngày trả phòng:</span>
                <span class="detail-value">${checkoutDate}</span>
            </div>
            <div class="detail-row">
                <span class="detail-label">Số đêm:</span>
                <span class="detail-value">${nights}</span>
            </div>
        </div>
        
        <div class="booking-detail-section">
            <h6>Thông tin khách hàng</h6>
            <div class="detail-row">
                <span class="detail-label">Họ và tên:</span>
                <span class="detail-value">${guestName}</span>
            </div>
            <div class="detail-row">
                <span class="detail-label">Số điện thoại:</span>
                <span class="detail-value">${guestPhone}</span>
            </div>
        </div>
        
        <div class="booking-detail-section">
            <h6>Thông tin phòng</h6>
            <div class="detail-row">
                <span class="detail-label">Số phòng:</span>
                <span class="detail-value">${roomInfo}</span>
            </div>
            <div class="detail-row">
                <span class="detail-label">Loại phòng:</span>
                <span class="detail-value">${roomType}</span>
            </div>
        </div>
        
        <div class="booking-detail-section">
            <h6>Dịch vụ đã sử dụng</h6>
            <div class="service-item">
                <div class="service-info">
                    <div class="service-name">Spa & Massage</div>
                    <div class="service-details">2 giờ - Ngày 26/09/2025</div>
                </div>
                <div class="service-amount">$120</div>
            </div>
            <div class="service-item">
                <div class="service-info">
                    <div class="service-name">Ăn tối tại nhà hàng</div>
                    <div class="service-details">Set menu 2 người - Ngày 26/09/2025</div>
                </div>
                <div class="service-amount">$85</div>
            </div>
        </div>
        
        <div class="booking-detail-section">
            <h6>Tổng thanh toán</h6>
            <div class="detail-row">
                <span class="detail-label">Tiền phòng:</span>
                <span class="detail-value">${totalAmount}</span>
            </div>
            <div class="detail-row">
                <span class="detail-label">Dịch vụ:</span>
                <span class="detail-value">$205</span>
            </div>
            <div class="detail-row" style="border-top: 2px solid #e9ecef; padding-top: 1rem; margin-top: 1rem;">
                <span class="detail-label"><strong>Tổng cộng:</strong></span>
                <span class="detail-value" style="color: var(--accent-gold); font-size: 1.25rem;"><strong>$505</strong></span>
            </div>
        </div>
    `;
    
    const modal = document.getElementById('bookingDetailModal');
    const modalContent_el = document.getElementById('bookingDetailContent');
    modalContent_el.innerHTML = modalContent;
    
    const bootstrapModal = new bootstrap.Modal(modal);
    bootstrapModal.show();
}

function checkInBooking(row) {
    const bookingCode = row.querySelector('.booking-code').textContent;
    const statusBadge = row.querySelector('.status-badge');
    const actionButtons = row.querySelector('.action-buttons');
    
    if (confirm(`Xác nhận check-in cho đặt phòng ${bookingCode}?`)) {
        showLoadingState(true);
        
        setTimeout(() => {
            statusBadge.className = 'status-badge checked-in';
            statusBadge.textContent = 'Đã check-in';
            
            // Update action buttons
            actionButtons.innerHTML = `
                <button class="btn btn-sm btn-outline-primary" title="Xem chi tiết">
                    <i class="fas fa-eye"></i>
                </button>
                <button class="btn btn-sm btn-outline-info" title="Dịch vụ">
                    <i class="fas fa-concierge-bell"></i>
                </button>
                <button class="btn btn-sm btn-outline-danger" title="Check-out">
                    <i class="fas fa-sign-out-alt"></i>
                </button>
            `;
            
            initRowActions(row);
            
            showLoadingState(false);
            showNotification(`Check-in thành công cho ${bookingCode}`, 'success');
            updateBookingStats();
        }, 1500);
    }
}

function checkOutBooking(row) {
    const bookingCode = row.querySelector('.booking-code').textContent;
    const statusBadge = row.querySelector('.status-badge');
    const actionButtons = row.querySelector('.action-buttons');
    
    if (confirm(`Xác nhận check-out cho đặt phòng ${bookingCode}?`)) {
        showLoadingState(true);
        
        setTimeout(() => {
            statusBadge.className = 'status-badge checked-out';
            statusBadge.textContent = 'Đã check-out';
            
            // Update action buttons
            actionButtons.innerHTML = `
                <button class="btn btn-sm btn-outline-primary" title="Xem chi tiết">
                    <i class="fas fa-eye"></i>
                </button>
                <button class="btn btn-sm btn-outline-secondary" title="Hóa đơn">
                    <i class="fas fa-receipt"></i>
                </button>
                <button class="btn btn-sm btn-outline-info" title="Đánh giá">
                    <i class="fas fa-star"></i>
                </button>
            `;
            
            initRowActions(row);
            
            showLoadingState(false);
            showNotification(`Check-out thành công cho ${bookingCode}`, 'success');
            updateBookingStats();
        }, 1500);
    }
}

// Table actions
function initTableActions() {
    const exportBtn = document.getElementById('export-btn');
    const refreshBtn = document.getElementById('refresh-btn');
    
    if (exportBtn) {
        exportBtn.addEventListener('click', exportBookings);
    }
    
    if (refreshBtn) {
        refreshBtn.addEventListener('click', refreshBookings);
    }
}

function exportBookings() {
    showNotification('Đang xuất dữ liệu...', 'info');
    
    setTimeout(() => {
        // Simulate export
        showNotification('Đã xuất danh sách đặt phòng thành công', 'success');
    }, 2000);
}

function refreshBookings() {
    showLoadingState(true);
    
    setTimeout(() => {
        showLoadingState(false);
        showNotification('Đã làm mới danh sách đặt phòng', 'success');
        updateBookingStats();
    }, 1500);
}

// Calendar functionality
function initCalendar() {
    const calendarGrid = document.getElementById('calendar-grid');
    const currentMonthElement = document.getElementById('current-month');
    const prevMonthBtn = document.getElementById('prev-month');
    const nextMonthBtn = document.getElementById('next-month');
    
    let currentDate = new Date();
    
    function renderCalendar(date) {
        const year = date.getFullYear();
        const month = date.getMonth();
        
        // Update month display
        currentMonthElement.textContent = `Tháng ${month + 1}, ${year}`;
        
        // Create calendar structure
        calendarGrid.innerHTML = `
            <div class="calendar-weekdays">
                <div class="calendar-weekday">CN</div>
                <div class="calendar-weekday">T2</div>
                <div class="calendar-weekday">T3</div>
                <div class="calendar-weekday">T4</div>
                <div class="calendar-weekday">T5</div>
                <div class="calendar-weekday">T6</div>
                <div class="calendar-weekday">T7</div>
            </div>
            <div class="calendar-days" id="calendar-days"></div>
        `;
        
        const calendarDays = document.getElementById('calendar-days');
        const firstDay = new Date(year, month, 1).getDay();
        const daysInMonth = new Date(year, month + 1, 0).getDate();
        const today = new Date();
        
        // Add empty cells for days before month starts
        for (let i = 0; i < firstDay; i++) {
            const dayCell = document.createElement('div');
            dayCell.className = 'calendar-day other-month';
            calendarDays.appendChild(dayCell);
        }
        
        // Add days of the month
        for (let day = 1; day <= daysInMonth; day++) {
            const dayCell = document.createElement('div');
            dayCell.className = 'calendar-day';
            
            if (year === today.getFullYear() && month === today.getMonth() && day === today.getDate()) {
                dayCell.classList.add('today');
            }
            
            // Simulate booking data
            const bookingCount = Math.floor(Math.random() * 4);
            let bookingDots = '';
            for (let i = 0; i < bookingCount; i++) {
                bookingDots += '<span class="calendar-booking-dot"></span>';
            }
            
            dayCell.innerHTML = `
                <div class="calendar-day-number">${day}</div>
                <div class="calendar-bookings">
                    ${bookingDots}
                    ${bookingCount > 0 ? `<small>${bookingCount} đặt phòng</small>` : ''}
                </div>
            `;
            
            calendarDays.appendChild(dayCell);
        }
    }
    
    if (prevMonthBtn) {
        prevMonthBtn.addEventListener('click', () => {
            currentDate.setMonth(currentDate.getMonth() - 1);
            renderCalendar(currentDate);
        });
    }
    
    if (nextMonthBtn) {
        nextMonthBtn.addEventListener('click', () => {
            currentDate.setMonth(currentDate.getMonth() + 1);
            renderCalendar(currentDate);
        });
    }
    
    // Initial render
    renderCalendar(currentDate);
}

// Update booking statistics
function updateBookingStats() {
    const statCards = document.querySelectorAll('.stat-card');
    
    statCards.forEach(card => {
        const currentValue = parseInt(card.querySelector('h3').textContent);
        const newValue = currentValue + Math.floor(Math.random() * 3) - 1; // Random change
        
        if (newValue >= 0) {
            card.querySelector('h3').textContent = newValue;
        }
    });
}

// Utility functions
function showLoadingState(show) {
    const mainContent = document.querySelector('.dashboard-main');
    if (show) {
        mainContent.classList.add('loading');
    } else {
        mainContent.classList.remove('loading');
    }
}

function showNotification(message, type = 'info') {
    // Create notification element
    const notification = document.createElement('div');
    notification.className = `alert alert-${type === 'error' ? 'danger' : type} alert-dismissible fade show notification`;
    notification.style.position = 'fixed';
    notification.style.top = '20px';
    notification.style.right = '20px';
    notification.style.zIndex = '9999';
    notification.style.minWidth = '300px';
    
    notification.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    
    document.body.appendChild(notification);
    
    // Auto remove after 5 seconds
    setTimeout(() => {
        if (notification.parentNode) {
            notification.remove();
        }
    }, 5000);
}

// Sidebar functionality (if not included in dashboard.js)
function initSidebar() {
    const sidebarToggle = document.querySelector('.sidebar-toggle');
    const sidebar = document.querySelector('.sidebar');
    
    if (sidebarToggle && sidebar) {
        sidebarToggle.addEventListener('click', function() {
            sidebar.classList.toggle('collapsed');
        });
    }
}

// Initialize sidebar if needed
document.addEventListener('DOMContentLoaded', function() {
    initSidebar();
});