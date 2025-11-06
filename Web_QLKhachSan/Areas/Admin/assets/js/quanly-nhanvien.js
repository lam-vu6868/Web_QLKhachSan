// JavaScript quản lý nhân viên - Phong cách giống quản lý tài chính
document.addEventListener('DOMContentLoaded', function() {
    const addEmployeeBtn = document.getElementById('addEmployeeBtn');
    const modal = document.getElementById('employeeModal');
    const closeModalBtn = document.getElementById('closeModalBtn');
    const cancelBtn = document.getElementById('cancelBtn');
    const employeeForm = document.getElementById('employeeForm');
    const modalTitle = document.getElementById('modalTitle');
    const tableBody = document.querySelector('.table tbody');
    
    // Lưu trữ tất cả nhân viên và trạng thái chỉnh sửa
    let allEmployees = [];
    let editingEmployeeId = null;
    
    // Khởi tạo dữ liệu từ bảng hiện có
    initializeEmployees();

    // Hàm hiển thị modal
    const showModal = (isEdit = false) => {
        modal.style.display = 'flex';
        setTimeout(() => modal.classList.add('show'), 10);
        modalTitle.textContent = isEdit ? 'Sửa Thông Tin Nhân Viên' : 'Thêm Nhân Viên Mới';
    };

    // Hàm ẩn modal
    const hideModal = () => {
        modal.classList.remove('show');
        setTimeout(() => {
            modal.style.display = 'none';
            employeeForm.reset();
            editingEmployeeId = null;
        }, 300);
    };

    // Event listeners để mở/đóng modal
    addEmployeeBtn.addEventListener('click', () => showModal(false));
    closeModalBtn.addEventListener('click', hideModal);
    cancelBtn.addEventListener('click', hideModal);
    modal.addEventListener('click', (e) => {
        if (e.target === modal) {
            hideModal();
        }
    });

    // Xử lý submit form
    employeeForm.addEventListener('submit', function(e) {
        e.preventDefault();

        const employeeData = {
            id: editingEmployeeId || 'NV' + String(Math.floor(100 + Math.random() * 900)).padStart(3, '0'),
            name: document.getElementById('employeeName').value,
            role: document.getElementById('employeeRole').value,
            email: document.getElementById('employeeEmail').value,
            phone: document.getElementById('employeePhone').value,
            status: document.getElementById('employeeStatus').value
        };

        if (editingEmployeeId) {
            // Cập nhật nhân viên
            const index = allEmployees.findIndex(emp => emp.id === editingEmployeeId);
            if (index !== -1) {
                allEmployees[index] = employeeData;
                updateEmployeeRow(editingEmployeeId, employeeData);
                showNotification('Cập nhật thông tin nhân viên thành công!', 'success');
            }
        } else {
            // Thêm nhân viên mới
            allEmployees.unshift(employeeData);
            addEmployeeRow(employeeData, true);
            showNotification('Thêm nhân viên mới thành công!', 'success');
        }

        hideModal();
    });

    // Khởi tạo dữ liệu từ bảng hiện có
    function initializeEmployees() {
        const rows = tableBody.querySelectorAll('tr:not(.no-results-row)');
        rows.forEach(row => {
            const statusSpan = row.querySelector('span[class^="status-"]');
            const status = statusSpan.classList.contains('status-active') ? 'active' : 'inactive';
            
            allEmployees.push({
                id: row.cells[0].textContent.trim(),
                name: row.cells[1].textContent.trim(),
                role: row.cells[2].textContent.trim().toLowerCase(),
                email: row.cells[3].textContent.trim(),
                phone: row.cells[4].textContent.trim(),
                status: status
            });
        });
        
        // Thêm event listeners cho các nút hiện có
        attachButtonListeners();
    }

    // Thêm event listeners cho các nút trong bảng
    function attachButtonListeners() {
        tableBody.addEventListener('click', function(e) {
            const button = e.target.closest('.action-icon');
            if (!button) return;

            const row = button.closest('tr');
            const employeeId = row.cells[0].textContent.trim();

            if (button.classList.contains('view')) {
                viewEmployee(employeeId);
            } else if (button.classList.contains('edit')) {
                editEmployee(employeeId);
            } else if (button.classList.contains('delete')) {
                deleteEmployee(employeeId);
            }
        });
    }

    // Hàm thêm dòng mới vào bảng
    function addEmployeeRow(employee, prepend = false) {
        const row = document.createElement('tr');
        
        const statusMap = {
            active: { class: 'status-active', text: 'Hoạt động', icon: 'fa-check-circle' },
            inactive: { class: 'status-inactive', text: 'Nghỉ phép', icon: 'fa-clock' }
        };

        const statusInfo = statusMap[employee.status];
        
        row.innerHTML = `
            <td>${employee.id}</td>
            <td>${employee.name}</td>
            <td>${capitalizeFirst(employee.role)}</td>
            <td>${employee.email}</td>
            <td>${employee.phone}</td>
            <td><span class="${statusInfo.class}"><i class="fas ${statusInfo.icon}"></i> ${statusInfo.text}</span></td>
            <td>
                <div class="actions-cell">
                    <button class="action-icon view" title="Xem">
                        <i class="fas fa-eye"></i>
                    </button>
                    <button class="action-icon edit" title="Sửa">
                        <i class="fas fa-edit"></i>
                    </button>
                    <button class="action-icon delete" title="Xóa">
                        <i class="fas fa-trash"></i>
                    </button>
                </div>
            </td>
        `;

        if (prepend) {
            tableBody.insertBefore(row, tableBody.firstChild);
        } else {
            tableBody.appendChild(row);
        }
    }

    // Hàm cập nhật dòng trong bảng
    function updateEmployeeRow(employeeId, employeeData) {
        const rows = tableBody.querySelectorAll('tr:not(.no-results-row)');
        rows.forEach(row => {
            if (row.cells[0].textContent.trim() === employeeId) {
                const statusMap = {
                    active: { class: 'status-active', text: 'Hoạt động', icon: 'fa-check-circle' },
                    inactive: { class: 'status-inactive', text: 'Nghỉ phép', icon: 'fa-clock' }
                };

                const statusInfo = statusMap[employeeData.status];
                
                row.cells[1].textContent = employeeData.name;
                row.cells[2].textContent = capitalizeFirst(employeeData.role);
                row.cells[3].textContent = employeeData.email;
                row.cells[4].textContent = employeeData.phone;
                row.cells[5].innerHTML = `<span class="${statusInfo.class}"><i class="fas ${statusInfo.icon}"></i> ${statusInfo.text}</span>`;
            }
        });
    }

    // Hàm xem thông tin nhân viên
    window.viewEmployee = function(employeeId) {
        const employee = allEmployees.find(emp => emp.id === employeeId);
        if (employee) {
            const statusText = employee.status === 'active' ? 'Hoạt động' : 'Nghỉ phép';
            alert(`Thông tin nhân viên:\n\nID: ${employee.id}\nTên: ${employee.name}\nChức vụ: ${capitalizeFirst(employee.role)}\nEmail: ${employee.email}\nSố điện thoại: ${employee.phone}\nTrạng thái: ${statusText}`);
        }
    };

    // Hàm sửa nhân viên
    window.editEmployee = function(employeeId) {
        const employee = allEmployees.find(emp => emp.id === employeeId);
        if (employee) {
            editingEmployeeId = employeeId;
            document.getElementById('employeeName').value = employee.name;
            document.getElementById('employeeRole').value = employee.role;
            document.getElementById('employeeEmail').value = employee.email;
            document.getElementById('employeePhone').value = employee.phone;
            document.getElementById('employeeStatus').value = employee.status;
            showModal(true);
        }
    };

    // Hàm xóa nhân viên
    window.deleteEmployee = function(employeeId) {
        const employee = allEmployees.find(emp => emp.id === employeeId);
        if (employee && confirm(`Bạn có chắc chắn muốn xóa nhân viên ${employee.name} không?`)) {
            // Xóa khỏi mảng
            allEmployees = allEmployees.filter(emp => emp.id !== employeeId);
            
            // Xóa khỏi bảng
            const rows = tableBody.querySelectorAll('tr:not(.no-results-row)');
            rows.forEach(row => {
                if (row.cells[0].textContent.trim() === employeeId) {
                    row.remove();
                }
            });
            
            showNotification('Đã xóa nhân viên thành công!', 'success');
        }
    };

    // ======== HÀM TIỆN ÍCH ========
    function capitalizeFirst(str) {
        return str.charAt(0).toUpperCase() + str.slice(1);
    }

    function showNotification(message, type = 'info') {
        const notification = document.createElement('div');
        notification.className = 'notification';
        notification.style.cssText = `
            position: fixed;
            top: 100px;
            right: 20px;
            padding: 1rem 1.5rem;
            background: ${type === 'success' ? '#10b981' : type === 'error' ? '#ef4444' : '#3b82f6'};
            color: white;
            border-radius: 10px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.2);
            z-index: 9999;
            animation: slideInRight 0.3s ease;
            font-weight: 500;
            min-width: 300px;
        `;
        notification.innerHTML = `
            <i class="fas fa-${type === 'success' ? 'check-circle' : type === 'error' ? 'exclamation-circle' : 'info-circle'}"></i>
            ${message}
        `;
        document.body.appendChild(notification);

        setTimeout(() => {
            notification.style.animation = 'slideOutRight 0.3s ease';
            setTimeout(() => notification.remove(), 300);
        }, 3000);
    }

    // Add animation styles
    const style = document.createElement('style');
    style.textContent = `
        @keyframes slideInRight {
            from { transform: translateX(100%); opacity: 0; }
            to { transform: translateX(0); opacity: 1; }
        }
        @keyframes slideOutRight {
            from { transform: translateX(0); opacity: 1; }
            to { transform: translateX(100%); opacity: 0; }
        }
    `;
    document.head.appendChild(style);
});
