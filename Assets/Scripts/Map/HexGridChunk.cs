using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class HexGridChunk : MonoBehaviour {

    [SerializeField]
    int width = 5;
    [SerializeField]
    int height = 5;

    public bool isMeshChange = false;

    [SerializeField]
    HexCell[] cells;

    public SceneObjectMgr sceneObjectMgr;

    public HexMesh waterMesh,waterEdgeMesh,terrainMesh;

    void OnEnable()
    {
        if (HexMetrics.instance != null)
        {
            width = HexMetrics.instance.chunkWidth;
            height = HexMetrics.instance.chunkHeight;
        }
        cells = new HexCell[width * height];
    }

    public void CreateCells(int chunkWidth = 5,int chunkHeight = 5)
    {
        width = chunkWidth;
        height = chunkHeight;
        cells = new HexCell[width * height];
    }

    // Use this for initialization
    void Start()
    {
        //terrainMesh.TrangulateByMeshClass(cells);
    }


    public void Clear()
    {
        waterMesh.Clear();
        waterEdgeMesh.Clear();
        terrainMesh.Clear();
        sceneObjectMgr.Clear();
    }

    public void Init()
    {
        isMeshChange = false;
        sceneObjectMgr.remindOriginList();
    }

    public void MeshInit()
    {
        waterMesh.Init();
        waterEdgeMesh.Init();
        terrainMesh.Init();
    }

    public void Refresh()
    {
        isMeshChange = true;
        waterMesh.TrangulateByMeshClass(cells);
        waterEdgeMesh.TrangulateByMeshClass(cells);
        terrainMesh.TrangulateByMeshClass(cells);
    }

    //对mesh进行刷新
    public void Refresh(MeshClass meshClass)
    {
        isMeshChange = true;
        switch (meshClass)
        {
            case MeshClass.terrainMesh:
                terrainMesh.TrangulateByMeshClass(cells);
                break;

            case MeshClass.waterMesh:
                waterMesh.TrangulateByMeshClass(cells);
                break;

            case MeshClass.waterEdgeMesh:
                waterEdgeMesh.TrangulateByMeshClass(cells);
                break;
        }
    }


    public void Refresh(HexMesh refreshMesh)
    {
        Refresh(refreshMesh.meshClass);
    }

    public void AddCell(int i,HexCell cell)
    {
        cells[i] = cell;
        cell.transform.SetParent(this.transform);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //以地形块为主体的保存
    public void Save(BinaryWriter writer)
    {
        int count = 0;
        foreach(Transform item in sceneObjectMgr.transform)
        {
            if (item.gameObject.activeSelf == true)
            {
                count++;
            }
        }
        writer.Write(count);
        foreach (Transform item in sceneObjectMgr.transform)
        {
            SceneObjectClass itemClass = null;
            if(item.gameObject.activeSelf == true)
            {
                itemClass = item.GetComponent<SceneObjectClass>();
                writer.Write(int.Parse(itemClass.sceneObjectInfo.modelPathType));
                writer.Write((double)itemClass.position.x);
                writer.Write((double)itemClass.position.y);
                writer.Write((double)itemClass.position.z);
                writer.Write((double)itemClass.rotation.x);
                writer.Write((double)itemClass.rotation.y);
                writer.Write((double)itemClass.rotation.z);
                writer.Write((double)itemClass.rotation.w);
            }
        }

    }

    //以地形块为主体的加载
    public List<SceneObjectClass> Load(BinaryReader reader)
    {
        int count = reader.ReadInt32();
        Vector3 itemPosition;
        Quaternion itemRotaiton;
        int modelPathType;
        List<SceneObjectClass> sceneObjectClassList = new List<SceneObjectClass>();
        for (int i = 0; i < count; i++)
        {
            SceneObjectClass itemClass = new SceneObjectClass();
            itemClass.sceneObjectInfo = new SceneObjectInfo();
            modelPathType = reader.ReadInt32();
            itemClass.sceneObjectInfo.modelPathType = modelPathType.ToString();
            itemClass.sceneObjectInfo.modelPath = FileManage.instance.CSVHashTable["sceneHashTable"][itemClass.sceneObjectInfo.modelPathType].ToString();
            itemPosition = new Vector3((float)reader.ReadDouble(), (float)reader.ReadDouble(), (float)reader.ReadDouble());
            itemRotaiton = new Quaternion((float)reader.ReadDouble(), (float)reader.ReadDouble(), (float)reader.ReadDouble(), (float)reader.ReadDouble());
            itemClass.position = itemPosition;
            itemClass.rotation = itemRotaiton;
            sceneObjectClassList.Add(itemClass);
        }
        return sceneObjectClassList;
    }

    //保存chunk动态建立的对象到指定目录，使这些对象与资源关联起来
    public void SaveChunkAssets(string path)
    {
        SaveMesh(path);
        SaveMaterial(path);
    }

    public void SaveMesh(string path)
    {
        string fileName = path +"/"+ gameObject.name;
        if (!File.Exists(fileName + "_waterMesh.asset"))
        {
            AssetDatabase.CreateAsset(waterMesh.GetComponent<MeshFilter>().sharedMesh, fileName + "_waterMesh.asset");
        }
        if (!File.Exists(fileName + "_waterEdgeMesh.asset"))
        {
            AssetDatabase.CreateAsset(waterEdgeMesh.GetComponent<MeshFilter>().sharedMesh, fileName + "_waterEdgeMesh.asset");
        }
        if (!File.Exists(fileName + "_terrainMesh.asset"))
        {
            AssetDatabase.CreateAsset(terrainMesh.GetComponent<MeshFilter>().sharedMesh, fileName + "_terrainMesh.asset");
        }
    }

    ////将数据加载到内存中
    //public void LoadMesh(string path)
    //{
    //    string fileName = path + "/" + gameObject.name;
    //    waterMesh.hexMesh = AssetDatabase.LoadAssetAtPath<Mesh>(fileName + "_waterMesh.asset");
    //    waterEdgeMesh.hexMesh = AssetDatabase.LoadAssetAtPath<Mesh>(fileName + "_waterEdgeMesh.asset");
    //    terrainMesh.hexMesh =  AssetDatabase.LoadAssetAtPath<Mesh>(fileName + "_terrainMesh.asset");
    //}

    public void SaveMaterial(string path)
    {
        string fileName = path + "/" + gameObject.name + "_mat.mat";
        if (!File.Exists(fileName))
        {
            AssetDatabase.CreateAsset(terrainMesh.GetComponent<MeshRenderer>().sharedMaterial, fileName);
        }
    }

    Texture2DArray textureArray;

    public void CreateMaterial(Texture2D defaultTerrainTex = null)
    {
        MeshRenderer meshRenderer = terrainMesh.GetComponent<MeshRenderer>();
        Material mtl = new Material(EditorShaderManager.TerrainShader);
        CreateTerrainTexutreArray();
        mtl.SetTexture("_AlbedoMap0", defaultTerrainTex);
        mtl.SetTexture("_GridTex", EditorShaderManager.TerrainGridTexutre);
        meshRenderer.sharedMaterial = mtl;
    }

    public void CreateTerrainTexutreArray()
    {
        Texture2D texture = EditorShaderManager.TerrainDefaultTexture;
        textureArray = new Texture2DArray(
            texture.width, texture.height, 1, texture.format, texture.mipmapCount > 1
        );
        textureArray.anisoLevel = texture.anisoLevel;
        textureArray.filterMode = texture.filterMode;
        textureArray.wrapMode = texture.wrapMode;

        for (int m = 0; m < texture.mipmapCount; m++)
        {
            Graphics.CopyTexture(texture, 0, m, textureArray, 0, m);
        }

    }

    public void DrawBorder(Color color)
    {
        List<EdgeVertices> edgeList = terrainMesh.borderList;
        Handles.color = color;
        for(int i=0;i<edgeList.Count;i++)
        {
            Handles.DrawLines(new Vector3[]{ transform.TransformPoint(terrainMesh.Perturb(edgeList[i].v1)), transform.TransformPoint(terrainMesh.Perturb(edgeList[i].v2)),
                transform.TransformPoint(terrainMesh.Perturb(edgeList[i].v2)), transform.TransformPoint(terrainMesh.Perturb(edgeList[i].v3)),
                transform.TransformPoint(terrainMesh.Perturb(edgeList[i].v3)), transform.TransformPoint(terrainMesh.Perturb(edgeList[i].v4)) });
        }
    }

   
}
