// Cargar componentes HTML (header, sidebar, footer)
function loadComponent(id, file) {
    return fetch(file)
      .then(res => res.text())
      .then(html => { document.getElementById(id).innerHTML = html; });
  }
  
  // Inicializa eventos del sidebar una vez que header y sidebar existen en el DOM
  function initSidebarControls() {
    const burger = document.getElementById('burgerBtn');
    const sidebar = document.querySelector('.sidebar');
    const closeBtn = document.getElementById('closeSidebar');
  
    if (!burger || !sidebar || !closeBtn) return;
  
    const openSidebar = () => {
      burger.classList.add('active');
      sidebar.classList.add('open');
    };
    const closeSidebarFn = () => {
      burger.classList.remove('active');
      sidebar.classList.remove('open');
    };
  
    burger.addEventListener('click', () => {
      const isOpen = sidebar.classList.contains('open');
      isOpen ? closeSidebarFn() : openSidebar();
    });
  
    closeBtn.addEventListener('click', closeSidebarFn);
  
    // Cerrar con ESC
    document.addEventListener('keydown', (e) => {
      if (e.key === 'Escape') closeSidebarFn();
    });
  }
  
  // (Opcional) Inicializa gráficos de ejemplo
  function initCharts() {
    if (typeof Chart === 'undefined') return;
    const ids = ['chart1', 'chart2', 'chart3', 'chart4'];
    ids.forEach(id => {
      const ctx = document.getElementById(id);
      if (!ctx) return;
      new Chart(ctx, {
        type: 'line',
        data: {
          labels: ['Jan','Feb','Mar','Apr','May','Jun'],
          datasets: [{ label: 'Series', data: [3,5,4,7,6,8] }]
        },
        options: { responsive: true, maintainAspectRatio: false }
      });
    });
  }
  
  // Cargar todos los componentes y luego iniciar controles
  Promise.all([
    loadComponent('header', 'header.html'),
    loadComponent('sidebar', 'sidebar.html'),
    loadComponent('footer', 'footer.html')
  ]).then(() => {
    initSidebarControls();
    initCharts();
  });
  