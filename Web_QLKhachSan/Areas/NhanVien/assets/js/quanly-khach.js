// Guest Management JavaScript
document.addEventListener('DOMContentLoaded', function() {
    console.log('Guest Management System Loaded');
    
    // Initialize the guest management system
    initGuestManagement();
});

function initGuestManagement() {
    initSearch();
    initFilters();
    initViewToggle();
    initModals();
    initGuestActions();
    initQuickActions();
}

// Search and filter functionality
function initSearch() {
    const searchInput = document.getElementById('guest-search');
    if (searchInput) {
        let searchTimeout;
        searchInput.addEventListener('input', function() {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(() => {
                filterGuests();
            }, 300);
        });
    }
}

function initFilters() {
    const filters = ['guest-status-filter', 'member-tier-filter', 'nationality-filter', 'sort-filter'];
    
    filters.forEach(filterId => {
        const filterElement = document.getElementById(filterId);
        if (filterElement) {
            filterElement.addEventListener('change', filterGuests);
        }
    });

    // Clear filters button
    const clearFiltersBtn = document.getElementById('clear-guest-filters');
    if (clearFiltersBtn) {
        clearFiltersBtn.addEventListener('click', clearAllFilters);
    }
}

function filterGuests() {
    const searchTerm = document.getElementById('guest-search').value.toLowerCase();
    const statusFilter = document.getElementById('guest-status-filter').value;
    const tierFilter = document.getElementById('member-tier-filter').value;
    const nationalityFilter = document.getElementById('nationality-filter').value;
    const sortFilter = document.getElementById('sort-filter').value;
    
    const guestCards = document.querySelectorAll('.guest-card');
    let visibleCards = [];
    
    guestCards.forEach(card => {
        let showCard = true;
        
        // Search filter
        if (searchTerm) {
            const guestName = card.querySelector('h5').textContent.toLowerCase();
            const guestContact = card.querySelector('.guest-contact').textContent.toLowerCase();
            
            if (!guestName.includes(searchTerm) && !guestContact.includes(searchTerm)) {
                showCard = false;
            }
        }
        
        // Status filter
        if (statusFilter && showCard) {
            const statusIndicator = card.querySelector('.guest-status');
            if (statusFilter === 'staying' && !statusIndicator.classList.contains('staying')) {
                showCard = false;
            } else if (statusFilter === 'checked-out' && !statusIndicator.classList.contains('checked-out')) {
                showCard = false;
            }
        }
        
        // Member tier filter
        if (tierFilter && showCard) {
            const memberTier = card.querySelector('.member-tier');
            if (!memberTier.classList.contains(tierFilter)) {
                showCard = false;
            }
        }
        
        card.style.display = showCard ? '' : 'none';
        
        if (showCard) {
            visibleCards.push(card);
        }
    });
    
    // Apply sorting
    if (sortFilter) {
        applySorting(visibleCards, sortFilter);
    }
    
    updateGuestStats(visibleCards.length);
}

function applySorting(cards, sortType) {
    const container = document.querySelector('.guests-grid');
    
    cards.sort((a, b) => {
        switch (sortType) {
            case 'name':
                const nameA = a.querySelector('h5').textContent;
                const nameB = b.querySelector('h5').textContent;
                return nameA.localeCompare(nameB);
            
            case 'checkin':
                // Simulated sorting by check-in date
                return Math.random() - 0.5;
            
            case 'spending':
                // Simulated sorting by spending
                return Math.random() - 0.5;
            
            case 'visits':
                // Simulated sorting by visits
                return Math.random() - 0.5;
            
            default:
                return 0;
        }
    });
    
    // Re-append sorted cards
    cards.forEach(card => {
        container.appendChild(card);
    });
}

function clearAllFilters() {
    document.getElementById('guest-search').value = '';
    document.getElementById('guest-status-filter').value = '';
    document.getElementById('member-tier-filter').value = '';
    document.getElementById('nationality-filter').value = '';
    document.getElementById('sort-filter').value = 'name';
    
    filterGuests();
    showNotification('Đã xóa tất cả bộ lọc', 'info');
}

function updateGuestStats(visibleCount) {
    const totalGuests = document.querySelectorAll('.guest-card').length;
    const paginationInfo = document.querySelector('.pagination-info');
    if (paginationInfo) {
        paginationInfo.textContent = `Hiển thị 1-${visibleCount} của ${totalGuests} khách hàng`;
    }
}

