# Configuraciones Optimizadas para Video Streaming

## 🎮 Presets de Calidad para Unity

### 🚀 **Ultra Performance** (Máxima fluidez)
```csharp
public float captureInterval = 0.033f; // 30 FPS
public int jpgQuality = 40;            // Calidad baja
public float resolutionScale = 0.5f;   // Resolución reducida
```

### ⚡ **High Performance** (Buena fluidez)
```csharp
public float captureInterval = 0.033f; // 30 FPS  
public int jpgQuality = 60;            // Calidad media
public float resolutionScale = 0.75f;  // Resolución media
```

### 🎯 **Balanced** (Equilibrado)
```csharp
public float captureInterval = 0.05f;  // 20 FPS
public int jpgQuality = 75;            // Calidad buena
public float resolutionScale = 1.0f;   // Resolución completa
```

### 🏆 **High Quality** (Mejor calidad)
```csharp
public float captureInterval = 0.067f; // 15 FPS
public int jpgQuality = 90;            // Calidad alta
public float resolutionScale = 1.0f;   // Resolución completa
```

## 📊 Comparación de Rendimiento

| Preset | FPS | Calidad | Resolución | Uso CPU | Uso Red |
|--------|-----|---------|------------|---------|---------|
| Ultra Performance | 30 | 40% | 50% | Bajo | Bajo |
| High Performance | 30 | 60% | 75% | Medio | Medio |
| Balanced | 20 | 75% | 100% | Medio | Medio |
| High Quality | 15 | 90% | 100% | Alto | Alto |

## 🔧 Recomendaciones de Uso

### Para Juegos de Acción Rápida:
- Usa **Ultra Performance** o **High Performance**
- Prioriza FPS sobre calidad
- La fluidez es más importante que la nitidez

### Para Demos o Presentaciones:
- Usa **Balanced** o **High Quality**
- La calidad visual es importante
- FPS moderados son aceptables

### Para Testing/Debug:
- Usa **Ultra Performance**
- Máxima velocidad de transmisión
- Facilita la detección de problemas

## 🌐 Configuración del Servidor Web

### Actualización Frecuente (Video-like):
```javascript
// Actualización cada 500ms para efecto video
setInterval(loadScreenshots, 500);
```

### Actualización Normal:
```javascript  
// Actualización cada 2 segundos
setInterval(loadScreenshots, 2000);
```

## 💡 Tips de Optimización

1. **Reduce la resolución** antes que la calidad para mejor rendimiento
2. **Usa RenderTexture** en lugar de ScreenCapture para mejor rendimiento
3. **Limita los logs** en producción para evitar overhead
4. **Configura timeout corto** en requests HTTP (2 segundos máximo)
5. **Implementa limpieza automática** de archivos antiguos

## 🎯 Script Recomendado

Para la mejor experiencia de video streaming, usa `VideoStreamer.cs` que incluye:

- ✅ Control automático de FPS
- ✅ Optimización de memoria
- ✅ Detección de lag
- ✅ Configuración en tiempo real
- ✅ Limpieza automática
- ✅ RenderTexture optimizado
