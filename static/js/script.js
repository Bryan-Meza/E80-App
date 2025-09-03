// Flask-based chart initialization
function initCharts() {
  if (typeof Chart === 'undefined') return;

  const canvases = Array.from(document.querySelectorAll('canvas[id^="chart"]'));
  const defaults = {
    labels: ['Jan','Feb','Mar','Apr','May','Jun'],
    datasets: [{ 
      label: 'Series', 
      data: [3,5,4,7,6,8],
      borderColor: '#ffd700',
      backgroundColor: 'rgba(255, 215, 0, 0.1)'
    }]
  };

  canvases.forEach(async (canvas) => {
    if (!canvas) return;
    
    const endpoint = canvas.getAttribute('data-endpoint');
    let data = defaults;
    
    try {
      if (endpoint) {
        const res = await fetch(endpoint, { 
          headers: { 'Accept': 'application/json' } 
        });
        if (res.ok) {
          const json = await res.json();
          if (json && Array.isArray(json.labels) && Array.isArray(json.datasets)) {
            data = json;
          }
        }
      }
    } catch (e) {
      console.warn('Using default chart data for', canvas.id, e);
    }

    new Chart(canvas, {
      type: 'line',
      data,
      options: { 
        responsive: true, 
        maintainAspectRatio: false,
        plugins: {
          legend: {
            labels: {
              color: '#333'
            }
          }
        },
        scales: {
          y: {
            ticks: {
              color: '#666'
            }
          },
          x: {
            ticks: {
              color: '#666'
            }
          }
        }
      }
    });
  });
}

// Highlight active sidebar link (redundant with Flask template but good for dynamic updates)
function highlightActiveSidebar() {
  const current = window.location.pathname;
  document.querySelectorAll('.sidebar .item').forEach(link => {
    const href = link.getAttribute('href');
    if (href === current || (current === '/' && href === '/')) {
      link.classList.add('active');
      link.setAttribute('aria-current', 'page');
    } else {
      link.classList.remove('active');
      link.removeAttribute('aria-current');
    }
  });
}

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
  initCharts();
  highlightActiveSidebar();
  
  // Add smooth transitions for charts
  const charts = document.querySelectorAll('.chart-section');
  charts.forEach((chart, index) => {
    chart.style.opacity = '0';
    chart.style.transform = 'translateY(20px)';
    setTimeout(() => {
      chart.style.transition = 'opacity 0.5s ease, transform 0.5s ease';
      chart.style.opacity = '1';
      chart.style.transform = 'translateY(0)';
    }, index * 100);
  });
  
  console.log('E80 Dashboard initialized with Flask backend');
});
