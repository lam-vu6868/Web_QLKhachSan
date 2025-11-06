// ==================== CUSTOMER TIER PAGE FUNCTIONALITY ==================== 

// Tier data configuration
const TIER_DATA = [
    {
        name: 'Hạng Đồng',
        englishName: 'Bronze',
        class: 'bronze',
        icon: 'fas fa-medal',
        subtitle: 'Thành viên mới',
        requirement: '0-4 đặt phòng',
        discount: '5%',
        currentBookings: 2,
        targetBookings: 5,
        nextTier: 'Hạng Bạc',
        progress: 40,
        benefits: [
            { icon: 'fas fa-percentage', text: 'Giảm 5% mọi đặt phòng' },
            { icon: 'fas fa-wifi', text: 'WiFi miễn phí' },
            { icon: 'fas fa-headset', text: 'Hỗ trợ khách hàng 24/7' },
            { icon: 'fas fa-star', text: 'Tích lũy điểm thưởng' }
        ]
    },
    {
        name: 'Hạng Bạc',
        englishName: 'Silver',
        class: 'silver',
        icon: 'fas fa-trophy',
        subtitle: 'Thành viên trung thành',
        requirement: '5-9 đặt phòng',
        discount: '10%',
        currentBookings: 7,
        targetBookings: 10,
        nextTier: 'Hạng Vàng',
        progress: 70,
        benefits: [
            { icon: 'fas fa-percentage', text: 'Giảm 10% mọi đặt phòng' },
            { icon: 'fas fa-wifi', text: 'WiFi miễn phí' },
            { icon: 'fas fa-clock', text: 'Late checkout miễn phí' },
            { icon: 'fas fa-level-up-alt', text: 'Nâng cấp phòng (tùy tình hình)' },
            { icon: 'fas fa-glass-cheers', text: 'Welcome drink' }
        ]
    },
    {
        name: 'Hạng Vàng',
        englishName: 'Gold',
        class: 'gold',
        icon: 'fas fa-crown',
        subtitle: 'Thành viên VIP',
        requirement: '10-19 đặt phòng',
        discount: '15%',
        currentBookings: 15,
        targetBookings: 20,
        nextTier: 'Hạng Bạch Kim',
        progress: 75,
        benefits: [
            { icon: 'fas fa-percentage', text: 'Giảm 15% mọi đặt phòng' },
            { icon: 'fas fa-wifi', text: 'WiFi Premium miễn phí' },
            { icon: 'fas fa-car', text: 'Đưa đón sân bay miễn phí' },
            { icon: 'fas fa-spa', text: 'Spa giảm giá 20%' },
            { icon: 'fas fa-concierge-bell', text: 'Dịch vụ phòng 24/7' }
        ]
    },
    {
        name: 'Hạng Bạch Kim',
        englishName: 'Platinum',
        class: 'platinum',
        icon: 'fas fa-chess-king',
        subtitle: 'Thành viên cao cấp',
        requirement: '20-29 đặt phòng',
        discount: '20%',
        currentBookings: 25,
        targetBookings: 30,
        nextTier: 'Hạng Lục Bảo',
        progress: 83,
        benefits: [
            { icon: 'fas fa-percentage', text: 'Giảm 20% mọi đặt phòng' },
            { icon: 'fas fa-user-tie', text: 'Butler service cá nhân' },
            { icon: 'fas fa-utensils', text: 'Complimentary breakfast' },
            { icon: 'fas fa-couch', text: 'VIP lounge access' },
            { icon: 'fas fa-level-up-alt', text: 'Guaranteed room upgrade' }
        ]
    },
    {
        name: 'Hạng Lục Bảo',
        englishName: 'Emerald',
        class: 'emerald',
        icon: 'fas fa-leaf',
        subtitle: 'Thành viên sinh thái',
        requirement: '30-34 đặt phòng',
        discount: '22%',
        currentBookings: 32,
        targetBookings: 35,
        nextTier: 'Hạng Kim Cương',
        progress: 91,
        benefits: [
            { icon: 'fas fa-percentage', text: 'Giảm 22% mọi đặt phòng' },
            { icon: 'fas fa-spa', text: 'Wellness & Spa unlimited' },
            { icon: 'fas fa-utensils', text: 'Private dining experience' },
            { icon: 'fas fa-mountain', text: 'Exclusive nature tours' },
            { icon: 'fas fa-leaf', text: 'Eco-luxury amenities' }
        ]
    },
    {
        name: 'Hạng Kim Cương',
        englishName: 'Diamond',
        class: 'diamond',
        icon: 'fas fa-diamond',
        subtitle: 'Thành viên kim cương',
        requirement: '35-44 đặt phòng',
        discount: '25%',
        currentBookings: 40,
        targetBookings: 45,
        nextTier: 'Hạng Tinh Anh',
        progress: 89,
        benefits: [
            { icon: 'fas fa-percentage', text: 'Giảm 25% mọi đặt phòng' },
            { icon: 'fas fa-plane', text: 'Private jet transfer' },
            { icon: 'fas fa-star', text: 'Michelin dining experience' },
            { icon: 'fas fa-home', text: 'Exclusive suite access' },
            { icon: 'fas fa-concierge-bell', text: 'Personal concierge 24/7' }
        ]
    },
    {
        name: 'Hạng Tinh Anh',
        englishName: 'Elite',
        class: 'elite',
        icon: 'fas fa-star',
        subtitle: 'Thành viên tinh hoa',
        requirement: '45-54 đặt phòng',
        discount: '30%',
        currentBookings: 50,
        targetBookings: 55,
        nextTier: 'Hạng Hồng Ngọc',
        progress: 91,
        benefits: [
            { icon: 'fas fa-percentage', text: 'Giảm 30% mọi đặt phòng' },
            { icon: 'fas fa-helicopter', text: 'Helicopter transfer' },
            { icon: 'fas fa-chef-hat', text: 'Private chef service' },
            { icon: 'fas fa-crown', text: 'Exclusive penthouse access' },
            { icon: 'fas fa-star', text: 'VIP event invitations' }
        ]
    },
    {
        name: 'Hạng Hồng Ngọc',
        englishName: 'Ruby',
        class: 'ruby',
        icon: 'fas fa-gem',
        subtitle: 'Thành viên huyền thoại',
        requirement: '55+ đặt phòng',
        discount: '35%',
        currentBookings: 60,
        targetBookings: null,
        nextTier: null,
        progress: 100,
        benefits: [
            { icon: 'fas fa-percentage', text: 'Giảm 35% mọi đặt phòng' },
            { icon: 'fas fa-gem', text: 'Exclusive ruby experience' },
            { icon: 'fas fa-crown', text: 'Royal treatment package' },
            { icon: 'fas fa-island', text: 'Private island resort access' },
            { icon: 'fas fa-concierge-bell', text: 'Personal ruby concierge' }
        ]
    }
];

