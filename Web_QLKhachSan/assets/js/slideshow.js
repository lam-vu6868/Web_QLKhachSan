// Simple slideshow for hero section
document.addEventListener("DOMContentLoaded", function () {
  const slides = document.querySelectorAll(".slide");
  const dots = document.querySelectorAll(".dot");
  let currentSlide = 0;

  // Show first slide
  if (slides.length > 0) {
    slides[0].classList.add("active");
    dots[0].classList.add("active");
  }

  // Function to show specific slide
  function showSlide(index) {
    // Hide all slides
    slides.forEach((slide) => slide.classList.remove("active"));
    dots.forEach((dot) => dot.classList.remove("active"));

    // Show current slide
    if (slides[index] && dots[index]) {
      slides[index].classList.add("active");
      dots[index].classList.add("active");
      currentSlide = index;
    }
  }

  // Next slide function
  function nextSlide() {
    const next = (currentSlide + 1) % slides.length;
    showSlide(next);
  }

  // Add click event to dots
  dots.forEach((dot, index) => {
    dot.addEventListener("click", () => showSlide(index));
  });

  // Auto-advance every 5 seconds
  if (slides.length > 1) {
    setInterval(nextSlide, 5000);
  }
});
