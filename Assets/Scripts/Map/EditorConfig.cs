using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorConfig : ScriptableObject {

    public const string CONFIG_FILE_NAME = "Assets/Resources/EditorConfig.asset";

    private static EditorConfig m_instance;

    public static EditorConfig instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = Resources.Load<EditorConfig>("EditorConfig");
#if UNITY_EDITOR
                if(m_instance == null)
                {
                    CreateConfig(CONFIG_FILE_NAME);
                }
#endif
                return m_instance;
            }
            else
            {
                return m_instance;
            }
        }
        set
        {
            m_instance = value;
        }
    }

    /// <summary>
    /// 地图文件目录，相对于Assets目录
    /// </summary>
    public string mapFileDirectory = "MapAssets";

    /// <summary>
    /// 场景中地形prefab的父物体
    /// </summary>
    public string terrainParentName = "Terrains";

    /// <summary>
    /// 默认地形贴图的路径
    /// </summary>
    public string terrainDefalutTextruePath = "Assets/Materials/Terrain/Grass.png";

    /// <summary>
    /// 默认地形Grid贴图的路径
    /// </summary>
    public string terrainGridTexturePath = "Assets/Materials/Terrain/Grid.png";

    /// <summary>
    /// 一个块中最多拥有的贴图个数
    /// </summary>
    public int maxChunkTextureCount = 6;


#if UNITY_EDITOR

    private static void CreateConfig(string path)
    {
        m_instance = ScriptableObject.CreateInstance<EditorConfig>();
        AssetDatabase.CreateAsset(m_instance, path);
    }

#endif

}