let currentTierIndex = 0; // Start with Bronze tier

document.addEventListener('DOMContentLoaded', function() {
    initializeTierPage();
});

function initializeTierPage() {
    // Initialize tier navigation
    initializeTierNavigation();
    
    // Initialize progress bar animation
    animateProgressBar();
    
    // Initialize tier cards animations
    initializeTierCardAnimations();
    
    // Initialize benefit items animations
    initializeBenefitAnimations();
    
    // Initialize comparison table highlighting
    initializeComparisonTable();
    
    // Initialize tips cards hover effects
    initializeTipsEffects();
    
    // Initialize scroll animations
    initializeScrollAnimations();
    
    // Update current tier display
    updateCurrentTierDisplay();
}

// ==================== TIER NAVIGATION ==================== 
function initializeTierNavigation() {
    const prevBtn = document.getElementById('prevTierBtn');
    const nextBtn = document.getElementById('nextTierBtn');
    
    if (prevBtn && nextBtn) {
        prevBtn.addEventListener('click', () => navigateTier(-1));
        nextBtn.addEventListener('click', () => navigateTier(1));
        
        // Update button states
        updateNavigationButtons();
    }
}

function navigateTier(direction) {
    const newIndex = currentTierIndex + direction;
    
    if (newIndex >= 0 && newIndex < TIER_DATA.length) {
        currentTierIndex = newIndex;
        updateCurrentTierDisplay();
        updateNavigationButtons();
        
        // Add transition effect
        const card = document.getElementById('currentTierCard');
        if (card) {
            card.style.transform = 'scale(0.95)';
            card.style.opacity = '0.7';
            
            setTimeout(() => {
                card.style.transform = 'scale(1)';
                card.style.opacity = '1';
            }, 200);
        }
    }
}

