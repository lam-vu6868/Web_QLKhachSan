// Quản Lý Đặt Phòng - JavaScript (Cleaned Version)

$(document).ready(function() {
    // Initialize AOS animations
    AOS.init({
        duration: 800,
        easing: 'ease-in-out',
        once: true
    });

    // Initialize event listeners
    initializeEventListeners();
    
    // Initialize calendar
    initializeCalendar();
    
    // Load bookings data
    loadBookingsData();
});

// Initialize Event Listeners
function initializeEventListeners() {
    // Sidebar toggle
    $('.sidebar-toggle').click(function() {
        $('.sidebar').toggleClass('collapsed');
        $('.main-content').toggleClass('expanded');
    });

    // Logout button
    $('.logout-btn').click(function() {
        if (confirm('Bạn có chắc chắn muốn đăng xuất?')) {
            window.location.href = '../dang-nhap.html';
        }
    });

    // New booking button
    $('#new-booking-btn').click(function() {
        $('#newBookingModal').modal('show');
        resetBookingForm();
    });

    // View toggle buttons
    $('#calendar-view-btn').click(function() {
        showCalendarView();
    });

    $('#list-view-btn').click(function() {
        showListView();
    });

    // Filter buttons
    $('#apply-filters-btn').click(function() {
        applyFilters();
    });

    $('#clear-filters-btn').click(function() {
        clearFilters();
    });

    // Export buttons
    $('#export-bookings-btn').click(function() {
        exportBookings();
    });

    // Create booking button
    $('#create-booking-btn').click(function() {
        createBooking();
    });

    // Action buttons
    $(document).on('click', '.btn-checkin', function() {
        const bookingId = $(this).data('booking-id');
        handleCheckIn(bookingId);
    });

    $(document).on('click', '.btn-checkout', function() {
        const bookingId = $(this).data('booking-id');
        handleCheckOut(bookingId);
    });

    $(document).on('click', '.btn-edit', function() {
        const bookingId = $(this).data('booking-id');
        editBooking(bookingId);
    });

    $(document).on('click', '.btn-cancel', function() {
        const bookingId = $(this).data('booking-id');
        cancelBooking(bookingId);
    });

    // Calendar navigation
    $('#prev-month').click(function() {
        navigateCalendar(-1);
    });

    $('#next-month').click(function() {
        navigateCalendar(1);
    });

    // Form calculations
    $(document).on('change', '#room-type, #checkin-date, #checkout-date, #guest-count', function() {
        calculateBookingTotal();
    });
}

// Calendar Functions
function initializeCalendar() {
    const currentDate = new Date();
    displayCalendar(currentDate);
}

function displayCalendar(date) {
    const year = date.getFullYear();
    const month = date.getMonth();
    
    // Update month/year display
    const monthNames = [
        'Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6',
        'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'
    ];
    
    $('#current-month-year').text(`${monthNames[month]} ${year}`);
    
    // Generate calendar grid
    generateCalendarGrid(year, month);
}

function generateCalendarGrid(year, month) {
    const firstDay = new Date(year, month, 1);
    const lastDay = new Date(year, month + 1, 0);
    const startDate = new Date(firstDay);
    startDate.setDate(startDate.getDate() - firstDay.getDay());
    
    let calendarHTML = '';
    const today = new Date();
    
    for (let week = 0; week < 6; week++) {
        calendarHTML += '<tr>';
        
        for (let day = 0; day < 7; day++) {
            const currentDate = new Date(startDate);
            currentDate.setDate(startDate.getDate() + (week * 7) + day);
            
            const isCurrentMonth = currentDate.getMonth() === month;
            const isToday = currentDate.toDateString() === today.toDateString();
            const hasBookings = Math.random() > 0.7; // Random bookings for demo
            
            let cellClass = 'calendar-day';
            if (!isCurrentMonth) cellClass += ' other-month';
            if (isToday) cellClass += ' today';
            if (hasBookings) cellClass += ' has-bookings';
            
            calendarHTML += `
                <td class="${cellClass}" data-date="${currentDate.toISOString().split('T')[0]}">
                    <div class="day-number">${currentDate.getDate()}</div>
                    ${hasBookings ? '<div class="booking-indicator">3</div>' : ''}
                </td>
            `;
        }
        
        calendarHTML += '</tr>';
    }
    
    $('#calendar-grid').html(calendarHTML);
}

