using Unity.Barracuda;
using UnityEngine;
using UnityEngine.Diagnostics;

public class MalariaDetector : MonoBehaviour
{
    [Header("Model")]
    public NNModel modelAsset; // Assign in Inspector

    private Model runtimeModel;
    private IWorker engine;

    [Header("Input")]
    public Texture2D inputTexture; // Optional: for testing

    [Header("Output")]
    public bool isInfected;
    public float infectionProbability;

    void Start()
    {
        if (modelAsset == null)
        {
            Debug.LogError("⚠️ Please assign the .nn model in the Inspector!");
            return;
        }

        // Load model into memory
        runtimeModel = ModelLoader.Load(modelAsset);
        engine = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharp, runtimeModel);

        Debug.Log("✅ Malaria detector initialized.");
    }

    /// <summary>
    /// Analyze a blood cell image for malaria infection.
    /// </summary>
    /// <param name="texture">RGB texture, preferably 128x128</param>
    public void AnalyzeCellImage(Texture2D texture)
    {
        if (texture == null || engine == null)
        {
            Debug.LogWarning("Texture or engine is null.");
            return;
        }

        // Ensure correct size (your model expects 128x128)
        Texture2D resized = texture;
        if (texture.width != 128 || texture.height != 128)
        {
            resized = ResizeTexture(texture, 128, 128);
        }

        // Convert to tensor: [1, 128, 128, 3], values in [0,1]
        var input = new Tensor(1, 128, 128, 3);
        Color[] pixels = resized.GetPixels();
        for (int y = 0; y < 128; y++)
        {
            for (int x = 0; x < 128; x++)
            {
                Color pixel = pixels[y * 128 + x];
                input[0, y, x, 0] = pixel.r;
                input[0, y, x, 1] = pixel.g;
                input[0, y, x, 2] = pixel.b;
            }
        }
        // Normalize to [0,1]
        for (int i = 0; i < input.length; i++)
        {
            input[i] /= 255f;
        }

        // Run inference
        engine.Execute(input);
        var output = engine.PeekOutput(); // shape: (1, 1)

        float raw = output[0];
        infectionProbability = raw;

        // 🔑 CRITICAL: Your model's label mapping:
        //   Output < 0.5 → Parasitized (infected) → Label 0
        //   Output >= 0.5 → Uninfected (healthy) → Label 1
        isInfected = raw < 0.5f;

        // ✅ NEW DEBUG LOGS (as requested)
        Debug.Log($"🩺 Raw Output: {raw:F4}");
        Debug.Log($"🩺 Predicted Class: {(isInfected ? "Parasitized (Label 0)" : "Uninfected (Label 1)")}");
        Debug.Log($"✅ Result: {(isInfected ? "INFECTED" : "HEALTHY")} | Confidence: {raw:F4}");

        input.Dispose();
        if (resized != texture) DestroyImmediate(resized);
    }

    // Helper: Resize texture to target dimensions
    Texture2D ResizeTexture(Texture2D source, int width, int height)
    {
        RenderTexture rt = RenderTexture.GetTemporary(width, height);
        Graphics.Blit(source, rt);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;
        Texture2D dest = new Texture2D(width, height);
        dest.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        dest.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);
        return dest;
    }

    public void OnTestButtonClick()
    {
        if (inputTexture != null)
        {
            AnalyzeCellImage(inputTexture);
        }
        else
        {
            Debug.LogWarning("No input texture assigned!");
        }
    }

    void OnDestroy()
    {
        engine?.Dispose();
    }
}