function updateNavigationButtons() {
    const prevBtn = document.getElementById('prevTierBtn');
    const nextBtn = document.getElementById('nextTierBtn');
    
    if (prevBtn && nextBtn) {
        prevBtn.disabled = currentTierIndex === 0;
        nextBtn.disabled = currentTierIndex === TIER_DATA.length - 1;
    }
}

function updateCurrentTierDisplay() {
    const tier = TIER_DATA[currentTierIndex];
    if (!tier) return;
    
    // Update tier badge
    const tierBadge = document.querySelector('.tier-badge');
    const tierIcon = document.querySelector('.tier-badge .tier-icon i');
    const tierName = document.querySelector('.tier-info h2');
    const tierSubtitle = document.querySelector('.tier-info p');
    const currentTierCard = document.getElementById('currentTierCard');
    
    if (tierBadge) {
        // Remove all tier classes
        tierBadge.className = 'tier-badge ' + tier.class;
    }
    
    // Update current tier card border color using CSS classes
    if (currentTierCard) {
        // Remove existing tier classes
        const tierClasses = ['bronze', 'silver', 'gold', 'platinum', 'emerald', 'diamond', 'elite', 'ruby'];
        tierClasses.forEach(cls => currentTierCard.classList.remove(cls));
        
        // Add current tier class
        currentTierCard.classList.add(tier.class);
    }
    
    if (tierIcon) {
        tierIcon.className = tier.icon;
    }
    
    if (tierName) {
        tierName.textContent = tier.name;
    }
    
    if (tierSubtitle) {
        tierSubtitle.textContent = tier.subtitle;
    }
    
    // Update progress section
    updateProgressSection(tier);
    
    // Update benefits
    updateBenefitsSection(tier);
}

function updateProgressSection(tier) {
    const currentBookingsEl = document.querySelector('.current-bookings');
    const targetBookingsEl = document.querySelector('.target-bookings');
    const progressFill = document.querySelector('.progress-fill');
    const progressText = document.querySelector('.progress-text');
    
    if (currentBookingsEl) {
        currentBookingsEl.textContent = tier.currentBookings + ' đặt phòng';
    }
    
    if (targetBookingsEl && tier.nextTier) {
        targetBookingsEl.textContent = ` / ${tier.targetBookings} để lên ${tier.nextTier}`;
    } else if (targetBookingsEl) {
        targetBookingsEl.textContent = ' - Cấp độ cao nhất';
    }
    
    if (progressFill) {
        progressFill.style.width = tier.progress + '%';
        progressFill.setAttribute('data-progress', tier.progress);
    }
    
    if (progressText && tier.nextTier) {
        const remaining = tier.targetBookings - tier.currentBookings;
        progressText.textContent = `Còn ${remaining} đặt phòng nữa để lên cấp độ tiếp theo`;
    } else if (progressText) {
        progressText.textContent = 'Chúc mừng! Bạn đã đạt cấp độ cao nhất!';
    }
}

function updateBenefitsSection(tier) {
    const benefitsGrid = document.querySelector('.benefits-grid');
    if (!benefitsGrid) return;
    
    benefitsGrid.innerHTML = tier.benefits.map(benefit => `
        <div class="benefit-item active">
            <i class="${benefit.icon}"></i>
            <span>${benefit.text}</span>
        </div>
    `).join('');
    
    // Reinitialize benefit animations
    initializeBenefitAnimations();
}

// ==================== PROGRESS BAR ANIMATION ==================== 
function animateProgressBar() {
    const progressFill = document.querySelector('.progress-fill');
    if (!progressFill) return;
    
    const targetProgress = progressFill.getAttribute('data-progress') || 75;
    
    // Reset width first
    progressFill.style.width = '0%';
    
    // Animate to target after a small delay
    setTimeout(() => {
        progressFill.style.transition = 'width 2s cubic-bezier(0.25, 0.46, 0.45, 0.94)';
        progressFill.style.width = targetProgress + '%';
    }, 500);
    
    // Add sparkle effect during animation
    setTimeout(() => {
        addSparkleEffect(progressFill);
    }, 1500);
}

