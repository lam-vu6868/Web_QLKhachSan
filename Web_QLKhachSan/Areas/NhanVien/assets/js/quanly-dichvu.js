// Service Management JavaScript
document.addEventListener('DOMContentLoaded', function() {
    console.log('Service Management System Loaded');
    
    // Initialize the service management system
    initServiceManagement();
});

function initServiceManagement() {
    initFilters();
    initCategoryCards();
    initServiceRequests();
    initModals();
    initPerformanceCharts();
}

// Filter functionality
function initFilters() {
    const filters = ['status-filter', 'category-filter', 'priority-filter'];
    
    filters.forEach(filterId => {
        const filterElement = document.getElementById(filterId);
        if (filterElement) {
            filterElement.addEventListener('change', filterServiceRequests);
        }
    });
    
    // Category toggle buttons
    const allCategoriesBtn = document.getElementById('all-categories-btn');
    const popularCategoriesBtn = document.getElementById('popular-categories-btn');
    
    if (allCategoriesBtn && popularCategoriesBtn) {
        allCategoriesBtn.addEventListener('click', function() {
            this.classList.add('active');
            popularCategoriesBtn.classList.remove('active');
            showAllCategories();
        });
        
        popularCategoriesBtn.addEventListener('click', function() {
            this.classList.add('active');
            allCategoriesBtn.classList.remove('active');
            showPopularCategories();
        });
    }
}

function filterServiceRequests() {
    const statusFilter = document.getElementById('status-filter').value;
    const categoryFilter = document.getElementById('category-filter').value;
    const priorityFilter = document.getElementById('priority-filter').value;
    
    const requestItems = document.querySelectorAll('.request-item');
    let visibleCount = 0;
    
    requestItems.forEach(item => {
        let showItem = true;
        
        // Status filter
        if (statusFilter && item.dataset.status !== statusFilter) {
            showItem = false;
        }
        
        // Category filter
        if (categoryFilter && item.dataset.category !== categoryFilter) {
            showItem = false;
        }
        
        // Priority filter
        if (priorityFilter && item.dataset.priority !== priorityFilter) {
            showItem = false;
        }
        
        item.style.display = showItem ? '' : 'none';
        if (showItem) visibleCount++;
    });
    
    updatePaginationInfo(visibleCount);
}

function updatePaginationInfo(visibleCount) {
    const totalRequests = document.querySelectorAll('.request-item').length;
    const paginationInfo = document.querySelector('.pagination-info');
    if (paginationInfo) {
        paginationInfo.textContent = `Hiển thị 1-${visibleCount} của ${totalRequests} yêu cầu`;
    }
}

function showAllCategories() {
    const categoryCards = document.querySelectorAll('.category-card');
    categoryCards.forEach(card => {
        card.style.display = '';
    });
}

function showPopularCategories() {
    const categoryCards = document.querySelectorAll('.category-card');
    const popularCategories = ['spa', 'restaurant', 'room-service'];
    
    categoryCards.forEach(card => {
        const category = card.dataset.category;
        card.style.display = popularCategories.includes(category) ? '' : 'none';
    });
}

// Category card functionality
function initCategoryCards() {
    const categoryCards = document.querySelectorAll('.category-card');
    
    categoryCards.forEach(card => {
        card.addEventListener('click', function() {
            const category = this.dataset.category;
            
            // Update category filter and trigger filtering
            const categoryFilter = document.getElementById('category-filter');
            if (categoryFilter) {
                categoryFilter.value = category;
                filterServiceRequests();
            }
            
            // Scroll to requests section
            const requestsSection = document.querySelector('.service-requests-section');
            if (requestsSection) {
                requestsSection.scrollIntoView({ behavior: 'smooth' });
            }
            
            showNotification(`Hiển thị yêu cầu dịch vụ: ${this.querySelector('h5').textContent}`, 'info');
        });
    });
}

// Service request actions
function initServiceRequests() {
    const requestItems = document.querySelectorAll('.request-item');
    
    requestItems.forEach(item => {
        initRequestActions(item);
    });
}

