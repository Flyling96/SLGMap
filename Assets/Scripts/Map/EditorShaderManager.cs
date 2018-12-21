using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class EditorShaderManager{

    private static Shader terrainShader;
    private static Texture2D terrainDefaultTexture;
    private static Texture2D terrainGridTexutre;


    static EditorShaderManager()
    {
        terrainShader = Shader.Find("Custom/Terrain");
        terrainDefaultTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(EditorConfig.instance.terrainDefalutTextruePath);
        terrainGridTexutre = AssetDatabase.LoadAssetAtPath<Texture2D>(EditorConfig.instance.terrainGridTexturePath);
    }

    public static Shader TerrainShader
    {
        get
        {
            return terrainShader;
        }
    }

    public static Texture2D TerrainDefaultTexture
    {
        get
        {
            return terrainDefaultTexture;
        }

    }

    public static Texture2D TerrainGridTexutre
    {
        get
        {
            return terrainGridTexutre;
        }
    }
}
