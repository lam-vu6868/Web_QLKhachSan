// ===== HỖ TRỢ KHÁCH HÀNG JAVASCRIPT =====

document.addEventListener('DOMContentLoaded', function() {
    // Initialize all components
    initializeChatWidget();
    initializeMapModal();
    initializeFAQ();
    updateCurrentTime();
    
    // Update time every minute
    setInterval(updateCurrentTime, 60000);
});

// ===== CHAT WIDGET FUNCTIONALITY =====
function initializeChatWidget() {
    const startChatBtn = document.getElementById('startChatBtn');
    const chatWidget = document.getElementById('chatWidget');
    const closeChatBtn = document.getElementById('closeChatBtn');
    const minimizeChatBtn = document.getElementById('minimizeChatBtn');
    const chatInput = document.getElementById('chatInput');
    const sendBtn = document.getElementById('sendBtn');
    const emojiBtn = document.getElementById('emojiBtn');
    const quickReplies = document.querySelectorAll('.quick-reply-item');
    const chatMessages = document.getElementById('chatMessages');
    const typingIndicator = document.getElementById('typingIndicator');

    // Chat widget state
    let chatState = {
        isOpen: false,
        isMinimized: false,
        messageCount: 0
    };

    // Sample bot responses
    const botResponses = {
        'Tôi muốn đặt phòng': [
            'Tuyệt vời! Tôi sẽ giúp bạn đặt phòng. Bạn muốn đặt phòng cho ngày nào?',
            'Bạn có thể truy cập trang "Phòng Nghỉ" để xem các loại phòng có sẵn và đặt trực tuyến.',
            'Hoặc tôi có thể chuyển bạn đến bộ phận đặt phòng để được hỗ trợ trực tiếp.'
        ],
        'Tôi muốn hủy đặt phòng': [
            'Tôi hiểu bạn cần hủy đặt phòng. Bạn có thể cung cấp mã đặt phòng để tôi kiểm tra không?',
            'Chính sách hủy phòng cho phép hủy miễn phí trước 24 giờ.',
            'Tôi sẽ chuyển bạn đến bộ phận hỗ trợ để xử lý việc hủy phòng nhanh nhất.'
        ],
        'Tôi cần thông tin về dịch vụ': [
            'Chúng tôi có nhiều dịch vụ tuyệt vời! Bạn quan tâm đến dịch vụ nào cụ thể?',
            '🏊‍♀️ Hồ bơi & Spa\n🍽️ Nhà hàng cao cấp\n🎯 Hoạt động giải trí\n🚗 Dịch vụ đưa đón',
            'Bạn có thể xem chi tiết tại trang "Dịch Vụ" hoặc tôi có thể tư vấn cụ thể cho bạn.'
        ],
        'Tôi gặp vấn đề với tài khoản': [
            'Tôi rất tiếc khi bạn gặp vấn đề với tài khoản. Bạn gặp vấn đề gì cụ thể?',
            'Có thể là vấn đề đăng nhập, quên mật khẩu, hoặc cập nhật thông tin?',
            'Tôi sẽ chuyển bạn đến bộ phận kỹ thuật để được hỗ trợ chuyên sâu.'
        ],
        'default': [
            'Cảm ơn bạn đã liên hệ! Tôi đã ghi nhận yêu cầu của bạn.',
            'Để được hỗ trợ tốt nhất, bạn có thể:',
            '📞 Gọi hotline: (+84) 123 456 789\n📧 Email: support@serenehorizon.com\n🗺️ Ghé thăm trực tiếp khách sạn'
        ]
    };

    // Open chat widget
    startChatBtn.addEventListener('click', function() {
        openChat();
    });

    // Close chat widget
    closeChatBtn.addEventListener('click', function() {
        closeChat();
    });

    // Minimize/restore chat widget
    minimizeChatBtn.addEventListener('click', function() {
        toggleMinimizeChat();
    });

    // Send message on Enter key
    chatInput.addEventListener('keypress', function(e) {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            sendMessage();
        }
    });

    // Send message on button click
    sendBtn.addEventListener('click', sendMessage);

    // Emoji button (placeholder functionality)
    emojiBtn.addEventListener('click', function() {
        const emojis = ['😊', '👍', '❤️', '😢', '😡', '🤔', '💯', '🔥'];
        const randomEmoji = emojis[Math.floor(Math.random() * emojis.length)];
        chatInput.value += randomEmoji;
        chatInput.focus();
    });

    // Quick reply buttons
    quickReplies.forEach(button => {
        button.addEventListener('click', function() {
            const message = this.getAttribute('data-message');
            sendQuickReply(message);
        });
    });

    function openChat() {
        chatWidget.classList.add('active');
        chatState.isOpen = true;
        chatState.isMinimized = false;
        chatInput.focus();
        
        // Add opening animation
        setTimeout(() => {
            chatWidget.style.transform = 'translateY(0)';
        }, 100);
    }

    function closeChat() {
        chatWidget.classList.remove('active');
        chatWidget.classList.remove('minimized');
        chatState.isOpen = false;
        chatState.isMinimized = false;
        
        // Add closing animation
        chatWidget.style.transform = 'translateY(100%)';
    }

    function toggleMinimizeChat() {
        if (chatState.isMinimized) {
            chatWidget.classList.remove('minimized');
            chatState.isMinimized = false;
            minimizeChatBtn.innerHTML = '<i class="fas fa-minus"></i>';
        } else {
            chatWidget.classList.add('minimized');
            chatState.isMinimized = true;
            minimizeChatBtn.innerHTML = '<i class="fas fa-window-maximize"></i>';
        }
    }

    function sendMessage() {
        const message = chatInput.value.trim();
        if (!message) return;

        // Add user message
        addMessage(message, 'user');
        chatInput.value = '';
        
        // Show typing indicator
        showTypingIndicator();
        
        // Hide quick replies after first message
        if (chatState.messageCount === 0) {
            document.getElementById('quickReplies').style.display = 'none';
        }
        
        // Simulate bot response after delay
        setTimeout(() => {
            hideTypingIndicator();
            const responses = botResponses[message] || botResponses['default'];
            responses.forEach((response, index) => {
                setTimeout(() => {
                    addMessage(response, 'bot');
                }, index * 1000);
            });
        }, 1500 + Math.random() * 1000);
        
        chatState.messageCount++;
    }

    function sendQuickReply(message) {
        addMessage(message, 'user');
        document.getElementById('quickReplies').style.display = 'none';
        
        // Show typing indicator
        showTypingIndicator();
        
        // Send bot response
        setTimeout(() => {
            hideTypingIndicator();
            const responses = botResponses[message] || botResponses['default'];
            responses.forEach((response, index) => {
                setTimeout(() => {
                    addMessage(response, 'bot');
                }, index * 1000);
            });
        }, 1500 + Math.random() * 1000);
        
        chatState.messageCount++;
    }

    function addMessage(message, sender) {
        const messageDiv = document.createElement('div');
        messageDiv.className = `message ${sender}-message`;
        
        const currentTime = new Date().toLocaleTimeString('vi-VN', {
            hour: '2-digit',
            minute: '2-digit'
        });
        
        if (sender === 'bot') {
            messageDiv.innerHTML = `
                <div class="message-avatar">
                    <img src="https://images.unsplash.com/photo-1573497019940-1c28c88b4f3e?w=40&h=40&fit=crop&crop=face" alt="Bot">
                </div>
                <div class="message-content">
                    <div class="message-bubble">${message.replace(/\n/g, '<br>')}</div>
                    <div class="message-time">${currentTime}</div>
                </div>
            `;
        } else {
            messageDiv.innerHTML = `
                <div class="message-avatar">
                    <img src="https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=40&h=40&fit=crop&crop=face" alt="User">
                </div>
                <div class="message-content">
                    <div class="message-bubble">${message}</div>
                    <div class="message-time">${currentTime}</div>
                </div>
            `;
        }
        
        chatMessages.appendChild(messageDiv);
        
        // Add entrance animation
        messageDiv.style.opacity = '0';
        messageDiv.style.transform = 'translateY(20px)';
        
        setTimeout(() => {
            messageDiv.style.transition = 'all 0.3s ease';
            messageDiv.style.opacity = '1';
            messageDiv.style.transform = 'translateY(0)';
        }, 100);
        
        // Scroll to bottom
        chatMessages.scrollTop = chatMessages.scrollHeight;
    }

    function showTypingIndicator() {
        typingIndicator.classList.add('active');
        chatMessages.scrollTop = chatMessages.scrollHeight;
    }

    function hideTypingIndicator() {
        typingIndicator.classList.remove('active');
    }
}

