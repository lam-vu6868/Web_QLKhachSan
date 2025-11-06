// Thanh Toán & Hóa Đơn - JavaScript

$(document).ready(function() {
    // Initialize AOS animations
    AOS.init({
        duration: 800,
        easing: 'ease-in-out',
        once: true
    });

    // Initialize event listeners
    initializeEventListeners();
    
    // Load payment data
    loadPaymentData();
    
    // Animate payment method chart
    animatePaymentChart();
    
    // Initialize invoice calculations
    initializeInvoiceCalculations();
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

    // New invoice button
    $('#new-invoice-btn').click(function() {
        $('#newInvoiceModal').modal('show');
        resetInvoiceForm();
    });

    // Quick action buttons
    $('#checkout-payment-btn').click(function() {
        handleCheckoutPayment();
    });

    $('#room-charge-btn').click(function() {
        handleRoomCharge();
    });

    $('#service-charge-btn').click(function() {
        handleServiceCharge();
    });

    $('#refund-btn').click(function() {
        handleRefund();
    });

    // Filter buttons
    $('#filter-invoices-btn').click(function() {
        filterInvoices();
    });

    // Export buttons
    $('#export-invoices-btn').click(function() {
        exportInvoices();
    });

    $('#print-all-invoices-btn').click(function() {
        printAllInvoices();
    });

    // Invoice form buttons
    $('#add-invoice-item').click(function() {
        addInvoiceItem();
    });

    $(document).on('click', '.remove-item', function() {
        removeInvoiceItem($(this));
    });

    $('#create-invoice-btn').click(function() {
        createInvoice();
    });

    $('#save-draft-btn').click(function() {
        saveDraftInvoice();
    });

    // Payment detail buttons
    $(document).on('click', '.btn-outline-primary', function() {
        if ($(this).find('.fa-eye').length > 0) {
            const row = $(this).closest('tr');
            if (row.length > 0) {
                showPaymentDetail(row);
            } else {
                showInvoiceDetail($(this).closest('.invoice-item'));
            }
        }
    });

    // Print invoice buttons
    $(document).on('click', '.btn-outline-success', function() {
        if ($(this).find('.fa-print').length > 0) {
            const row = $(this).closest('tr');
            if (row.length > 0) {
                printPaymentReceipt(row);
            } else {
                printInvoice($(this).closest('.invoice-item'));
            }
        }
    });

    // Payment filters
    $('#payment-status-filter, #payment-method-filter').change(function() {
        filterPayments();
    });

    // Invoice form input changes
    $(document).on('input change', 'input[name="quantity[]"], input[name="price[]"]', function() {
        calculateInvoiceTotal();
    });

    // Form validation
    $('#new-invoice-form input, #new-invoice-form select, #new-invoice-form textarea').on('input change', function() {
        validateFormField($(this));
    });
}

// Load Payment Data
function loadPaymentData() {
    // Simulate loading payment data
    setTimeout(function() {
        // Animate stat cards
        $('.stat-card').each(function(index) {
            $(this).delay(index * 100).animate({
                opacity: 1,
                transform: 'translateY(0)'
            }, 500);
        });
        
        // Update payment method chart with animation
        animatePaymentChart();
    }, 500);
}

// Animate Payment Chart
function animatePaymentChart() {
    $('.method-progress').each(function(index) {
        const $this = $(this);
        const width = $this.css('width');
        $this.css('width', '0%');
        
        setTimeout(function() {
            $this.animate({
                width: width
            }, 1000, 'easeOutCubic');
        }, index * 200);
    });
}

// Handle Quick Actions
function handleCheckoutPayment() {
    showNotification('Đang khởi tạo quy trình thanh toán check-out...', 'info');
    
    // Simulate processing
    setTimeout(function() {
        const paymentData = {
            customer: 'Trần Văn An',
            room: 'Suite 301',
            amount: '$1,250.00',
            method: 'card'
        };
        
        showPaymentProcessModal(paymentData);
    }, 1000);
}

function handleRoomCharge() {
    showNotification('Đang mở form tính phí phòng...', 'info');
    
    // Create simple room charge form
    const formHtml = `
        <div class="room-charge-form">
            <h5>Tính phí phòng</h5>
            <div class="mb-3">
                <label class="form-label">Phòng</label>
                <select class="form-select">
                    <option>Suite 301 - Trần Văn An</option>
                    <option>Deluxe 205 - Nguyễn Thị Hoa</option>
                    <option>Standard 102 - Lê Minh Đức</option>
                </select>
            </div>
            <div class="mb-3">
                <label class="form-label">Số đêm</label>
                <input type="number" class="form-control" value="2" min="1">
            </div>
            <div class="mb-3">
                <label class="form-label">Giá phòng/đêm ($)</label>
                <input type="number" class="form-control" value="625" step="0.01">
            </div>
        </div>
    `;
    
    showQuickActionModal('Tính phí phòng', formHtml, function() {
        showNotification('Đã thêm phí phòng vào hóa đơn!', 'success');
    });
}

function handleServiceCharge() {
    showNotification('Đang mở form tính phí dịch vụ...', 'info');
    
    const formHtml = `
        <div class="service-charge-form">
            <h5>Tính phí dịch vụ</h5>
            <div class="mb-3">
                <label class="form-label">Khách hàng</label>
                <select class="form-select">
                    <option>Trần Văn An - Suite 301</option>
                    <option>Nguyễn Thị Hoa - Deluxe 205</option>
                    <option>Lê Minh Đức - Standard 102</option>
                </select>
            </div>
            <div class="mb-3">
                <label class="form-label">Dịch vụ</label>
                <select class="form-select">
                    <option>Spa & Massage - $150</option>
                    <option>Ăn sáng - $25</option>
                    <option>Giặt ủi - $30</option>
                    <option>Minibar - $45</option>
                </select>
            </div>
            <div class="mb-3">
                <label class="form-label">Số lượng</label>
                <input type="number" class="form-control" value="1" min="1">
            </div>
        </div>
    `;
    
    showQuickActionModal('Tính phí dịch vụ', formHtml, function() {
        showNotification('Đã thêm phí dịch vụ vào hóa đơn!', 'success');
    });
}

function handleRefund() {
    showNotification('Đang mở form xử lý hoàn tiền...', 'info');
    
    const formHtml = `
        <div class="refund-form">
            <h5>Xử lý hoàn tiền</h5>
            <div class="mb-3">
                <label class="form-label">Mã giao dịch</label>
                <input type="text" class="form-control" placeholder="Nhập mã giao dịch cần hoàn tiền">
            </div>
            <div class="mb-3">
                <label class="form-label">Số tiền hoàn ($)</label>
                <input type="number" class="form-control" step="0.01" min="0">
            </div>
            <div class="mb-3">
                <label class="form-label">Lý do hoàn tiền</label>
                <textarea class="form-control" rows="3" placeholder="Mô tả lý do hoàn tiền..."></textarea>
            </div>
        </div>
    `;
    
    showQuickActionModal('Xử lý hoàn tiền', formHtml, function() {
        showNotification('Đã xử lý hoàn tiền thành công!', 'success');
    });
}

// Show Quick Action Modal
function showQuickActionModal(title, content, callback) {
    const modalHtml = `
        <div class="modal fade" id="quickActionModal" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">${title}</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        ${content}
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Hủy</button>
                        <button type="button" class="btn btn-primary" id="confirm-action">Xác nhận</button>
                    </div>
                </div>
            </div>
        </div>
    `;
    
    // Remove existing modal
    $('#quickActionModal').remove();
    
    // Add new modal
    $('body').append(modalHtml);
    
    // Show modal
    $('#quickActionModal').modal('show');
    
    // Handle confirm button
    $('#confirm-action').click(function() {
        $('#quickActionModal').modal('hide');
        if (callback) callback();
    });
    
    // Clean up on hide
    $('#quickActionModal').on('hidden.bs.modal', function() {
        $(this).remove();
    });
}

// Filter Functions
function filterPayments() {
    const statusFilter = $('#payment-status-filter').val();
    const methodFilter = $('#payment-method-filter').val();
    
    showNotification('Đang áp dụng bộ lọc thanh toán...', 'info');
    
    // Simulate filtering
    setTimeout(function() {
        showNotification('Đã áp dụng bộ lọc thành công!', 'success');
        // Here you would typically reload the payment table with filtered data
    }, 1000);
}

function filterInvoices() {
    const dateFrom = $('#invoice-date-from').val();
    const dateTo = $('#invoice-date-to').val();
    const status = $('#invoice-status-filter').val();
    const customer = $('#customer-search').val();
    
    showNotification('Đang tìm kiếm hóa đơn...', 'info');
    
    // Show loading state
    $('#filter-invoices-btn').html('<i class="fas fa-spinner fa-spin"></i> Đang tìm...');
    $('#filter-invoices-btn').prop('disabled', true);
    
    // Simulate search
    setTimeout(function() {
        $('#filter-invoices-btn').html('<i class="fas fa-search"></i> Tìm kiếm');
        $('#filter-invoices-btn').prop('disabled', false);
        
        showNotification('Đã tìm thấy các hóa đơn phù hợp!', 'success');
    }, 1500);
}

// Export Functions
function exportInvoices() {
    $('#export-invoices-btn').html('<i class="fas fa-spinner fa-spin"></i> Đang xuất...');
    $('#export-invoices-btn').prop('disabled', true);
    
    setTimeout(function() {
        $('#export-invoices-btn').html('<i class="fas fa-download"></i> Xuất Excel');
        $('#export-invoices-btn').prop('disabled', false);
        
        showNotification('Đã xuất file Excel thành công!', 'success');
        
        // Simulate file download
        const link = document.createElement('a');
        link.href = 'data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet,';
        link.download = 'danh-sach-hoa-don-' + new Date().toISOString().slice(0, 10) + '.xlsx';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }, 2000);
}

function printAllInvoices() {
    const printContent = `
        <html>
        <head>
            <title>Danh Sách Hóa Đơn - The Serene Horizon Hotel</title>
            <style>
                body { font-family: Arial, sans-serif; margin: 20px; }
                .header { text-align: center; margin-bottom: 30px; }
                .header h1 { color: #0a1931; margin-bottom: 10px; }
                .invoice-item { border: 1px solid #ddd; padding: 15px; margin-bottom: 15px; }
                .invoice-number { font-weight: bold; color: #0a1931; }
                .amount { font-weight: bold; color: #28a745; }
                @media print { body { margin: 0; } }
            </style>
        </head>
        <body>
            <div class="header">
                <h1>DANH SÁCH HÓA ĐƠN</h1>
                <p>The Serene Horizon Hotel</p>
                <p>Ngày in: ${new Date().toLocaleDateString('vi-VN')}</p>
            </div>
            <div class="invoice-item">
                <div class="invoice-number">#INV-2025-001</div>
                <p>Khách hàng: Trần Văn An</p>
                <p>Phòng: Suite 301</p>
                <p class="amount">Số tiền: $1,250.00</p>
                <p>Trạng thái: Đã thanh toán</p>
            </div>
        </body>
        </html>
    `;
    
    const printWindow = window.open('', '_blank');
    printWindow.document.write(printContent);
    printWindow.document.close();
    printWindow.print();
    printWindow.close();
    
    showNotification('Đã gửi danh sách hóa đơn đến máy in!', 'success');
}

// Invoice Form Functions
function resetInvoiceForm() {
    $('#new-invoice-form')[0].reset();
    $('#new-invoice-form .form-control, #new-invoice-form .form-select, #new-invoice-form textarea')
        .removeClass('is-valid is-invalid');
    
    // Set default values
    const today = new Date().toISOString().slice(0, 10);
    $('input[name="invoice_date"]').val(today);
    
    // Reset invoice items to default
    $('.invoice-items').html(`
        <div class="invoice-item-row">
            <div class="row">
                <div class="col-md-5">
                    <label class="form-label">Dịch vụ</label>
                    <select class="form-select" name="service[]">
                        <option value="room">Phí phòng</option>
                        <option value="breakfast">Ăn sáng</option>
                        <option value="spa">Dịch vụ Spa</option>
                        <option value="laundry">Giặt ủi</option>
                        <option value="minibar">Minibar</option>
                    </select>
                </div>
                <div class="col-md-2">
                    <label class="form-label">Số lượng</label>
                    <input type="number" class="form-control" name="quantity[]" value="1" min="1">
                </div>
                <div class="col-md-3">
                    <label class="form-label">Đơn giá ($)</label>
                    <input type="number" class="form-control" name="price[]" step="0.01" min="0">
                </div>
                <div class="col-md-2">
                    <label class="form-label">Thao tác</label>
                    <button type="button" class="btn btn-outline-danger btn-sm remove-item">
                        <i class="fas fa-trash"></i>
                    </button>
                </div>
            </div>
        </div>
    `);
    
    calculateInvoiceTotal();
}

function addInvoiceItem() {
    const newItem = `
        <div class="invoice-item-row">
            <div class="row">
                <div class="col-md-5">
                    <label class="form-label">Dịch vụ</label>
                    <select class="form-select" name="service[]">
                        <option value="room">Phí phòng</option>
                        <option value="breakfast">Ăn sáng</option>
                        <option value="spa">Dịch vụ Spa</option>
                        <option value="laundry">Giặt ủi</option>
                        <option value="minibar">Minibar</option>
                    </select>
                </div>
                <div class="col-md-2">
                    <label class="form-label">Số lượng</label>
                    <input type="number" class="form-control" name="quantity[]" value="1" min="1">
                </div>
                <div class="col-md-3">
                    <label class="form-label">Đơn giá ($)</label>
                    <input type="number" class="form-control" name="price[]" step="0.01" min="0">
                </div>
                <div class="col-md-2">
                    <label class="form-label">Thao tác</label>
                    <button type="button" class="btn btn-outline-danger btn-sm remove-item">
                        <i class="fas fa-trash"></i>
                    </button>
                </div>
            </div>
        </div>
    `;
    
    $('.invoice-items').append(newItem);
    calculateInvoiceTotal();
}

function removeInvoiceItem(button) {
    if ($('.invoice-item-row').length > 1) {
        button.closest('.invoice-item-row').remove();
        calculateInvoiceTotal();
    } else {
        showNotification('Phải có ít nhất một dịch vụ trong hóa đơn!', 'error');
    }
}

function initializeInvoiceCalculations() {
    calculateInvoiceTotal();
}

function calculateInvoiceTotal() {
    let subtotal = 0;
    
    $('.invoice-item-row').each(function() {
        const quantity = parseFloat($(this).find('input[name="quantity[]"]').val()) || 0;
        const price = parseFloat($(this).find('input[name="price[]"]').val()) || 0;
        subtotal += quantity * price;
    });
    
    const tax = subtotal * 0.1; // 10% tax
    const serviceFee = subtotal * 0.05; // 5% service fee
    const total = subtotal + tax + serviceFee;
    
    $('#subtotal').text('$' + subtotal.toFixed(2));
    $('#tax').text('$' + tax.toFixed(2));
    $('#service-fee').text('$' + serviceFee.toFixed(2));
    $('#total').text('$' + total.toFixed(2));
}

function createInvoice() {
    if (!validateInvoiceForm()) {
        return;
    }
    
    $('#create-invoice-btn').html('<i class="fas fa-spinner fa-spin"></i> Đang tạo...');
    $('#create-invoice-btn').prop('disabled', true);
    
    // Simulate invoice creation
    setTimeout(function() {
        $('#create-invoice-btn').html('<i class="fas fa-file-invoice"></i> Tạo hóa đơn');
        $('#create-invoice-btn').prop('disabled', false);
        
        $('#newInvoiceModal').modal('hide');
        showNotification('Đã tạo hóa đơn thành công!', 'success');
        
        // Reload invoice list
        setTimeout(function() {
            location.reload();
        }, 1500);
    }, 2000);
}

function saveDraftInvoice() {
    $('#save-draft-btn').html('<i class="fas fa-spinner fa-spin"></i> Đang lưu...');
    $('#save-draft-btn').prop('disabled', true);
    
    setTimeout(function() {
        $('#save-draft-btn').html('<i class="fas fa-save"></i> Lưu nháp');
        $('#save-draft-btn').prop('disabled', false);
        
        showNotification('Đã lưu nháp hóa đơn thành công!', 'success');
    }, 1000);
}

function validateInvoiceForm() {
    let isValid = true;
    const requiredFields = ['customer', 'invoice_date'];
    
    requiredFields.forEach(function(fieldName) {
        const field = $(`[name="${fieldName}"]`);
        if (!field.val().trim()) {
            field.addClass('is-invalid').removeClass('is-valid');
            isValid = false;
        } else {
            field.addClass('is-valid').removeClass('is-invalid');
        }
    });
    
    // Check if at least one service item has valid data
    let hasValidItem = false;
    $('.invoice-item-row').each(function() {
        const quantity = parseFloat($(this).find('input[name="quantity[]"]').val()) || 0;
        const price = parseFloat($(this).find('input[name="price[]"]').val()) || 0;
        if (quantity > 0 && price > 0) {
            hasValidItem = true;
        }
    });
    
    if (!hasValidItem) {
        showNotification('Vui lòng thêm ít nhất một dịch vụ có số lượng và giá hợp lệ!', 'error');
        isValid = false;
    }
    
    if (!isValid) {
        showNotification('Vui lòng điền đầy đủ thông tin bắt buộc!', 'error');
    }
    
    return isValid;
}

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

// Detail Functions
function showPaymentDetail(row) {
    const txnId = row.find('td:eq(0) strong').text();
    const customer = row.find('.customer-info strong').text();
    const room = row.find('.room-number').text();
    const amount = row.find('.amount-highlight').text();
    const method = row.find('.payment-method').text().trim();
    const status = row.find('.status-badge').text();
    const time = row.find('td:eq(6)').text();
    
    const detailContent = `
        <div class="payment-detail">
            <div class="row">
                <div class="col-md-6">
                    <h5>Thông tin giao dịch</h5>
                    <table class="table table-borderless">
                        <tr><td><strong>Mã giao dịch:</strong></td><td>${txnId}</td></tr>
                        <tr><td><strong>Khách hàng:</strong></td><td>${customer}</td></tr>
                        <tr><td><strong>Phòng:</strong></td><td>${room}</td></tr>
                        <tr><td><strong>Thời gian:</strong></td><td>${time}</td></tr>
                    </table>
                </div>
                <div class="col-md-6">
                    <h5>Chi tiết thanh toán</h5>
                    <table class="table table-borderless">
                        <tr><td><strong>Số tiền:</strong></td><td>${amount}</td></tr>
                        <tr><td><strong>Phương thức:</strong></td><td>${method}</td></tr>
                        <tr><td><strong>Trạng thái:</strong></td><td>${status}</td></tr>
                        <tr><td><strong>Phí giao dịch:</strong></td><td>$12.50</td></tr>
                    </table>
                </div>
            </div>
            <div class="row mt-4">
                <div class="col-12">
                    <h5>Ghi chú</h5>
                    <p class="bg-light p-3 rounded">
                        Giao dịch thanh toán check-out cho khách ${customer}. 
                        Thanh toán được xử lý thành công qua ${method.toLowerCase()}.
                    </p>
                </div>
            </div>
        </div>
    `;
    
    $('#paymentDetailContent').html(detailContent);
    $('#paymentDetailModal').modal('show');
}

function showInvoiceDetail(invoiceItem) {
    const invoiceNumber = invoiceItem.find('.invoice-number').text();
    const customer = invoiceItem.find('.invoice-customer strong').text();
    const amount = invoiceItem.find('.amount').text();
    const status = invoiceItem.find('.status-badge').text();
    
    const detailContent = `
        <div class="invoice-detail">
            <div class="row">
                <div class="col-md-6">
                    <h5>Thông tin hóa đơn</h5>
                    <table class="table table-borderless">
                        <tr><td><strong>Số hóa đơn:</strong></td><td>${invoiceNumber}</td></tr>
                        <tr><td><strong>Khách hàng:</strong></td><td>${customer}</td></tr>
                        <tr><td><strong>Trạng thái:</strong></td><td>${status}</td></tr>
                        <tr><td><strong>Tổng tiền:</strong></td><td>${amount}</td></tr>
                    </table>
                </div>
                <div class="col-md-6">
                    <h5>Chi tiết dịch vụ</h5>
                    <table class="table">
                        <tr><td>Phí phòng (2 đêm)</td><td>$1,000.00</td></tr>
                        <tr><td>Ăn sáng (2 người)</td><td>$100.00</td></tr>
                        <tr><td>Dịch vụ Spa</td><td>$150.00</td></tr>
                        <tr><td><strong>Tổng cộng:</strong></td><td><strong>${amount}</strong></td></tr>
                    </table>
                </div>
            </div>
        </div>
    `;
    
    $('#paymentDetailContent').html(detailContent);
    $('#paymentDetailModal').modal('show');
}

// Print Functions
function printPaymentReceipt(row) {
    const txnId = row.find('td:eq(0) strong').text();
    const customer = row.find('.customer-info strong').text();
    const amount = row.find('.amount-highlight').text();
    
    const receiptContent = `
        <html>
        <head>
            <title>Biên Lai Thanh Toán - ${txnId}</title>
            <style>
                body { font-family: Arial, sans-serif; margin: 20px; }
                .header { text-align: center; margin-bottom: 30px; }
                .receipt-info { margin-bottom: 20px; }
                table { width: 100%; margin-bottom: 20px; }
                .total { font-weight: bold; font-size: 18px; }
                @media print { body { margin: 0; } }
            </style>
        </head>
        <body>
            <div class="header">
                <h2>THE SERENE HORIZON HOTEL</h2>
                <p>BIÊN LAI THANH TOÁN</p>
                <p>${txnId}</p>
            </div>
            <div class="receipt-info">
                <p><strong>Khách hàng:</strong> ${customer}</p>
                <p><strong>Ngày:</strong> ${new Date().toLocaleDateString('vi-VN')}</p>
                <p><strong>Số tiền:</strong> ${amount}</p>
            </div>
            <p class="total">Cảm ơn quý khách đã sử dụng dịch vụ!</p>
        </body>
        </html>
    `;
    
    const printWindow = window.open('', '_blank');
    printWindow.document.write(receiptContent);
    printWindow.document.close();
    printWindow.print();
    printWindow.close();
    
    showNotification('Đã in biên lai thanh toán!', 'success');
}

function printInvoice(invoiceItem) {
    const invoiceNumber = invoiceItem.find('.invoice-number').text();
    const customer = invoiceItem.find('.invoice-customer strong').text();
    const amount = invoiceItem.find('.amount').text();
    
    const invoiceContent = `
        <html>
        <head>
            <title>Hóa Đơn - ${invoiceNumber}</title>
            <style>
                body { font-family: Arial, sans-serif; margin: 20px; }
                .header { text-align: center; margin-bottom: 30px; }
                .invoice-info { margin-bottom: 20px; }
                table { width: 100%; border-collapse: collapse; margin-bottom: 20px; }
                th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
                th { background-color: #f8f9fa; }
                .total { font-weight: bold; font-size: 18px; }
                @media print { body { margin: 0; } }
            </style>
        </head>
        <body>
            <div class="header">
                <h2>THE SERENE HORIZON HOTEL</h2>
                <p>HÓA ĐƠN DỊCH VỤ</p>
                <p>${invoiceNumber}</p>
            </div>
            <div class="invoice-info">
                <p><strong>Khách hàng:</strong> ${customer}</p>
                <p><strong>Ngày:</strong> ${new Date().toLocaleDateString('vi-VN')}</p>
            </div>
            <table>
                <tr><th>Dịch vụ</th><th>Số lượng</th><th>Đơn giá</th><th>Thành tiền</th></tr>
                <tr><td>Phí phòng</td><td>2</td><td>$500.00</td><td>$1,000.00</td></tr>
                <tr><td>Ăn sáng</td><td>2</td><td>$50.00</td><td>$100.00</td></tr>
                <tr><td>Dịch vụ Spa</td><td>1</td><td>$150.00</td><td>$150.00</td></tr>
            </table>
            <p class="total">Tổng cộng: ${amount}</p>
        </body>
        </html>
    `;
    
    const printWindow = window.open('', '_blank');
    printWindow.document.write(invoiceContent);
    printWindow.document.close();
    printWindow.print();
    printWindow.close();
    
    showNotification('Đã in hóa đơn!', 'success');
}

// Show Notification
// showNotification và responsive sidebar functions được sử dụng từ shared.js