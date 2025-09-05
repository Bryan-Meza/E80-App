# 📊 BATTERY TELEMETRY SYSTEM - SETUP GUIDE

## 🎯 **¿Qué se implementó?**

Se agregó un sistema completo de telemetría de batería en tiempo real que:

✅ **Consulta datos del servidor Python** (puerto 8080)  
✅ **Muestra gráfica de líneas** con 5 agentes (colores diferentes)  
✅ **Actualiza cada segundo** con últimos 30 puntos  
✅ **Tiempo relativo** (-30s a 0s) deslizándose hacia la derecha  
✅ **Colores según nivel de batería** (rojo <200, amarillo 200-600, verde >600)  
✅ **Almacena datos** en carpeta `telemetryData` por sesión  
✅ **Gráfica responsive** debajo del stream de Unity  

## 📁 **Archivos modificados:**

### 1. **`app.py`** - Backend Flask
```python
# Nuevos imports agregados
import requests
import json
import time

# Nuevas variables globales
PYTHON_SERVER_URL = "http://localhost:8080"
TELEMETRY_DATA_DIR = "telemetryData"
session_start_time = time.time()

# Nuevo endpoint /api/telemetry
@app.route('/api/telemetry')
def api_telemetry():
    # Consulta servidor Python y devuelve datos de batería
```

### 2. **`templates/index.html`** - Frontend
```html
<!-- Nueva sección de telemetría -->
<section class="telemetry-section">
  <div class="telemetry-header">
    <h3>Battery Telemetry</h3>
    <div id="telemetryStatus">...</div>
  </div>
  <div class="chart-container">
    <canvas id="batteryChart"></canvas>
  </div>
</section>
```

### 3. **`requirements.txt`** - Dependencias
```
Flask==2.3.3
Werkzeug==2.3.7
requests==2.31.0  # ← NUEVA DEPENDENCIA
```

## 🚀 **Cómo usar:**

### 1. **Instalar dependencias:**
```bash
pip install requests
```

### 2. **Iniciar servidor Python (puerto 8080):**
```bash
# Tu script de Python debe estar corriendo en http://localhost:8080
# con endpoints: /get_data que devuelva datos de agentes
```

### 3. **Iniciar Flask (puerto 5000):**
```bash
cd E80-App
python app.py
```

### 4. **Abrir navegador:**
```
http://localhost:5000
```

## 📊 **Estructura de datos:**

### Entrada esperada desde Python (puerto 8080):
```json
{
  "agents": [
    {
      "id": 1,
      "battery": 850,
      "position": {"x": -10, "z": 8},
      "has_object": false
    },
    {
      "id": 2,
      "battery": 650,
      "position": {"x": 0, "z": 0},
      "has_object": true
    }
    // ... hasta 5 agentes
  ]
}
```

### Salida del endpoint `/api/telemetry`:
```json
{
  "timestamp": 1694123456.789,
  "relative_time": 45,
  "agents": [
    {
      "id": 1,
      "battery": 850,
      "position": {"x": -10, "z": 8},
      "has_object": false,
      "has_task": true
    }
  ],
  "success": true
}
```

## 🎨 **Configuración visual:**

### Colores de las líneas por agente:
- **Agent 1**: Rojo (`#ff6b6b`)
- **Agent 2**: Turquesa (`#4ecdc4`)
- **Agent 3**: Azul (`#45b7d1`)
- **Agent 4**: Amarillo (`#f9ca24`)
- **Agent 5**: Morado (`#6c5ce7`)

### Colores de fondo según batería:
- **Verde**: Batería > 600 (HIGH)
- **Amarillo**: Batería 200-600 (MEDIUM)
- **Rojo**: Batería < 200 (LOW/CRITICAL)

## 📂 **Almacenamiento de datos:**

### Ubicación:
```
E80-App/
├── telemetryData/
│   ├── session_1694123456.json
│   ├── session_1694123789.json
│   └── ...
```

### Formato del archivo de sesión:
```json
{
  "session_start": 1694123456.789,
  "session_start_datetime": "2025-09-03T16:44:16.789",
  "telemetry_points": [
    {
      "timestamp": 1694123456.789,
      "relative_time": 0,
      "agents": [...]
    }
  ]
}
```

## 🔧 **Configuración personalizable:**

### En `app.py`:
```python
PYTHON_SERVER_URL = "http://localhost:8080"  # URL del servidor Python
TELEMETRY_DATA_DIR = "telemetryData"         # Carpeta de datos
```

### En `index.html` (JavaScript):
```javascript
setInterval(loadTelemetryData, 1000);  // Frecuencia: 1 segundo
const maxDataPoints = 30;             // Puntos máximos: 30
```

## 🔍 **Troubleshooting:**

### Problema: "Failed to connect to Python server"
**Solución**: Verificar que tu servidor Python esté corriendo en puerto 8080

### Problema: "No agent data available"
**Solución**: Verificar que tu servidor Python devuelva datos con estructura correcta

### Problema: Gráfica no se actualiza
**Solución**: Abrir DevTools (F12) y revisar errores en consola

### Problema: Datos no se guardan
**Solución**: Verificar permisos de escritura en carpeta `telemetryData`

## 📈 **Funcionalidades avanzadas:**

### 1. **Tiempo relativo automático:**
- La gráfica siempre muestra los últimos 30 segundos
- El eje X se actualiza automáticamente (-30s a 0s)
- Los datos antiguos se eliminan automáticamente

### 2. **Indicadores de estado:**
- **Verde**: Conexión activa con Python
- **Rojo**: Sin conexión o error
- Mensajes descriptivos de error

### 3. **Persistencia de datos:**
- Cada sesión genera un archivo único
- Máximo 1000 puntos por archivo (para evitar archivos muy grandes)
- Datos con timestamp para análisis posterior

## 🎯 **Próximas funcionalidades sugeridas:**

1. **Exportar datos** a CSV/Excel
2. **Alertas automáticas** cuando batería < 100
3. **Gráfica de posiciones** en mapa 2D
4. **Métricas adicionales** (tareas completadas, eficiencia)
5. **Zoom y pan** en la gráfica
6. **Filtros por agente** (mostrar/ocultar líneas)

---

## ✅ **Estado de implementación:**
- [x] Endpoint `/api/telemetry` funcionando
- [x] Gráfica Chart.js implementada
- [x] 5 líneas con colores diferentes
- [x] Actualización cada segundo
- [x] Últimos 30 puntos deslizándose
- [x] Tiempo relativo
- [x] Almacenamiento en `telemetryData`
- [x] Colores según nivel de batería
- [x] Responsive design

**🎉 Sistema completamente funcional y listo para usar!**
