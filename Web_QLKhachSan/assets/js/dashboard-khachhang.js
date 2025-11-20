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

// Hàm lưu tất cả thay đổi (nút Lưu Thay Đổi lớn) với validation
function saveAllChanges() {
  console.log('💾 Saving all changes');
  
  // Lấy giá trị từ form
  const fullName = document.getElementById('fullName').value.trim();
  const phone = document.getElementById('phone').value.trim();
  const birthdate = document.getElementById('birthdate').value;
  const address = document.getElementById('address').value.trim();
  
  // Object để lưu lỗi
  const errors = {};
  
  // ===== VALIDATION =====
  
  // 1. Validate Họ và Tên
  if (!fullName) {
    errors.fullName = 'Họ và tên không được để trống';
  } else if (fullName.length > 100) {
    errors.fullName = 'Họ và tên không được quá 100 ký tự';
  } else if (!/^[a-zA-ZÀ-ỹ\s]+$/.test(fullName)) {
    errors.fullName = 'Họ và tên chỉ được chứa chữ cái và khoảng trắng';
  }
  
  // 2. Validate Số Điện Thoại (nếu có nhập)
  if (phone && phone !== '') {
    if (!/^(0[3|5|7|8|9])+([0-9]{8})$/.test(phone)) {
      errors.phone = 'Số điện thoại không hợp lệ (VD: 0912345678)';
    }
  }
  
  // 3. Validate Ngày Sinh - phải trên 18 tuổi
  if (birthdate && birthdate !== '') {
    const birthDate = new Date(birthdate);
    const today = new Date();
    let age = today.getFullYear() - birthDate.getFullYear();
    const monthDiff = today.getMonth() - birthDate.getMonth();
    
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
      age--;
    }
    
    if (age < 18) {
      errors.birthdate = 'Bạn phải từ 18 tuổi trở lên';
    }
    
    if (birthDate > today) {
      errors.birthdate = 'Ngày sinh không được là ngày trong tương lai';
    }
  }
  
  // 4. Validate Địa Chỉ (nếu có nhập)
  if (address && address.length > 500) {
    errors.address = 'Địa chỉ không được quá 500 ký tự';
  }
  
  // ===== HIỂN THỊ LỖI =====
  // Xóa tất cả lỗi cũ
  document.querySelectorAll('.error-message').forEach(el => el.remove());
  document.querySelectorAll('.input-error').forEach(el => el.classList.remove('input-error'));
  
  // Nếu có lỗi, hiển thị và dừng lại
  if (Object.keys(errors).length > 0) {
    for (const [fieldId, errorMsg] of Object.entries(errors)) {
      const input = document.getElementById(fieldId);
      if (input) {
        // Thêm class error cho input
        input.classList.add('input-error');
        
        // Tạo thông báo lỗi
        const errorDiv = document.createElement('div');
        errorDiv.className = 'error-message';
        errorDiv.style.cssText = `
          color: #dc3545;
          font-size: 0.85rem;
          margin-top: 6px;
          font-weight: 500;
          display: flex;
          align-items: center;
          gap: 5px;
        `;
        errorDiv.innerHTML = `<i class="fas fa-exclamation-circle"></i> ${errorMsg}`;
        
        // Thêm vào sau input
        input.parentElement.appendChild(errorDiv);
      }
    }
    
    // Hiển thị thông báo lỗi chung
    showNotification('Vui lòng kiểm tra lại thông tin!', 'error');
    return;
  }
  
  // ===== GỬI DỮ LIỆU LÊN SERVER =====
  // Hiển thị loading
  const btnSave = document.querySelector('.btn-save-all');
  const originalBtnText = btnSave.innerHTML;
  btnSave.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Đang lưu...';
  btnSave.disabled = true;
  
  // Gửi Ajax request
  fetch('/DashboardKhachHang/CapNhatThongTin', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/x-www-form-urlencoded',
    },
    body: new URLSearchParams({
      hoVaTen: fullName,
      soDienThoai: phone,
      ngaySinh: birthdate,
      diaChi: address
    })
  })
  .then(response => response.json())
  .then(data => {
    if (data.success) {
      // Cập nhật session name trên UI
      const welcomeName = document.getElementById('welcomeName');
      const profileName = document.getElementById('profileName');
      if (welcomeName) welcomeName.textContent = fullName;
      if (profileName) profileName.textContent = fullName;
      
      // Hiển thị thông báo thành công
      showNotification(data.message, 'success');
    } else {
      showNotification(data.message, 'error');
    }
  })
  .catch(error => {
    console.error('Error:', error);
    showNotification('Đã xảy ra lỗi khi lưu thông tin', 'error');
  })
  .finally(() => {
    // Reset button
    btnSave.innerHTML = originalBtnText;
    btnSave.disabled = false;
  });
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
    hideChangePasswordForm();
  }
  
  // Enter key to submit password change
  const passwordModal = document.getElementById('changePasswordModal');
  if (e.key === 'Enter' && passwordModal && passwordModal.style.display === 'flex') {
    submitChangePassword();
  }
});

