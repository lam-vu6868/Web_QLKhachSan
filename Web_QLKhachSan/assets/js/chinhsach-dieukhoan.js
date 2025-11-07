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
    
    // Handle URL hash changes (only if there's a hash)
    handleUrlHash();
    
    // Add smooth scrolling for internal links
    initializeSmoothScrolling();
    
    // Check if content is properly displayed on load
    setTimeout(validateContentDisplay, 100); // Small delay to ensure DOM is fully rendered
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
    
    // Log initial state from server
    const initialActiveButton = document.querySelector('.toggle-btn.active');
    const initialActiveSection = document.querySelector('.content-section.active');
    console.log('🏁 Initial state - Active button:', initialActiveButton?.getAttribute('data-target'));
    console.log('🏁 Initial state - Active section:', initialActiveSection?.id);
    
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
                // Force hide
                section.style.display = 'none';
                section.style.opacity = '0';
                section.style.visibility = 'hidden';
            });
            
            const targetSection = document.getElementById(targetId);
            if (targetSection) {
                targetSection.classList.add('active');
                targetSection.setAttribute('aria-hidden', 'false');
                // Force show
                targetSection.style.display = 'block';
                targetSection.style.opacity = '1';
                targetSection.style.visibility = 'visible';
                
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
        }
        // Remove the else if that was interfering with server-side activeTab setting
    }
    
    // Only process hash if there is actually a hash in the URL
    if (window.location.hash) {
        processHash();
    }
    
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
 * Validate that content is properly displayed on page load
 */
function validateContentDisplay() {
    const activeButton = document.querySelector('.toggle-btn.active');
    const activeSection = document.querySelector('.content-section.active');
    
    console.log('🔍 Validating content display...');
    console.log('Active button:', activeButton?.getAttribute('data-target'));
    console.log('Active section:', activeSection?.id);
    
    // Ensure ARIA attributes are set correctly for existing active elements
    if (activeButton) {
        activeButton.setAttribute('aria-pressed', 'true');
        console.log('✅ Set aria-pressed=true for active button');
    }
    
    if (activeSection) {
        activeSection.setAttribute('aria-hidden', 'false');
        // Force display for active section (in case CSS is not applying)
        activeSection.style.display = 'block';
        activeSection.style.opacity = '1';
        activeSection.style.visibility = 'visible';
        console.log('✅ Ensured active section is visible');
    }
    
    // If we have an active button but no active section, or vice versa
    if (activeButton && !activeSection) {
        console.warn('⚠️ Active button found but no active section, fixing...');
        const targetId = activeButton.getAttribute('data-target');
        const targetSection = document.getElementById(targetId);
        if (targetSection) {
            targetSection.classList.add('active');
            targetSection.setAttribute('aria-hidden', 'false');
            targetSection.style.display = 'block';
            targetSection.style.opacity = '1';
            targetSection.style.visibility = 'visible';
        }
    } else if (!activeButton && activeSection) {
        console.warn('⚠️ Active section found but no active button, fixing...');
        const sectionId = activeSection.id;
        const correspondingButton = document.querySelector(`[data-target="${sectionId}"]`);
        if (correspondingButton) {
            correspondingButton.classList.add('active');
            correspondingButton.setAttribute('aria-pressed', 'true');
        }
    } else if (!activeButton && !activeSection) {
        console.warn('⚠️ No active elements found, setting default...');
        // Default to terms section
        const termsButton = document.querySelector('[data-target="terms-content"]');
        const termsSection = document.getElementById('terms-content');
        if (termsButton && termsSection) {
            termsButton.classList.add('active');
            termsButton.setAttribute('aria-pressed', 'true');
            termsSection.classList.add('active');
            termsSection.setAttribute('aria-hidden', 'false');
            termsSection.style.display = 'block';
            termsSection.style.opacity = '1';
            termsSection.style.visibility = 'visible';
        }
    }
    
    console.log('✅ Content display validation completed');
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
    console.log('Active button:', document.querySelector('.toggle-btn.active')?.getAttribute('data-target'));
    console.log('Active section:', document.querySelector('.content-section.active')?.id);
};

/**
 * Force show privacy policy content (for debugging)
 */
window.forceShowPrivacy = function() {
    console.log('🔧 Forcing privacy policy display...');
    
    // Remove active from all buttons and sections
    document.querySelectorAll('.toggle-btn').forEach(btn => {
        btn.classList.remove('active');
        btn.setAttribute('aria-pressed', 'false');
    });
    
    document.querySelectorAll('.content-section').forEach(section => {
        section.classList.remove('active');
        section.setAttribute('aria-hidden', 'true');
    });
    
    // Activate privacy button and section
    const privacyBtn = document.querySelector('[data-target="privacy-content"]');
    const privacySection = document.getElementById('privacy-content');
    
    if (privacyBtn && privacySection) {
        privacyBtn.classList.add('active');
        privacyBtn.setAttribute('aria-pressed', 'true');
        
        privacySection.classList.add('active');
        privacySection.setAttribute('aria-hidden', 'false');
        
        console.log('✅ Privacy policy forced to display');
    } else {
        console.error('❌ Privacy elements not found');
    }
};

/**
 * Force show terms content (for debugging)
 */
window.forceShowTerms = function() {
    console.log('🔧 Forcing terms display...');
    
    // Remove active from all buttons and sections
    document.querySelectorAll('.toggle-btn').forEach(btn => {
        btn.classList.remove('active');
        btn.setAttribute('aria-pressed', 'false');
    });
    
    document.querySelectorAll('.content-section').forEach(section => {
        section.classList.remove('active');
        section.setAttribute('aria-hidden', 'true');
    });
    
    // Activate terms button and section
    const termsBtn = document.querySelector('[data-target="terms-content"]');
    const termsSection = document.getElementById('terms-content');
    
    if (termsBtn && termsSection) {
        termsBtn.classList.add('active');
        termsBtn.setAttribute('aria-pressed', 'true');
        
        termsSection.classList.add('active');
        termsSection.setAttribute('aria-hidden', 'false');
        
        console.log('✅ Terms forced to display');
    } else {
        console.error('❌ Terms elements not found');
    }
};

// Export functions for potential external use
window.TermsPrivacyPage = {
    getCurrentSection,
    checkPreviousAcceptance,
    updatePageTitle,
    showAcceptanceMessage,
    forceShowPrivacy: window.forceShowPrivacy,
    forceShowTerms: window.forceShowTerms
};