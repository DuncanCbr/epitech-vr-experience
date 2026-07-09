using UnityEditor;
using UnityEngine;

namespace Weather.Editor
{
    public class WeatherIconImporter : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            if (assetPath.Contains("Weather/Icons") && assetPath.EndsWith(".png"))
            {
                TextureImporter textureImporter = (TextureImporter)assetImporter;
                if (textureImporter.textureType != TextureImporterType.Sprite)
                {
                    textureImporter.textureType = TextureImporterType.Sprite;
                    textureImporter.spriteImportMode = SpriteImportMode.Single;
                    textureImporter.alphaIsTransparency = true;
                    textureImporter.mipmapEnabled = false;
                    Debug.Log($"[WeatherIconImporter] Automatically set {assetPath} to Sprite.");
                }
            }
        }
    }
}
