# 🔧 SOLUCIÓN DE PROBLEMAS - Unity Stream

## ❌ **Problemas Identificados en tu Código Original:**

### 1. **URL Incorrecta**
```csharp
// ❌ MALO - Puede ser más lento
public string serverUrl = "http://localhost:5000/api/upload";

// ✅ BUENO - Máxima velocidad
public string serverUrl = "http://localhost:5000/api/live";
```

### 2. **Método HTTP Conflictivo**
```csharp
// ❌ MALO - Conflicto entre PUT y POST
using (UnityWebRequest request = UnityWebRequest.Put(serverUrl, bodyRaw))
{
    request.method = "POST"; // Esto causa problemas
}

// ✅ BUENO - Método POST directo
using (UnityWebRequest request = new UnityWebRequest(serverUrl, "POST"))
{
    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
    request.downloadHandler = new DownloadHandlerBuffer();
}
```

### 3. **Falta Using Statement**
```csharp
// ❌ MALO - Missing import
using UnityEngine;
using System.Collections;

// ✅ BUENO - Import completo
using UnityEngine;
using System.Collections;
using UnityEngine.Networking; // ← IMPORTANTE
```

### 4. **Calidad JPG No Especificada**
```csharp
// ❌ MALO - Calidad máxima (lento)
byte[] imageBytes = tex.EncodeToJPG();

// ✅ BUENO - Calidad optimizada
byte[] imageBytes = tex.EncodeToJPG(50); // 50% calidad
```

## 🛠️ **Scripts Corregidos Disponibles:**

### 1. **FrameSenderFixed.cs** - Tu código corregido
- ✅ Todos los errores solucionados
- ✅ Logs mejorados para debugging
- ✅ Control de errores robusto
- ✅ Configuración en Inspector

### 2. **HyperSpeedStreamer.cs** - Máxima velocidad
- ✅ Stream en memoria (sin archivos)
- ✅ Resolución optimizada
- ✅ Sin esperas innecesarias

### 3. **FrameSenderSimple.cs** - Versión simple optimizada

## 🚀 **Pasos para Solucionar:**

### Paso 1: Verificar Servidor
```bash
# El servidor debe estar corriendo:
python app.py

# Verifica que veas:
# * Running on http://127.0.0.1:5000
```

### Paso 2: Usar Script Corregido
1. Borra tu script `FrameSender.cs` actual
2. Usa `FrameSenderFixed.cs` 
3. Configura en Inspector:
   - Server URL: `http://localhost:5000/api/live`
   - Capture Interval: `0.033` (30 FPS)
   - JPG Quality: `50`

### Paso 3: Verificar Conexión
Ve a: http://localhost:5000/unity
- Deberías ver "LIVE STREAM ACTIVE"
- El indicador debe estar verde

## 🔍 **Debugging:**

### En Unity Console, busca:
```
✅ "FrameSender iniciado - conectando a: http://localhost:5000/api/live"
✅ "Frame enviado. Tamaño: XXXX bytes"

❌ "Error enviando frame: XXXX"
❌ "Código de respuesta: 405" (Método no permitido)
```

### En Flask Terminal, busca:
```
✅ "POST /api/live HTTP/1.1" 200 -

❌ "POST /api/upload HTTP/1.1" 405 - (URL incorrecta)
❌ "PUT /api/upload HTTP/1.1" 405 - (Método incorrecto)
```

## 🎯 **Configuración Recomendada:**

```csharp
[Header("Server Configuration")]
public string serverUrl = "http://localhost:5000/api/live";

[Header("Performance Settings")]
public float captureInterval = 0.033f; // 30 FPS
public int jpgQuality = 50;            // Calidad balanceada
public bool enableDebugLogs = true;    // Para debugging
```

## 🆘 **Si Aún No Funciona:**

1. **Verifica que Flask esté corriendo** en puerto 5000
2. **Usa `FrameSenderFixed.cs`** en lugar de tu código original
3. **Revisa Unity Console** para errores específicos
4. **Verifica la URL** en el Inspector de Unity
5. **Prueba con calidad más baja** (jpgQuality = 20)

¡Con estos cambios debería funcionar perfectamente! 🚀
