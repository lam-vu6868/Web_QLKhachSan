// ==================== PROFILE EDIT FUNCTIONALITY ====================
// Lưu trữ giá trị ban đầu của các trường khi bắt đầu chỉnh sửa
const originalValues = {};

// Hàm để chỉnh sửa từng trường
function editField(fieldId) {
  console.log('✏️ Editing field:', fieldId);
  
  const input = document.getElementById(fieldId);
  const button = input.parentElement.querySelector('.btn-edit');
  
  if (!input || !button) {
    console.error('❌ Input or button not found for:', fieldId);
    return;
  }
  
  // Kiểm tra trạng thái hiện tại
  const isReadOnly = input.hasAttribute('readonly');
  
  if (isReadOnly) {
    // Chuyển sang chế độ chỉnh sửa
    console.log('📝 Switching to edit mode');
    
    // Lưu giá trị ban đầu
    originalValues[fieldId] = input.value;
    
    // Bỏ readonly và focus vào input
    input.removeAttribute('readonly');
    input.focus();
    
    // Đổi nút thành "Lưu"
    button.innerHTML = '<i class="fas fa-save"></i> Lưu';
    button.style.background = 'linear-gradient(135deg, #28a745 0%, #20c997 100%)';
    button.style.boxShadow = '0 2px 8px rgba(40, 167, 69, 0.3)';
    
  } else {
    // Lưu thay đổi và quay về chế độ readonly
    console.log('💾 Saving changes');
    
    // Thêm hiệu ứng lưu
    input.setAttribute('readonly', 'readonly');
    
    // Hiển thị thông báo lưu thành công
    showSaveNotification(fieldId);
    
    // Reset nút về trạng thái ban đầu
    resetButton(fieldId, button);
    
    // Xóa giá trị lưu trữ
    delete originalValues[fieldId];
  }
}

// Hàm reset nút về trạng thái ban đầu
function resetButton(fieldId, button) {
  const buttonConfigs = {
    fullName: { icon: 'edit', text: 'Đổi Tên' },
    email: { icon: 'envelope', text: 'Đổi Email' },
    phone: { icon: 'phone', text: 'Thêm SĐT' },
    birthdate: { icon: 'calendar', text: 'Đổi Ngày Sinh' },
    address: { icon: 'map-marker-alt', text: 'Thêm Địa Chỉ' }
  };
  
  const config = buttonConfigs[fieldId] || { icon: 'edit', text: 'Chỉnh Sửa' };
  
  button.innerHTML = `<i class="fas fa-${config.icon}"></i> ${config.text}`;
  button.style.background = '';
  button.style.boxShadow = '';
}

// Hàm hiển thị thông báo lưu thành công
function showSaveNotification(fieldId) {
  const input = document.getElementById(fieldId);
  const container = input.parentElement.parentElement;
  
  // Tạo thông báo
  const notification = document.createElement('div');
  notification.className = 'save-notification';
  notification.innerHTML = '<i class="fas fa-check-circle"></i> Đã lưu thay đổi';
  notification.style.cssText = `
    position: absolute;
    top: -30px;
    right: 0;
    background: #28a745;
    color: white;
    padding: 8px 16px;
    border-radius: 6px;
    font-size: 0.85rem;
    font-weight: 600;
    display: flex;
    align-items: center;
    gap: 6px;
    animation: slideDown 0.3s ease;
    box-shadow: 0 4px 12px rgba(40, 167, 69, 0.3);
    z-index: 10;
  `;
  
  container.style.position = 'relative';
  container.appendChild(notification);
  
  // Xóa thông báo sau 2 giây
  setTimeout(() => {
    notification.style.animation = 'slideUp 0.3s ease';
    setTimeout(() => notification.remove(), 300);
  }, 2000);
}

