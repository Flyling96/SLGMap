using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class HexGridChunk : MonoBehaviour {

    int width = 5;
    int height = 5;

    public HexCell[] cells;

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


    // Use this for initialization
    void Start()
    {
        terrainMesh.TrangulateByMeshClass(cells);
    }


    public void Clear()
    {
        waterMesh.Clear();
        waterEdgeMesh.Clear();
        terrainMesh.Clear();
        sceneObjectMgr.Clear();
    }

    public void Refresh()
    {
        waterMesh.TrangulateByMeshClass(cells);
        waterEdgeMesh.TrangulateByMeshClass(cells);
        terrainMesh.TrangulateByMeshClass(cells);
    }

    //对mesh进行刷新
    public void Refresh(MeshClass meshClass)
    {
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
}
