using UnityEngine;
using System.IO;

public class LutGenerator : MonoBehaviour
{
    public Texture2D paletteTexture; // Textura de la paleta, asignar en el inspector
    public int lutSize = 32; // Tamaño del LUT
    public string outputFileName = "LUTTexture.png"; // Nombre del archivo

    void Start()
    {
        Texture2D lutTexture = GenerateLutTexture(paletteTexture, lutSize);
        SaveTextureToFile(lutTexture, outputFileName);
        Debug.Log($"LUT guardado en {Application.dataPath}/{outputFileName}");
    }

    Texture2D GenerateLutTexture(Texture2D palette, int size)
    {
        // Crear textura LUT con dimensiones (size * size) x size para formato horizontal
        Texture2D lutTexture = new Texture2D(size * size, size, TextureFormat.RGB24, false);

        Color[] paletteColors = palette.GetPixels();
        int paletteLength = paletteColors.Length;

        for (int r = 0; r < size; r++)
        {
            for (int g = 0; g < size; g++)
            {
                for (int b = 0; b < size; b++)
                {
                    // Intercambia R y G en la definición del color
                    Color targetColor = new Color(g / (float)(size - 1), r / (float)(size - 1), b / (float)(size - 1));
                    Color closestColor = FindClosestColor(paletteColors, paletteLength, targetColor);
                    // Invierte Y y ajusta la asignación de los píxeles
                    lutTexture.SetPixel(r + b * size, g, closestColor);
                }
            }
        }

        lutTexture.Apply();
        return lutTexture;
    }

    Color FindClosestColor(Color[] paletteColors, int paletteLength, Color targetColor)
    {
        float minDistance = float.MaxValue;
        Color closestColor = Color.black;

        for (int i = 0; i < paletteLength; i++)
        {
            float distance = Vector3.SqrMagnitude(new Vector3(paletteColors[i].r - targetColor.r, paletteColors[i].g - targetColor.g, paletteColors[i].b - targetColor.b));
            if (distance < minDistance)
            {
                minDistance = distance;
                closestColor = paletteColors[i];
            }
        }

        return closestColor;
    }

    void SaveTextureToFile(Texture2D texture, string fileName)
    {
        byte[] bytes = texture.EncodeToPNG();
        string path = Path.Combine(Application.dataPath, fileName);
        File.WriteAllBytes(path, bytes);
    }
}
