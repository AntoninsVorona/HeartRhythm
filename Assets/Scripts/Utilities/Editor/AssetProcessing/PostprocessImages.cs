using UnityEditor;
using UnityEngine;

public class PostprocessImages : AssetPostprocessor
{
	void OnPreprocessTexture()
	{
		TextureImporter textureImporter = (TextureImporter) assetImporter;
		textureImporter.filterMode = FilterMode.Point;
		textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
		textureImporter.spritePixelsPerUnit = 16;
	}
}