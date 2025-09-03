using UnityEngine;
using System.Collections;
using UnityEngine.Networking; // IMPORTANTE: Agregar esta línea

public class FrameSenderFixed : MonoBehaviour
{
    [Header("Server Configuration")]
    public string serverUrl = "http://localhost:5000/api/live"; // Cambiado a /api/live para velocidad
    
    [Header("Performance Settings")]
    public float captureInterval = 0.033f; // 30 FPS
    public int jpgQuality = 50; // Calidad especificada para mejor rendimiento
    public bool enableDebugLogs = true;

    void Start()
    {
        if (enableDebugLogs) Debug.Log("FrameSender iniciado - conectando a: " + serverUrl);
        StartCoroutine(CaptureFrame());
    }

    IEnumerator CaptureFrame()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();

            try
            {
                // Captura de pantalla
                Texture2D tex = ScreenCapture.CaptureScreenshotAsTexture();

                if (tex == null)
                {
                    if (enableDebugLogs) Debug.LogError("Error: No se pudo capturar screenshot");
                    yield return new WaitForSeconds(captureInterval);
                    continue;
                }

                // Lo convierte a JPG con calidad especificada
                byte[] imageBytes = tex.EncodeToJPG(jpgQuality);

                // Lo convierte a Base64
                string base64String = System.Convert.ToBase64String(imageBytes);

                // JSON con la imagen
                string jsonPayload = JsonUtility.ToJson(new Payload(base64String));

                // Enviar por POST (corregido)
                StartCoroutine(SendToServer(jsonPayload));

                // Liberar memoria
                DestroyImmediate(tex);

                if (enableDebugLogs && Time.frameCount % 100 == 0) // Log cada 100 frames
                {
                    Debug.Log($"Frame enviado. Tamaño: {imageBytes.Length} bytes");
                }
            }
            catch (System.Exception e)
            {
                if (enableDebugLogs) Debug.LogError($"Error capturando frame: {e.Message}");
            }

            // Espera antes del siguiente frame
            yield return new WaitForSeconds(captureInterval);
        }
    }

    IEnumerator SendToServer(string jsonData)
    {
        if (string.IsNullOrEmpty(jsonData))
        {
            if (enableDebugLogs) Debug.LogError("JSON data está vacío");
            yield break;
        }

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        
        // CORREGIDO: Usar método POST correctamente
        using (UnityWebRequest request = new UnityWebRequest(serverUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = 5; // Timeout de 5 segundos

            yield return request.SendWebRequest();

            // Verificación de errores mejorada
            if (request.result == UnityWebRequest.Result.Success)
            {
                if (enableDebugLogs && Time.frameCount % 100 == 0)
                {
                    Debug.Log("Frame enviado exitosamente");
                }
            }
            else
            {
                if (enableDebugLogs)
                {
                    Debug.LogError($"Error enviando frame: {request.error}");
                    Debug.LogError($"Código de respuesta: {request.responseCode}");
                    
                    if (!string.IsNullOrEmpty(request.downloadHandler.text))
                    {
                        Debug.LogError($"Respuesta del servidor: {request.downloadHandler.text}");
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class Payload
    {
        public string imageBase64;
        public Payload(string img) { imageBase64 = img; }
    }

    // Métodos para control en tiempo real
    public void ChangeServerUrl(string newUrl)
    {
        serverUrl = newUrl;
        if (enableDebugLogs) Debug.Log($"URL del servidor cambiada a: {serverUrl}");
    }

    public void SetCaptureInterval(float interval)
    {
        captureInterval = interval;
        if (enableDebugLogs) Debug.Log($"Intervalo de captura cambiado a: {captureInterval}s");
    }

    public void SetJpgQuality(int quality)
    {
        jpgQuality = Mathf.Clamp(quality, 1, 100);
        if (enableDebugLogs) Debug.Log($"Calidad JPG cambiada a: {jpgQuality}");
    }
}
