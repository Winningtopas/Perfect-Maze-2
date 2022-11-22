using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public class ExportManager : MonoBehaviour
{
    [SerializeField]
    private RenderTexture topDownViewTexture;
    [SerializeField]
    private Texture2D mazeTexture;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void DownloadFile(byte[] array, int byteLength, string fileName);
#endif

    IEnumerator RecordUpscaledFrame(int screenshotUpscale)
    {
        yield return new WaitForEndOfFrame();

#if UNITY_WEBGL && !UNITY_EDITOR
        string dateFormat = "yyyy-MM-dd-HH-mm-ss";
        string filename = "perfect-maze-" + System.DateTime.Now.ToString(dateFormat);

        byte[] texture = mazeTexture.EncodeToPNG();
        DownloadFile(texture, texture.Length, filename + ".png");
#endif

    }

    public void GeneratePicture()
    {
        mazeTexture = ToTexture2D(topDownViewTexture, topDownViewTexture.width, topDownViewTexture.height);
        StartCoroutine(RecordUpscaledFrame(1));
    }

    private Texture2D ToTexture2D(RenderTexture rendertexture, int width, int height)
    {
        Texture2D newTexture2D = new Texture2D(width, height, TextureFormat.RGBA32, false);
        // ReadPixels looks at the active RenderTexture.
        RenderTexture.active = rendertexture;
        newTexture2D.ReadPixels(new Rect(0, 0, rendertexture.width, rendertexture.height), 0, 0);
        newTexture2D.Apply();
        return newTexture2D;
    }
}