function initRequestActions(item) {
    const acceptBtn = item.querySelector('.btn-outline-success');
    const rejectBtn = item.querySelector('.btn-outline-danger');
    const updateBtn = item.querySelector('.btn-outline-primary');
    const completeBtn = item.querySelector('.btn-success, .btn-outline-success:not(:first-child)');
    const detailBtn = item.querySelector('.btn-outline-info');
    const contactBtn = item.querySelector('.btn-outline-warning');
    
    if (acceptBtn && acceptBtn.title === 'Chấp nhận') {
        acceptBtn.addEventListener('click', () => acceptServiceRequest(item));
    }
    
    if (rejectBtn) {
        rejectBtn.addEventListener('click', () => rejectServiceRequest(item));
    }
    
    if (updateBtn) {
        updateBtn.addEventListener('click', () => updateServiceRequest(item));
    }
    
    if (completeBtn && (completeBtn.title === 'Hoàn thành' || completeBtn.classList.contains('btn-success'))) {
        completeBtn.addEventListener('click', () => completeServiceRequest(item));
    }
    
    if (detailBtn) {
        detailBtn.addEventListener('click', () => viewServiceRequestDetail(item));
    }
    
    if (contactBtn) {
        contactBtn.addEventListener('click', () => contactCustomer(item));
    }
}

function acceptServiceRequest(item) {
    const requestId = item.querySelector('.request-id').textContent;
    const customerName = item.querySelector('.customer-info strong').textContent;
    
    if (confirm(`Xác nhận chấp nhận yêu cầu ${requestId} của ${customerName}?`)) {
        showLoadingState(true);
        
        setTimeout(() => {
            // Update status
            item.dataset.status = 'in-progress';
            const statusBadge = item.querySelector('.status-badge');
            statusBadge.className = 'status-badge in-progress';
            statusBadge.textContent = 'Đang thực hiện';
            
            // Update action buttons
            const actionButtons = item.querySelector('.action-buttons');
            actionButtons.innerHTML = `
                <button class="btn btn-sm btn-outline-primary" title="Cập nhật">
                    <i class="fas fa-edit"></i>
                </button>
                <button class="btn btn-sm btn-outline-info" title="Chi tiết">
                    <i class="fas fa-eye"></i>
                </button>
                <button class="btn btn-sm btn-outline-success" title="Hoàn thành">
                    <i class="fas fa-check-circle"></i>
                </button>
            `;
            
            initRequestActions(item);
            
            showLoadingState(false);
            showNotification(`Đã chấp nhận yêu cầu ${requestId}`, 'success');
            updateServiceStats();
        }, 1500);
    }
}

function rejectServiceRequest(item) {
    const requestId = item.querySelector('.request-id').textContent;
    const customerName = item.querySelector('.customer-info strong').textContent;
    
    const reason = prompt(`Lý do từ chối yêu cầu ${requestId} của ${customerName}:`);
    if (reason) {
        showLoadingState(true);
        
        setTimeout(() => {
            // Update status
            item.dataset.status = 'cancelled';
            const statusBadge = item.querySelector('.status-badge');
            statusBadge.className = 'status-badge cancelled';
            statusBadge.textContent = 'Đã hủy';
            
            // Update action buttons
            const actionButtons = item.querySelector('.action-buttons');
            actionButtons.innerHTML = `
                <button class="btn btn-sm btn-outline-info" title="Chi tiết">
                    <i class="fas fa-eye"></i>
                </button>
                <button class="btn btn-sm btn-outline-secondary" title="Lý do hủy">
                    <i class="fas fa-info-circle"></i>
                </button>
                <button class="btn btn-sm btn-outline-success" title="Khôi phục">
                    <i class="fas fa-undo"></i>
                </button>
            `;
            
            initRequestActions(item);
            
            showLoadingState(false);
            showNotification(`Đã từ chối yêu cầu ${requestId}. Lý do: ${reason}`, 'warning');
            updateServiceStats();
        }, 1500);
    }
}

function updateServiceRequest(item) {
    const requestId = item.querySelector('.request-id').textContent;
    showNotification(`Cập nhật trạng thái yêu cầu ${requestId}`, 'info');
    
    // In a real application, this would open an update modal
    setTimeout(() => {
        showNotification(`Đã cập nhật yêu cầu ${requestId}`, 'success');
    }, 2000);
}

