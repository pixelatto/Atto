/*
using System.IO;
using UnityEngine;
using UnityEditor;

public static class AestheticsFolderSync
{
    public static string sourcePath = @"C:/Repos/ReventHub/Aesthetics";
    public static string destinationPath = Path.Combine(Application.dataPath, "Aesthetics");

    public static string[] ignoredExtensions = { ".meta" };

    [MenuItem("Atto/Sync Aesthetics Folder")]
    public static void SyncFolders()
    {
        if (!Directory.Exists(destinationPath))
            Directory.CreateDirectory(destinationPath);

        SyncDirectory(sourcePath, destinationPath);
    }

    private static void SyncDirectory(string sourceDir, string destDir)
    {
        bool newAssets = false;
        if (!Directory.Exists(destDir))
            Directory.CreateDirectory(destDir);

        foreach (var sourceFile in Directory.GetFiles(sourceDir))
        {
            if (IsIgnoredExtension(sourceFile))
                continue;

            string destFile = Path.Combine(destDir, Path.GetFileName(sourceFile));

            if (!File.Exists(destFile))
            {
                File.Copy(sourceFile, destFile, true);
                Debug.Log($"File {destFile} cloned from source {sourceFile}");
                newAssets = true;
            }
            else if (File.GetLastWriteTime(sourceFile) > File.GetLastWriteTime(destFile))
            {
                File.Copy(sourceFile, destFile, true);
                Debug.Log($"File {destFile} synced from source {sourceFile}");
                newAssets = true;
            }
            else if (File.GetLastWriteTime(sourceFile) < File.GetLastWriteTime(destFile))
            {
                File.Copy(destFile, sourceFile, true);
                Debug.Log($"File {sourceFile} synced from destination {destFile}");
            }
        }

        foreach (var destFile in Directory.GetFiles(destDir))
        {
            if (IsIgnoredExtension(destFile))
                continue;

            string sourceFile = Path.Combine(sourceDir, Path.GetFileName(destFile));

            if (!File.Exists(sourceFile))
            {
                File.Delete(destFile);
            }
        }

        foreach (var sourceSubDir in Directory.GetDirectories(sourceDir))
        {
            string destSubDir = Path.Combine(destDir, Path.GetFileName(sourceSubDir));
            SyncDirectory(sourceSubDir, destSubDir);
        }

        foreach (var destSubDir in Directory.GetDirectories(destDir))
        {
            string sourceSubDir = Path.Combine(sourceDir, Path.GetFileName(destSubDir));

            if (!Directory.Exists(sourceSubDir))
            {
                Directory.Delete(destSubDir, true);
            }
        }

        if (newAssets)
        {
            AssetDatabase.Refresh();
        }
    }

    private static bool IsIgnoredExtension(string filePath)
    {
        string extension = Path.GetExtension(filePath);
        foreach (var ignored in ignoredExtensions)
        {
            if (string.Equals(extension, ignored, System.StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }
}

[InitializeOnLoad]
public class FolderSyncEditor
{
    private static bool isChecking = true;
    private static float checkInterval = 5f;
    private static float lastCheckTime;

    static FolderSyncEditor()
    {
        EditorApplication.update += Update;
        AestheticsFolderSync.SyncFolders();
    }

    static void Update()
    {
        if (!isChecking)
            return;

        if (Time.realtimeSinceStartup - lastCheckTime > checkInterval)
        {
            AestheticsFolderSync.SyncFolders();
            lastCheckTime = Time.realtimeSinceStartup;
        }
    }

}
*/