// ==================== CHANGE PASSWORD FORM (HoSo Page) ====================
function showChangePasswordForm() {
  const form = document.getElementById('changePasswordForm');
  if (form) {
    form.style.display = 'block';
    form.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
    
    // Attach lại event listener sau khi form hiển thị
    setTimeout(() => {
      const newPasswordInput = document.getElementById('newPassword');
      if (newPasswordInput) {
        newPasswordInput.removeEventListener('input', handlePasswordInput);
        newPasswordInput.removeEventListener('keyup', handlePasswordInput);
        newPasswordInput.addEventListener('input', handlePasswordInput);
        newPasswordInput.addEventListener('keyup', handlePasswordInput);
      }
    }, 100);
  }
}

function hideChangePasswordForm() {
  const form = document.getElementById('changePasswordForm');
  if (form) {
    form.style.display = 'none';
    // Reset form fields
    const currentPassword = document.getElementById('MatKhauHienTai');
    const newPassword = document.getElementById('MatKhauMoi');
    const confirmPassword = document.getElementById('XacNhanMatKhauMoi');
    
    if (currentPassword) currentPassword.value = '';
    if (newPassword) newPassword.value = '';
    if (confirmPassword) confirmPassword.value = '';
    
    // Reset strength indicator
    checkPasswordStrength('');
  }
}

function togglePasswordVisibility(inputId) {
  const input = document.getElementById(inputId);
  const button = input.parentElement.querySelector('.toggle-password');
  const icon = button.querySelector('i');
  
  if (input.type === 'password') {
    input.type = 'text';
    icon.classList.remove('fa-eye');
    icon.classList.add('fa-eye-slash');
  } else {
    input.type = 'password';
    icon.classList.remove('fa-eye-slash');
    icon.classList.add('fa-eye');
  }
}

// Password strength checker - sử dụng event delegation để đảm bảo luôn hoạt động
function checkPasswordStrength(password) {
  const strengthLevel = document.getElementById('strengthLevel');
  const strengthText = document.getElementById('strengthText');
  
  if (!strengthLevel || !strengthText) {
    console.warn('Strength indicator elements not found');
    return;
  }
  
  if (!password || password.length === 0) {
    strengthLevel.className = 'strength-level';
    strengthLevel.style.width = '0%';
    strengthText.textContent = 'Độ mạnh: Chưa nhập';
    strengthText.className = 'strength-text';
    return;
  }
  
  let strength = 0;
  
  // Check length
  if (password.length >= 8) strength++;
  if (password.length >= 12) strength++;
  
  // Check for lowercase and uppercase
  if (/[a-z]/.test(password) && /[A-Z]/.test(password)) strength++;
  
  // Check for numbers
  if (/\d/.test(password)) strength++;
  
  // Check for special characters
  if (/[^a-zA-Z0-9]/.test(password)) strength++;
  
  // Update UI
  if (strength <= 2) {
    strengthLevel.className = 'strength-level weak';
    strengthText.textContent = 'Độ mạnh: Yếu';
    strengthText.className = 'strength-text weak';
  } else if (strength <= 4) {
    strengthLevel.className = 'strength-level medium';
    strengthText.textContent = 'Độ mạnh: Trung bình';
    strengthText.className = 'strength-text medium';
  } else {
    strengthLevel.className = 'strength-level strong';
    strengthText.textContent = 'Độ mạnh: Mạnh';
    strengthText.className = 'strength-text strong';
  }
}