// View toggle functionality
function initViewToggle() {
    const gridViewBtn = document.getElementById('grid-view-btn');
    const listViewBtn = document.getElementById('list-view-btn');
    const guestsGrid = document.getElementById('guests-grid');
    const guestsTable = document.getElementById('guests-table');
    
    if (gridViewBtn && listViewBtn) {
        gridViewBtn.addEventListener('click', function() {
            this.classList.add('active');
            listViewBtn.classList.remove('active');
            guestsGrid.style.display = 'grid';
            guestsTable.style.display = 'none';
        });
        
        listViewBtn.addEventListener('click', function() {
            this.classList.add('active');
            gridViewBtn.classList.remove('active');
            guestsGrid.style.display = 'none';
            guestsTable.style.display = 'block';
        });
    }
}

// Modal functionality
function initModals() {
    const addGuestBtn = document.getElementById('add-guest-btn');
    const addGuestModal = document.getElementById('addGuestModal');
    const saveGuestBtn = document.getElementById('save-guest-btn');
    
    if (addGuestBtn && addGuestModal) {
        addGuestBtn.addEventListener('click', function() {
            const modal = new bootstrap.Modal(addGuestModal);
            modal.show();
        });
    }
    
    if (saveGuestBtn) {
        saveGuestBtn.addEventListener('click', saveNewGuest);
    }
}

function saveNewGuest() {
    const form = document.getElementById('add-guest-form');
    const formData = new FormData(form);
    
    // Validate required fields
    const requiredFields = ['full_name', 'phone', 'id_number'];
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
    
    // Simulate guest creation
    showLoadingState(true);
    
    setTimeout(() => {
        // Add new guest to grid
        addGuestToGrid({
            name: formData.get('full_name'),
            phone: formData.get('phone'),
            email: formData.get('email'),
            memberTier: formData.get('member_tier'),
            gender: formData.get('gender')
        });
        
        showLoadingState(false);
        
        // Close modal
        const modal = bootstrap.Modal.getInstance(document.getElementById('addGuestModal'));
        modal.hide();
        
        // Reset form
        form.reset();
        
        showNotification(`Đã thêm khách hàng ${formData.get('full_name')} thành công!`, 'success');
        
        // Update stats
        updateDashboardStats();
        
    }, 2000);
}

function addGuestToGrid(guestData) {
    const guestsGrid = document.querySelector('.guests-grid');
    const avatarUrl = guestData.gender === 'female' 
        ? `https://randomuser.me/api/portraits/women/${Math.floor(Math.random() * 90)}.jpg`
        : `https://randomuser.me/api/portraits/men/${Math.floor(Math.random() * 90)}.jpg`;
    
    const guestCard = document.createElement('div');
    guestCard.className = 'guest-card';
    guestCard.innerHTML = `
        <div class="guest-avatar">
            <img src="${avatarUrl}" alt="Guest">
            <div class="guest-status checked-out"></div>
        </div>
        <div class="guest-info">
            <h5>${guestData.name}</h5>
            <p class="guest-contact">
                <i class="fas fa-phone"></i> ${guestData.phone}<br>
                ${guestData.email ? `<i class="fas fa-envelope"></i> ${guestData.email}` : ''}
            </p>
            <div class="guest-details">
                <span class="member-tier ${guestData.memberTier}">
                    <i class="fas fa-user"></i> ${guestData.memberTier}
                </span>
                <span class="room-info">
                    Khách mới
                </span>
            </div>
        </div>
        <div class="guest-actions">
            <button class="btn btn-sm btn-outline-primary" title="Xem chi tiết">
                <i class="fas fa-eye"></i>
            </button>
            <button class="btn btn-sm btn-outline-success" title="Đặt phòng">
                <i class="fas fa-calendar-plus"></i>
            </button>
            <button class="btn btn-sm btn-outline-warning" title="Chỉnh sửa">
                <i class="fas fa-edit"></i>
            </button>
        </div>
    `;
    
    // Insert at the beginning of the grid
    guestsGrid.insertBefore(guestCard, guestsGrid.firstChild);
    
    // Add event listeners to new buttons
    initCardActions(guestCard);
}

// Guest actions
function initGuestActions() {
    const guestCards = document.querySelectorAll('.guest-card');
    guestCards.forEach(card => initCardActions(card));
}

