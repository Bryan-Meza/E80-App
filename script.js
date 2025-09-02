// Cargar un parcial HTML por id
function loadComponent(id, file) {
  return fetch(file)
    .then(res => res.text())
    .then(html => { document.getElementById(id).innerHTML = html; });
}

// Resalta el enlace activo según la URL
function highlightActiveSidebar() {
  const path = (window.location.pathname.split("/").pop() || "index.html").toLowerCase();
  document.querySelectorAll(".sidebar .item").forEach(link => {
    const href = (link.getAttribute("href") || "").toLowerCase();
    if (href === path) {
      link.classList.add("active");
      link.setAttribute("aria-current", "page");
    } else {
      link.classList.remove("active");
      link.removeAttribute("aria-current");
    }
  });
}

// Demo de charts
function initCharts() {
  if (typeof Chart === "undefined") return;
  const ids = ["chart1","chart2","chart3","chart4"];
  ids.forEach(id => {
    const el = document.getElementById(id);
    if (!el) return;
    new Chart(el, {
      type: "line",
      data: {
        labels: ["Jan","Feb","Mar","Apr","May","Jun"],
        datasets: [{ label: "Series", data: [3,5,4,7,6,8] }]
      },
      options: { responsive: true, maintainAspectRatio: false }
    });
  });
}

// Carga de componentes y luego inicialización
Promise.all([
  loadComponent("header", "header.html"),
  loadComponent("sidebar", "sidebar.html"),
  loadComponent("footer", "footer.html")
]).then(() => {
  highlightActiveSidebar();
  initCharts();
});