function completeServiceRequest(item) {
    const requestId = item.querySelector('.request-id').textContent;
    const customerName = item.querySelector('.customer-info strong').textContent;
    
    if (confirm(`Xác nhận hoàn thành yêu cầu ${requestId} của ${customerName}?`)) {
        showLoadingState(true);
        
        setTimeout(() => {
            // Update status
            item.dataset.status = 'completed';
            const statusBadge = item.querySelector('.status-badge');
            statusBadge.className = 'status-badge completed';
            statusBadge.textContent = 'Hoàn thành';
            
            // Update action buttons
            const actionButtons = item.querySelector('.action-buttons');
            actionButtons.innerHTML = `
                <button class="btn btn-sm btn-outline-info" title="Chi tiết">
                    <i class="fas fa-eye"></i>
                </button>
                <button class="btn btn-sm btn-outline-secondary" title="Hóa đơn">
                    <i class="fas fa-receipt"></i>
                </button>
                <button class="btn btn-sm btn-outline-warning" title="Đánh giá">
                    <i class="fas fa-star"></i>
                </button>
            `;
            
            initRequestActions(item);
            
            showLoadingState(false);
            showNotification(`Đã hoàn thành yêu cầu ${requestId}`, 'success');
            updateServiceStats();
        }, 1500);
    }
}

function viewServiceRequestDetail(item) {
    const requestId = item.querySelector('.request-id').textContent;
    const serviceName = item.querySelector('h5').textContent;
    const customerName = item.querySelector('.customer-info strong').textContent;
    const customerRoom = item.querySelector('.customer-info').textContent.match(/Phòng (\d+)/)?.[1] || 'N/A';
    const requestTime = item.querySelector('.request-time').textContent;
    const requestNote = item.querySelector('.request-note').textContent;
    const price = item.querySelector('.request-price').textContent;
    const status = item.querySelector('.status-badge').textContent;
    const category = item.querySelector('.request-category').textContent.trim();
    
    const modalContent = `
        <div class="service-detail-section">
            <div class="row">
                <div class="col-md-8">
                    <h6>Thông tin yêu cầu</h6>
                    <div class="detail-row">
                        <span class="detail-label">Mã yêu cầu:</span>
                        <span class="detail-value">${requestId}</span>
                    </div>
                    <div class="detail-row">
                        <span class="detail-label">Tên dịch vụ:</span>
                        <span class="detail-value">${serviceName}</span>
                    </div>
                    <div class="detail-row">
                        <span class="detail-label">Danh mục:</span>
                        <span class="detail-value">${category}</span>
                    </div>
                    <div class="detail-row">
                        <span class="detail-label">Trạng thái:</span>
                        <span class="detail-value">${status}</span>
                    </div>
                    <div class="detail-row">
                        <span class="detail-label">Giá dịch vụ:</span>
                        <span class="detail-value">${price}</span>
                    </div>
                </div>
                <div class="col-md-4">
                    <h6>Thông tin khách hàng</h6>
                    <div class="detail-row">
                        <span class="detail-label">Họ tên:</span>
                        <span class="detail-value">${customerName}</span>
                    </div>
                    <div class="detail-row">
                        <span class="detail-label">Phòng:</span>
                        <span class="detail-value">${customerRoom}</span>
                    </div>
                    <div class="detail-row">
                        <span class="detail-label">Điện thoại:</span>
                        <span class="detail-value">0901234567</span>
                    </div>
                </div>
            </div>
        </div>
        
        <div class="service-detail-section">
            <h6>Chi tiết yêu cầu</h6>
            <div class="detail-row">
                <span class="detail-label">Thời gian yêu cầu:</span>
                <span class="detail-value">${requestTime.replace('Yêu cầu lúc: ', '').replace('Đã bắt đầu: ', '').replace('Hoàn thành lúc: ', '')}</span>
            </div>
            <div class="detail-row">
                <span class="detail-label">Ghi chú:</span>
                <span class="detail-value">${requestNote.replace('Khách yêu cầu massage thư giãn toàn thân, ưu tiên nhân viên nữ', '').replace(/^[^:]*:\s*/, '')}</span>
            </div>
        </div>
        
        <div class="service-detail-section">
            <h6>Lịch sử xử lý</h6>
            <div class="timeline">
                <div class="timeline-item">
                    <div class="timeline-time">14:30</div>
                    <div class="timeline-content">
                        <strong>Yêu cầu được tạo</strong>
                        <p>Khách hàng gửi yêu cầu dịch vụ</p>
                    </div>
                </div>
                <div class="timeline-item">
                    <div class="timeline-time">14:32</div>
                    <div class="timeline-content">
                        <strong>Đã tiếp nhận</strong>
                        <p>Nhân viên lễ tân xác nhận yêu cầu</p>
                    </div>
                </div>
                <div class="timeline-item">
                    <div class="timeline-time">14:35</div>
                    <div class="timeline-content">
                        <strong>Đang xử lý</strong>
                        <p>Chuyển yêu cầu đến bộ phận liên quan</p>
                    </div>
                </div>
            </div>
        </div>
    `;
    
    const modal = document.getElementById('serviceDetailModal');
    const modalContent_el = document.getElementById('serviceDetailContent');
    modalContent_el.innerHTML = modalContent;
    
    const bootstrapModal = new bootstrap.Modal(modal);
    bootstrapModal.show();
}

