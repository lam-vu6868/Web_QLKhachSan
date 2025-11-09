/* =============================================
   Layout JavaScript cho Nhân Viên Lễ Tân
   File: layout.js
============================================= */

(function () {
    'use strict';

    // Initialize when DOM is ready
    document.addEventListener('DOMContentLoaded', function () {
        initializeLayout();
    });

    /**
     * Initialize all layout functions
     */
    function initializeLayout() {
        updateTime();
        initializeAlerts();
        initializeSidebar();
        initializeMobileMenu();
        initializeAOS();
    }

    /**
     * Update current time and date
     */
    function updateTime() {
        function update() {
            const now = new Date();

            // Format time (HH:mm:ss)
            const hours = String(now.getHours()).padStart(2, '0');
            const minutes = String(now.getMinutes()).padStart(2, '0');
            const seconds = String(now.getSeconds()).padStart(2, '0');
            const timeElement = document.getElementById('currentTime');
            if (timeElement) {
                timeElement.textContent = `${hours}:${minutes}:${seconds}`;
            }

            // Format date (Vietnamese locale)
            const options = {
                weekday: 'long',
                year: 'numeric',
                month: 'long',
                day: 'numeric'
            };
            const dateStr = now.toLocaleDateString('vi-VN', options);
            const dateElement = document.getElementById('currentDate');
            if (dateElement) {
                dateElement.textContent = dateStr;
            }
        }

        // Update immediately
        update();

        // Update every second
        setInterval(update, 1000);
    }

    /**
     * Initialize alert auto-hide functionality
     */
    function initializeAlerts() {
        const alerts = document.querySelectorAll('.alert');

        alerts.forEach(function (alert) {
            // Auto hide after 5 seconds
            setTimeout(function () {
                if (typeof $ !== 'undefined' && $.fn.fadeOut) {
                    $(alert).fadeOut('slow');
                } else {
                    alert.style.transition = 'opacity 0.5s';
                    alert.style.opacity = '0';
                    setTimeout(function () {
                        alert.style.display = 'none';
                    }, 500);
                }
            }, 5000);
        });
    }

    /**
     * Initialize sidebar functionality
     */
    function initializeSidebar() {
        // Highlight active menu item based on current URL
        const currentPath = window.location.pathname;
        const navLinks = document.querySelectorAll('.sidebar-nav .nav-link');

        navLinks.forEach(function (link) {
            if (link.getAttribute('href') === currentPath) {
                link.classList.add('active-link');
                link.closest('.nav-item').classList.add('active');
            }
        });
    }

    /**
     * Initialize mobile menu toggle functionality
     */
    function initializeMobileMenu() {
        const sidebar = document.querySelector('.sidebar');
        const hamburger = document.getElementById('hamburgerMenu');
        const overlay = document.getElementById('sidebarOverlay');
        const navLinks = document.querySelectorAll('.sidebar-nav .nav-link');

        if (!hamburger || !sidebar || !overlay) {
            console.warn('Mobile menu elements not found');
            return;
        }

        // Toggle sidebar when clicking hamburger menu
        hamburger.addEventListener('click', function (e) {
            e.stopPropagation();
            toggleSidebar();
        });

        // Close sidebar when clicking overlay
        overlay.addEventListener('click', function () {
            closeSidebar();
        });

        // Close sidebar when clicking nav link (mobile only)
        navLinks.forEach(function (link) {
            link.addEventListener('click', function () {
                if (window.innerWidth <= 768) {
                    closeSidebar();
                }
            });
        });

        // Close sidebar when pressing ESC key
        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape' && sidebar.classList.contains('active')) {
                closeSidebar();
            }
        });

        // Handle window resize
        let resizeTimer;
        window.addEventListener('resize', function () {
            clearTimeout(resizeTimer);
            resizeTimer = setTimeout(function () {
                // Close sidebar if resizing to desktop
                if (window.innerWidth > 768) {
                    closeSidebar();
                }
            }, 250);
        });
    }

    /**
     * Toggle sidebar (open/close)
     */
    function toggleSidebar() {
        const sidebar = document.querySelector('.sidebar');
        const hamburger = document.getElementById('hamburgerMenu');
        const overlay = document.getElementById('sidebarOverlay');
        const body = document.body;

        const isActive = sidebar.classList.contains('active');

        if (isActive) {
            closeSidebar();
        } else {
            openSidebar();
        }
    }

    /**
     * Open sidebar
     */
    function openSidebar() {
        const sidebar = document.querySelector('.sidebar');
        const hamburger = document.getElementById('hamburgerMenu');
        const overlay = document.getElementById('sidebarOverlay');
        const body = document.body;

        sidebar.classList.add('active');
        hamburger.classList.add('active');
        overlay.classList.add('active');
        body.style.overflow = 'hidden'; // Prevent body scroll when sidebar is open

        console.log('Sidebar opened');
    }

    /**
     * Close sidebar
     */
    function closeSidebar() {
        const sidebar = document.querySelector('.sidebar');
        const hamburger = document.getElementById('hamburgerMenu');
        const overlay = document.getElementById('sidebarOverlay');
        const body = document.body;

        sidebar.classList.remove('active');
        hamburger.classList.remove('active');
        overlay.classList.remove('active');
        body.style.overflow = ''; // Restore body scroll

        console.log('Sidebar closed');
    }

    /**
     * Initialize AOS (Animate On Scroll)
     */
    function initializeAOS() {
        if (typeof AOS !== 'undefined') {
            AOS.init({
                duration: 800,
                once: true,
                offset: 100,
                easing: 'ease-out'
            });
        }
    }

    /**
     * Logout function
     */
    window.dangXuat = function () {
        if (confirm('Bạn có chắc muốn đăng xuất?')) {
            // Get logout URL from data attribute or use default
            const logoutUrl = document.querySelector('[data-logout-url]')?.dataset.logoutUrl ||
                '/Login/DangXuat';
            window.location.href = logoutUrl;
        }
    };

    /**
     * View notifications function
     */
    window.xemThongBao = function () {
        // TODO: Implement notification view
        alert('Chức năng thông báo đang được phát triển!');

        // Example of future implementation:
        // $('#modalThongBao').modal('show');
        // loadNotifications();
    };

    /**
     * Load notifications (for future implementation)
     */
    function loadNotifications() {
        // TODO: AJAX call to get notifications
        console.log('Loading notifications...');
    }

    /**
     * Show toast notification
     * @param {string} message - Message to display
     * @param {string} type - Type of notification (success, error, warning, info)
     */
    window.showToast = function (message, type) {
        type = type || 'info';

        const toast = document.createElement('div');
        toast.className = `alert alert-${type} alert-dismissible fade show`;
        toast.style.position = 'fixed';
        toast.style.top = '20px';
        toast.style.right = '20px';
        toast.style.zIndex = '9999';
        toast.style.minWidth = '300px';
        toast.innerHTML = `
            ${message}
            <button type="button" class="close" data-dismiss="alert">
          <span>&times;</span>
 </button>
        `;

        document.body.appendChild(toast);

        // Auto remove after 3 seconds
        setTimeout(function () {
            if (typeof $ !== 'undefined' && $.fn.fadeOut) {
                $(toast).fadeOut('slow', function () {
                    toast.remove();
                });
            } else {
                toast.style.transition = 'opacity 0.5s';
                toast.style.opacity = '0';
                setTimeout(function () {
                    toast.remove();
                }, 500);
            }
        }, 3000);
    };

    /**
     * Format currency to VND
     * @param {number} value - Value to format
     * @returns {string} Formatted currency string
     */
    window.formatCurrency = function (value) {
        if (!value && value !== 0) return 'N/A';
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND'
        }).format(value);
    };

    /**
     * Format date to Vietnamese locale
     * @param {string|Date} dateString - Date to format
     * @returns {string} Formatted date string
     */
    window.formatDate = function (dateString) {
        if (!dateString) return 'N/A';
        const date = new Date(dateString);
        return date.toLocaleDateString('vi-VN');
    };

    /**
     * Format datetime to Vietnamese locale
     * @param {string|Date} dateString - Datetime to format
     * @returns {string} Formatted datetime string
     */
    window.formatDateTime = function (dateString) {
        if (!dateString) return 'N/A';
        const date = new Date(dateString);
        return date.toLocaleString('vi-VN');
    };

    // Expose utility functions globally
    window.LayoutUtils = {
        toggleSidebar: toggleSidebar,
        openSidebar: openSidebar,
        closeSidebar: closeSidebar,
        showToast: window.showToast,
        formatCurrency: window.formatCurrency,
        formatDate: window.formatDate,
        formatDateTime: window.formatDateTime
    };

})();
