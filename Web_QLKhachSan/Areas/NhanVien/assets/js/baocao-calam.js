// Báo Cáo Ca Làm - JavaScript (Cleaned Version)

$(document).ready(function() {
    // Initialize AOS animations
    AOS.init({
        duration: 800,
        easing: 'ease-in-out',
        once: true
    });

    // Shift timer functionality
    startShiftTimer();
    
    // Initialize event listeners
    initializeEventListeners();
    
    // Load reports data
    loadReportsData();
    
    // Update chart data
    updatePerformanceChart();
});

// Shift Timer
function startShiftTimer() {
    const timerElement = $('#shift-timer');
    const shiftStartTime = new Date();
    shiftStartTime.setHours(6, 0, 0, 0); // 6:00 AM start time
    
    function updateTimer() {
        const now = new Date();
        const elapsed = now - shiftStartTime;
        
        const hours = Math.floor(elapsed / (1000 * 60 * 60));
        const minutes = Math.floor((elapsed % (1000 * 60 * 60)) / (1000 * 60));
        const seconds = Math.floor((elapsed % (1000 * 60)) / 1000);
        
        const formattedTime = 
            String(hours).padStart(2, '0') + ':' +
            String(minutes).padStart(2, '0') + ':' +
            String(seconds).padStart(2, '0');
            
        timerElement.text(formattedTime);
    }
    
    // Update timer every second
    updateTimer();
    setInterval(updateTimer, 1000);
}

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

    // New report button
    $('#new-report-btn').click(function() {
        $('#newReportModal').modal('show');
        resetNewReportForm();
    });

    // Apply filters button
    $('#apply-filters-btn').click(function() {
        applyReportFilters();
    });

    // Export reports button
    $('#export-reports-btn').click(function() {
        exportReportsToExcel();
    });

    // Print reports button
    $('#print-reports-btn').click(function() {
        printReports();
    });

    // Create report button
    $('#create-report-btn').click(function() {
        createNewReport();
    });

    // Report detail buttons
    $(document).on('click', '.btn-outline-primary', function() {
        if ($(this).find('.fa-eye').length > 0) {
            showReportDetail($(this).closest('tr'));
        }
    });

    // Edit report buttons
    $(document).on('click', '.btn-outline-warning', function() {
        if ($(this).find('.fa-edit').length > 0) {
            editReport($(this).closest('tr'));
        }
    });

    // Approve report buttons
    $(document).on('click', '.btn-outline-success', function() {
        if ($(this).find('.fa-check').length > 0) {
            approveReport($(this).closest('tr'));
        }
    });

    // Form input validation
    $('#new-report-form input, #new-report-form select, #new-report-form textarea').on('input change', function() {
        validateFormField($(this));
    });
}