function addSparkleEffect(element) {
    const sparkle = document.createElement('div');
    sparkle.style.cssText = `
        position: absolute;
        top: 50%;
        right: 10px;
        width: 8px;
        height: 8px;
        background: white;
        border-radius: 50%;
        transform: translateY(-50%);
        animation: sparkle 1s ease-in-out;
    `;
    
    // Add sparkle keyframes if not exists
    if (!document.querySelector('#sparkle-style')) {
        const style = document.createElement('style');
        style.id = 'sparkle-style';
        style.textContent = `
            @keyframes sparkle {
                0%, 100% { opacity: 0; transform: translateY(-50%) scale(0); }
                50% { opacity: 1; transform: translateY(-50%) scale(1.5); }
            }
        `;
        document.head.appendChild(style);
    }
    
    element.appendChild(sparkle);
    
    // Remove sparkle after animation
    setTimeout(() => {
        if (sparkle.parentNode) {
            sparkle.parentNode.removeChild(sparkle);
        }
    }, 1000);
}

// ==================== TIER CARDS ANIMATIONS ==================== 
function initializeTierCardAnimations() {
    const tierCards = document.querySelectorAll('.tier-card');
    
    tierCards.forEach((card, index) => {
        // Stagger animation delays
        card.style.animationDelay = (index * 0.1) + 's';
        
        // Add hover sound effect (optional)
        card.addEventListener('mouseenter', function() {
            card.style.transform = 'translateY(-10px) scale(1.02)';
            
            // Add ripple effect
            addRippleEffect(card);
        });
        
        card.addEventListener('mouseleave', function() {
            card.style.transform = '';
        });
        
        // Special effects for current tier
        if (card.classList.contains('current')) {
            addCurrentTierEffects(card);
        }
        
        // Locked tier effects
        if (card.classList.contains('locked')) {
            addLockedTierEffects(card);
        }
    });
}

function addRippleEffect(element) {
    const ripple = document.createElement('div');
    const rect = element.getBoundingClientRect();
    
    ripple.style.cssText = `
        position: absolute;
        top: 50%;
        left: 50%;
        width: 0;
        height: 0;
        border-radius: 50%;
        background: rgba(212, 175, 55, 0.3);
        transform: translate(-50%, -50%);
        animation: ripple 0.6s ease-out;
        pointer-events: none;
        z-index: 1;
    `;
    
    // Add ripple keyframes if not exists
    if (!document.querySelector('#ripple-style')) {
        const style = document.createElement('style');
        style.id = 'ripple-style';
        style.textContent = `
            @keyframes ripple {
                to {
                    width: 200px;
                    height: 200px;
                    opacity: 0;
                }
            }
        `;
        document.head.appendChild(style);
    }
    
    element.style.position = 'relative';
    element.appendChild(ripple);
    
    setTimeout(() => {
        if (ripple.parentNode) {
            ripple.parentNode.removeChild(ripple);
        }
    }, 600);
}

function addCurrentTierEffects(card) {
    // Add pulsing glow effect
    setInterval(() => {
        const icon = card.querySelector('.tier-icon');
        if (icon) {
            icon.style.transform = 'scale(1.1) rotate(5deg)';
            setTimeout(() => {
                icon.style.transform = '';
            }, 300);
        }
    }, 3000);
}

function addLockedTierEffects(card) {
    card.addEventListener('click', function(e) {
        e.preventDefault();
        
        // Shake animation for locked tiers
        card.style.animation = 'shake 0.5s ease-in-out';
        
        // Add shake keyframes if not exists
        if (!document.querySelector('#shake-style')) {
            const style = document.createElement('style');
            style.id = 'shake-style';
            style.textContent = `
                @keyframes shake {
                    0%, 100% { transform: translateX(0); }
                    25% { transform: translateX(-5px); }
                    75% { transform: translateX(5px); }
                }
            `;
            document.head.appendChild(style);
        }
        
        // Show tooltip
        showTooltip(card, 'Bạn cần đặt thêm phòng để mở khóa cấp độ này!');
        
        // Reset animation
        setTimeout(() => {
            card.style.animation = '';
        }, 500);
    });
}