// ===== MAP MODAL FUNCTIONALITY =====
function initializeMapModal() {
    const openMapBtn = document.getElementById('openMapBtn');
    const mapModal = document.getElementById('mapModal');
    const closeMapBtn = document.getElementById('closeMapBtn');

    openMapBtn.addEventListener('click', function() {
        mapModal.classList.add('active');
        document.body.style.overflow = 'hidden';
    });

    closeMapBtn.addEventListener('click', function() {
        closeMapModal();
    });

    // Close on overlay click
    mapModal.addEventListener('click', function(e) {
        if (e.target === mapModal) {
            closeMapModal();
        }
    });

    // Close on escape key
    document.addEventListener('keydown', function(e) {
        if (e.key === 'Escape' && mapModal.classList.contains('active')) {
            closeMapModal();
        }
    });

    function closeMapModal() {
        mapModal.classList.remove('active');
        document.body.style.overflow = '';
    }
}

// ===== FAQ FUNCTIONALITY =====
function initializeFAQ() {
    const faqCategories = document.querySelectorAll('.faq-category');
    const faqLists = document.querySelectorAll('.faq-list');
    const faqItems = document.querySelectorAll('.faq-item');

    // Category switching
    faqCategories.forEach(category => {
        category.addEventListener('click', function() {
            const targetCategory = this.getAttribute('data-category');
            
            // Update active category
            faqCategories.forEach(cat => cat.classList.remove('active'));
            this.classList.add('active');
            
            // Show/hide FAQ lists
            faqLists.forEach(list => {
                if (list.getAttribute('data-category') === targetCategory) {
                    list.style.display = 'flex';
                    // Add entrance animation
                    list.style.opacity = '0';
                    list.style.transform = 'translateY(20px)';
                    setTimeout(() => {
                        list.style.transition = 'all 0.3s ease';
                        list.style.opacity = '1';
                        list.style.transform = 'translateY(0)';
                    }, 100);
                } else {
                    list.style.display = 'none';
                }
            });
        });
    });

    // FAQ accordion functionality
    faqItems.forEach(item => {
        const question = item.querySelector('.faq-question');
        
        question.addEventListener('click', function() {
            const isActive = item.classList.contains('active');
            
            // Close all other FAQ items
            faqItems.forEach(otherItem => {
                if (otherItem !== item) {
                    otherItem.classList.remove('active');
                }
            });
            
            // Toggle current item
            if (isActive) {
                item.classList.remove('active');
            } else {
                item.classList.add('active');
            }
        });
    });
}