// Load Reports Data
function loadReportsData() {
    // Simulate loading reports data
    const loadingIndicator = $('<div class="text-center py-4"><i class="fas fa-spinner fa-spin"></i> Đang tải dữ liệu...</div>');
    $('.reports-table tbody').html(loadingIndicator);
    
    setTimeout(function() {
        // Remove loading indicator and show actual data
        $('.reports-table tbody').html(`
            <tr>
                <td>27/09/2025</td>
                <td><span class="shift-badge morning">Ca sáng</span></td>
                <td>
                    <div class="staff-info-compact">
                        <img src="https://randomuser.me/api/portraits/women/44.jpg" alt="Staff" class="staff-avatar-small">
                        <div>
                            <strong>Nguyễn Thị Mai</strong>
                            <small>Lễ tân</small>
                        </div>
                    </div>
                </td>
                <td><span class="number-highlight">24</span></td>
                <td><span class="number-highlight">18</span></td>
                <td><span class="number-highlight">15</span></td>
                <td><span class="revenue-highlight">$12,450</span></td>
                <td>
                    <div class="rating">
                        <i class="fas fa-star"></i>
                        <i class="fas fa-star"></i>
                        <i class="fas fa-star"></i>
                        <i class="fas fa-star"></i>
                        <i class="fas fa-star"></i>
                        <span class="rating-text">Xuất sắc</span>
                    </div>
                </td>
                <td>
                    <div class="action-buttons">
                        <button class="btn btn-sm btn-outline-primary" title="Xem chi tiết">
                            <i class="fas fa-eye"></i>
                        </button>
                        <button class="btn btn-sm btn-outline-warning" title="Chỉnh sửa">
                            <i class="fas fa-edit"></i>
                        </button>
                        <button class="btn btn-sm btn-outline-success" title="Duyệt">
                            <i class="fas fa-check"></i>
                        </button>
                    </div>
                </td>
            </tr>
            <tr>
                <td>26/09/2025</td>
                <td><span class="shift-badge afternoon">Ca chiều</span></td>
                <td>
                    <div class="staff-info-compact">
                        <img src="https://randomuser.me/api/portraits/men/32.jpg" alt="Staff" class="staff-avatar-small">
                        <div>
                            <strong>Trần Văn Đức</strong>
                            <small>Lễ tân</small>
                        </div>
                    </div>
                </td>
                <td><span class="number-highlight">22</span></td>
                <td><span class="number-highlight">25</span></td>
                <td><span class="number-highlight">18</span></td>
                <td><span class="revenue-highlight">$15,680</span></td>
                <td>
                    <div class="rating">
                        <i class="fas fa-star"></i>
                        <i class="fas fa-star"></i>
                        <i class="fas fa-star"></i>
                        <i class="fas fa-star"></i>
                        <i class="far fa-star"></i>
                        <span class="rating-text">Tốt</span>
                    </div>
                </td>
                <td>
                    <div class="action-buttons">
                        <button class="btn btn-sm btn-outline-primary" title="Xem chi tiết">
                            <i class="fas fa-eye"></i>
                        </button>
                        <button class="btn btn-sm btn-outline-secondary" title="Đã duyệt">
                            <i class="fas fa-check-circle"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `);
        
        // Add fade-in animation
        $('.reports-table tbody tr').hide().fadeIn(500);
    }, 1000);
}

// Apply Report Filters
function applyReportFilters() {
    const dateFrom = $('#date-from').val();
    const dateTo = $('#date-to').val();
    const shiftFilter = $('#shift-filter').val();
    const staffFilter = $('#staff-filter').val();
    const departmentFilter = $('#department-filter').val();
    
    // Show loading state
    $('#apply-filters-btn').html('<i class="fas fa-spinner fa-spin"></i> Đang lọc...');
    $('#apply-filters-btn').prop('disabled', true);
    
    // Simulate API call
    setTimeout(function() {
        // Reset button
        $('#apply-filters-btn').html('<i class="fas fa-search"></i> Lọc dữ liệu');
        $('#apply-filters-btn').prop('disabled', false);
        
        // Show success message
        showNotification('Đã áp dụng bộ lọc thành công!', 'success');
        
        // Reload data with filters
        loadReportsData();
    }, 1500);
}

// Export Reports to Excel
function exportReportsToExcel() {
    // Show loading state
    $('#export-reports-btn').html('<i class="fas fa-spinner fa-spin"></i> Đang xuất...');
    $('#export-reports-btn').prop('disabled', true);
    
    // Simulate export process
    setTimeout(function() {
        // Reset button
        $('#export-reports-btn').html('<i class="fas fa-download"></i> Xuất Excel');
        $('#export-reports-btn').prop('disabled', false);
        
        // Show success message
        showNotification('Đã xuất báo cáo Excel thành công!', 'success');
        
        // Simulate file download
        const link = document.createElement('a');
        link.href = 'data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet,';
        link.download = 'bao-cao-ca-lam-' + new Date().toISOString().slice(0, 10) + '.xlsx';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }, 2000);
}

// Print Reports
function printReports() {
    const printContent = `
        <html>
        <head>
            <title>Báo Cáo Ca Làm - The Serene Horizon Hotel</title>
            <style>
                body { font-family: Arial, sans-serif; margin: 20px; }
                .header { text-align: center; margin-bottom: 30px; }
                .header h1 { color: #0a1931; margin-bottom: 10px; }
                .header p { color: #666; }
                table { width: 100%; border-collapse: collapse; margin-top: 20px; }
                th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
                th { background-color: #f8f9fa; font-weight: bold; }
                .shift-badge { padding: 4px 8px; border-radius: 4px; font-size: 12px; }
                .morning { background: #28a745; color: white; }
                .afternoon { background: #ffc107; color: #333; }
                .night { background: #6f42c1; color: white; }
                @media print { body { margin: 0; } }
            </style>
        </head>
        <body>
            <div class="header">
                <h1>BÁO CÁO CA LÀM</h1>
                <p>The Serene Horizon Hotel</p>
                <p>Ngày in: ${new Date().toLocaleDateString('vi-VN')}</p>
            </div>
            ${$('.reports-table')[0].outerHTML}
        </body>
        </html>
    `;
    
    const printWindow = window.open('', '_blank');
    printWindow.document.write(printContent);
    printWindow.document.close();
    printWindow.print();
    printWindow.close();
    
    showNotification('Đã gửi báo cáo đến máy in!', 'success');
}

