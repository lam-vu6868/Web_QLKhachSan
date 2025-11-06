/**
 * Terms & Privacy Policy Page JavaScript
 * Handles toggle functionality between Terms of Service and Privacy Policy
 */

document.addEventListener('DOMContentLoaded', function() {
    console.log('🔄 Terms & Privacy Policy page loaded');

    // Initialize toggle functionality
    initializeToggle();
    
    // Initialize accept button functionality
    initializeAcceptButton();
    
    // Handle URL hash changes
    handleUrlHash();
    
    // Add smooth scrolling for internal links
    initializeSmoothScrolling();
});

/**
 * Initialize toggle functionality between terms and privacy sections
 */
function initializeToggle() {
    const toggleButtons = document.querySelectorAll('.toggle-btn');
    const contentSections = document.querySelectorAll('.content-section');
    
    if (!toggleButtons.length || !contentSections.length) {
        console.warn('⚠️ Toggle elements not found');
        return;
    }
    
    console.log(`📋 Found ${toggleButtons.length} toggle buttons and ${contentSections.length} content sections`);
    
    toggleButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            e.preventDefault();
            
            const targetId = this.getAttribute('data-target');
            if (!targetId) {
                console.warn('⚠️ No target specified for toggle button');
                return;
            }
            
            console.log(`🎯 Switching to: ${targetId}`);
            
            // Update button states
            toggleButtons.forEach(btn => {
                btn.classList.remove('active');
                btn.setAttribute('aria-pressed', 'false');
            });
            
            this.classList.add('active');
            this.setAttribute('aria-pressed', 'true');
            
            // Update content sections
            contentSections.forEach(section => {
                section.classList.remove('active');
                section.setAttribute('aria-hidden', 'true');
            });
            
            const targetSection = document.getElementById(targetId);
            if (targetSection) {
                targetSection.classList.add('active');
                targetSection.setAttribute('aria-hidden', 'false');
                
                // Update page title based on active section
                updatePageTitle(targetId);
                
                // Update URL hash without scrolling
                updateUrlHash(targetId);
                
                // Scroll to top of content
                targetSection.scrollIntoView({ 
                    behavior: 'smooth', 
                    block: 'start' 
                });
                
                console.log(`✅ Successfully switched to: ${targetId}`);
            } else {
                console.error(`❌ Target section not found: ${targetId}`);
            }
        });
        
        // Add keyboard support
        button.addEventListener('keydown', function(e) {
            if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                this.click();
            }
        });
    });
    
    console.log('✅ Toggle functionality initialized');
}

/**
 * Update page title based on active section
 */
function updatePageTitle(targetId) {
    const titles = {
        'terms-content': 'Điều Khoản Sử Dụng - The Serene Horizon',
        'privacy-content': 'Chính Sách Bảo Mật - The Serene Horizon'
    };
    
    const newTitle = titles[targetId];
    if (newTitle) {
        document.title = newTitle;
        console.log(`📄 Page title updated: ${newTitle}`);
    }
}

/**
 * Update URL hash without causing page scroll
 */
function updateUrlHash(targetId) {
    const hashMap = {
        'terms-content': 'dieu-khoan',
        'privacy-content': 'chinh-sach'
    };
    
    const hash = hashMap[targetId];
    if (hash) {
        // Update URL without triggering hashchange event
        history.replaceState(null, null, `#${hash}`);
        console.log(`🔗 URL hash updated: #${hash}`);
    }
}

/**
 * Handle URL hash on page load and hash changes
 */
function handleUrlHash() {
    function processHash() {
        const hash = window.location.hash.substring(1); // Remove #
        
        const hashToTarget = {
            'dieu-khoan': 'terms-content',
            'chinh-sach': 'privacy-content',
            'terms': 'terms-content',
            'privacy': 'privacy-content'
        };
        
        const targetId = hashToTarget[hash];
        
        if (targetId) {
            console.log(`🔗 Processing hash: #${hash} -> ${targetId}`);
            
            // Find and click the corresponding button
            const targetButton = document.querySelector(`[data-target="${targetId}"]`);
            if (targetButton) {
                targetButton.click();
            }
        } else if (hash === '') {
            // Default to terms if no hash
            const termsButton = document.querySelector('[data-target="terms-content"]');
            if (termsButton && !termsButton.classList.contains('active')) {
                termsButton.click();
            }
        }
    }
    
    // Process hash on page load
    processHash();
    
    // Listen for hash changes
    window.addEventListener('hashchange', processHash);
    
    console.log('✅ URL hash handling initialized');
}

/**
 * Initialize accept button functionality
 */
