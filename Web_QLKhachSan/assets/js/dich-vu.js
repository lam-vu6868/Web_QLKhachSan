//Toggle service category dropdown
function toggleServiceCategory(categoryId) {
    const content = document.getElementById(categoryId);
    const category = content.previousElementSibling.parentElement;
    const icon = category.querySelector('.expand-icon i');
    
    // Close all other categories
    const allCategories = document.querySelectorAll('.service-category');
    
    allCategories.forEach((cat) => {
        if (cat !== category) {
            cat.classList.remove('active');
            const otherContent = cat.querySelector('.category-content');
            const otherIcon = cat.querySelector('.expand-icon i');
            if (otherContent) {
                otherContent.style.display = 'none';
            }
            if (otherIcon) {
                otherIcon.style.transform = 'rotate(0deg)';
            }
        }
    });
    
    // Toggle current category
    if (content.style.display === 'block') {
        content.style.display = 'none';
        icon.style.transform = 'rotate(0deg)';
        category.classList.remove('active');
    } else {
        content.style.display = 'block';
        icon.style.transform = 'rotate(180deg)';
        category.classList.add('active');
    }
}

// Animation on scroll
window.addEventListener('scroll', () => {
    const elements = document.querySelectorAll('.service-category');
    elements.forEach(element => {
        const elementTop = element.getBoundingClientRect().top;
        const elementVisible = 150;
        
        if (elementTop < window.innerHeight - elementVisible) {
            element.classList.add('animate');
        }
    });
});

// Initialize animations
document.addEventListener('DOMContentLoaded', () => {
    const elements = document.querySelectorAll('.service-category');
    elements.forEach((element, index) => {
        element.style.animationDelay = `${index * 0.1}s`;
    });
});
