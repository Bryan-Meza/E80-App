# Unity Integration Setup

## Configuración del Servidor Flask

### Nuevas Rutas Añadidas:

1. **`/api/upload` (POST)** - Recibe capturas de pantalla desde Unity
2. **`/api/screenshots` (GET)** - Lista las capturas recientes
3. **`/unity` (GET)** - Página web para visualizar las capturas

### Funcionalidades:

- ✅ Recibe imágenes en formato Base64 desde Unity
- ✅ Guarda las capturas como archivos JPG con timestamp
- ✅ Interfaz web para visualizar capturas en tiempo real
- ✅ Auto-actualización cada 2 segundos
- ✅ Indicador de estado de conexión
- ✅ Visualización de la captura más reciente

## Configuración en Unity

### 1. Agregar el Script
- Copia el contenido de `FrameSender_Improved.cs` a tu proyecto Unity
- Crea un GameObject vacío y añade el script `FrameSender`

### 2. Configurar Parámetros
En el Inspector del GameObject:
- **Server Url**: `http://localhost:5000/api/upload`
- **Capture Interval**: `0.2` (para 5 FPS)
- **JPG Quality**: `75` (calidad de imagen)
- **Enable Logging**: `true` (para debugging)

### 3. Probar la Conexión
1. Inicia el servidor Flask:
   ```bash
   python app.py
   ```

2. Ve a: http://localhost:5000/unity

3. Ejecuta tu juego en Unity

4. Deberías ver las capturas aparecer en tiempo real

## URLs Disponibles

- **Dashboard Principal**: http://localhost:5000/
- **Gráficos**: http://localhost:5000/graphics
- **Unity Screenshots**: http://localhost:5000/unity
- **API Uploads**: http://localhost:5000/api/upload
- **API Screenshots**: http://localhost:5000/api/screenshots

## Estructura de Directorios

```
E80-App/
├── static/
│   ├── screenshots/          # ← Capturas guardadas aquí
│   ├── css/
│   ├── img/
│   └── js/
├── templates/
│   ├── unity.html           # ← Nueva página para Unity
│   ├── base.html            # ← Menú actualizado
│   ├── index.html
│   └── graphics.html
└── app.py                   # ← Rutas nuevas añadidas
```

## Solución de Problemas

### Unity no se conecta:
1. Verifica que el servidor Flask esté corriendo
2. Confirma que la URL sea correcta: `http://localhost:5000/api/upload`
3. Revisa la consola de Unity para errores
4. Asegúrate de que no haya firewall bloqueando el puerto 5000

### Las imágenes no aparecen:
1. Verifica que el directorio `static/screenshots/` existe
2. Revisa los logs del servidor Flask
3. Confirma que Unity esté enviando datos válidos

### Rendimiento:
- Ajusta `captureInterval` para cambiar FPS
- Reduce `jpgQuality` para imágenes más pequeñas
- Las capturas se guardan automáticamente con timestamp

## Personalización

### Cambiar Formato de Imagen:
Modifica en Unity:
```csharp
byte[] imageBytes = tex.EncodeToPNG(); // Para PNG
// o
byte[] imageBytes = tex.EncodeToJPG(quality); // Para JPG
```

### Cambiar Resolución:
```csharp
// Antes de capturar
Screen.SetResolution(1920, 1080, false);
```

### Filtros de Imagen:
Puedes procesar la imagen antes de enviarla:
```csharp
// Redimensionar, aplicar filtros, etc.
```
