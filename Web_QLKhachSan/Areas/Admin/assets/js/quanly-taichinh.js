// JavaScript quản lý hóa đơn - Phiên bản cải tiến
document.addEventListener('DOMContentLoaded', function() {
    const createBtn = document.querySelector('.btn-primary');
    const modal = document.getElementById('invoiceModal');
    const closeModalBtn = document.getElementById('closeModalBtn');
    const cancelBtn = document.getElementById('cancelBtn');
    const invoiceForm = document.getElementById('invoiceForm');
    const tableBody = document.querySelector('.table tbody');
    
    // Lưu trữ tất cả hóa đơn
    let allInvoices = [];
    
    // Khởi tạo dữ liệu từ bảng hiện có
    initializeInvoices();

    // Hàm hiển thị modal
    const showModal = () => {
        modal.style.display = 'flex';
        setTimeout(() => modal.classList.add('show'), 10);
        // Set ngày mặc định là hôm nay
        document.getElementById('invoiceDate').valueAsDate = new Date();
    };

    // Hàm ẩn modal
    const hideModal = () => {
        modal.classList.remove('show');
        setTimeout(() => {
            modal.style.display = 'none';
            invoiceForm.reset();
        }, 300);
    };

    // Event listeners để mở/đóng modal
    createBtn.addEventListener('click', showModal);
    closeModalBtn.addEventListener('click', hideModal);
    cancelBtn.addEventListener('click', hideModal);
    modal.addEventListener('click', (e) => {
        if (e.target === modal) {
            hideModal();
        }
    });

    // Xử lý submit form
    invoiceForm.addEventListener('submit', function(e) {
        e.preventDefault();

        const newInvoice = {
            id: 'HD' + Math.floor(1000 + Math.random() * 9000),
            customer: document.getElementById('customerName').value,
            date: new Date(document.getElementById('invoiceDate').value).toLocaleDateString('vi-VN'),
            dateObj: new Date(document.getElementById('invoiceDate').value),
            amount: parseFloat(document.getElementById('invoiceAmount').value),
            status: document.getElementById('invoiceStatus').value
        };

        allInvoices.unshift(newInvoice);
        addInvoiceRow(newInvoice);
        hideModal();
        showNotification('Tạo hóa đơn thành công!', 'success');
    });

    // Khởi tạo dữ liệu từ bảng hiện có
    function initializeInvoices() {
        const rows = tableBody.querySelectorAll('tr');
        rows.forEach(row => {
            const dateStr = row.cells[2].textContent.trim();
            const amountStr = row.cells[3].textContent.replace(/[^\d]/g, '');
            const statusClass = row.querySelector('span[class^="status-"]').className;
            let status = 'pending';
            if (statusClass.includes('paid')) status = 'paid';
            else if (statusClass.includes('cancelled')) status = 'cancelled';
            
            allInvoices.push({
                id: row.cells[0].textContent,
                customer: row.cells[1].textContent,
                date: dateStr,
                dateObj: parseVietnameseDate(dateStr),
                amount: parseFloat(amountStr),
                status: status
            });
        });
    }

    // Hàm parse ngày Việt Nam
    function parseVietnameseDate(dateStr) {
        const parts = dateStr.split('/');
        if (parts.length === 3) {
            return new Date(parts[2], parts[1] - 1, parts[0]);
        }
        return new Date();
    }

    // Hàm thêm dòng mới vào bảng
    function addInvoiceRow(invoice) {
        const row = document.createElement('tr');
        
        const statusMap = {
            paid: { class: 'status-paid', text: 'Đã thanh toán', icon: 'fa-check-circle' },
            pending: { class: 'status-pending', text: 'Chờ thanh toán', icon: 'fa-clock' },
            cancelled: { class: 'status-cancelled', text: 'Đã hủy', icon: 'fa-times-circle' }
        };
        const statusInfo = statusMap[invoice.status];

        row.innerHTML = `
            <td>${invoice.id}</td>
            <td>${invoice.customer}</td>
            <td>${invoice.date}</td>
            <td>${new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(invoice.amount)}</td>
            <td><span class="${statusInfo.class}"><i class="fas ${statusInfo.icon}"></i> ${statusInfo.text}</span></td>
            <td>
                <div class="actions-cell">
                    <button class="action-icon view" title="Xem" data-id="${invoice.id}"><i class="fas fa-eye"></i></button>
                    <button class="action-icon print" title="In" data-id="${invoice.id}"><i class="fas fa-print"></i></button>
                    <button class="action-icon delete" title="Xóa" data-id="${invoice.id}"><i class="fas fa-trash"></i></button>
                </div>
            </td>
        `;

        row.querySelectorAll('.action-icon').forEach(icon => icon.addEventListener('click', handleActionClick));
        tableBody.prepend(row);
    }

    // Hàm xử lý chung cho các nút hành động
    const handleActionClick = function(e) {
        e.stopPropagation();
        const title = this.getAttribute('title');
        const invoiceId = this.getAttribute('data-id');
        const row = this.closest('tr');
        
        if (title === 'Xem') {
            viewInvoice(invoiceId);
        } else if (title === 'In') {
            printInvoice(invoiceId, row);
        } else if (title === 'Xóa') {
            if (confirm('Bạn có chắc muốn xóa hóa đơn ' + invoiceId + '?')) {
                allInvoices = allInvoices.filter(inv => inv.id !== invoiceId);
                row.remove();
                showNotification('Đã xóa hóa đơn!', 'success');
            }
        }
    };

    // Hàm xem chi tiết hóa đơn
    function viewInvoice(invoiceId) {
        const invoice = allInvoices.find(inv => inv.id === invoiceId);
        if (!invoice) return;

        const detailModal = document.createElement('div');
        detailModal.className = 'invoice-modal show';
        detailModal.innerHTML = `
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h3>Chi tiết hóa đơn ${invoice.id}</h3>
                        <button class="close-btn">&times;</button>
                    </div>
                    <div class="modal-body">
                        <div class="invoice-detail">
                            <div class="detail-row">
                                <strong>Mã hóa đơn:</strong>
                                <span>${invoice.id}</span>
                            </div>
                            <div class="detail-row">
                                <strong>Khách hàng:</strong>
                                <span>${invoice.customer}</span>
                            </div>
                            <div class="detail-row">
                                <strong>Ngày tạo:</strong>
                                <span>${invoice.date}</span>
                            </div>
                            <div class="detail-row">
                                <strong>Số tiền:</strong>
                                <span>${new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(invoice.amount)}</span>
                            </div>
                            <div class="detail-row">
                                <strong>Trạng thái:</strong>
                                <span>${invoice.status === 'paid' ? 'Đã thanh toán' : invoice.status === 'pending' ? 'Chờ thanh toán' : 'Đã hủy'}</span>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button class="btn btn-secondary close-detail">Đóng</button>
                    </div>
                </div>
            </div>
        `;
        document.body.appendChild(detailModal);

        detailModal.querySelector('.close-btn').addEventListener('click', () => detailModal.remove());
        detailModal.querySelector('.close-detail').addEventListener('click', () => detailModal.remove());
        detailModal.addEventListener('click', (e) => {
            if (e.target === detailModal) detailModal.remove();
        });
    }

    // Hàm in hóa đơn
    function printInvoice(invoiceId, row) {
        const invoice = allInvoices.find(inv => inv.id === invoiceId);
        if (!invoice) return;

        const printWindow = window.open('', '', 'width=800,height=600');
        printWindow.document.write(`
            <!DOCTYPE html>
            <html>
            <head>
                <title>Hóa Đơn ${invoice.id}</title>
                <style>
                    body {
                        font-family: 'Arial', sans-serif;
                        padding: 40px;
                        max-width: 800px;
                        margin: 0 auto;
                    }
                    .invoice-header {
                        text-align: center;
                        margin-bottom: 30px;
                        border-bottom: 3px solid #d4af37;
                        padding-bottom: 20px;
                    }
                    .invoice-header h1 {
                        color: #0a1931;
                        margin: 10px 0;
                    }
                    .invoice-info {
                        margin: 30px 0;
                    }
                    .info-row {
                        display: flex;
                        justify-content: space-between;
                        padding: 12px 0;
                        border-bottom: 1px solid #eee;
                    }
                    .info-label {
                        font-weight: bold;
                        color: #333;
                    }
                    .info-value {
                        color: #666;
                    }
                    .total-amount {
                        font-size: 24px;
                        font-weight: bold;
                        color: #0a1931;
                        text-align: right;
                        margin-top: 30px;
                        padding-top: 20px;
                        border-top: 2px solid #d4af37;
                    }
                    .footer {
                        margin-top: 50px;
                        text-align: center;
                        color: #999;
                        font-size: 12px;
                    }
                    @media print {
                        body { padding: 20px; }
                    }
                </style>
            </head>
            <body>
                <div class="invoice-header">
                    <h1>KHÁCH SẠN PARADISE</h1>
                    <p>123 Đường ABC, Quận 1, TP.HCM</p>
                    <p>Tel: 0123 456 789 | Email: info@paradisehotel.vn</p>
                </div>
                <h2 style="text-align: center; color: #d4af37;">HÓA ĐƠN THANH TOÁN</h2>
                <div class="invoice-info">
                    <div class="info-row">
                        <span class="info-label">Mã hóa đơn:</span>
                        <span class="info-value">${invoice.id}</span>
                    </div>
                    <div class="info-row">
                        <span class="info-label">Ngày tạo:</span>
                        <span class="info-value">${invoice.date}</span>
                    </div>
                    <div class="info-row">
                        <span class="info-label">Khách hàng:</span>
                        <span class="info-value">${invoice.customer}</span>
                    </div>
                    <div class="info-row">
                        <span class="info-label">Trạng thái:</span>
                        <span class="info-value">${invoice.status === 'paid' ? 'Đã thanh toán' : invoice.status === 'pending' ? 'Chờ thanh toán' : 'Đã hủy'}</span>
                    </div>
                </div>
                <div class="total-amount">
                    Tổng tiền: ${new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(invoice.amount)}
                </div>
                <div class="footer">
                    <p>Cảm ơn quý khách đã sử dụng dịch vụ của chúng tôi!</p>
                    <p>In lúc: ${new Date().toLocaleString('vi-VN')}</p>
                </div>
            </body>
            </html>
        `);
        printWindow.document.close();
        printWindow.focus();
        setTimeout(() => {
            printWindow.print();
            printWindow.close();
        }, 250);
        
        showNotification('Đang chuẩn bị in hóa đơn...', 'info');
    }

    // Hàm hiển thị thông báo
    function showNotification(message, type = 'info') {
        const notification = document.createElement('div');
        notification.className = `notification ${type}`;
        notification.style.cssText = `
            position: fixed;
            top: 100px;
            right: 20px;
            padding: 1rem 1.5rem;
            background: ${type === 'success' ? '#27ae60' : type === 'error' ? '#e74c3c' : '#3498db'};
            color: white;
            border-radius: 8px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.2);
            z-index: 9999;
            animation: slideIn 0.3s ease;
            font-weight: 500;
        `;
        notification.textContent = message;
        document.body.appendChild(notification);

        setTimeout(() => {
            notification.style.animation = 'slideOut 0.3s ease';
            setTimeout(() => notification.remove(), 300);
        }, 3000);
    }

    // Thêm CSS cho animations và chi tiết hóa đơn
    const style = document.createElement('style');
    style.textContent = `
        @keyframes slideIn {
            from { transform: translateX(100%); opacity: 0; }
            to { transform: translateX(0); opacity: 1; }
        }
        @keyframes slideOut {
            from { transform: translateX(0); opacity: 1; }
            to { transform: translateX(100%); opacity: 0; }
        }
        .invoice-detail {
            padding: 1rem 0;
        }
        .detail-row {
            display: flex;
            justify-content: space-between;
            padding: 1rem;
            border-bottom: 1px solid #eee;
        }
        .detail-row:last-child {
            border-bottom: none;
        }
        .detail-row strong {
            color: var(--charcoal-grey);
        }
        .detail-row span {
            color: var(--deep-navy);
            font-weight: 500;
        }
    `;
    document.head.appendChild(style);

    // Gắn event listener cho các nút đã có sẵn
    document.querySelectorAll('.action-icon').forEach(icon => {
        icon.addEventListener('click', handleActionClick);
    });

    // ======== FILTER FUNCTIONALITY ======== 
    const filterFromDate = document.getElementById('filterFromDate');
    const filterToDate = document.getElementById('filterToDate');
    const filterStatus = document.getElementById('filterStatus');
    const filterSearch = document.getElementById('filterSearch');
    const applyFiltersBtn = document.getElementById('applyFiltersBtn');
    const resetFiltersBtn = document.getElementById('resetFiltersBtn');
    
    // Set ngày mặc định (30 ngày trước đến hôm nay)
    const today = new Date();
    const thirtyDaysAgo = new Date(today);
    thirtyDaysAgo.setDate(today.getDate() - 30);
    filterFromDate.valueAsDate = thirtyDaysAgo;
    filterToDate.valueAsDate = today;

    // Áp dụng bộ lọc
    applyFiltersBtn.addEventListener('click', applyFilters);
    resetFiltersBtn.addEventListener('click', resetFilters);
    
    // Lọc theo thời gian thực khi gõ tìm kiếm
    filterSearch.addEventListener('input', applyFilters);

    function applyFilters() {
        const fromDate = filterFromDate.value ? new Date(filterFromDate.value) : null;
        const toDate = filterToDate.value ? new Date(filterToDate.value) : null;
        const status = filterStatus.value;
        const searchTerm = filterSearch.value.toLowerCase().trim();

        // Lọc danh sách hóa đơn
        const filteredInvoices = allInvoices.filter(invoice => {
            // Lọc theo ngày
            if (fromDate && invoice.dateObj < fromDate) return false;
            if (toDate) {
                const endDate = new Date(toDate);
                endDate.setHours(23, 59, 59, 999);
                if (invoice.dateObj > endDate) return false;
            }
            
            // Lọc theo trạng thái
            if (status !== 'all' && invoice.status !== status) return false;
            
            // Lọc theo tìm kiếm
            if (searchTerm && 
                !invoice.id.toLowerCase().includes(searchTerm) && 
                !invoice.customer.toLowerCase().includes(searchTerm)) {
                return false;
            }
            
            return true;
        });

        // Cập nhật bảng
        updateTable(filteredInvoices);
        
        // Hiển thị thông báo
        showNotification(`Tìm thấy ${filteredInvoices.length} hóa đơn`, 'info');
    }

    function resetFilters() {
        filterFromDate.valueAsDate = thirtyDaysAgo;
        filterToDate.valueAsDate = today;
        filterStatus.value = 'all';
        filterSearch.value = '';
        updateTable(allInvoices);
        showNotification('Đã reset bộ lọc', 'info');
    }

    function updateTable(invoices) {
        tableBody.innerHTML = '';
        
        if (invoices.length === 0) {
            tableBody.innerHTML = `
                <tr>
                    <td colspan="6" style="text-align: center; padding: 2rem; color: #999;">
                        <i class="fas fa-inbox" style="font-size: 3rem; margin-bottom: 1rem; display: block;"></i>
                        Không tìm thấy hóa đơn nào
                    </td>
                </tr>
            `;
            return;
        }

        invoices.forEach(invoice => {
            const row = document.createElement('tr');
            const statusMap = {
                paid: { class: 'status-paid', text: 'Đã thanh toán', icon: 'fa-check-circle' },
                pending: { class: 'status-pending', text: 'Chờ thanh toán', icon: 'fa-clock' },
                cancelled: { class: 'status-cancelled', text: 'Đã hủy', icon: 'fa-times-circle' }
            };
            const statusInfo = statusMap[invoice.status];

            row.innerHTML = `
                <td>${invoice.id}</td>
                <td>${invoice.customer}</td>
                <td>${invoice.date}</td>
                <td>${new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(invoice.amount)}</td>
                <td><span class="${statusInfo.class}"><i class="fas ${statusInfo.icon}"></i> ${statusInfo.text}</span></td>
                <td>
                    <div class="actions-cell">
                        <button class="action-icon view" title="Xem" data-id="${invoice.id}"><i class="fas fa-eye"></i></button>
                        <button class="action-icon print" title="In" data-id="${invoice.id}"><i class="fas fa-print"></i></button>
                        <button class="action-icon delete" title="Xóa" data-id="${invoice.id}"><i class="fas fa-trash"></i></button>
                    </div>
                </td>
            `;

            row.querySelectorAll('.action-icon').forEach(icon => icon.addEventListener('click', handleActionClick));
            tableBody.appendChild(row);
        });
    }

    // ======== EXPORT FUNCTIONALITY ======== 
    const exportExcelBtn = document.getElementById('exportExcelBtn');
    const exportPdfBtn = document.getElementById('exportPdfBtn');

    exportExcelBtn.addEventListener('click', exportToExcel);
    exportPdfBtn.addEventListener('click', exportToPDF);

    function exportToExcel() {
        // Lấy dữ liệu hiện tại từ bảng
        const rows = Array.from(tableBody.querySelectorAll('tr'));
        
        if (rows.length === 0 || rows[0].cells.length === 1) {
            showNotification('Không có dữ liệu để xuất', 'error');
            return;
        }

        // Tạo CSV content
        let csvContent = '\uFEFF'; // UTF-8 BOM
        csvContent += 'Mã hóa đơn,Khách hàng,Ngày tạo,Số tiền,Trạng thái\n';
        
        rows.forEach(row => {
            const cells = row.querySelectorAll('td');
            if (cells.length > 1) {
                const statusText = cells[4].textContent.trim().replace(/\s+/g, ' ');
                csvContent += `${cells[0].textContent},`;
                csvContent += `"${cells[1].textContent}",`;
                csvContent += `${cells[2].textContent},`;
                csvContent += `"${cells[3].textContent}",`;
                csvContent += `"${statusText}"\n`;
            }
        });

        // Tạo và tải file
        const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
        const link = document.createElement('a');
        const url = URL.createObjectURL(blob);
        const fileName = `BaoCao_TaiChinh_${new Date().toISOString().slice(0,10)}.csv`;
        
        link.setAttribute('href', url);
        link.setAttribute('download', fileName);
        link.style.visibility = 'hidden';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        
        showNotification('Đã xuất báo cáo Excel thành công!', 'success');
    }

    function exportToPDF() {
        const rows = Array.from(tableBody.querySelectorAll('tr'));
        
        if (rows.length === 0 || rows[0].cells.length === 1) {
            showNotification('Không có dữ liệu để xuất', 'error');
            return;
        }

        // Tính tổng tiền
        let totalAmount = 0;
        rows.forEach(row => {
            const cells = row.querySelectorAll('td');
            if (cells.length > 1) {
                const amountStr = cells[3].textContent.replace(/[^\d]/g, '');
                totalAmount += parseFloat(amountStr);
            }
        });

        // Tạo cửa sổ in PDF
        const printWindow = window.open('', '', 'width=900,height=700');
        
        let tableRows = '';
        rows.forEach(row => {
            const cells = row.querySelectorAll('td');
            if (cells.length > 1) {
                const statusText = cells[4].textContent.trim().replace(/\s+/g, ' ');
                tableRows += `
                    <tr>
                        <td>${cells[0].textContent}</td>
                        <td>${cells[1].textContent}</td>
                        <td>${cells[2].textContent}</td>
                        <td style="text-align: right;">${cells[3].textContent}</td>
                        <td>${statusText}</td>
                    </tr>
                `;
            }
        });

        printWindow.document.write(`
            <!DOCTYPE html>
            <html>
            <head>
                <title>Báo Cáo Tài Chính</title>
                <meta charset="utf-8">
                <style>
                    * { margin: 0; padding: 0; box-sizing: border-box; }
                    body {
                        font-family: 'Arial', sans-serif;
                        padding: 40px;
                        background: white;
                    }
                    .header {
                        text-align: center;
                        margin-bottom: 40px;
                        border-bottom: 3px solid #d4af37;
                        padding-bottom: 20px;
                    }
                    .header h1 {
                        color: #0a1931;
                        font-size: 28px;
                        margin-bottom: 10px;
                    }
                    .header h2 {
                        color: #d4af37;
                        font-size: 20px;
                        margin: 10px 0;
                    }
                    .header p {
                        color: #666;
                        font-size: 14px;
                    }
                    .info-section {
                        margin: 20px 0;
                        display: flex;
                        justify-content: space-between;
                        padding: 15px;
                        background: #f8f9fa;
                        border-radius: 8px;
                    }
                    .info-item {
                        font-size: 14px;
                        color: #333;
                    }
                    .info-item strong {
                        color: #0a1931;
                    }
                    table {
                        width: 100%;
                        border-collapse: collapse;
                        margin: 20px 0;
                        box-shadow: 0 2px 8px rgba(0,0,0,0.1);
                    }
                    thead {
                        background: linear-gradient(135deg, #0a1931, #1e3a5f);
                        color: white;
                    }
                    th {
                        padding: 15px;
                        text-align: left;
                        font-weight: 600;
                        font-size: 14px;
                    }
                    td {
                        padding: 12px 15px;
                        border-bottom: 1px solid #eee;
                        font-size: 13px;
                        color: #333;
                    }
                    tbody tr:nth-child(even) {
                        background: #f8f9fa;
                    }
                    tbody tr:hover {
                        background: #e9ecef;
                    }
                    .summary {
                        margin-top: 30px;
                        padding: 20px;
                        background: linear-gradient(135deg, #0a1931, #1e3a5f);
                        color: white;
                        border-radius: 8px;
                        display: flex;
                        justify-content: space-between;
                        align-items: center;
                    }
                    .summary-label {
                        font-size: 18px;
                        font-weight: 600;
                    }
                    .summary-value {
                        font-size: 24px;
                        font-weight: bold;
                        color: #d4af37;
                    }
                    .footer {
                        margin-top: 40px;
                        text-align: center;
                        color: #999;
                        font-size: 12px;
                        border-top: 2px solid #eee;
                        padding-top: 20px;
                    }
                    @media print {
                        body { padding: 20px; }
                        .no-print { display: none; }
                    }
                </style>
            </head>
            <body>
                <div class="header">
                    <h1>KHÁCH SẠN PARADISE</h1>
                    <p>123 Đường ABC, Quận 1, TP.HCM | Tel: 0123 456 789</p>
                    <h2>BÁO CÁO TÀI CHÍNH</h2>
                </div>
                
                <div class="info-section">
                    <div class="info-item">
                        <strong>Ngày báo cáo:</strong> ${new Date().toLocaleDateString('vi-VN')}
                    </div>
                    <div class="info-item">
                        <strong>Tổng số hóa đơn:</strong> ${rows.length}
                    </div>
                    <div class="info-item">
                        <strong>Người lập:</strong> Nguyễn Thị An
                    </div>
                </div>
                
                <table>
                    <thead>
                        <tr>
                            <th>Mã hóa đơn</th>
                            <th>Khách hàng</th>
                            <th>Ngày tạo</th>
                            <th style="text-align: right;">Số tiền</th>
                            <th>Trạng thái</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${tableRows}
                    </tbody>
                </table>
                
                <div class="summary">
                    <span class="summary-label">TỔNG CỘNG:</span>
                    <span class="summary-value">${new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(totalAmount)}</span>
                </div>
                
                <div class="footer">
                    <p>Báo cáo được tạo tự động bởi hệ thống quản lý Khách sạn Paradise</p>
                    <p>In lúc: ${new Date().toLocaleString('vi-VN')}</p>
                </div>
            </body>
            </html>
        `);
        
        printWindow.document.close();
        printWindow.focus();
        
        setTimeout(() => {
            printWindow.print();
            printWindow.close();
        }, 250);
        
        showNotification('Đang chuẩn bị xuất PDF...', 'info');
    }
});
