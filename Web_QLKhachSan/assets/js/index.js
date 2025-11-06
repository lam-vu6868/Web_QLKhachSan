document.addEventListener("DOMContentLoaded", function () {
  // ======== NAVBAR & MOBILE MENU ========
  const header = document.querySelector(".main-header");
  const navbarToggler = document.querySelector(".navbar-toggler");
  const mobileMenuOverlay = document.querySelector(".mobile-menu-overlay");
  const closeMenuBtn = document.querySelector(".close-menu-btn");
  const menuBackdrop = document.querySelector(".menu-backdrop");
  let lastScrollTop = 0;

  // --- Scroll behavior ---
  window.addEventListener("scroll", function () {
    const scrollTop = window.pageYOffset || document.documentElement.scrollTop;

    // Add scrolled class
    if (scrollTop > 50) {
      header.classList.add("scrolled");
    } else {
      header.classList.remove("scrolled");
    }

    // Hide/show navbar on scroll
    if (scrollTop > lastScrollTop && scrollTop > 200) {
      header.classList.add("hidden");
    } else {
      header.classList.remove("hidden");
    }
    lastScrollTop = scrollTop <= 0 ? 0 : scrollTop; // For Mobile or negative scrolling
  });

  // --- Toggle mobile menu ---
  function toggleMenu() {
    navbarToggler.classList.toggle("active");
    mobileMenuOverlay.classList.toggle("active");
    document.body.classList.toggle("menu-open");
  }

  // Close menu when clicking on menu items
  function closeMenu() {
    navbarToggler.classList.remove("active");
    mobileMenuOverlay.classList.remove("active");
    document.body.classList.remove("menu-open");
  }

  if (navbarToggler && mobileMenuOverlay) {
    navbarToggler.addEventListener("click", toggleMenu);
  }

  if (closeMenuBtn) {
    closeMenuBtn.addEventListener("click", toggleMenu);
  }

  if (menuBackdrop) {
    menuBackdrop.addEventListener("click", toggleMenu);
  }

  // Close menu when clicking on navigation links
  const mobileNavLinks = document.querySelectorAll('.mobile-menu-overlay a:not(.quick-action):not(.social-link)');
  mobileNavLinks.forEach(link => {
    link.addEventListener('click', function(e) {
      // Only close if it's a navigation link (not external links like phone)
      if (!link.href.startsWith('tel:') && !link.href.startsWith('mailto:')) {
        setTimeout(closeMenu, 200); // Small delay for better UX
      }
    });
  });

  // Handle ESC key to close menu
  document.addEventListener('keydown', function(e) {
    if (e.key === 'Escape' && mobileMenuOverlay.classList.contains('active')) {
      closeMenu();
    }
  });

  // ======== REVIEWS SLIDER ========
  const slider = document.querySelector(".reviews-slider");
  if (slider) {
    const prevBtn = document.querySelector(".prev-btn");
    const nextBtn = document.querySelector(".next-btn");
    const cards = document.querySelectorAll(".review-card");
    let currentIndex = 0;

    function getVisibleCards() {
      if (window.innerWidth <= 768) return 1;
      if (window.innerWidth <= 1200) return 2;
      return 3;
    }

    function updateSlider() {
      const cardWidth = cards[0].offsetWidth;
      const gap = 24;
      slider.style.transform = `translateX(-${
        currentIndex * (cardWidth + gap)
      }px)`;

      prevBtn.disabled = currentIndex === 0;
      nextBtn.disabled = currentIndex >= cards.length - getVisibleCards();
    }

    prevBtn.addEventListener("click", () => {
      if (currentIndex > 0) {
        currentIndex--;
        updateSlider();
      }
    });

    nextBtn.addEventListener("click", () => {
      if (currentIndex < cards.length - getVisibleCards()) {
        currentIndex++;
        updateSlider();
      }
    });

    window.addEventListener("resize", () => {
      if (currentIndex > cards.length - getVisibleCards()) {
        currentIndex = cards.length - getVisibleCards();
      }
      updateSlider();
    });

    updateSlider();
  }

  // ======== AOS ANIMATION INITIALIZATION ========
  AOS.init({
    duration: 1000,
    easing: "ease-in-out",
    once: true,
    mirror: false,
  });
});
