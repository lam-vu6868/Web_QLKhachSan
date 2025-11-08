/**
 * =============================================
 * LAYOUT LỄ TÂN - JAVASCRIPT
 * =============================================
 */

(function() {
    'use strict';

    // Wait for DOM to be ready
    document.addEventListener('DOMContentLoaded', function() {
        initSidebar();
        initMobileMenu();
    });

    /**
     * Initialize Sidebar Toggle
     */
    function initSidebar() {
        const toggleBtn = document.getElementById('toggleSidebar');
        const sidebar = document.getElementById('sidebar');

        if (toggleBtn && sidebar) {
  toggleBtn.addEventListener('click', function() {
       sidebar.classList.toggle('collapsed');
          
                // Save state to localStorage
    const isCollapsed = sidebar.classList.contains('collapsed');
         localStorage.setItem('sidebarCollapsed', isCollapsed);
    });

            // Restore sidebar state from localStorage
            const savedState = localStorage.getItem('sidebarCollapsed');
            if (savedState === 'true') {
          sidebar.classList.add('collapsed');
 }
        }
    }

    /**
     * Initialize Mobile Menu
     */
    function initMobileMenu() {
        if (window.innerWidth <= 768) {
       const toggleBtn = document.getElementById('toggleSidebar');
    const sidebar = document.getElementById('sidebar');

     if (toggleBtn && sidebar) {
      toggleBtn.addEventListener('click', function() {
     sidebar.classList.toggle('active');
          });

            // Close sidebar when clicking outside
    document.addEventListener('click', function(event) {
    const isClickInsideSidebar = sidebar.contains(event.target);
         const isClickOnToggle = toggleBtn.contains(event.target);

      if (!isClickInsideSidebar && !isClickOnToggle && sidebar.classList.contains('active')) {
        sidebar.classList.remove('active');
 }
      });
            }
        }
    }

})();