function contactCustomer(item) {
    const customerName = item.querySelector('.customer-info strong').textContent;
    const requestId = item.querySelector('.request-id').textContent;
    
    showNotification(`Đang liên hệ với ${customerName} về yêu cầu ${requestId}`, 'info');
    
    setTimeout(() => {
        showNotification(`Đã liên hệ thành công với ${customerName}`, 'success');
    }, 2000);
}

// Modal functionality
function initModals() {
    const newServiceRequestBtn = document.getElementById('new-service-request-btn');
    const newServiceRequestModal = document.getElementById('newServiceRequestModal');
    const createServiceRequestBtn = document.getElementById('create-service-request-btn');
    
    if (newServiceRequestBtn && newServiceRequestModal) {
        newServiceRequestBtn.addEventListener('click', function() {
            const modal = new bootstrap.Modal(newServiceRequestModal);
            modal.show();
        });
    }
    
    if (createServiceRequestBtn) {
        createServiceRequestBtn.addEventListener('click', createNewServiceRequest);
    }
}

function createNewServiceRequest() {
    const form = document.getElementById('new-service-request-form');
    const formData = new FormData(form);
    
    // Validate required fields
    const requiredFields = ['customer', 'category', 'service_name'];
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
        showNotification('Vui lòng điền đầy đủ thông tin bắt buộc', 'error');
        return;
    }
    
    // Simulate service request creation
    showLoadingState(true);
    
    setTimeout(() => {
        const requestId = generateServiceRequestId();
        
        // Add new service request to list
        addServiceRequestToList({
            id: requestId,
            customer: formData.get('customer'),
            category: formData.get('category'),
            serviceName: formData.get('service_name'),
            priority: formData.get('priority'),
            price: formData.get('price') || '50',
            description: formData.get('description'),
            notes: formData.get('notes')
        });
        
        showLoadingState(false);
        
        // Close modal
        const modal = bootstrap.Modal.getInstance(document.getElementById('newServiceRequestModal'));
        modal.hide();
        
        // Reset form
        form.reset();
        
        showNotification(`Yêu cầu dịch vụ ${requestId} đã được tạo thành công!`, 'success');
        
        // Update stats
        updateServiceStats();
        
    }, 2000);
}

function generateServiceRequestId() {
    const year = new Date().getFullYear();
    const random = Math.floor(Math.random() * 10000).toString().padStart(3, '0');
    return `#SV${year}${random}`;
}