// Reset New Report Form
function resetNewReportForm() {
    $('#new-report-form')[0].reset();
    $('#new-report-form .form-control, #new-report-form .form-select, #new-report-form textarea')
        .removeClass('is-valid is-invalid');
    
    // Set default values
    const today = new Date().toISOString().slice(0, 10);
    $('input[name="work_date"]').val(today);
    $('select[name="rating"]').val('5');
}

// Create New Report
function createNewReport() {
    if (!validateNewReportForm()) {
        return;
    }
    
    // Show loading state
    $('#create-report-btn').html('<i class="fas fa-spinner fa-spin"></i> Đang tạo...');
    $('#create-report-btn').prop('disabled', true);
    
    // Get form data
    const formData = {
        work_date: $('input[name="work_date"]').val(),
        shift: $('select[name="shift"]').val(),
        staff: $('select[name="staff"]').val(),
        checkin_count: $('input[name="checkin_count"]').val() || 0,
        checkout_count: $('input[name="checkout_count"]').val() || 0,
        service_count: $('input[name="service_count"]').val() || 0,
        revenue: $('input[name="revenue"]').val() || 0,
        rating: $('select[name="rating"]').val(),
        notes: $('textarea[name="notes"]').val()
    };
    
    // Simulate API call
    setTimeout(function() {
        // Reset button
        $('#create-report-btn').html('Tạo báo cáo');
        $('#create-report-btn').prop('disabled', false);
        
        // Hide modal
        $('#newReportModal').modal('hide');
        
        // Show success message
        showNotification('Đã tạo báo cáo ca làm thành công!', 'success');
        
        // Reload reports data
        loadReportsData();
    }, 2000);
}

// Validate New Report Form
function validateNewReportForm() {
    let isValid = true;
    const requiredFields = ['work_date', 'shift', 'staff'];
    
    requiredFields.forEach(function(fieldName) {
        const field = $(`[name="${fieldName}"]`);
        if (!field.val().trim()) {
            field.addClass('is-invalid').removeClass('is-valid');
            isValid = false;
        } else {
            field.addClass('is-valid').removeClass('is-invalid');
        }
    });
    
    if (!isValid) {
        showNotification('Vui lòng điền đầy đủ thông tin bắt buộc!', 'error');
    }
    
    return isValid;
}

// Validate Form Field
function validateFormField(field) {
    const value = field.val().trim();
    const isRequired = field.attr('required') !== undefined;
    
    if (isRequired && !value) {
        field.addClass('is-invalid').removeClass('is-valid');
    } else if (value) {
        field.addClass('is-valid').removeClass('is-invalid');
    } else {
        field.removeClass('is-valid is-invalid');
    }
}