// Hàm lưu tất cả thay đổi (nút Lưu Thay Đổi lớn)
function saveAllChanges() {
  console.log('💾 Saving all changes');
  
  const fields = ['fullName', 'email', 'phone', 'birthdate', 'address'];
  let hasChanges = false;
  
  fields.forEach(fieldId => {
    const input = document.getElementById(fieldId);
    if (input && !input.hasAttribute('readonly')) {
      hasChanges = true;
      const button = input.parentElement.querySelector('.btn-edit');
      
      // Lưu trường này
      input.setAttribute('readonly', 'readonly');
      resetButton(fieldId, button);
      delete originalValues[fieldId];
    }
  });
  
  if (hasChanges) {
    // Hiển thị thông báo chung
    showGlobalSaveNotification();
  } else {
    // Không có thay đổi
    showNoChangesNotification();
  }
}

// Hiển thị thông báo lưu toàn bộ thành công
function showGlobalSaveNotification() {
  const notification = document.createElement('div');
  notification.className = 'global-notification';
  notification.innerHTML = '<i class="fas fa-check-circle"></i> Đã lưu tất cả thay đổi thành công!';
  notification.style.cssText = `
    position: fixed;
    top: 20px;
    right: 20px;
    background: #28a745;
    color: white;
    padding: 16px 24px;
    border-radius: 10px;
    font-size: 1rem;
    font-weight: 600;
    display: flex;
    align-items: center;
    gap: 10px;
    animation: slideInRight 0.3s ease;
    box-shadow: 0 6px 20px rgba(40, 167, 69, 0.4);
    z-index: 9999;
  `;
  
  document.body.appendChild(notification);
  
  setTimeout(() => {
    notification.style.animation = 'slideOutRight 0.3s ease';
    setTimeout(() => notification.remove(), 300);
  }, 3000);
}

// Hiển thị thông báo không có thay đổi
function showNoChangesNotification() {
  const notification = document.createElement('div');
  notification.className = 'global-notification';
  notification.innerHTML = '<i class="fas fa-info-circle"></i> Không có thay đổi nào để lưu';
  notification.style.cssText = `
    position: fixed;
    top: 20px;
    right: 20px;
    background: #ffc107;
    color: #1a1a1a;
    padding: 16px 24px;
    border-radius: 10px;
    font-size: 1rem;
    font-weight: 600;
    display: flex;
    align-items: center;
    gap: 10px;
    animation: slideInRight 0.3s ease;
    box-shadow: 0 6px 20px rgba(255, 193, 7, 0.4);
    z-index: 9999;
  `;
  
  document.body.appendChild(notification);
  
  setTimeout(() => {
    notification.style.animation = 'slideOutRight 0.3s ease';
    setTimeout(() => notification.remove(), 300);
  }, 3000);
}

// Thêm CSS animations vào document
const style = document.createElement('style');
style.textContent = `
  @keyframes slideDown {
    from {
      opacity: 0;
      transform: translateY(-10px);
    }
    to {
      opacity: 1;
      transform: translateY(0);
    }
  }
  
  @keyframes slideUp {
    from {
      opacity: 1;
      transform: translateY(0);
    }
    to {
      opacity: 0;
      transform: translateY(-10px);
    }
  }
  
  @keyframes slideInRight {
    from {
      opacity: 0;
      transform: translateX(100px);
    }
    to {
      opacity: 1;
      transform: translateX(0);
    }
  }
  
  @keyframes slideOutRight {
    from {
      opacity: 1;
      transform: translateX(0);
    }
    to {
      opacity: 0;
      transform: translateX(100px);
    }
  }
`;
document.head.appendChild(style);

// ==================== CHANGE PASSWORD MODAL ====================
function showChangePasswordModal() {
  const modal = document.getElementById('changePasswordModal');
  if (modal) {
    modal.style.display = 'flex';
    document.body.style.overflow = 'hidden';
    
    // Reset form
    const currentPassword = document.getElementById('currentPassword');
    const newPassword = document.getElementById('newPassword');
    const confirmPassword = document.getElementById('confirmPassword');
    
    if (currentPassword) currentPassword.value = '';
    if (newPassword) newPassword.value = '';
    if (confirmPassword) confirmPassword.value = '';
  }
}

function closeChangePasswordModal() {
  const modal = document.getElementById('changePasswordModal');
  if (modal) {
    modal.style.display = 'none';
    document.body.style.overflow = 'auto';
  }
}

