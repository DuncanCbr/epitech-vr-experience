using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Reads key-value pairs from a .env file located at the project root
/// (Editor) or next to the executable (Build). Values are cached in a
/// static dictionary so the file is only read once per session.
/// </summary>
public static class EnvReader
{
    private static Dictionary<string, string> variables;
    private static bool loaded;

    /// <summary>
    /// Returns the value associated with the given key, or null if not found.
    /// Automatically loads the .env file on first call.
    /// </summary>
    public static string Get(string key)
    {
        if (!loaded)
            Load();

        if (variables != null && variables.TryGetValue(key, out string value))
            return value;

        return null;
    }

    /// <summary>
    /// Parses the .env file and populates the internal dictionary.
    /// Supports comments (#) and empty lines. Trims surrounding quotes
    /// from values (both single and double quotes).
    /// </summary>
    public static void Load()
    {
        loaded = true;
        variables = new Dictionary<string, string>();

        string envPath = FindEnvFile();
        if (string.IsNullOrEmpty(envPath))
        {
            Debug.LogWarning("[EnvReader] No .env file found. API keys will not be available.");
            return;
        }

        Debug.Log($"[EnvReader] Loading environment variables from: {envPath}");

        string[] lines = File.ReadAllLines(envPath);
        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();

            // Skip empty lines and comments
            if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                continue;

            int separatorIndex = line.IndexOf('=');
            if (separatorIndex < 0)
                continue;

            string key = line.Substring(0, separatorIndex).Trim();
            string value = line.Substring(separatorIndex + 1).Trim();

            // Strip surrounding quotes if present
            if (value.Length >= 2)
            {
                if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                    (value.StartsWith("'") && value.EndsWith("'")))
                {
                    value = value.Substring(1, value.Length - 2);
                }
            }

            variables[key] = value;
        }

        Debug.Log($"[EnvReader] Loaded {variables.Count} variable(s).");
    }

    /// <summary>
    /// Searches for the .env file in several likely locations:
    /// 1. Project root (one level above Assets/) - for Editor mode
    /// 2. Application.dataPath parent - for builds
    /// 3. Application.streamingAssetsPath - alternative location
    /// </summary>
    private static string FindEnvFile()
    {
        // In the Editor, Application.dataPath points to Assets/
        // so the project root is its parent directory.
        string projectRoot = Directory.GetParent(Application.dataPath)?.FullName;
        if (!string.IsNullOrEmpty(projectRoot))
        {
            string path = Path.Combine(projectRoot, ".env");
            if (File.Exists(path))
                return path;
        }

        // Fallback: next to the executable (standalone builds)
        string buildPath = Path.Combine(Application.dataPath, ".env");
        if (File.Exists(buildPath))
            return buildPath;

        return null;
    }
}
