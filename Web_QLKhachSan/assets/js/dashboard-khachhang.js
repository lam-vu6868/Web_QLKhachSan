// Dashboard Navigation - Final Fix
document.addEventListener('DOMContentLoaded', function() {
  console.log('🚀 Dashboard script loaded');

  // Navigation function with detailed logging
  function goToSection(sectionId) {
    console.log('🎯 Attempting to navigate to:', sectionId);
    
    if (!sectionId) {
      console.error('❌ No section ID provided');
      return;
    }

    // Remove active from all sections
    const allSections = document.querySelectorAll('.content-section');
    console.log('📄 Found sections:', allSections.length);
    
    allSections.forEach(section => {
      section.classList.remove('active');
      console.log('➖ Removed active from:', section.id);
    });

    // Add active to target section
    const targetSection = document.getElementById(sectionId);
    if (targetSection) {
      targetSection.classList.add('active');
      console.log('✅ Added active to:', sectionId);
      console.log('🔍 Section classes:', targetSection.className);
    } else {
      console.error('❌ Section not found:', sectionId);
      return;
    }

    // Update sidebar navigation
    const allNavItems = document.querySelectorAll('.sidebar-nav .nav-item');
    allNavItems.forEach(item => item.classList.remove('active'));
    
    const activeNavItem = document.querySelector(`.sidebar-nav .nav-item[data-section="${sectionId}"]`);
    if (activeNavItem) {
      activeNavItem.classList.add('active');
      console.log('🎨 Updated sidebar active state');
    }

    // Scroll to top
    window.scrollTo({ top: 0, behavior: 'smooth' });
    console.log('⬆️ Scrolled to top');
  }

  // Setup click handlers for all navigation elements
  function setupNavigation() {
    console.log('🔧 Setting up navigation...');

    // Universal click handler for any element with data-section
    document.addEventListener('click', function(e) {
      const element = e.target.closest('[data-section]');
      if (element && element.dataset.section) {
        // Check if we're on the dashboard page
        const isDashboardPage = document.querySelector('.content-section') !== null;
        
        if (isDashboardPage) {
          // On dashboard page: prevent default and use section navigation
          e.preventDefault();
          e.stopPropagation();
          console.log('🖱️ Clicked navigation element (Dashboard page):', element.dataset.section);
          
          // Update dropdown active state if clicked element is in dropdown
          if (element.classList.contains('dropdown-item')) {
            document.querySelectorAll('.user-dropdown .dropdown-item').forEach(item => {
              item.classList.remove('active');
            });
            element.classList.add('active');
            console.log('🎨 Updated dropdown active state');
          }
          
          // Update mobile menu active state if clicked element is in mobile menu
          if (element.closest('.mobile-account-nav')) {
            document.querySelectorAll('.mobile-account-nav .mobile-nav-list a').forEach(item => {
              item.classList.remove('active-link');
            });
            element.classList.add('active-link');
            console.log('📱 Updated mobile menu active state');
          }
          
          goToSection(element.dataset.section);
        } else {
          // Not on dashboard page: allow default link behavior (navigate to URL with hash)
          console.log('🖱️ Clicked navigation element (other page):', element.dataset.section);
          // Let the browser follow the href
        }
      }
    });

    console.log('✅ Universal click handler attached');
  }

  // Setup booking filter functionality
  function setupBookingFilters() {
    console.log('🏷️ Setting up booking filters...');
    
    const filterTabs = document.querySelectorAll('.filter-tabs .tab-btn');
    const bookingCards = document.querySelectorAll('.booking-card');
    
    if (filterTabs.length === 0) {
      console.log('⚠️ No filter tabs found');
      return;
    }
    
    console.log('📋 Found', filterTabs.length, 'filter tabs and', bookingCards.length, 'booking cards');
    
    filterTabs.forEach(tab => {
      tab.addEventListener('click', function(e) {
        e.preventDefault();
        e.stopPropagation();
        
        const filter = this.dataset.filter;
        console.log('🎯 Filter clicked:', filter);
        
        // Remove active from all tabs
        filterTabs.forEach(t => t.classList.remove('active'));
        // Add active to clicked tab
        this.classList.add('active');
        
        // Filter booking cards
        bookingCards.forEach(card => {
          const shouldShow = filter === 'all' || card.classList.contains(filter);
          
          if (shouldShow) {
            card.style.display = 'block';
            console.log('👁️ Showing card:', card.querySelector('.booking-id')?.textContent);
          } else {
            card.style.display = 'none';
            console.log('👁️‍🗨️ Hiding card:', card.querySelector('.booking-id')?.textContent);
          }
        });
        
        console.log('✅ Filter applied:', filter);
      });
    });
    
    console.log('✅ Booking filters setup complete');
  }

  // Setup service dropdowns in the transaction history (expand/collapse lists)
  function setupServiceDropdowns() {
    console.log('🧾 Setting up service dropdowns...');

    const toggles = document.querySelectorAll('.service-toggle');
    if (!toggles || toggles.length === 0) {
      console.log('⚠️ No service toggles found');
      return;
    }

    toggles.forEach(btn => {
      // Ensure ARIA defaults
      if (!btn.hasAttribute('aria-expanded')) btn.setAttribute('aria-expanded', 'false');

      btn.addEventListener('click', function(e) {
        e.stopPropagation();

        const summary = btn.closest('.service-summary');
        if (!summary) return;

        const list = summary.querySelector('.service-list');
        if (!list) return;

        const isExpanded = btn.getAttribute('aria-expanded') === 'true';

        if (isExpanded) {
          // Collapse
          list.hidden = true;
          btn.setAttribute('aria-expanded', 'false');
          btn.classList.remove('expanded');
        } else {
          // Collapse any other open lists first (single-open behavior)
          document.querySelectorAll('.service-summary .service-list').forEach(l => {
            l.hidden = true;
            const b = l.closest('.service-summary').querySelector('.service-toggle');
            if (b) b.setAttribute('aria-expanded', 'false');
            if (b) b.classList.remove('expanded');
          });

          // Expand this one
          list.hidden = false;
          btn.setAttribute('aria-expanded', 'true');
          btn.classList.add('expanded');
        }
      });
    });

    // Close dropdowns when clicking outside
    document.addEventListener('click', function(e) {
      if (!e.target.closest('.service-summary')) {
        document.querySelectorAll('.service-summary .service-list').forEach(l => {
          l.hidden = true;
          const b = l.closest('.service-summary').querySelector('.service-toggle');
          if (b) b.setAttribute('aria-expanded', 'false');
          if (b) b.classList.remove('expanded');
        });
      }
    });

    console.log('✅ Service dropdowns setup complete');
  }

  // Initialize
  function init() {
    console.log('🎉 Initializing dashboard...');
    
    // Setup navigation
    setupNavigation();
    
    // Setup booking filters
    setupBookingFilters();
    
      // Setup service dropdowns
      setupServiceDropdowns();
    
    // Check if there's a hash in the URL and navigate to that section
    const hash = window.location.hash.substring(1); // Remove the # character
    if (hash) {
      console.log('🔗 Found hash in URL:', hash);
      goToSection(hash);
      
      // Update dropdown active state based on hash
      document.querySelectorAll('.user-dropdown .dropdown-item').forEach(item => {
        item.classList.remove('active');
        if (item.dataset.section === hash) {
          item.classList.add('active');
        }
      });
      
    // (service dropdowns handled by setupServiceDropdowns)
      // Update mobile menu active state based on hash
      document.querySelectorAll('.mobile-account-nav .mobile-nav-list a').forEach(item => {
        item.classList.remove('active-link');
        if (item.dataset.section === hash) {
          item.classList.add('active-link');
        }
      });
    } else {
      // Ensure overview is shown by default
      const overviewSection = document.getElementById('overview');
      if (overviewSection && !overviewSection.classList.contains('active')) {
        console.log('🏠 Setting overview as default active section');
        goToSection('overview');
        
        // Set default active states for navigation
        const defaultDropdownItem = document.querySelector('.user-dropdown .dropdown-item[data-section="overview"]');
        if (defaultDropdownItem) {
          defaultDropdownItem.classList.add('active');
        }
        
        const defaultMobileItem = document.querySelector('.mobile-account-nav .mobile-nav-list a[data-section="overview"]');
        if (defaultMobileItem) {
          defaultMobileItem.classList.add('active-link');
        }
      }
    }
    
    // Log what we found
    const sections = document.querySelectorAll('.content-section');
    const navItems = document.querySelectorAll('[data-section]');
    
    console.log('📊 Summary:');
    console.log('  - Sections found:', sections.length);
    console.log('  - Nav items found:', navItems.length);
    console.log('  - Section IDs:', Array.from(sections).map(s => s.id));
    console.log('  - Nav data-sections:', Array.from(navItems).map(n => n.dataset.section));
    
    console.log('🎯 Dashboard initialization complete!');
  }



  // Start everything
  init();
  
  // Global functions for debugging
  window.goToSection = goToSection;
  window.debugSections = function() {
    const sections = document.querySelectorAll('.content-section');
    sections.forEach(s => {
      console.log(`Section ${s.id}:`, {
        classes: s.className,
        display: getComputedStyle(s).display,
        visible: s.offsetHeight > 0
      });
    });
  };
});

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


  



