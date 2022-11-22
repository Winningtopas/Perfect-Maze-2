using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public class ExportManager : MonoBehaviour
{
    [SerializeField]
    Texture2D testTexture;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void DownloadFile(byte[] array, int byteLength, string fileName);

    [DllImport("__Internal")]
    private static extern void Hello();

    [DllImport("__Internal")]
    private static extern void HelloString(string str);

    [DllImport("__Internal")]
    private static extern void PrintFloatArray(float[] array, int size);

    [DllImport("__Internal")]
    private static extern int AddNumbers(int x, int y);

    [DllImport("__Internal")]
    private static extern string StringReturnValueFunction();

#endif

    void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR

        Hello();

        HelloString("This is a string.");

        float[] myArray = new float[10];
        PrintFloatArray(myArray, myArray.Length);

        int result = AddNumbers(5, 7);
        Debug.Log(result);

        Debug.Log(StringReturnValueFunction());
#endif
    }

    IEnumerator RecordUpscaledFrame(int screenshotUpscale)
    {
        yield return new WaitForEndOfFrame();

#if UNITY_WEBGL && !UNITY_EDITOR
        int resWidthN = Camera.main.pixelWidth * screenshotUpscale;
        int resHeightN = Camera.main.pixelHeight * screenshotUpscale;
        string dateFormat = "yyyy-MM-dd-HH-mm-ss";
        string filename = resWidthN.ToString() + "x" + resHeightN.ToString() + "px_" + System.DateTime.Now.ToString(dateFormat);
        Texture2D screenShot = ScreenCapture.CaptureScreenshotAsTexture(screenshotUpscale);
        byte[] texture = screenShot.EncodeToPNG();
        DownloadFile(texture, texture.Length, filename + ".png");
        Object.Destroy(screenShot);
#endif

    }

    void Update()
    {
        // Example of how to call the coroutine
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(RecordUpscaledFrame(1));
        }
    }
}