// Attach event listener khi DOM ready
document.addEventListener('DOMContentLoaded', function() {
  const newPasswordInput = document.getElementById('MatKhauMoi');
  if (newPasswordInput) {
    newPasswordInput.addEventListener('input', function() {
      checkPasswordStrength(this.value);
    });
    
    // Cũng check khi keyup để đảm bảo
    newPasswordInput.addEventListener('keyup', function() {
      checkPasswordStrength(this.value);
    });
  }
});

// Gọi lại khi form được hiển thị
function showChangePasswordForm() {
  const form = document.getElementById('changePasswordForm');
  if (form) {
    form.style.display = 'block';
    form.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
    
    // Attach lại event listener sau khi form hiển thị
    setTimeout(() => {
      const newPasswordInput = document.getElementById('newPassword');
      if (newPasswordInput) {
        newPasswordInput.removeEventListener('input', handlePasswordInput);
        newPasswordInput.removeEventListener('keyup', handlePasswordInput);
        newPasswordInput.addEventListener('input', handlePasswordInput);
        newPasswordInput.addEventListener('keyup', handlePasswordInput);
      }
    }, 100);
  }
}

function handlePasswordInput(e) {
  checkPasswordStrength(e.target.value);
}

// ==================== UPLOAD AVATAR ====================
function uploadAvatar(input) {
  if (input.files && input.files[0]) {
    const file = input.files[0];
    
    // Validate file type
    if (!file.type.match('image.*')) {
      showNotification('Vui lòng chọn file ảnh!', 'error');
      return;
    }
    
    // Validate file size (max 5MB)
    if (file.size > 5 * 1024 * 1024) {
      showNotification('Kích thước ảnh không được vượt quá 5MB!', 'error');
      return;
    }
    
    // Preview image
    const reader = new FileReader();
    reader.onload = function(e) {
      const avatarImage = document.getElementById('avatarImage');
      const defaultIcon = document.querySelector('.profile-avatar .fa-user-circle');
      
      if (avatarImage) {
        avatarImage.src = e.target.result;
        avatarImage.style.display = 'block';
      }
      
      // Ẩn icon mặc định khi có ảnh
      if (defaultIcon) {
        defaultIcon.style.display = 'none';
      }
    };
    reader.readAsDataURL(file);
    
    // Upload to server
    const formData = new FormData();
    formData.append('avatar', file);
    
    showNotification('Đang tải ảnh lên...', 'info');
    
    fetch('/DashboardKhachHang/UploadAvatar', {
      method: 'POST',
      body: formData
    })
    .then(response => response.json())
    .then(data => {
      if (data.success) {
        showNotification(data.message, 'success');
        // Update avatar URL in session
        if (data.avatarUrl) {
          const avatarImage = document.getElementById('avatarImage');
          const defaultIcon = document.querySelector('.profile-avatar .fa-user-circle');
          
          avatarImage.src = data.avatarUrl;
          avatarImage.style.display = 'block';
          
          // Đảm bảo icon mặc định bị ẩn
          if (defaultIcon) {
            defaultIcon.style.display = 'none';
          }
        }
      } else {
        showNotification(data.message, 'error');
      }
    })
    .catch(error => {
      console.error('Error:', error);
      showNotification('Đã xảy ra lỗi khi tải ảnh lên!', 'error');
    });
  }
}
// ==================== DUAL ACTION BUTTONS ====================
// Xử lý click cho các button dual action (Xem chi tiết / Hủy đặt phòng)
document.addEventListener('DOMContentLoaded', function() {
  // Lắng nghe sự kiện click cho tất cả button dual
  document.addEventListener('click', function(e) {
    const btn = e.target.closest('.btn-dual-left, .btn-dual-right');
    if (!btn) return;
    
    const action = btn.getAttribute('data-action');
    const row = btn.closest('tr');
    const bookingId = row?.querySelector('.booking-id')?.textContent.trim();
    
    if (action === 'detail') {
      // Xử lý xem chi tiết
      console.log('Xem chi tiết đơn:', bookingId);
      // TODO: Thêm logic xem chi tiết ở đây
    } else if (action === 'cancel') {
      // Xử lý hủy đặt phòng
      console.log('Hủy đặt phòng:', bookingId);
      // TODO: Thêm logic hủy đặt phòng ở đây
    }
  });
});



