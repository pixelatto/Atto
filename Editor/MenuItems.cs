using UnityEngine;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

#if UNITY_EDITOR
using UnityEditor;

public class MenuItems
{
    [MenuItem("Tools/Pixelatto/Clear player prefs")]
    public static void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs cleared.");
    }

    private const string ScenesDirectory = "Assets/@Projects/Boring/Scenes";

    [MenuItem("Tools/Pixelatto/Boring/AddScenesToBuildSettings")]
    public static void AddScenesToBuildSettings()
    {
        EditorBuildSettings.scenes = new EditorBuildSettingsScene[0];

        // Obtener todas las escenas en el directorio especificado
        string[] scenePaths = Directory.GetFiles(ScenesDirectory, "*.unity", SearchOption.AllDirectories);

        if (scenePaths.Length == 0)
        {
            Debug.LogError("No scenes found in: " + ScenesDirectory);
            return;
        }

        // Ordenar las escenas, primero detectando n�meros para ordenarlos num�ricamente si existen
        scenePaths = scenePaths.OrderBy(path => ExtractSceneNumber(Path.GetFileNameWithoutExtension(path)))
                               .ThenBy(path => Path.GetFileNameWithoutExtension(path))
                               .ToArray();

        // Obtener las escenas ya incluidas en las Build Settings
        var currentScenes = EditorBuildSettings.scenes.ToList();

        // A�adir las nuevas escenas si no est�n ya incluidas
        foreach (string scenePath in scenePaths)
        {
            if (!currentScenes.Any(s => s.path == scenePath))
            {
                currentScenes.Add(new EditorBuildSettingsScene(scenePath, true));
                //Debug.Log("Added scene to Build Settings: " + scenePath);
            }
        }

        // Actualizar las Build Settings con las escenas nuevas
        EditorBuildSettings.scenes = currentScenes.ToArray();
        //Debug.Log("Scenes added to Build Settings successfully.");
    }

    // M�todo auxiliar para extraer el n�mero de la escena
    private static int ExtractSceneNumber(string sceneName)
    {
        // Expresi�n regular para encontrar un n�mero en el nombre de la escena
        var match = Regex.Match(sceneName, @"\d+");
        return match.Success ? int.Parse(match.Value) : int.MaxValue; // Si no hay n�mero, devolver un valor muy alto
    }
}
#endif