function showTooltip(element, message) {
    const tooltip = document.createElement('div');
    tooltip.textContent = message;
    tooltip.style.cssText = `
        position: absolute;
        top: -50px;
        left: 50%;
        transform: translateX(-50%);
        background: #333;
        color: white;
        padding: 8px 12px;
        border-radius: 5px;
        font-size: 0.85rem;
        white-space: nowrap;
        z-index: 1000;
        opacity: 0;
        animation: tooltipFade 2s ease-in-out;
    `;
    
    // Add tooltip keyframes if not exists
    if (!document.querySelector('#tooltip-style')) {
        const style = document.createElement('style');
        style.id = 'tooltip-style';
        style.textContent = `
            @keyframes tooltipFade {
                0%, 100% { opacity: 0; transform: translateX(-50%) translateY(5px); }
                10%, 90% { opacity: 1; transform: translateX(-50%) translateY(0); }
            }
        `;
        document.head.appendChild(style);
    }
    
    element.style.position = 'relative';
    element.appendChild(tooltip);
    
    setTimeout(() => {
        if (tooltip.parentNode) {
            tooltip.parentNode.removeChild(tooltip);
        }
    }, 2000);
}

// ==================== BENEFIT ITEMS ANIMATIONS ==================== 
function initializeBenefitAnimations() {
    const benefitItems = document.querySelectorAll('.benefit-item');
    
    benefitItems.forEach((item, index) => {
        item.style.animationDelay = (index * 0.1) + 's';
        
        item.addEventListener('mouseenter', function() {
            const icon = item.querySelector('i');
            if (icon) {
                icon.style.transform = 'scale(1.3) rotate(360deg)';
                icon.style.transition = 'transform 0.5s ease';
            }
        });
        
        item.addEventListener('mouseleave', function() {
            const icon = item.querySelector('i');
            if (icon) {
                icon.style.transform = '';
            }
        });
    });
}

// ==================== COMPARISON TABLE HIGHLIGHTING ==================== 
function initializeComparisonTable() {
    const table = document.querySelector('.comparison-table');
    if (!table) return;
    
    const rows = table.querySelectorAll('tbody tr');
    
    rows.forEach(row => {
        row.addEventListener('mouseenter', function() {
            row.style.backgroundColor = 'rgba(212, 175, 55, 0.1)';
            row.style.transform = 'scale(1.02)';
            row.style.transition = 'all 0.3s ease';
        });
        
        row.addEventListener('mouseleave', function() {
            row.style.backgroundColor = '';
            row.style.transform = '';
        });
    });
    
    // Highlight current tier column
    const currentCells = table.querySelectorAll('.current');
    currentCells.forEach(cell => {
        cell.style.position = 'relative';
        cell.style.overflow = 'hidden';
        
        // Add glowing border effect
        setInterval(() => {
            cell.style.boxShadow = '0 0 20px rgba(212, 175, 55, 0.5)';
            setTimeout(() => {
                cell.style.boxShadow = '';
            }, 1000);
        }, 3000);
    });
}

// ==================== TIPS EFFECTS ==================== 
function initializeTipsEffects() {
    const tipCards = document.querySelectorAll('.tip-card');
    
    tipCards.forEach((card, index) => {
        card.style.animationDelay = (index * 0.15) + 's';
        
        card.addEventListener('mouseenter', function() {
            // Rotate icon on hover
            const icon = card.querySelector('.tip-icon');
            if (icon) {
                icon.style.transform = 'scale(1.1) rotate(10deg)';
                icon.style.transition = 'transform 0.3s ease';
            }
            
            // Add floating effect
            card.style.transform = 'translateY(-15px)';
            card.style.transition = 'transform 0.3s cubic-bezier(0.25, 0.46, 0.45, 0.94)';
        });
        
        card.addEventListener('mouseleave', function() {
            const icon = card.querySelector('.tip-icon');
            if (icon) {
                icon.style.transform = '';
            }
            card.style.transform = '';
        });
    });
}

