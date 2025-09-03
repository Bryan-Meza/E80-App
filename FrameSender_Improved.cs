using UnityEngine;
using System.Collections;

public class FrameSender : MonoBehaviour
{
    // URL de tu servidor Flask (asegúrate de que coincida con tu servidor)
    public string serverUrl = "http://localhost:5000/api/upload";
    
    // Intervalo entre capturas (en segundos)
    public float captureInterval = 0.2f; // 5 FPS
    
    // Calidad de la imagen JPG (0-100)
    public int jpgQuality = 75;
    
    // Para debugging
    public bool enableLogging = true;

    void Start()
    {
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
                    if (enableLogging) Debug.LogError("Failed to capture screenshot");
                    yield return new WaitForSeconds(captureInterval);
                    continue;
                }

                // Convierte a JPG con calidad especificada
                byte[] imageBytes = tex.EncodeToJPG(jpgQuality);

                // Convierte a Base64
                string base64String = System.Convert.ToBase64String(imageBytes);

                // Crea el JSON payload
                Payload payload = new Payload(base64String);
                string jsonPayload = JsonUtility.ToJson(payload);

                // Envía al servidor
                StartCoroutine(SendToServer(jsonPayload));

                // Libera la memoria
                DestroyImmediate(tex);

                if (enableLogging) Debug.Log($"Screenshot sent to server. Size: {imageBytes.Length} bytes");
            }
            catch (System.Exception e)
            {
                if (enableLogging) Debug.LogError($"Error capturing frame: {e.Message}");
            }

            // Espera antes del siguiente frame
            yield return new WaitForSeconds(captureInterval);
        }
    }

    IEnumerator SendToServer(string jsonData)
    {
        if (string.IsNullOrEmpty(jsonData))
        {
            if (enableLogging) Debug.LogError("JSON data is null or empty");
            yield break;
        }

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        
        // Método correcto para POST request
        using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.PostWwwForm(serverUrl, ""))
        {
            // Reemplazamos el upload handler para enviar JSON
            request.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                if (enableLogging) Debug.Log($"Screenshot sent successfully. Response: {request.downloadHandler.text}");
            }
            else
            {
                if (enableLogging) Debug.LogError($"Failed to send screenshot: {request.error}");
                if (enableLogging) Debug.LogError($"Response Code: {request.responseCode}");
                if (enableLogging) Debug.LogError($"Response: {request.downloadHandler.text}");
            }
        }
    }

    [System.Serializable]
    public class Payload
    {
        public string imageBase64;
        
        public Payload(string img) 
        { 
            imageBase64 = img; 
        }
    }

    // Método para cambiar la URL del servidor desde el inspector o código
    public void SetServerUrl(string newUrl)
    {
        serverUrl = newUrl;
        if (enableLogging) Debug.Log($"Server URL changed to: {serverUrl}");
    }

    // Método para cambiar el intervalo de captura
    public void SetCaptureInterval(float interval)
    {
        captureInterval = interval;
        if (enableLogging) Debug.Log($"Capture interval changed to: {captureInterval}s");
    }
}