function navigateCalendar(direction) {
    const currentMonth = $('#current-month-year').text();
    // Logic to navigate calendar months
    console.log(`Navigate calendar ${direction > 0 ? 'next' : 'previous'} month`);
}

// View Functions
function showCalendarView() {
    $('#calendar-view').show();
    $('#list-view').hide();
    $('#calendar-view-btn').addClass('active');
    $('#list-view-btn').removeClass('active');
}

function showListView() {
    $('#calendar-view').hide();
    $('#list-view').show();
    $('#calendar-view-btn').removeClass('active');
    $('#list-view-btn').addClass('active');
}

// Data Loading
function loadBookingsData() {
    // Simulate loading bookings data
    const sampleBookings = [
        {
            id: 'SH20250001',
            guestName: 'Nguyễn Văn An',
            roomType: 'Suite',
            checkIn: '2025-09-27',
            checkOut: '2025-09-29',
            guests: 2,
            status: 'confirmed',
            total: 2500
        },
        {
            id: 'SH20250002',
            guestName: 'Trần Thị Bình',
            roomType: 'Deluxe',
            checkIn: '2025-09-28',
            checkOut: '2025-09-30',
            guests: 1,
            status: 'pending',
            total: 1800
        }
    ];
    
    displayBookings(sampleBookings);
}

function displayBookings(bookings) {
    let html = '';
    
    bookings.forEach(booking => {
        const statusClass = getStatusClass(booking.status);
        const statusText = getStatusText(booking.status);
        
        html += `
            <tr>
                <td><strong>${booking.id}</strong></td>
                <td>${booking.guestName}</td>
                <td>${booking.roomType}</td>
                <td>${formatDate(booking.checkIn)}</td>
                <td>${formatDate(booking.checkOut)}</td>
                <td>${booking.guests}</td>
                <td><span class="status-badge ${statusClass}">${statusText}</span></td>
                <td>$${booking.total}</td>
                <td>
                    <div class="action-buttons">
                        <button class="btn btn-sm btn-outline-primary btn-checkin" data-booking-id="${booking.id}">
                            <i class="fas fa-sign-in-alt"></i>
                        </button>
                        <button class="btn btn-sm btn-outline-success btn-checkout" data-booking-id="${booking.id}">
                            <i class="fas fa-sign-out-alt"></i>
                        </button>
                        <button class="btn btn-sm btn-outline-warning btn-edit" data-booking-id="${booking.id}">
                            <i class="fas fa-edit"></i>
                        </button>
                        <button class="btn btn-sm btn-outline-danger btn-cancel" data-booking-id="${booking.id}">
                            <i class="fas fa-times"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `;
    });
    
    $('#bookings-table tbody').html(html);
}

// Filter Functions
function applyFilters() {
    const status = $('#status-filter').val();
    const dateFrom = $('#date-from').val();
    const dateTo = $('#date-to').val();
    const roomType = $('#room-type-filter').val();
    
    console.log('Applying filters:', { status, dateFrom, dateTo, roomType });
    
    // Filter logic would go here
    // For now, just reload data
    loadBookingsData();
}

function clearFilters() {
    $('#status-filter').val('');
    $('#date-from').val('');
    $('#date-to').val('');
    $('#room-type-filter').val('');
    
    loadBookingsData();
}

