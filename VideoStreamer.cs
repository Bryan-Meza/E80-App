using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class VideoStreamer : MonoBehaviour
{
    [Header("Streaming Configuration")]
    public string serverUrl = "http://localhost:5000/api/upload";
    
    [Header("Video Quality Settings")]
    [Range(15, 120)]
    public int targetFPS = 60; // FPS ultra alto para máxima fluidez
    
    [Range(10, 100)]
    public int jpgQuality = 25; // Calidad muy baja para velocidad extrema
    
    [Range(0.25f, 2.0f)]
    public float resolutionScale = 0.5f; // Resolución baja para máxima velocidad
    
    [Header("Performance Settings")]
    public bool enableLogging = false; // Deshabilitado por defecto para mejor rendimiento
    public bool skipFramesOnLag = true; // Saltar frames si hay lag
    
    private float captureInterval;
    private Camera targetCamera;
    private RenderTexture renderTexture;
    private Texture2D texture2D;
    private int frameSkipCounter = 0;
    private bool isStreaming = false;

    void Start()
    {
        // Calcula el intervalo basado en FPS objetivo
        captureInterval = 1.0f / targetFPS;
        
        // Obtiene la cámara principal si no se especifica
        if (targetCamera == null)
            targetCamera = Camera.main;
            
        // Calcula la resolución optimizada
        int width = Mathf.RoundToInt(Screen.width * resolutionScale);
        int height = Mathf.RoundToInt(Screen.height * resolutionScale);
        
        // Crea RenderTexture para mejor rendimiento
        renderTexture = new RenderTexture(width, height, 24);
        texture2D = new Texture2D(width, height, TextureFormat.RGB24, false);
        
        if (enableLogging) Debug.Log($"Video Streamer initialized - {width}x{height} @ {targetFPS}fps");
        
        StartStreaming();
    }

    public void StartStreaming()
    {
        if (!isStreaming)
        {
            isStreaming = true;
            StartCoroutine(StreamVideo());
            if (enableLogging) Debug.Log("Video streaming started");
        }
    }

    public void StopStreaming()
    {
        if (isStreaming)
        {
            isStreaming = false;
            StopAllCoroutines();
            if (enableLogging) Debug.Log("Video streaming stopped");
        }
    }

    IEnumerator StreamVideo()
    {
        while (isStreaming)
        {
            float startTime = Time.realtimeSinceStartup;
            
            try
            {
                // Captura usando RenderTexture para mejor rendimiento
                RenderTexture currentRT = RenderTexture.active;
                targetCamera.targetTexture = renderTexture;
                targetCamera.Render();
                
                RenderTexture.active = renderTexture;
                texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                texture2D.Apply();
                
                // Restaura el RenderTexture
                targetCamera.targetTexture = null;
                RenderTexture.active = currentRT;

                // Convierte a JPG
                byte[] imageBytes = texture2D.EncodeToJPG(jpgQuality);

                // Convierte a Base64
                string base64String = System.Convert.ToBase64String(imageBytes);

                // Envía al servidor (sin bloquear)
                StartCoroutine(SendFrameAsync(base64String));

                if (enableLogging && frameSkipCounter % 30 == 0) // Log cada 30 frames
                {
                    Debug.Log($"Frame {frameSkipCounter} sent. Size: {imageBytes.Length} bytes");
                }
            }
            catch (System.Exception e)
            {
                if (enableLogging) Debug.LogError($"Error capturing frame: {e.Message}");
            }

            frameSkipCounter++;
            
            // Control de FPS con detección de lag
            float processTime = Time.realtimeSinceStartup - startTime;
            float waitTime = captureInterval - processTime;
            
            if (waitTime > 0)
            {
                yield return new WaitForSeconds(waitTime);
            }
            else if (skipFramesOnLag && waitTime < -captureInterval)
            {
                // Si el lag es muy grande, salta el siguiente frame
                yield return null;
                if (enableLogging) Debug.LogWarning("Frame skipped due to performance lag");
            }
            else
            {
                yield return null; // Al menos espera un frame
            }
        }
    }

    IEnumerator SendFrameAsync(string base64Data)
    {
        string jsonPayload = JsonUtility.ToJson(new VideoFrame(base64Data));
        
        using (UnityWebRequest www = new UnityWebRequest(serverUrl, "POST"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonPayload);
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            
            // Timeout corto para evitar bloqueos
            www.timeout = 2;

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success && enableLogging)
            {
                Debug.LogError($"Error sending frame: {www.error}");
            }
        }
    }

    void OnDestroy()
    {
        StopStreaming();
        
        if (renderTexture != null)
            renderTexture.Release();
            
        if (texture2D != null)
            DestroyImmediate(texture2D);
    }

    // Métodos públicos para control en runtime
    public void SetTargetFPS(int fps)
    {
        targetFPS = Mathf.Clamp(fps, 15, 60);
        captureInterval = 1.0f / targetFPS;
        if (enableLogging) Debug.Log($"Target FPS changed to: {targetFPS}");
    }

    public void SetQuality(int quality)
    {
        jpgQuality = Mathf.Clamp(quality, 20, 100);
        if (enableLogging) Debug.Log($"JPG Quality changed to: {jpgQuality}");
    }

    public void SetResolutionScale(float scale)
    {
        resolutionScale = Mathf.Clamp(scale, 0.5f, 2.0f);
        if (enableLogging) Debug.Log($"Resolution scale changed to: {resolutionScale}");
        
        // Recrear textures con nueva resolución
        if (renderTexture != null) renderTexture.Release();
        if (texture2D != null) DestroyImmediate(texture2D);
        
        int width = Mathf.RoundToInt(Screen.width * resolutionScale);
        int height = Mathf.RoundToInt(Screen.height * resolutionScale);
        
        renderTexture = new RenderTexture(width, height, 24);
        texture2D = new Texture2D(width, height, TextureFormat.RGB24, false);
    }

    [System.Serializable]
    public class VideoFrame
    {
        public string imageBase64;
        
        public VideoFrame(string img) 
        { 
            imageBase64 = img; 
        }
    }
}
