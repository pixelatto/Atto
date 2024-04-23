using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AttoPalette", menuName = "Atto/Palette")]
public class Palette : ScriptableObject
{
    public Texture2D texture;
    [SerializeField, HideInInspector]
    private List<Color> colors = new List<Color>();

    void OnEnable()
    {
        LoadTexture();
        ExtractColors();
    }

    private void OnValidate()
    {
        LoadTexture();
        ExtractColors();
    }

    private void LoadTexture()
    {
        // Intenta cargar la textura con el mismo nombre que este ScriptableObject
        string path = $"Palettes/{name}";
        texture = Resources.Load<Texture2D>(path);
        if (texture == null)
        {
            Debug.LogError($"No se pudo cargar la textura para la paleta '{name}' en el path '{path}'");
        }
    }

    private void ExtractColors()
    {
        colors.Clear();
        if (texture != null)
        {
            for (int i = 0; i < texture.width; i++)
            {
                // Asumiendo que la paleta de colores está en la primera fila de la textura
                Color color = texture.GetPixel(i, 0);
                colors.Add(color);
            }
        }
        else
        {
            Debug.LogError("Texture not set for extracting colors.");
        }
    }

    public Color GetColor(int index)
    {
        if (index >= 0 && index < colors.Count)
        {
            return colors[index];
        }
        else
        {
            Debug.LogError("Index out of range.");
            return new Color(0, 0, 0, 0); // Retorna un color transparente si el índice no es válido
        }
    }
}