function initCardActions(card) {
    const viewBtn = card.querySelector('.btn-outline-primary');
    const checkoutBtn = card.querySelector('.btn-outline-success');
    const serviceBtn = card.querySelector('.btn-outline-info');
    const editBtn = card.querySelector('.btn-outline-warning');
    const historyBtn = card.querySelector('.btn-outline-secondary');
    
    if (viewBtn) {
        viewBtn.addEventListener('click', () => viewGuestDetails(card));
    }
    
    if (checkoutBtn) {
        checkoutBtn.addEventListener('click', () => {
            if (checkoutBtn.title === 'Check-out') {
                checkOutGuest(card);
            } else {
                // It's a booking button for new guests
                showNotification('Chức năng đặt phòng sẽ được chuyển hướng đến trang quản lý đặt phòng', 'info');
            }
        });
    }
    
    if (serviceBtn) {
        serviceBtn.addEventListener('click', () => manageGuestServices(card));
    }
    
    if (editBtn) {
        editBtn.addEventListener('click', () => editGuest(card));
    }
    
    if (historyBtn) {
        historyBtn.addEventListener('click', () => viewGuestHistory(card));
    }
}

function viewGuestDetails(card) {
    const guestName = card.querySelector('h5').textContent;
    const guestPhone = card.querySelector('.guest-contact').textContent.match(/\d+/)[0];
    const memberTier = card.querySelector('.member-tier').textContent.trim();
    const roomInfo = card.querySelector('.room-info').textContent;
    
    const modalContent = `
        <div class="guest-detail-section">
            <div class="row">
                <div class="col-md-3 text-center">
                    <img src="${card.querySelector('.guest-avatar img').src}" 
                         alt="Guest" class="rounded-circle mb-3" style="width: 120px; height: 120px; object-fit: cover;">
                    <h4>${guestName}</h4>
                    <span class="member-tier ${memberTier.toLowerCase()}">${memberTier}</span>
                </div>
                <div class="col-md-9">
                    <div class="row">
                        <div class="col-md-6">
                            <h6>Thông tin liên hệ</h6>
                            <div class="detail-row">
                                <span class="detail-label">Số điện thoại:</span>
                                <span class="detail-value">${guestPhone}</span>
                            </div>
                            <div class="detail-row">
                                <span class="detail-label">Email:</span>
                                <span class="detail-value">guest@email.com</span>
                            </div>
                            <div class="detail-row">
                                <span class="detail-label">CMND/CCCD:</span>
                                <span class="detail-value">123456789</span>
                            </div>
                            <div class="detail-row">
                                <span class="detail-label">Quốc tịch:</span>
                                <span class="detail-value">Việt Nam</span>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <h6>Thông tin lưu trú</h6>
                            <div class="detail-row">
                                <span class="detail-label">Trạng thái:</span>
                                <span class="detail-value">Đang lưu trú</span>
                            </div>
                            <div class="detail-row">
                                <span class="detail-label">Phòng hiện tại:</span>
                                <span class="detail-value">${roomInfo}</span>
                            </div>
                            <div class="detail-row">
                                <span class="detail-label">Check-in:</span>
                                <span class="detail-value">25/09/2025</span>
                            </div>
                            <div class="detail-row">
                                <span class="detail-label">Check-out dự kiến:</span>
                                <span class="detail-value">27/09/2025</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        
        <div class="guest-detail-section">
            <h6>Lịch sử lưu trú</h6>
            <div class="visit-history-item">
                <div class="visit-info">
                    <div class="visit-date">20/06/2025 - 23/06/2025</div>
                    <div class="visit-details">Phòng 302 • 3 đêm • Deluxe Ocean View</div>
                </div>
                <div class="visit-amount">$450</div>
            </div>
            <div class="visit-history-item">
                <div class="visit-info">
                    <div class="visit-date">15/03/2025 - 18/03/2025</div>
                    <div class="visit-details">Phòng 405 • 3 đêm • Executive Suite</div>
                </div>
                <div class="visit-amount">$750</div>
            </div>
            <div class="visit-history-item">
                <div class="visit-info">
                    <div class="visit-date">10/12/2024 - 14/12/2024</div>
                    <div class="visit-details">Phòng 108 • 4 đêm • Family Bungalow</div>
                </div>
                <div class="visit-amount">$1,280</div>
            </div>
        </div>
        
        <div class="guest-detail-section">
            <h6>Thống kê</h6>
            <div class="row">
                <div class="col-md-3">
                    <div class="detail-row">
                        <span class="detail-label">Tổng lần lưu trú:</span>
                        <span class="detail-value">4</span>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="detail-row">
                        <span class="detail-label">Tổng số đêm:</span>
                        <span class="detail-value">12</span>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="detail-row">
                        <span class="detail-label">Tổng chi tiêu:</span>
                        <span class="detail-value">$2,780</span>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="detail-row">
                        <span class="detail-label">Điểm thưởng:</span>
                        <span class="detail-value">2,780</span>
                    </div>
                </div>
            </div>
        </div>
    `;
    
    const modal = document.getElementById('guestDetailModal');
    const modalContent_el = document.getElementById('guestDetailContent');
    modalContent_el.innerHTML = modalContent;
    
    const bootstrapModal = new bootstrap.Modal(modal);
    bootstrapModal.show();
}