// Show Report Detail
function showReportDetail(row) {
    const date = row.find('td:eq(0)').text();
    const shift = row.find('td:eq(1)').text();
    const staff = row.find('td:eq(2) strong').text();
    const department = row.find('td:eq(2) small').text();
    
    const detailContent = `
        <div class="report-detail">
            <div class="row">
                <div class="col-md-6">
                    <h5>Thông tin ca làm</h5>
                    <table class="table table-borderless">
                        <tr><td><strong>Ngày làm việc:</strong></td><td>${date}</td></tr>
                        <tr><td><strong>Ca làm việc:</strong></td><td>${shift}</td></tr>
                        <tr><td><strong>Nhân viên:</strong></td><td>${staff}</td></tr>
                        <tr><td><strong>Bộ phận:</strong></td><td>${department}</td></tr>
                    </table>
                </div>
                <div class="col-md-6">
                    <h5>Thống kê hoạt động</h5>
                    <table class="table table-borderless">
                        <tr><td><strong>Check-in xử lý:</strong></td><td>24 khách</td></tr>
                        <tr><td><strong>Check-out xử lý:</strong></td><td>18 khách</td></tr>
                        <tr><td><strong>Yêu cầu dịch vụ:</strong></td><td>15 yêu cầu</td></tr>
                        <tr><td><strong>Doanh thu:</strong></td><td>$12,450</td></tr>
                    </table>
                </div>
            </div>
            <div class="row mt-4">
                <div class="col-12">
                    <h5>Ghi chú từ nhân viên</h5>
                    <p class="bg-light p-3 rounded">
                        Ca làm việc diễn ra suôn sẻ. Xử lý thành công tất cả yêu cầu check-in/check-out. 
                        Khách hàng đánh giá tích cực về dịch vụ. Không có sự cố nào xảy ra trong ca làm.
                    </p>
                </div>
            </div>
        </div>
    `;
    
    $('#reportDetailContent').html(detailContent);
    $('#reportDetailModal').modal('show');
}

// Edit Report
function editReport(row) {
    // Get current data from row
    const date = row.find('td:eq(0)').text();
    const staff = row.find('td:eq(2) strong').text();
    
    // Pre-fill form with current data
    $('#newReportModal').modal('show');
    $('#newReportModal .modal-title').text('Chỉnh sửa báo cáo ca làm');
    
    // Set form values (simplified for demo)
    $('input[name="work_date"]').val('2025-09-27');
    $('select[name="staff"]').val('mai');
    $('select[name="shift"]').val('morning');
    $('input[name="checkin_count"]').val('24');
    $('input[name="checkout_count"]').val('18');
    $('input[name="service_count"]').val('15');
    $('input[name="revenue"]').val('12450');
    $('select[name="rating"]').val('5');
    
    // Change button text
    $('#create-report-btn').text('Cập nhật báo cáo');
}

// Approve Report
function approveReport(row) {
    const staff = row.find('td:eq(2) strong').text();
    const date = row.find('td:eq(0)').text();
    
    if (confirm(`Bạn có chắc chắn muốn duyệt báo cáo ca làm của ${staff} ngày ${date}?`)) {
        // Show loading state
        const approveBtn = row.find('.btn-outline-success');
        const originalContent = approveBtn.html();
        approveBtn.html('<i class="fas fa-spinner fa-spin"></i>');
        approveBtn.prop('disabled', true);
        
        setTimeout(function() {
            // Change to approved state
            approveBtn.removeClass('btn-outline-success').addClass('btn-outline-secondary');
            approveBtn.html('<i class="fas fa-check-circle"></i>');
            approveBtn.attr('title', 'Đã duyệt');
            
            showNotification('Đã duyệt báo cáo thành công!', 'success');
        }, 1500);
    }
}

// Update Performance Chart
function updatePerformanceChart() {
    // Animate chart bars on load
    setTimeout(function() {
        $('.chart-day').each(function(index) {
            $(this).delay(index * 100).animate({
                opacity: 1
            }, 500);
            
            $(this).find('.bar').each(function(barIndex) {
                const bar = $(this);
                const originalHeight = bar.css('height');
                bar.css('height', '0%');
                
                setTimeout(function() {
                    bar.animate({
                        height: originalHeight
                    }, 800, 'easeOutBounce');
                }, index * 100 + barIndex * 50);
            });
        });
    }, 500);
}

// Show Notification
function showNotification(message, type = 'info') {
    const alertClass = type === 'success' ? 'alert-success' : 
                      type === 'error' ? 'alert-danger' : 'alert-info';
    
    const notification = $(`
        <div class="alert ${alertClass} alert-dismissible fade show notification-toast" 
             style="position: fixed; top: 20px; right: 20px; z-index: 9999; min-width: 300px;">
            <i class="fas fa-${type === 'success' ? 'check-circle' : type === 'error' ? 'exclamation-circle' : 'info-circle'} me-2"></i>
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `);
    
    $('body').append(notification);
    
    // Auto remove after 5 seconds
    setTimeout(function() {
        notification.fadeOut(500, function() {
            $(this).remove();
        });
    }, 5000);
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