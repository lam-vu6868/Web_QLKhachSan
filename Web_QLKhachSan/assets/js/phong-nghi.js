// JavaScript for Room Listing Page - Sticky Sidebar
document.addEventListener("DOMContentLoaded", function () {
  const filterSidebar = document.querySelector(".filter-sidebar");

  if (filterSidebar) {
    // Debug: Log initial position
    console.log("Filter sidebar found:", filterSidebar);
    console.log(
      "Initial position:",
      window.getComputedStyle(filterSidebar).position
    );

    // Force sticky position if not already set
    const ensureSticky = () => {
      const currentPosition = window.getComputedStyle(filterSidebar).position;
      if (currentPosition !== "sticky" && window.innerWidth > 991) {
        filterSidebar.style.position = "sticky";
        filterSidebar.style.top = "80px";
        filterSidebar.style.zIndex = "999";
        console.log("Forced sticky position");
      }
    };

    // Ensure sticky on load and resize
    ensureSticky();
    window.addEventListener("resize", ensureSticky);

    // Debug scroll events
    window.addEventListener("scroll", function () {
      const rect = filterSidebar.getBoundingClientRect();
      console.log("Scroll position:", window.scrollY, "Sidebar top:", rect.top);
    });

    // Add visual indicator when sticky is active
    const observer = new IntersectionObserver(
      ([entry]) => {
        if (entry.intersectionRatio < 1) {
          filterSidebar.classList.add("is-sticky");
        } else {
          filterSidebar.classList.remove("is-sticky");
        }
      },
      {
        threshold: 1,
        rootMargin: "-80px 0px 0px 0px",
      }
    );

    observer.observe(filterSidebar);
  }

  // Price range slider functionality
  const priceSlider = document.querySelector(
    '.price-range-slider input[type="range"]'
  );
  const priceDisplay = document.querySelector(".price-range-slider p");

  if (priceSlider && priceDisplay) {
    priceSlider.addEventListener("input", function () {
      const value = this.value;
      priceDisplay.textContent = `Giá: $50 - $${value}`;
    });
  }

  // Filter functionality
  const applyFilterBtn = document.querySelector(".filter-apply-btn");
  if (applyFilterBtn) {
    applyFilterBtn.addEventListener("click", function () {
      // Here you would implement actual filtering logic
      console.log("Applying filters...");

      // For now, just show a message
      alert("Bộ lọc đã được áp dụng!");
    });
  }

  // --- Logic cho Off-Canvas Filter ---
  const openFilterBtn = document.querySelector(".open-filter-btn");
  const closeFilterBtn = document.querySelector(".close-filter-btn");

  // Tạo và chèn backdrop vào body
  const filterBackdrop = document.createElement("div");
  filterBackdrop.className = "filter-backdrop";
  document.body.appendChild(filterBackdrop);

  // Hàm để đóng/mở filter
  const toggleFilter = (show) => {
    if (filterSidebar && filterBackdrop) {
      filterSidebar.classList.toggle("active", show);
      filterBackdrop.classList.toggle("active", show);
      // Chặn cuộn trang nền khi filter mở
      document.body.style.overflow = show ? "hidden" : "";
    }
  };

  // Gán sự kiện cho các nút
  if (openFilterBtn) {
    openFilterBtn.addEventListener("click", () => toggleFilter(true));
  }

  if (closeFilterBtn) {
    closeFilterBtn.addEventListener("click", () => toggleFilter(false));
  }

  // Gán sự kiện cho backdrop để đóng filter
  filterBackdrop.addEventListener("click", () => toggleFilter(false));

  // --- Enhanced Filter Functionality ---

  // Collapsible Filter
  const filterHeader = document.querySelector(".filter-header");
  const filterContent = document.querySelector(".filter-content");
  const filterToggle = document.querySelector(".filter-toggle");

  if (filterHeader && filterContent && filterToggle) {
    filterHeader.addEventListener("click", () => {
      const isActive = filterContent.classList.contains("active");
      filterContent.classList.toggle("active", !isActive);
      filterToggle.classList.toggle("active", !isActive);
    });
  }

  // Quick Filter Tags
  const quickFilterTags = document.querySelectorAll(".quick-filter-tag");
  quickFilterTags.forEach((tag) => {
    tag.addEventListener("click", () => {
      // Remove active from all tags
      quickFilterTags.forEach((t) => t.classList.remove("active"));
      // Add active to clicked tag
      tag.classList.add("active");

      // Here you can add logic to filter rooms based on the selected quick filter
      console.log("Quick filter selected:", tag.textContent);
    });
  });

  // Enhanced Apply Button with Loading State
  const enhancedApplyBtn = document.querySelector(".filter-apply-btn");
  if (enhancedApplyBtn) {
    enhancedApplyBtn.addEventListener("click", function () {
      // Add loading state
      this.classList.add("loading");

      // Simulate API call
      setTimeout(() => {
        this.classList.remove("loading");
        console.log("Filters applied successfully!");

        // Close filter on mobile after applying
        if (window.innerWidth <= 767 && filterContent) {
          filterContent.classList.remove("active");
          filterToggle.classList.remove("active");
        }
      }, 1500);
    });
  }

  // --- Enhanced Sticky Filter ---
  let lastScrollTop = 0;
  const stickyFilter = document.querySelector(".filter-sidebar");

  // Force sticky positioning detection
  const testSticky = () => {
    const testEl = document.createElement("div");
    testEl.style.position = "sticky";
    document.body.appendChild(testEl);
    const isSticky = testEl.style.position === "sticky";
    document.body.removeChild(testEl);

    if (!isSticky) {
      document.documentElement.classList.add("no-sticky");
    }
  };

  testSticky();

  // Only apply sticky for tablet range (768px - 991px)
  if (stickyFilter && window.innerWidth <= 991 && window.innerWidth >= 768) {
    // Force sticky positioning only for tablet
    const forceSticky = () => {
      if (window.innerWidth <= 991 && window.innerWidth >= 768) {
        stickyFilter.style.position = "sticky";
        stickyFilter.style.position = "-webkit-sticky";
        stickyFilter.style.top = "80px";
        stickyFilter.style.zIndex = "1000";
      } else if (window.innerWidth < 768) {
        // Reset to normal position for mobile
        stickyFilter.style.position = "static";
        stickyFilter.style.top = "auto";
        stickyFilter.style.zIndex = "auto";
      }
    };

    forceSticky();
    window.addEventListener("resize", forceSticky);

    // Only add scroll effects for tablet sticky
    window.addEventListener("scroll", () => {
      const scrollTop =
        window.pageYOffset || document.documentElement.scrollTop;

      // Only apply scroll effects if sticky (tablet range)
      if (window.innerWidth <= 991 && window.innerWidth >= 768) {
        if (scrollTop > 50) {
          stickyFilter.classList.add("scrolled");
          stickyFilter.style.boxShadow = "0 6px 20px rgba(0, 0, 0, 0.2)";
        } else {
          stickyFilter.classList.remove("scrolled");
          stickyFilter.style.boxShadow = "0 4px 15px rgba(0, 0, 0, 0.15)";
        }
      }

      lastScrollTop = scrollTop;
    });

    // Auto-collapse filter when scrolling down on mobile
    if (window.innerWidth <= 767) {
      let scrollTimer = null;

      window.addEventListener("scroll", () => {
        // Clear previous timer
        if (scrollTimer) {
          clearTimeout(scrollTimer);
        }

        // Set timer to collapse filter after scroll stops
        scrollTimer = setTimeout(() => {
          const scrollTop =
            window.pageYOffset || document.documentElement.scrollTop;

          if (
            scrollTop > 200 &&
            filterContent &&
            filterContent.classList.contains("active")
          ) {
            filterContent.classList.remove("active");
            if (filterToggle) {
              filterToggle.classList.remove("active");
            }
          }
        }, 1000); // Collapse after 1 second of no scrolling
      });
    }
  }
});