// Export Functions
function exportBookings() {
    console.log('Exporting bookings to Excel...');
    
    // Simulate file download
    const link = document.createElement('a');
    link.href = 'data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet,';
    link.download = 'danh-sach-dat-phong-' + new Date().toISOString().slice(0, 10) + '.xlsx';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

// Booking Actions
function handleCheckIn(bookingId) {
    if (confirm(`Xác nhận check-in cho đặt phòng ${bookingId}?`)) {
        console.log(`Check-in booking ${bookingId}`);
        // Update booking status
        loadBookingsData();
    }
}

function handleCheckOut(bookingId) {
    if (confirm(`Xác nhận check-out cho đặt phòng ${bookingId}?`)) {
        console.log(`Check-out booking ${bookingId}`);
        // Update booking status
        loadBookingsData();
    }
}

function editBooking(bookingId) {
    console.log(`Edit booking ${bookingId}`);
    // Open edit modal with booking data
    $('#newBookingModal').modal('show');
}

function cancelBooking(bookingId) {
    if (confirm(`Bạn có chắc chắn muốn hủy đặt phòng ${bookingId}?`)) {
        console.log(`Cancel booking ${bookingId}`);
        // Update booking status
        loadBookingsData();
    }
}

// Form Functions
function resetBookingForm() {
    $('#new-booking-form')[0].reset();
    $('#total-amount').text('$0');
    
    // Set default dates
    const today = new Date();
    const tomorrow = new Date(today);
    tomorrow.setDate(tomorrow.getDate() + 1);
    
    $('#checkin-date').val(today.toISOString().split('T')[0]);
    $('#checkout-date').val(tomorrow.toISOString().split('T')[0]);
    
    calculateBookingTotal();
}

function createBooking() {
    const formData = new FormData($('#new-booking-form')[0]);
    
    // Simple validation
    const requiredFields = ['guest_name', 'guest_phone', 'guest_email', 'room_type', 'checkin_date', 'checkout_date'];
    let isValid = true;
    
    requiredFields.forEach(field => {
        const value = formData.get(field);
        if (!value || value.trim() === '') {
            isValid = false;
        }
    });
    
    if (!isValid) {
        alert('Vui lòng điền đầy đủ thông tin bắt buộc');
        return;
    }
    
    console.log('Creating new booking...');
    
    // Close modal and reload data
    $('#newBookingModal').modal('hide');
    loadBookingsData();
}

function calculateBookingTotal() {
    const roomType = $('#room-type').val();
    const checkIn = new Date($('#checkin-date').val());
    const checkOut = new Date($('#checkout-date').val());
    const guests = parseInt($('#guest-count').val()) || 1;
    
    if (!roomType || !checkIn || !checkOut || checkOut <= checkIn) {
        $('#total-amount').text('$0');
        return;
    }
    
    const nights = Math.ceil((checkOut - checkIn) / (1000 * 60 * 60 * 24));
    
    const roomPrices = {
        'standard': 200,
        'deluxe': 350,
        'suite': 600,
        'penthouse': 1200
    };
    
    const basePrice = roomPrices[roomType] || 200;
    const total = basePrice * nights;
    
    $('#total-amount').text(`$${total}`);
}

// Utility Functions
function getStatusClass(status) {
    const statusClasses = {
        'confirmed': 'status-confirmed',
        'pending': 'status-pending',
        'checked-in': 'status-checked-in',
        'checked-out': 'status-checked-out',
        'cancelled': 'status-cancelled'
    };
    return statusClasses[status] || 'status-pending';
}

function getStatusText(status) {
    const statusTexts = {
        'confirmed': 'Đã xác nhận',
        'pending': 'Chờ xác nhận',
        'checked-in': 'Đã check-in',
        'checked-out': 'Đã check-out',
        'cancelled': 'Đã hủy'
    };
    return statusTexts[status] || 'Chờ xác nhận';
}

function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('vi-VN');
}

// Responsive sidebar handling
function handleResponsiveSidebar() {
    if ($(window).width() <= 768) {
        $('.sidebar').addClass('collapsed');
        $('.main-content').addClass('expanded');
    }
}

// Window resize handler
$(window).resize(function() {
    handleResponsiveSidebar();
});

// Initial responsive check
handleResponsiveSidebar();