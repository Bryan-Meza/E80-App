using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class FrameSenderSimple : MonoBehaviour
{
    // URL de tu servidor Flask
    public string serverUrl = "http://localhost:5000/api/live"; // Cambio a live stream
    
    // Intervalo entre capturas (en segundos) - HÍPER RÁPIDO
    public float captureInterval = 0.0f; // SIN ESPERA - Máxima velocidad
    
    // Para debugging
    public bool enableLogging = false; // Deshabilitado para máxima velocidad

    void Start()
    {
        StartCoroutine(CaptureAndSendFrames());
    }

    IEnumerator CaptureAndSendFrames()
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

                // Convierte a JPG con calidad híper baja para velocidad extrema
                byte[] imageBytes = tex.EncodeToJPG(10); // Calidad mínima absoluta

                // Convierte a Base64
                string base64String = System.Convert.ToBase64String(imageBytes);

                // Crea el JSON payload
                string jsonPayload = JsonUtility.ToJson(new ImagePayload(base64String));

                // Envía al servidor
                yield return StartCoroutine(SendImageToServer(jsonPayload));

                // Libera la memoria
                DestroyImmediate(tex);

                if (enableLogging) Debug.Log($"Screenshot processed. Size: {imageBytes.Length} bytes");
            }
            catch (System.Exception e)
            {
                if (enableLogging) Debug.LogError($"Error capturing frame: {e.Message}");
            }

            // Espera HÍPER mínima para velocidad extrema
            if (captureInterval > 0)
                yield return new WaitForSeconds(captureInterval);
            else
                yield return null; // Solo un frame si es 0
        }
    }

    IEnumerator SendImageToServer(string jsonData)
    {
        // Método más directo usando POST
        using (UnityWebRequest www = new UnityWebRequest(serverUrl, "POST"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                if (enableLogging) Debug.Log("Image sent successfully!");
                if (enableLogging) Debug.Log($"Server response: {www.downloadHandler.text}");
            }
            else
            {
                if (enableLogging) Debug.LogError($"Error sending image: {www.error}");
                if (enableLogging) Debug.LogError($"Response code: {www.responseCode}");
                if (enableLogging) Debug.LogError($"Response: {www.downloadHandler.text}");
            }
        }
    }

    [System.Serializable]
    public class ImagePayload
    {
        public string imageBase64;
        
        public ImagePayload(string img) 
        { 
            imageBase64 = img; 
        }
    }

    // Método para detener el envío (útil para testing)
    public void StopSending()
    {
        StopAllCoroutines();
        if (enableLogging) Debug.Log("Frame sending stopped");
    }

    // Método para reanudar el envío
    public void StartSending()
    {
        StopAllCoroutines();
        StartCoroutine(CaptureAndSendFrames());
        if (enableLogging) Debug.Log("Frame sending started");
    }
}
