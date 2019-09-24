using UnityEditor;
using UnityEngine;

public class PostprocessImages : AssetPostprocessor
{
	void OnPostprocessTexture(Texture2D texture)
	{
		TextureImporter textureImporter = (TextureImporter) assetImporter;
		textureImporter.filterMode = FilterMode.Point;
		textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
		textureImporter.spritePixelsPerUnit = 32;
	}
}