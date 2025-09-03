using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class UltraFastStreamer : MonoBehaviour
{
    [Header("ULTRA FAST STREAMING")]
    public string serverUrl = "http://localhost:5000/api/upload";
    
    [Header("Speed Settings")]
    public float captureInterval = 0.0f;      // SIN ESPERA - Máxima velocidad posible
    public int jpgQuality = 10;               // Calidad mínima absoluta
    public float resolutionScale = 0.125f;    // Resolución ultra baja (1/8)
    
    private Camera cam;
    private RenderTexture rt;
    private Texture2D tex;
    private bool isStreaming = true;

    void Start()
    {
        // Setup ultra rápido
        cam = Camera.main;
        
        // Resolución ultra baja para máxima velocidad
        int w = Mathf.RoundToInt(Screen.width * resolutionScale);
        int h = Mathf.RoundToInt(Screen.height * resolutionScale);
        
        rt = new RenderTexture(w, h, 16); // Depth buffer mínimo
        tex = new Texture2D(w, h, TextureFormat.RGB24, false);
        
        StartCoroutine(StreamUltraFast());
    }

    IEnumerator StreamUltraFast()
    {
        while (isStreaming)
        {
            try
            {
                // Captura ultra rápida
                RenderTexture prev = RenderTexture.active;
                cam.targetTexture = rt;
                cam.Render();
                
                RenderTexture.active = rt;
                tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
                tex.Apply();
                
                cam.targetTexture = null;
                RenderTexture.active = prev;

                // Compresión mínima para máxima velocidad
                byte[] bytes = tex.EncodeToJPG(jpgQuality);
                string b64 = System.Convert.ToBase64String(bytes);

                // Envío asíncrono sin esperar respuesta
                StartCoroutine(SendFast(b64));
            }
            catch { }

            // Espera CERO - Velocidad máxima absoluta
            yield return null; // Solo 1 frame mínimo
            
            // NO esperar nada más para velocidad extrema
            // if (captureInterval > 0) - ELIMINADO para máxima velocidad
        }
    }

    IEnumerator SendFast(string data)
    {
        string json = "{\"imageBase64\":\"" + data + "\"}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        
        using (UnityWebRequest www = new UnityWebRequest(serverUrl, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.timeout = 1; // Timeout ultra corto
            
            yield return www.SendWebRequest();
            // No verificamos errores para máxima velocidad
        }
    }

    void OnDestroy()
    {
        isStreaming = false;
        if (rt) rt.Release();
        if (tex) DestroyImmediate(tex);
    }

    // Control en runtime
    public void SetSpeed(float interval)
    {
        captureInterval = interval;
    }

    public void SetQuality(int quality)
    {
        jpgQuality = Mathf.Clamp(quality, 10, 100);
    }
}
