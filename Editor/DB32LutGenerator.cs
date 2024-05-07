using UnityEngine;
using UnityEditor; // Necesario para guardar la textura en la carpeta Assets

public class DB32LUTGenerator : MonoBehaviour
{

    private void Start()
    {
        Texture2D lut = GenerateDB32LUT();
        SaveLUTToAssets(lut, "DB32_LUT");
    }

    public Texture2D GenerateDB32LUT()
    {
        int lutSize = 32; // Usando 32 ya que lo quieres en un rango de 0 a 31
        Texture2D lut = new Texture2D(lutSize * lutSize, lutSize, TextureFormat.RGBA32, false);

        for (int r = 0; r < lutSize; r++)
        {
            for (int g = 0; g < lutSize; g++)
            {
                for (int b = 0; b < lutSize; b++)
                {
                    Color color = new Color(r / (float)(lutSize - 1), g / (float)(lutSize - 1), b / (float)(lutSize - 1), 1f);
                    Color closest = FindClosestColor(color);

                    // Calcular los índices correctamente para el despliegue en una textura 2D
                    int x = r * lutSize + b; // Eje Z se despliega en el eje X
                    int y = g; // Eje Y

                    lut.SetPixel(x, y, closest);
                }
            }
        }

        lut.Apply();
        return lut;
    }

    private Color FindClosestColor(Color color)
    {
        float minDist = float.MaxValue;
        Color closest = ColorExtensions.db32Palette[0];

        foreach (Color paletteColor in ColorExtensions.db32Palette)
        {
            float dist = Vector3.Distance(new Vector3(color.r, color.g, color.b), new Vector3(paletteColor.r, paletteColor.g, paletteColor.b));
            if (dist < minDist)
            {
                minDist = dist;
                closest = paletteColor;
            }
        }
        return closest;
    }

    private void SaveLUTToAssets(Texture2D lut, string fileName)
    {
        byte[] bytes = lut.EncodeToPNG();
        string filePath = Application.dataPath + "/" + fileName + ".png";
        System.IO.File.WriteAllBytes(filePath, bytes);

        // Refrescar el AssetDatabase para mostrar la nueva textura en Unity
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }
}