// ==================== SCROLL ANIMATIONS ==================== 
function initializeScrollAnimations() {
    // Intersection Observer for fade-in animations
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };
    
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('fade-in-visible');
            }
        });
    }, observerOptions);
    
    // Add fade-in class to elements
    const elements = document.querySelectorAll('.tier-card, .tip-card, .benefit-item, .current-tier-card');
    elements.forEach(element => {
        element.classList.add('fade-in-element');
        observer.observe(element);
    });
    
    // Add CSS for fade-in effect
    if (!document.querySelector('#scroll-animation-style')) {
        const style = document.createElement('style');
        style.id = 'scroll-animation-style';
        style.textContent = `
            .fade-in-element {
                opacity: 0;
                transform: translateY(30px);
                transition: opacity 0.6s ease, transform 0.6s ease;
            }
            .fade-in-visible {
                opacity: 1;
                transform: translateY(0);
            }
        `;
        document.head.appendChild(style);
    }
}

// ==================== UTILITY FUNCTIONS ==================== 
function simulateProgressUpdate(newBookings) {
    const currentBookingsEl = document.querySelector('.current-bookings');
    const progressFill = document.querySelector('.progress-fill');
    const progressText = document.querySelector('.progress-text');
    
    if (!currentBookingsEl || !progressFill) return;
    
    // Calculate new progress
    const targetTier = 20; // Next tier requirement
    const progressPercentage = Math.min((newBookings / targetTier) * 100, 100);
    const remaining = Math.max(targetTier - newBookings, 0);
    
    // Animate progress update
    progressFill.style.transition = 'width 1s ease';
    progressFill.style.width = progressPercentage + '%';
    
    // Update text
    currentBookingsEl.textContent = newBookings + ' đặt phòng';
    if (progressText) {
        progressText.textContent = remaining > 0 ? 
            `Còn ${remaining} đặt phòng nữa để lên cấp độ tiếp theo` : 
            'Chúc mừng! Bạn đã đủ điều kiện lên cấp độ tiếp theo!';
    }
    
    // Celebration effect if reached next tier
    if (remaining === 0) {
        triggerCelebration();
    }
}

function triggerCelebration() {
    // Create confetti effect
    const confettiColors = ['#d4af37', '#f4d03f', '#3498db', '#27ae60', '#e74c3c'];
    
    for (let i = 0; i < 50; i++) {
        setTimeout(() => {
            createConfetti(confettiColors[Math.floor(Math.random() * confettiColors.length)]);
        }, i * 50);
    }
    
    // Show congratulations message
    showCongratulationsModal();
}

function createConfetti(color) {
    const confetti = document.createElement('div');
    confetti.style.cssText = `
        position: fixed;
        top: -10px;
        left: ${Math.random() * 100}%;
        width: 10px;
        height: 10px;
        background: ${color};
        z-index: 10000;
        pointer-events: none;
        animation: confettiFall 3s linear forwards;
    `;
    
    // Add confetti animation if not exists
    if (!document.querySelector('#confetti-style')) {
        const style = document.createElement('style');
        style.id = 'confetti-style';
        style.textContent = `
            @keyframes confettiFall {
                to {
                    transform: translateY(100vh) rotate(720deg);
                    opacity: 0;
                }
            }
        `;
        document.head.appendChild(style);
    }
    
    document.body.appendChild(confetti);
    
    setTimeout(() => {
        if (confetti.parentNode) {
            confetti.parentNode.removeChild(confetti);
        }
    }, 3000);
}

function showCongratulationsModal() {
    const modal = document.createElement('div');
    modal.innerHTML = `
        <div style="
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0, 0, 0, 0.8);
            display: flex;
            align-items: center;
            justify-content: center;
            z-index: 10001;
        ">
            <div style="
                background: white;
                padding: 40px;
                border-radius: 20px;
                text-align: center;
                max-width: 400px;
                animation: modalPop 0.5s ease;
            ">
                <div style="font-size: 60px; margin-bottom: 20px;">🎉</div>
                <h2 style="color: #d4af37; margin-bottom: 15px;">Chúc mừng!</h2>
                <p style="margin-bottom: 20px;">Bạn đã đủ điều kiện lên cấp độ tiếp theo!</p>
                <button onclick="this.parentElement.parentElement.remove()" style="
                    padding: 12px 30px;
                    background: #d4af37;
                    color: white;
                    border: none;
                    border-radius: 25px;
                    font-weight: bold;
                    cursor: pointer;
                ">Tuyệt vời!</button>
            </div>
        </div>
    `;
    
    // Add modal animation if not exists
    if (!document.querySelector('#modal-style')) {
        const style = document.createElement('style');
        style.id = 'modal-style';
        style.textContent = `
            @keyframes modalPop {
                from { transform: scale(0.8); opacity: 0; }
                to { transform: scale(1); opacity: 1; }
            }
        `;
        document.head.appendChild(style);
    }
    
    document.body.appendChild(modal);
}