function submitChangePassword() {
  // TODO: Implement password change logic
  showNotification('Chức năng đang được phát triển', 'info');
}

// ==================== LOGIN ACTIVITY MODAL ====================
function showLoginActivityModal() {
  const modal = document.getElementById('loginActivityModal');
  if (modal) {
    modal.style.display = 'flex';
    document.body.style.overflow = 'hidden';
  }
}

function closeLoginActivityModal() {
  const modal = document.getElementById('loginActivityModal');
  if (modal) {
    modal.style.display = 'none';
    document.body.style.overflow = 'auto';
  }
}

// ==================== DANGER ZONE ACTIONS ====================
function confirmDeactivate() {
  if (confirm('⚠️ Bạn có chắc chắn muốn vô hiệu hóa tài khoản?\n\nTài khoản của bạn sẽ tạm thời bị vô hiệu hóa và bạn sẽ không thể đăng nhập cho đến khi kích hoạt lại.')) {
    // TODO: Send to server
    console.log('Deactivating account...');
    showNotification('Tài khoản đã được vô hiệu hóa', 'warning');
  }
}

function confirmDelete() {
  const confirmation = prompt('⚠️ CẢNH BÁO: Hành động này không thể hoàn tác!\n\nNhập "XOA TAI KHOAN" để xác nhận xóa tài khoản:');
  
  if (confirmation === 'XOA TAI KHOAN') {
    if (confirm('Bạn có hoàn toàn chắc chắn? Tất cả dữ liệu sẽ bị xóa vĩnh viễn.')) {
      // TODO: Send to server
      console.log('Deleting account...');
      showNotification('Đang xử lý yêu cầu xóa tài khoản...', 'error');
    }
  } else if (confirmation !== null) {
    showNotification('Xác nhận không chính xác', 'error');
  }
}

// ==================== NOTIFICATION SYSTEM ====================
function showNotification(message, type = 'info') {
  const notification = document.createElement('div');
  notification.className = `notification-toast notification-${type}`;
  
  const icons = {
    success: 'fa-check-circle',
    error: 'fa-exclamation-circle',
    warning: 'fa-exclamation-triangle',
    info: 'fa-info-circle'
  };
  
  const colors = {
    success: '#28a745',
    error: '#dc3545',
    warning: '#ffc107',
    info: '#17a2b8'
  };
  
  notification.innerHTML = `<i class="fas ${icons[type]}"></i> ${message}`;
  notification.style.cssText = `
    position: fixed;
    top: 20px;
    right: 20px;
    background: ${colors[type]};
    color: ${type === 'warning' ? '#1a1a1a' : '#fff'};
    padding: 16px 24px;
    border-radius: 10px;
    font-size: 1rem;
    font-weight: 600;
    display: flex;
    align-items: center;
    gap: 10px;
    animation: slideInRight 0.3s ease;
    box-shadow: 0 6px 20px rgba(0, 0, 0, 0.3);
    z-index: 9999;
  `;
  
  document.body.appendChild(notification);
  
  setTimeout(() => {
    notification.style.animation = 'slideOutRight 0.3s ease';
    setTimeout(() => notification.remove(), 300);
  }, 3000);
}

// ==================== BOOKING DETAILS ====================
function viewBookingDetails(bookingId) {
  console.log('Viewing booking details for:', bookingId);
  showNotification('Đang tải chi tiết đơn hàng...', 'info');
  // TODO: Implement booking details modal or redirect
}

// ==================== MODAL CLOSE ON OUTSIDE CLICK ====================
window.addEventListener('click', (e) => {
  if (e.target.classList.contains('modal-overlay')) {
    closeChangePasswordModal();
    closeLoginActivityModal();
  }
});

// ==================== KEYBOARD SHORTCUTS ====================
document.addEventListener('keydown', (e) => {
  // ESC key to close modals
  if (e.key === 'Escape') {
    closeChangePasswordModal();
    closeLoginActivityModal();
  }
  
  // Enter key to submit password change
  const passwordModal = document.getElementById('changePasswordModal');
  if (e.key === 'Enter' && passwordModal && passwordModal.style.display === 'flex') {
    submitChangePassword();
  }
});


  





  



