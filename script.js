// Cargar componentes HTML (header, sidebar, footer)
const isFileProtocol = location.protocol === 'file:';

function loadComponent(id, file) {
  return fetch(file)
    .then(res => {
      if (!res.ok) throw new Error(`HTTP ${res.status} while fetching ${file}`);
      return res.text();
    })
    .then(html => { document.getElementById(id).innerHTML = html; })
    .catch(err => {
      console.warn('No se pudo cargar el componente:', file, err);
      const el = document.getElementById(id);
      if (!el) return;
      const hint = isFileProtocol
        ? 'Componentes no disponibles al abrir el archivo directamente. Inicia un servidor local (http://) para cargar header/sidebar/footer.'
        : 'No se pudo cargar el componente.';
      el.innerHTML = `<div style="padding:8px;color:#b00;background:#fee;border:1px solid #f99;border-radius:6px;">${hint}</div>`;
    });
}
  
  // Inicializa eventos del sidebar una vez que header y sidebar existen en el DOM
  function initSidebarControls() {
    const burger = document.getElementById('burgerBtn');
    const sidebar = document.querySelector('.sidebar');
    const closeBtn = document.getElementById('closeSidebar');
    // Ajustar rutas de links segun profundidad (independiente de los botones)
    if (sidebar) {
      const isInPages = /(\/|\\)pages(\/|\\)/.test(location.pathname);
      const prefix = isInPages ? '../' : '';
      sidebar.querySelectorAll('a[data-href]')
        .forEach(a => { a.href = prefix + a.getAttribute('data-href'); });
    }

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

  // rutas ya ajustadas arriba
  }
  
  // Inicializa gráficos. Si hay atributo data-endpoint, trae datos del backend.
  async function initCharts() {
    if (typeof Chart === 'undefined') return;

    const canvases = Array.from(document.querySelectorAll('canvas[id^="chart"]'));
    const defaults = {
      labels: ['Jan','Feb','Mar','Apr','May','Jun'],
      datasets: [{ label: 'Series', data: [3,5,4,7,6,8] }]
    };

    await Promise.all(canvases.map(async (canvas) => {
      if (!canvas) return;
      const endpoint = canvas.getAttribute('data-endpoint');
      let data = defaults;
      try {
        if (endpoint) {
          const res = await fetch(endpoint, { headers: { 'Accept': 'application/json' } });
          if (res.ok) {
            const json = await res.json();
            // Espera forma: { labels: string[], datasets: [{ label, data, ... }] }
            if (json && Array.isArray(json.labels) && Array.isArray(json.datasets)) {
              data = json;
            }
          }
        }
      } catch (e) {
        // Silenciar y usar datos por defecto
        console.warn('Falling back to default chart data for', canvas.id, e);
      }

      new Chart(canvas, {
        type: 'line',
        data,
        options: { responsive: true, maintainAspectRatio: false }
      });
    }));
  }
  
  // Cargar todos los componentes y luego iniciar controles
  const isInPages = /\/(pages)\//.test(location.pathname.replace(/\\/g, '/'));
  const prefix = isInPages ? '../' : '';
  Promise.all([
    loadComponent('header', `${prefix}header.html`),
    loadComponent('sidebar', `${prefix}sidebar.html`),
    loadComponent('footer', `${prefix}footer.html`)
  ]).then(() => {
    initSidebarControls();
    initCharts();
  });
  