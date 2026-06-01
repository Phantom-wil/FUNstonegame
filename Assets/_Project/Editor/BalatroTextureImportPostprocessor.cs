#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace PasserCard.Editor
{
    /// <summary>
    /// Keeps Balatro temp sheets pixel-crisp and CPU-readable for runtime Sprite.Create.
    /// </summary>
    public sealed class BalatroTextureImportPostprocessor : AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            if (!assetPath.Replace('\\', '/').Contains("/Resources/Balatro/"))
            {
                return;
            }

            var importer = (TextureImporter)assetImporter;
            var isCardFace = assetPath.Replace('\\', '/').EndsWith("/CardFace.png");
            importer.filterMode = isCardFace ? FilterMode.Bilinear : FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.isReadable = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.alphaIsTransparency = true;
        }
    }
}
#endif
