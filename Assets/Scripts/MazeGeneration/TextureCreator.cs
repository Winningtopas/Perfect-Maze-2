using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextureCreator : MonoBehaviour
{
    public Dictionary<int, Texture2D> textureByFloor = new Dictionary<int, Texture2D>();

    [SerializeField]
    private RawImage image;
    private int cellWidth = 1;

    private void Start()
    {
        //ApplyTexture(0, 256);
    }

    public void ApplyTexture(int floor, int floorWidth, int floorLength, Color[] colors)
    {
        //colors = new Color[pixelCount];

        Texture2D newTexture = new Texture2D(floorWidth, floorLength, TextureFormat.ARGB32, false);
        newTexture.name = "topDownViewFloor" + floor;

        // set the pixel values

        //for (int i = 0; i < pixelCount; i++)
        //{
        //    if (i < pixelCount / 2f)
        //        colors[i] = Color.red;
        //    else
        //        colors[i] = Color.blue;
        //}

        newTexture.SetPixels(colors, 0);

        // Apply all SetPixel calls
        newTexture.Apply();

        image.texture = newTexture;

        textureByFloor.Add(floor, newTexture);
    }
}
