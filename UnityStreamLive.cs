using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class UnityStreamLive : MonoBehaviour
{
    [Header("🚀 CONFIGURACIÓN DE STREAMING")]
    [Space(10)]
    public string serverUrl = "http://localhost:5000/api/live";
    
    [Header("⚡ CONFIGURACIÓN DE RENDIMIENTO")]
    [Range(0.01f, 1.0f)]
    public float captureInterval = 0.033f; // 30 FPS por defecto
    
    [Range(10, 100)]
    public int jpgQuality = 50; // Calidad balanceada
    
    [Range(0.1f, 1.0f)]
    public float resolutionScale = 0.5f; // Resolución media para velocidad
    
    [Header("🔧 CONFIGURACIÓN DEBUG")]
    public bool enableLogs = true;
    public bool showFPS = true;
    
    private Camera targetCamera;
    private RenderTexture renderTexture;
    private Texture2D texture2D;
    private bool isStreaming = false;
    private int frameCount = 0;
    private float lastFPSTime = 0f;
    private float currentFPS = 0f;

    void Start()
    {
        // Configuración inicial
        SetupCamera();
        SetupTextures();
        
        if (enableLogs)
        {
            Debug.Log($"🚀 Unity Stream Live iniciado!");
            Debug.Log($"📡 Servidor: {serverUrl}");
            Debug.Log($"🎯 FPS objetivo: {1f/captureInterval:F1}");
            Debug.Log($"🖼️ Resolución: {renderTexture.width}x{renderTexture.height}");
            Debug.Log($"💎 Calidad JPG: {jpgQuality}%");
        }
        
        StartStreaming();
    }

    void SetupCamera()
    {
        targetCamera = Camera.main;
        if (targetCamera == null)
        {
            targetCamera = FindObjectOfType<Camera>();
            if (targetCamera == null)
            {
                Debug.LogError("❌ No se encontró cámara! Creando cámara por defecto...");
                GameObject cameraObj = new GameObject("StreamCamera");
                targetCamera = cameraObj.AddComponent<Camera>();
            }
        }
    }

    void SetupTextures()
    {
        int width = Mathf.RoundToInt(Screen.width * resolutionScale);
        int height = Mathf.RoundToInt(Screen.height * resolutionScale);
        
        // Asegurar tamaños mínimos
        width = Mathf.Max(64, width);
        height = Mathf.Max(48, height);
        
        renderTexture = new RenderTexture(width, height, 16);
        texture2D = new Texture2D(width, height, TextureFormat.RGB24, false);
        
        if (enableLogs)
            Debug.Log($"📐 Texturas creadas: {width}x{height}");
    }

    public void StartStreaming()
    {
        if (!isStreaming)
        {
            isStreaming = true;
            StartCoroutine(StreamingLoop());
            if (enableLogs) Debug.Log("▶️ Streaming iniciado");
        }
    }

    public void StopStreaming()
    {
        if (isStreaming)
        {
            isStreaming = false;
            StopAllCoroutines();
            if (enableLogs) Debug.Log("⏹️ Streaming detenido");
        }
    }

    IEnumerator StreamingLoop()
    {
        while (isStreaming)
        {
            yield return new WaitForEndOfFrame();

            try
            {
                CaptureAndSendFrame();
                UpdateFPS();
            }
            catch (System.Exception e)
            {
                if (enableLogs)
                    Debug.LogError($"❌ Error en streaming: {e.Message}");
            }

            // Control de FPS
            yield return new WaitForSeconds(captureInterval);
        }
    }

    void CaptureAndSendFrame()
    {
        // Captura usando RenderTexture para mejor rendimiento
        RenderTexture currentRT = RenderTexture.active;
        
        targetCamera.targetTexture = renderTexture;
        targetCamera.Render();
        
        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();
        
        // Restaurar configuración
        targetCamera.targetTexture = null;
        RenderTexture.active = currentRT;

        // Comprimir y enviar
        byte[] imageBytes = texture2D.EncodeToJPG(jpgQuality);
        string base64String = System.Convert.ToBase64String(imageBytes);
        
        StartCoroutine(SendFrameToServer(base64String));
        
        frameCount++;
    }

    IEnumerator SendFrameToServer(string base64Data)
    {
        string jsonPayload = $"{{\"imageBase64\":\"{base64Data}\"}}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        
        using (UnityWebRequest request = new UnityWebRequest(serverUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = 3; // Timeout corto para evitar bloqueos
            
            yield return request.SendWebRequest();
            
            if (request.result != UnityWebRequest.Result.Success && enableLogs)
            {
                Debug.LogError($"❌ Error enviando frame: {request.error}");
                Debug.LogError($"📡 Código de respuesta: {request.responseCode}");
            }
        }
    }

    void UpdateFPS()
    {
        if (showFPS && Time.time - lastFPSTime >= 1f)
        {
            currentFPS = frameCount / (Time.time - lastFPSTime);
            frameCount = 0;
            lastFPSTime = Time.time;
            
            if (enableLogs)
                Debug.Log($"📊 FPS actual: {currentFPS:F1} | Objetivo: {1f/captureInterval:F1}");
        }
    }

    void OnDestroy()
    {
        StopStreaming();
        
        if (renderTexture != null)
        {
            renderTexture.Release();
            DestroyImmediate(renderTexture);
        }
        
        if (texture2D != null)
        {
            DestroyImmediate(texture2D);
        }
    }

    // Métodos públicos para control en tiempo real
    [ContextMenu("🚀 Configuración Ultra Rápida")]
    public void SetUltraFast()
    {
        captureInterval = 0.016f; // 60 FPS
        jpgQuality = 30;
        resolutionScale = 0.25f;
        ApplySettings();
        if (enableLogs) Debug.Log("🚀 Configuración ultra rápida aplicada!");
    }

    [ContextMenu("⚡ Configuración Rápida")]
    public void SetFast()
    {
        captureInterval = 0.033f; // 30 FPS
        jpgQuality = 50;
        resolutionScale = 0.5f;
        ApplySettings();
        if (enableLogs) Debug.Log("⚡ Configuración rápida aplicada!");
    }

    [ContextMenu("🎯 Configuración Balanceada")]
    public void SetBalanced()
    {
        captureInterval = 0.05f; // 20 FPS
        jpgQuality = 70;
        resolutionScale = 0.75f;
        ApplySettings();
        if (enableLogs) Debug.Log("🎯 Configuración balanceada aplicada!");
    }

    [ContextMenu("💎 Configuración Alta Calidad")]
    public void SetHighQuality()
    {
        captureInterval = 0.1f; // 10 FPS
        jpgQuality = 90;
        resolutionScale = 1.0f;
        ApplySettings();
        if (enableLogs) Debug.Log("💎 Configuración alta calidad aplicada!");
    }

    void ApplySettings()
    {
        // Recrear texturas si cambió la resolución
        if (renderTexture != null)
        {
            renderTexture.Release();
            DestroyImmediate(renderTexture);
        }
        if (texture2D != null)
        {
            DestroyImmediate(texture2D);
        }
        
        SetupTextures();
    }

    // Mostrar información en el Inspector
    void OnValidate()
    {
        if (Application.isPlaying)
        {
            ApplySettings();
        }
    }

    // GUI en pantalla para debugging
    void OnGUI()
    {
        if (showFPS && Application.isPlaying)
        {
            GUI.color = Color.green;
            GUI.Label(new Rect(10, 10, 200, 20), $"Streaming FPS: {currentFPS:F1}");
            GUI.Label(new Rect(10, 30, 200, 20), $"Estado: {(isStreaming ? "🟢 ACTIVO" : "🔴 INACTIVO")}");
            GUI.color = Color.white;
        }
    }
}