function checkOutGuest(card) {
    const guestName = card.querySelector('h5').textContent;
    const guestStatus = card.querySelector('.guest-status');
    const roomInfo = card.querySelector('.room-info');
    const actionButtons = card.querySelector('.guest-actions');
    
    if (confirm(`Xác nhận check-out cho khách ${guestName}?`)) {
        showLoadingState(true);
        
        setTimeout(() => {
            guestStatus.className = 'guest-status checked-out';
            roomInfo.textContent = 'Đã check-out • ' + new Date().toLocaleDateString('vi-VN');
            
            // Update action buttons
            actionButtons.innerHTML = `
                <button class="btn btn-sm btn-outline-primary" title="Xem chi tiết">
                    <i class="fas fa-eye"></i>
                </button>
                <button class="btn btn-sm btn-outline-secondary" title="Lịch sử">
                    <i class="fas fa-history"></i>
                </button>
                <button class="btn btn-sm btn-outline-warning" title="Đặt lại">
                    <i class="fas fa-redo"></i>
                </button>
            `;
            
            initCardActions(card);
            
            showLoadingState(false);
            showNotification(`Check-out thành công cho khách ${guestName}`, 'success');
            updateDashboardStats();
        }, 1500);
    }
}

function manageGuestServices(card) {
    const guestName = card.querySelector('h5').textContent;
    showNotification(`Chức năng quản lý dịch vụ cho ${guestName} sẽ được chuyển hướng đến trang quản lý dịch vụ`, 'info');
}

function editGuest(card) {
    const guestName = card.querySelector('h5').textContent;
    showNotification(`Chức năng chỉnh sửa thông tin cho ${guestName}`, 'info');
}

function viewGuestHistory(card) {
    const guestName = card.querySelector('h5').textContent;
    viewGuestDetails(card); // Reuse the detail modal which includes history
}

// Quick actions
function initQuickActions() {
    const quickActionCards = document.querySelectorAll('.quick-action-card');
    
    quickActionCards.forEach(card => {
        const button = card.querySelector('.btn');
        if (button) {
            button.addEventListener('click', function() {
                const actionTitle = card.querySelector('h5').textContent;
                showNotification(`Đang thực hiện: ${actionTitle}`, 'info');
                
                // Simulate action
                setTimeout(() => {
                    showNotification(`Đã hoàn thành: ${actionTitle}`, 'success');
                }, 2000);
            });
        }
    });
    
    // Export functionality
    const exportBtn = document.getElementById('export-guests-btn');
    if (exportBtn) {
        exportBtn.addEventListener('click', exportGuestData);
    }
}

function exportGuestData() {
    showNotification('Đang xuất dữ liệu khách hàng...', 'info');
    
    setTimeout(() => {
        showNotification('Đã xuất danh sách khách hàng thành công', 'success');
    }, 2000);
}

// Update dashboard statistics
function updateDashboardStats() {
    const statCards = document.querySelectorAll('.guest-stats-section .stat-card');
    
    statCards.forEach(card => {
        const currentValue = parseInt(card.querySelector('h3').textContent.replace(/,/g, ''));
        let change = 0;
        
        if (card.classList.contains('total-guests')) {
            change = 1; // New guest added
        } else if (card.classList.contains('current-guests')) {
            // Check if any guests were checked out
            const stayingGuests = document.querySelectorAll('.guest-status.staying').length;
            change = stayingGuests - currentValue;
        } else if (card.classList.contains('checkin-today')) {
            // Random change for demo
            change = Math.floor(Math.random() * 3) - 1;
        }
        
        const newValue = Math.max(0, currentValue + change);
        card.querySelector('h3').textContent = newValue.toLocaleString();
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

// showNotification và sidebar functions được sử dụng từ shared.js