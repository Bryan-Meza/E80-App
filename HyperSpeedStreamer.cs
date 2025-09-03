using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class HyperSpeedStreamer : MonoBehaviour
{
    [Header("HYPER SPEED - SIN LÍMITES")]
    public string serverUrl = "http://localhost:5000/api/live";
    
    [Header("Configuración Extrema")]
    public int jpgQuality = 5;           // Calidad mínima absoluta
    public float resolutionScale = 0.1f; // Resolución ínfima (1/10)
    public bool skipFrames = true;       // Saltar frames si hay lag
    
    private Camera cam;
    private RenderTexture rt;
    private Texture2D tex;
    private bool isStreaming = true;
    private int frameCounter = 0;

    void Start()
    {
        cam = Camera.main;
        
        // Resolución mínima para velocidad extrema
        int w = Mathf.Max(32, Mathf.RoundToInt(Screen.width * resolutionScale));
        int h = Mathf.Max(24, Mathf.RoundToInt(Screen.height * resolutionScale));
        
        rt = new RenderTexture(w, h, 0); // Sin depth buffer
        tex = new Texture2D(w, h, TextureFormat.RGB24, false);
        
        Debug.Log($"Hyper Stream: {w}x{h} @ quality {jpgQuality}");
        
        StartCoroutine(HyperStream());
    }

    IEnumerator HyperStream()
    {
        while (isStreaming)
        {
            frameCounter++;
            
            // Saltar frames alternos para velocidad extrema
            if (skipFrames && frameCounter % 2 == 0)
            {
                yield return null;
                continue;
            }

            try
            {
                // Captura instantánea
                RenderTexture prev = RenderTexture.active;
                cam.targetTexture = rt;
                cam.Render();
                
                RenderTexture.active = rt;
                tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
                tex.Apply();
                
                cam.targetTexture = null;
                RenderTexture.active = prev;

                // Compresión mínima
                byte[] bytes = tex.EncodeToJPG(jpgQuality);
                
                // Envío fire-and-forget (sin esperar respuesta)
                FireAndForget(System.Convert.ToBase64String(bytes));
            }
            catch { }

            // NO ESPERAR NADA - Velocidad máxima
            yield return null;
        }
    }

    void FireAndForget(string data)
    {
        // Envío asíncrono sin coroutine para máxima velocidad
        StartCoroutine(InstantSend(data));
    }

    IEnumerator InstantSend(string data)
    {
        string json = "{\"imageBase64\":\"" + data + "\"}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        
        using (UnityWebRequest www = new UnityWebRequest(serverUrl, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.SetRequestHeader("Content-Type", "application/json");
            www.timeout = 1;
            
            yield return www.SendWebRequest();
            // Ignorar completamente el resultado para velocidad
        }
    }

    void OnDestroy()
    {
        isStreaming = false;
        if (rt) rt.Release();
        if (tex) DestroyImmediate(tex);
    }

    // Configuración en tiempo real
    [ContextMenu("Boost Speed")]
    public void BoostSpeed()
    {
        jpgQuality = 1;
        resolutionScale = 0.05f;
        skipFrames = true;
        Debug.Log("SPEED BOOSTED TO MAXIMUM!");
    }
}
