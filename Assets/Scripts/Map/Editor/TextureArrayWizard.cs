using UnityEditor;
using UnityEngine;

public class TextureArrayWizard : ScriptableWizard {

	public Texture2D[] textures;

	[MenuItem ("Assets/Create/Texture Array")]
	static void CreateWizard () {
		ScriptableWizard.DisplayWizard<TextureArrayWizard>(
			"Create Texture Array", "Create"
		);
	}

	void OnWizardCreate () {
		if (textures.Length == 0) {
			return;
		}
		string path = EditorUtility.SaveFilePanelInProject(
			"Save Texture Array", "Texture Array", "asset", "Save Texture Array"
		);
		if (path.Length == 0) {
			return;
		}

		Texture2D t = textures[0];
		Texture2DArray textureArray = new Texture2DArray(
			t.width, t.height, textures.Length, t.format, t.mipmapCount > 1
		);
		textureArray.anisoLevel = t.anisoLevel;
		textureArray.filterMode = t.filterMode;
		textureArray.wrapMode = t.wrapMode;

		for (int i = 0; i < textures.Length; i++) {
			for (int m = 0; m < t.mipmapCount; m++) {
				Graphics.CopyTexture(textures[i], 0, m, textureArray, i, m);
			}
		}

		AssetDatabase.CreateAsset(textureArray, path);
	}
}