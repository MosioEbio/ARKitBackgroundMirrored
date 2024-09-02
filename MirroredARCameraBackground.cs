using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.XR.ARSubsystems;

public class MirroredARCameraBackground : MonoBehaviour
{
    public ARCameraManager cameraManager;
    public Material targetMaterial;
    public string mainTexPropertyName = "_MainTex";

    private Texture2D m_Texture;
    private int m_MainTexId;

    void Start()
    {
        if (cameraManager == null)
        {
            Debug.LogError("AR Camera Manager is not assigned!");
            enabled = false;
            return;
        }

        if (targetMaterial == null)
        {
            Debug.LogError("Target Material is not assigned!");
            enabled = false;
            return;
        }

        m_MainTexId = Shader.PropertyToID(mainTexPropertyName);
    }

    void Update()
    {
        UpdateCameraTexture();
    }

    unsafe void UpdateCameraTexture()
    {
        // Attempt to get the latest camera image
        if (!cameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
        {
            return;
        }

        try
        {
            // Choose an RGBA format
            var format = TextureFormat.RGBA32;

            // Create or resize the texture if needed
            if (m_Texture == null || m_Texture.width != image.width || m_Texture.height != image.height)
            {
                m_Texture = new Texture2D(image.width, image.height, format, false);
            }

            // Set up conversion parameters
            var conversionParams = new XRCpuImage.ConversionParams
            {
                inputRect = new RectInt(0, 0, image.width, image.height),
                outputDimensions = new Vector2Int(image.width, image.height),
                outputFormat = TextureFormat.RGBA32,
                transformation = XRCpuImage.Transformation.MirrorY | XRCpuImage.Transformation.MirrorX
            };

            // Get the raw texture data
            var rawTextureData = m_Texture.GetRawTextureData<byte>();

            // Convert the image to the texture
            image.Convert(conversionParams, new IntPtr(rawTextureData.GetUnsafePtr()), rawTextureData.Length);

            // Apply the updated texture data
            m_Texture.Apply();

            // Set the texture to the material
            targetMaterial.SetTexture(m_MainTexId, m_Texture);
        }
        finally
        {
            // Always dispose the image to avoid resource leaks
            image.Dispose();
        }
    }
}