function addServiceRequestToList(requestData) {
    const requestsList = document.querySelector('.requests-list');
    const categoryNames = {
        'spa': 'Spa & Massage',
        'restaurant': 'Nhà hàng',
        'room-service': 'Room Service',
        'laundry': 'Giặt ủi',
        'transport': 'Đưa đón',
        'maintenance': 'Bảo trì'
    };
    
    const categoryIcons = {
        'spa': 'fas fa-spa',
        'restaurant': 'fas fa-utensils',
        'room-service': 'fas fa-concierge-bell',
        'laundry': 'fas fa-tshirt',
        'transport': 'fas fa-car',
        'maintenance': 'fas fa-tools'
    };
    
    const customerInfo = requestData.customer.split(' - ');
    const customerName = customerInfo[0];
    const customerRoom = customerInfo[1] || 'Phòng không xác định';
    
    const requestItem = document.createElement('div');
    requestItem.className = 'request-item';
    requestItem.dataset.status = 'pending';
    requestItem.dataset.category = requestData.category;
    requestItem.dataset.priority = requestData.priority;
    
    requestItem.innerHTML = `
        <div class="request-info">
            <div class="request-header">
                <span class="request-id">${requestData.id}</span>
                <span class="request-category ${requestData.category}">
                    <i class="${categoryIcons[requestData.category]}"></i> ${categoryNames[requestData.category]}
                </span>
                <span class="priority-badge ${requestData.priority}">${getPriorityText(requestData.priority)}</span>
            </div>
            <h5>${requestData.serviceName}</h5>
            <div class="request-details">
                <div class="customer-info">
                    <i class="fas fa-user"></i>
                    <strong>${customerName}</strong> - ${customerRoom}
                </div>
                <div class="request-time">
                    <i class="fas fa-clock"></i>
                    Yêu cầu lúc: ${new Date().toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' })}
                </div>
                <div class="request-note">
                    <i class="fas fa-comment"></i>
                    ${requestData.description || 'Không có mô tả'}
                </div>
            </div>
        </div>
        <div class="request-actions">
            <div class="request-price">$${requestData.price}</div>
            <div class="action-buttons">
                <button class="btn btn-sm btn-outline-success" title="Chấp nhận">
                    <i class="fas fa-check"></i>
                </button>
                <button class="btn btn-sm btn-outline-info" title="Chi tiết">
                    <i class="fas fa-eye"></i>
                </button>
                <button class="btn btn-sm btn-outline-danger" title="Từ chối">
                    <i class="fas fa-times"></i>
                </button>
            </div>
            <div class="status-badge pending">Chờ xử lý</div>
        </div>
    `;
    
    // Insert at the top of the list
    requestsList.insertBefore(requestItem, requestsList.firstChild);
    
    // Add event listeners to new buttons
    initRequestActions(requestItem);
}

function getPriorityText(priority) {
    const priorityTexts = {
        'urgent': 'Khẩn cấp',
        'high': 'Cao',
        'normal': 'Bình thường',
        'low': 'Thấp'
    };
    return priorityTexts[priority] || 'Bình thường';
}

// Performance charts
function initPerformanceCharts() {
    // Animate chart bars on load
    const chartFills = document.querySelectorAll('.chart-fill');
    
    setTimeout(() => {
        chartFills.forEach(fill => {
            const width = fill.style.width;
            fill.style.width = '0%';
            setTimeout(() => {
                fill.style.width = width;
            }, 100);
        });
    }, 500);
}

// Update service statistics
function updateServiceStats() {
    const statCards = document.querySelectorAll('.service-stats-section .stat-card');
    
    statCards.forEach(card => {
        const currentValue = parseInt(card.querySelector('h3').textContent.replace(/[\$,]/g, ''));
        let change = 0;
        
        if (card.classList.contains('pending-requests')) {
            change = Math.floor(Math.random() * 3) - 1; // Random change
        } else if (card.classList.contains('completed-today')) {
            change = 1; // Increment completed
        } else if (card.classList.contains('revenue-today')) {
            change = Math.floor(Math.random() * 100); // Random revenue increase
        }
        
        const newValue = Math.max(0, currentValue + change);
        const valueElement = card.querySelector('h3');
        
        if (card.classList.contains('revenue-today')) {
            valueElement.textContent = `$${newValue.toLocaleString()}`;
        } else {
            valueElement.textContent = newValue.toString();
        }
    });
    
    // Update category request counts
    const categoryCards = document.querySelectorAll('.category-card');
    categoryCards.forEach(card => {
        const requestCount = parseInt(card.querySelector('.requests-count').textContent);
        const change = Math.floor(Math.random() * 3) - 1;
        const newCount = Math.max(0, requestCount + change);
        card.querySelector('.requests-count').textContent = newCount;
        
        // Update description
        const infoP = card.querySelector('.category-info p');
        if (infoP.textContent.includes('yêu cầu')) {
            infoP.textContent = `${newCount} yêu cầu đang chờ`;
        } else if (infoP.textContent.includes('đơn')) {
            infoP.textContent = `${newCount} đơn hàng đang xử lý`;
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