function initializeAcceptButton() {
    const acceptBtn = document.getElementById('acceptBtn');
    
    if (!acceptBtn) {
        console.warn('⚠️ Accept button not found');
        return;
    }
    
    acceptBtn.addEventListener('click', function(e) {
        e.preventDefault();
        
        console.log('📝 User accepted terms/privacy policy');
        
        // Store acceptance in localStorage
        const acceptance = {
            accepted: true,
            timestamp: new Date().toISOString(),
            sections: []
        };
        
        // Track which sections were viewed
        const activeSections = document.querySelectorAll('.content-section.active');
        activeSections.forEach(section => {
            acceptance.sections.push(section.id);
        });
        
        localStorage.setItem('terms_privacy_acceptance', JSON.stringify(acceptance));
        
        // Show success message
        showAcceptanceMessage();
        
        // Optional: Redirect after acceptance
        setTimeout(() => {
            const returnUrl = new URLSearchParams(window.location.search).get('return');
            if (returnUrl) {
                window.location.href = decodeURIComponent(returnUrl);
            } else {
                // Default redirect to home page
                window.location.href = '/';
            }
        }, 2000);
    });
    
    console.log('✅ Accept button functionality initialized');
}

/**
 * Show acceptance confirmation message
 */
function showAcceptanceMessage() {
    // Create success notification
    const notification = document.createElement('div');
    notification.className = 'acceptance-notification';
    notification.innerHTML = `
        <div class="notification-content">
            <i class="fas fa-check-circle"></i>
            <span>Cảm ơn bạn đã đọc và đồng ý với các điều khoản!</span>
        </div>
    `;
    
    // Add styles
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: linear-gradient(135deg, #27ae60, #2ecc71);
        color: white;
        padding: 1rem 1.5rem;
        border-radius: 10px;
        box-shadow: 0 4px 15px rgba(39, 174, 96, 0.3);
        z-index: 9999;
        animation: slideInRight 0.5s ease;
        max-width: 350px;
    `;
    
    // Add animation styles
    const style = document.createElement('style');
    style.textContent = `
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
        
        .notification-content {
            display: flex;
            align-items: center;
            gap: 0.8rem;
        }
        
        .notification-content i {
            font-size: 1.2rem;
        }
    `;
    
    document.head.appendChild(style);
    document.body.appendChild(notification);
    
    // Remove notification after 3 seconds
    setTimeout(() => {
        notification.style.animation = 'slideOutRight 0.5s ease';
        setTimeout(() => {
            notification.remove();
            style.remove();
        }, 500);
    }, 3000);
    
    console.log('✅ Acceptance message displayed');
}

/**
 * Initialize smooth scrolling for internal links
 */
function initializeSmoothScrolling() {
    const links = document.querySelectorAll('a[href^="#"]');
    
    links.forEach(link => {
        link.addEventListener('click', function(e) {
            const href = this.getAttribute('href');
            if (href === '#') {
                e.preventDefault();
                return;
            }
            
            const targetId = href.substring(1);
            const targetElement = document.getElementById(targetId);
            
            if (targetElement) {
                e.preventDefault();
                targetElement.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
                
                console.log(`🎯 Smooth scroll to: ${targetId}`);
            }
        });
    });
    
    console.log(`✅ Smooth scrolling initialized for ${links.length} links`);
}

/**
 * Check if user has previously accepted terms/privacy
 */
function checkPreviousAcceptance() {
    const acceptance = localStorage.getItem('terms_privacy_acceptance');
    
    if (acceptance) {
        try {
            const data = JSON.parse(acceptance);
            console.log('📋 Previous acceptance found:', data);
            
            // Optional: Show indicator that user has previously accepted
            const acceptBtn = document.getElementById('acceptBtn');
            if (acceptBtn) {
                acceptBtn.innerHTML = `
                    <i class="fas fa-check-double"></i>
                    Đã Đồng Ý Trước Đó
                `;
                acceptBtn.style.background = 'linear-gradient(135deg, #95a5a6, #7f8c8d)';
            }
            
            return true;
        } catch (e) {
            console.warn('⚠️ Error parsing previous acceptance:', e);
            localStorage.removeItem('terms_privacy_acceptance');
        }
    }
    
    return false;
}

/**
 * Utility function to get current active section
 */
function getCurrentSection() {
    const activeSection = document.querySelector('.content-section.active');
    return activeSection ? activeSection.id : null;
}

/**
 * Utility function for debugging
 */
window.debugTermsPrivacy = function() {
    console.log('=== Terms & Privacy Debug Info ===');
    console.log('Current section:', getCurrentSection());
    console.log('URL hash:', window.location.hash);
    console.log('Page title:', document.title);
    console.log('Previous acceptance:', localStorage.getItem('terms_privacy_acceptance'));
    console.log('Toggle buttons:', document.querySelectorAll('.toggle-btn').length);
    console.log('Content sections:', document.querySelectorAll('.content-section').length);
};

// Export functions for potential external use
window.TermsPrivacyPage = {
    getCurrentSection,
    checkPreviousAcceptance,
    updatePageTitle,
    showAcceptanceMessage
};