// ===== UTILITY FUNCTIONS =====
function updateCurrentTime() {
    const timeElement = document.getElementById('currentTime');
    if (timeElement) {
        const now = new Date();
        const timeString = now.toLocaleTimeString('vi-VN', {
            hour: '2-digit',
            minute: '2-digit'
        });
        timeElement.textContent = timeString;
    }
}

function showNotification(message, type = 'info') {
    // Remove existing notifications
    const existingNotifications = document.querySelectorAll('.notification');
    existingNotifications.forEach(notification => notification.remove());
    
    // Create notification element
    const notification = document.createElement('div');
    notification.className = `notification notification-${type}`;
    notification.innerHTML = `
        <div class="notification-content">
            <div class="notification-icon">
                ${getNotificationIcon(type)}
            </div>
            <div class="notification-message">${message}</div>
            <button class="notification-close">
                <i class="fas fa-times"></i>
            </button>
        </div>
    `;
    
    // Add styles
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        z-index: 9999;
        background: ${getNotificationColor(type)};
        color: white;
        padding: 16px 20px;
        border-radius: 8px;
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
        transform: translateX(400px);
        transition: all 0.3s ease;
        max-width: 400px;
        min-width: 300px;
    `;
    
    // Add to page
    document.body.appendChild(notification);
    
    // Animate in
    setTimeout(() => {
        notification.style.transform = 'translateX(0)';
    }, 100);
    
    // Close button functionality
    const closeBtn = notification.querySelector('.notification-close');
    closeBtn.addEventListener('click', () => {
        removeNotification(notification);
    });
    
    // Auto remove after 5 seconds
    setTimeout(() => {
        if (document.body.contains(notification)) {
            removeNotification(notification);
        }
    }, 5000);
}

function removeNotification(notification) {
    notification.style.transform = 'translateX(400px)';
    setTimeout(() => {
        if (document.body.contains(notification)) {
            notification.remove();
        }
    }, 300);
}

function getNotificationIcon(type) {
    const icons = {
        success: '<i class="fas fa-check-circle"></i>',
        error: '<i class="fas fa-exclamation-circle"></i>',
        warning: '<i class="fas fa-exclamation-triangle"></i>',
        info: '<i class="fas fa-info-circle"></i>'
    };
    return icons[type] || icons.info;
}

function getNotificationColor(type) {
    const colors = {
        success: '#28a745',
        error: '#dc3545',
        warning: '#ffc107',
        info: '#17a2b8'
    };
    return colors[type] || colors.info;
}

// ===== SMOOTH SCROLLING =====
function smoothScrollTo(target, duration = 1000) {
    const targetElement = document.querySelector(target);
    if (!targetElement) return;
    
    const targetPosition = targetElement.offsetTop;
    const startPosition = window.pageYOffset;
    const distance = targetPosition - startPosition;
    let startTime = null;
    
    function animation(currentTime) {
        if (startTime === null) startTime = currentTime;
        const timeElapsed = currentTime - startTime;
        const run = ease(timeElapsed, startPosition, distance, duration);
        window.scrollTo(0, run);
        if (timeElapsed < duration) requestAnimationFrame(animation);
    }
    
    function ease(t, b, c, d) {
        t /= d / 2;
        if (t < 1) return c / 2 * t * t + b;
        t--;
        return -c / 2 * (t * (t - 2) - 1) + b;
    }
    
    requestAnimationFrame(animation);
}

// ===== LAZY LOADING FOR IMAGES =====
function initializeLazyLoading() {
    const images = document.querySelectorAll('img[data-src]');
    const imageObserver = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const img = entry.target;
                img.src = img.dataset.src;
                img.classList.remove('lazy');
                imageObserver.unobserve(img);
            }
        });
    });
    
    images.forEach(img => imageObserver.observe(img));
}

// ===== INITIALIZE ANIMATIONS ON SCROLL =====
function initializeScrollAnimations() {
    const animatedElements = document.querySelectorAll('.animate-on-scroll');
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('animated');
            }
        });
    }, {
        threshold: 0.1,
        rootMargin: '0px'
    });
    
    animatedElements.forEach(element => observer.observe(element));
}

// ===== PERFORMANCE OPTIMIZATION =====
// Debounce function for scroll events
function debounce(func, wait, immediate) {
    let timeout;
    return function executedFunction() {
        const context = this;
        const args = arguments;
        const later = function() {
            timeout = null;
            if (!immediate) func.apply(context, args);
        };
        const callNow = immediate && !timeout;
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
        if (callNow) func.apply(context, args);
    };
}

// Throttle function for resize events
function throttle(func, limit) {
    let inThrottle;
    return function() {
        const args = arguments;
        const context = this;
        if (!inThrottle) {
            func.apply(context, args);
            inThrottle = true;
            setTimeout(() => inThrottle = false, limit);
        }
    };
}

// Initialize performance optimizations
window.addEventListener('scroll', debounce(() => {
    // Handle scroll events here if needed
}, 100));

window.addEventListener('resize', throttle(() => {
    // Handle resize events here if needed
}, 250));

// ===== ERROR HANDLING =====
window.addEventListener('error', function(e) {
    console.error('Global error:', e.error);
    // You can add error reporting here
});

window.addEventListener('unhandledrejection', function(e) {
    console.error('Unhandled promise rejection:', e.reason);
    // You can add promise rejection handling here
});

// ===== EXPORT FUNCTIONS FOR TESTING =====
if (typeof module !== 'undefined' && module.exports) {
    module.exports = {
        initializeChatWidget,
        initializeMapModal,
        initializeFAQ,
        showNotification,
        smoothScrollTo
    };
}