// ==================== TIER STATUS UPDATE ==================== 
function updateTierStatus(currentBookings) {
    let tierName, tierClass, tierIcon, tierBenefits, discount;
    
    if (currentBookings < 5) {
        tierName = 'Hạng Đồng';
        tierClass = 'bronze';
        tierIcon = 'fas fa-medal';
        discount = '5%';
        tierBenefits = [
            'Tích lũy điểm thưởng',
            'Hỗ trợ khách hàng 24/7',
            'WiFi miễn phí'
        ];
    } else if (currentBookings < 10) {
        tierName = 'Hạng Bạc';
        tierClass = 'silver';
        tierIcon = 'fas fa-trophy';
        discount = '10%';
        tierBenefits = [
            'Tất cả quyền lợi Hạng Đồng',
            'Late checkout miễn phí',
            'Nâng cấp phòng (tùy tình hình)',
            'Welcome drink'
        ];
    } else if (currentBookings < 20) {
        tierName = 'Hạng Vàng';
        tierClass = 'gold';
        tierIcon = 'fas fa-crown';
        discount = '15%';
        tierBenefits = [
            'Tất cả quyền lợi Hạng Bạc',
            'Đưa đón sân bay miễn phí',
            'Spa giảm 20%',
            'Dịch vụ phòng 24/7',
            'WiFi Premium'
        ];
    } else if (currentBookings < 35) {
        tierName = 'Hạng Kim Cương';
        tierClass = 'diamond';
        tierIcon = 'fas fa-gem';
        discount = '20%';
        tierBenefits = [
            'Tất cả quyền lợi Hạng Vàng',
            'Butler service cá nhân',
            'Complimentary breakfast',
            'VIP lounge access',
            'Guaranteed room upgrade'
        ];
    } else {
        tierName = 'Hạng Bạch Kim';
        tierClass = 'platinum';
        tierIcon = 'fas fa-chess-king';
        discount = '25%';
        tierBenefits = [
            'Tất cả quyền lợi Hạng Kim Cương',
            'Private jet transfer',
            'Michelin dining experience',
            'Exclusive suite access',
            'Personal concierge'
        ];
    }
    
    // Update UI elements
    updateTierDisplay(tierName, tierClass, tierIcon, tierBenefits, discount);
}

function updateTierDisplay(tierName, tierClass, tierIcon, tierBenefits, discount) {
    // Update current tier badge
    const tierNameEl = document.querySelector('.tier-info h2');
    const tierIconEl = document.querySelector('.tier-badge .tier-icon i');
    
    if (tierNameEl) tierNameEl.textContent = tierName;
    if (tierIconEl) tierIconEl.className = tierIcon;
    
    // Update benefits
    const benefitsGrid = document.querySelector('.benefits-grid');
    if (benefitsGrid && tierBenefits) {
        benefitsGrid.innerHTML = tierBenefits.map(benefit => `
            <div class="benefit-item active">
                <i class="fas fa-check"></i>
                <span>${benefit}</span>
            </div>
        `).join('');
        
        // Reinitialize benefit animations
        initializeBenefitAnimations();
    }
}

// ==================== EXPORT FOR TESTING ==================== 
window.tierPageFunctions = {
    simulateProgressUpdate,
    updateTierStatus,
    triggerCelebration,
    navigateTier,
    getCurrentTier: () => TIER_DATA[currentTierIndex],
    setTierIndex: (index) => {
        if (index >= 0 && index < TIER_DATA.length) {
            currentTierIndex = index;
            updateCurrentTierDisplay();
            updateNavigationButtons();
        }
    },
    getTierData: () => TIER_DATA
};