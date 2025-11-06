// Modal Điều Khoản và Chính Sách only
document.addEventListener('DOMContentLoaded', function() {
  // ==================== MODAL ĐIỀU KHOẢN VÀ CHÍNH SÁCH ====================
  
  // Lấy các elements
  const termsModal = document.getElementById('termsModal');
  const policyModal = document.getElementById('policyModal');
  const showTermsLink = document.getElementById('showTermsLink');
  const showPolicyLink = document.getElementById('showPolicyLink');
  const closeTermsBtn = document.getElementById('closeTermsBtn');
  const closePolicyBtn = document.getElementById('closePolicyBtn');
  const acceptTermsBtn = document.getElementById('acceptTermsBtn');
  const acceptPolicyBtn = document.getElementById('acceptPolicyBtn');
  const agreeTermsCheckbox = document.getElementById('agreeTerms');

  // Mở modal Điều khoản
  if (showTermsLink) {
    showTermsLink.addEventListener('click', function(e) {
      e.preventDefault();
      if (termsModal) {
        termsModal.classList.add('active');
        document.body.style.overflow = 'hidden';
      }
    });
  }

  // Mở modal Chính sách
  if (showPolicyLink) {
    showPolicyLink.addEventListener('click', function(e) {
      e.preventDefault();
      if (policyModal) {
        policyModal.classList.add('active');
        document.body.style.overflow = 'hidden';
      }
    });
  }

  // Đóng modal Điều khoản - nút X
  if (closeTermsBtn) {
    closeTermsBtn.addEventListener('click', function() {
      if (termsModal) {
        termsModal.classList.remove('active');
        document.body.style.overflow = '';
      }
    });
  }

  // Đóng modal Chính sách - nút X
  if (closePolicyBtn) {
    closePolicyBtn.addEventListener('click', function() {
      if (policyModal) {
        policyModal.classList.remove('active');
        document.body.style.overflow = '';
      }
    });
  }

  // Nút "Tôi đã đọc và đồng ý" - Điều khoản
  if (acceptTermsBtn) {
    acceptTermsBtn.addEventListener('click', function() {
      if (agreeTermsCheckbox) {
        agreeTermsCheckbox.checked = true;
      }
      if (termsModal) {
        termsModal.classList.remove('active');
        document.body.style.overflow = '';
      }
      
      // Hiển thị thông báo
      showAcceptNotification('Cảm ơn bạn đã đồng ý với Điều khoản sử dụng!');
    });
  }

  // Nút "Tôi đã đọc và đồng ý" - Chính sách
  if (acceptPolicyBtn) {
    acceptPolicyBtn.addEventListener('click', function() {
      if (agreeTermsCheckbox) {
        agreeTermsCheckbox.checked = true;
      }
      if (policyModal) {
        policyModal.classList.remove('active');
        document.body.style.overflow = '';
      }
      
      // Hiển thị thông báo
      showAcceptNotification('Cảm ơn bạn đã đồng ý với Chính sách bảo mật!');
    });
  }

  // Đóng modal khi click bên ngoài
  if (termsModal) {
    termsModal.addEventListener('click', function(e) {
      if (e.target === termsModal) {
        termsModal.classList.remove('active');
        document.body.style.overflow = '';
      }
    });
  }

  if (policyModal) {
    policyModal.addEventListener('click', function(e) {
      if (e.target === policyModal) {
        policyModal.classList.remove('active');
        document.body.style.overflow = '';
      }
    });
  }

  // Đóng modal khi nhấn ESC
  document.addEventListener('keydown', function(e) {
    if (e.key === 'Escape') {
      if (termsModal && termsModal.classList.contains('active')) {
        termsModal.classList.remove('active');
        document.body.style.overflow = '';
      }
      if (policyModal && policyModal.classList.contains('active')) {
        policyModal.classList.remove('active');
        document.body.style.overflow = '';
      }
    }
  });

  // Hàm hiển thị thông báo
  function showAcceptNotification(message) {
    const notification = document.createElement('div');
    notification.style.cssText = `
      position: fixed;
      top: 20px;
      right: 20px;
      background: linear-gradient(135deg, #28a745 0%, #20c997 100%);
      color: white;
      padding: 16px 24px;
      border-radius: 10px;
      font-size: 0.95rem;
      font-weight: 600;
      display: flex;
      align-items: center;
      gap: 10px;
      box-shadow: 0 6px 20px rgba(40, 167, 69, 0.4);
      z-index: 99999;
      animation: slideInRight 0.3s ease;
    `;
    notification.innerHTML = `<i class="fas fa-check-circle"></i> ${message}`;
    
    document.body.appendChild(notification);
    
    setTimeout(() => {
      notification.style.animation = 'slideOutRight 0.3s ease';
      setTimeout(() => notification.remove(), 300);
    }, 3000);
  }
});

// ========== TOAST NOTIFICATION FUNCTION ==========
function showToast(type, title, message, duration = 6000) {
    // Tạo container nếu chưa có
    let container = document.querySelector('.toast-container');
    if (!container) {
 container = document.createElement('div');
  container.className = 'toast-container';
  document.body.appendChild(container);
    }

    // Tạo toast element
    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    
    const iconClass = type === 'success' ? 'fa-check-circle' : 'fa-exclamation-circle';
    
    toast.innerHTML = `
  <div class="toast-icon">
         <i class="fas ${iconClass}"></i>
</div>
        <div class="toast-content">
       <div class="toast-title">${title}</div>
            <div class="toast-message">${message}</div>
      </div>
        <button class="toast-close" onclick="this.parentElement.remove()">
     <i class="fas fa-times"></i>
     </button>
    `;
    
    container.appendChild(toast);
    
  // Tự động xóa sau duration
    setTimeout(() => {
 if (toast.parentElement) {
        toast.remove();
    }
    }, duration);
}

// Kiểm tra và hiển thị thông báo từ TempData
document.addEventListener('DOMContentLoaded', function() {
    // Kiểm tra xem có thông báo success không
    const urlParams = new URLSearchParams(window.location.search);
    const successMessage = urlParams.get('success');
    
    if (successMessage) {
        showToast('success', 'Thành công!', decodeURIComponent(successMessage), 6000);
     
        // Xóa parameter khỏi URL sau khi hiển thị
      window.history.replaceState({}, document.title, window.location.pathname);
    }
});