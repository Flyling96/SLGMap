using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoRedoOperation {
    
    public struct UndoRedoInfo
    {
        public HexCell hexCell;

        public object[] parma;
    }

    public string name;

    public TerrainBrush brush;

    public List<UndoRedoInfo> undoRedoInfoList = new List<UndoRedoInfo>();

    public List<HexGridChunk> refreshChunkList = new List<HexGridChunk>();

    public UndoRedoOperation(string tname,List<UndoRedoInfo> undoRedoInfos)
    {
        name = tname;
        undoRedoInfoList = undoRedoInfos;
    }

    public virtual void JustDoIt(UndoRedoInfo info)
    {

    }

    public virtual void DoIt()
    {
        for (int i=0;i< undoRedoInfoList.Count;i++)
        {
            if(!refreshChunkList.Contains(undoRedoInfoList[i].hexCell.chunkParent))
            {
                refreshChunkList.Add(undoRedoInfoList[i].hexCell.chunkParent);
            }

            List<HexGridChunk> neigborChunk = undoRedoInfoList[i].hexCell.NeighorChunk();
            for(int j=0;j< neigborChunk.Count;j++)
            {
                if (!refreshChunkList.Contains(neigborChunk[j]))
                {
                    refreshChunkList.Add(neigborChunk[j]);
                }
            }
            JustDoIt(undoRedoInfoList[i]);
        }
    }


}

public class MeshOperation:UndoRedoOperation
{
    public enum OperationType
    {
        HeightEdit,
        WaterLevelEdit,
        EdgeEdit,
    }

    OperationType operation = OperationType.HeightEdit;

    public MeshOperation(OperationType type, string name,List<UndoRedoInfo> undoRedoInfos) : base(name, undoRedoInfos)
    {
        operation = type;
    }

    public override void DoIt()
    {
        base.DoIt();

        for(int i=0;i< refreshChunkList.Count;i++)
        {
            switch(operation)
            {
                case OperationType.HeightEdit:
                case OperationType.EdgeEdit:
                    refreshChunkList[i].Refresh(MeshClass.terrainMesh);
                    refreshChunkList[i].sceneObjectMgr.Refresh();
                    break;
                case OperationType.WaterLevelEdit:
                    refreshChunkList[i].Refresh(MeshClass.waterEdgeMesh);
                    refreshChunkList[i].Refresh(MeshClass.waterMesh);
                    break;
            }
        }
    }

    public override void JustDoIt(UndoRedoInfo info)
    {
        base.JustDoIt(info);

        switch(operation)
        {
            case OperationType.HeightEdit:
                info.hexCell.Elevation = (int)info.parma[0];
                break;
            case OperationType.WaterLevelEdit:
                info.hexCell.WaterLevel = (int)info.parma[0];
                break;
            case OperationType.EdgeEdit:
                for (int i = 0; i < 6; i++)
                {
                    info.hexCell.isStepDirection[i] = (bool)info.parma[i];
                    if (info.hexCell.GetNeighbor((HexDirection)i) != null)
                        info.hexCell.GetNeighbor((HexDirection)i).isStepDirection[(int)((HexDirection)i).Opposite()] = (bool)info.parma[i];
                }
                break;
        }
    }
}

public class MaterialOperation:UndoRedoOperation
{
    public enum OperationType
    {
        WholeCellEdit,
    }

    OperationType operation = OperationType.WholeCellEdit;

    public MaterialOperation(OperationType type,string name, List<UndoRedoInfo> undoRedoInfos) : base(name, undoRedoInfos)
    {
        operation = type;
    }

    public override void DoIt()
    {
        base.DoIt();

        for (int i = 0; i < refreshChunkList.Count; i++)
        {
            refreshChunkList[i].Refresh(MeshClass.terrainMesh);
            refreshChunkList[i].sceneObjectMgr.Refresh();
        }

    }

    public override void JustDoIt(UndoRedoInfo info)
    {
        base.JustDoIt(info);

        switch(operation)
        {
            case OperationType.WholeCellEdit:
                if(HexMetrics.instance.isEditorTexture)
                {
                    info.hexCell.TerrainTypeIndex = (TerrainTypes)info.parma[0];
                }
                else
                {
                    info.hexCell.color = (Color)info.parma[0];
                }
                break;
        }

    }
}

public class SceneObjOperation:UndoRedoOperation
{
    public enum OperationType
    {
        AddSceneObj,
        DeleteSceneObj,
    }

    OperationType operation = OperationType.AddSceneObj;


    public SceneObjOperation(OperationType type,string name, List<UndoRedoInfo> undoRedoInfos) : base(name, undoRedoInfos)
    {
        operation = type;
    }

    public override void DoIt()
    {
        for (int i = 0; i < undoRedoInfoList.Count; i++)
        {
            JustDoIt(undoRedoInfoList[i]);
        }
    }

    public override void JustDoIt(UndoRedoInfo info)
    {
        base.JustDoIt(info);

        switch(operation)
        {
            case OperationType.AddSceneObj:
                for(int i=0;i<info.parma.Length;i++)
                {
                    SceneObjectClass sceneObjectClass = info.parma[i] as SceneObjectClass;
                    GameObjectPool.instance.InsertChild(sceneObjectClass.gameObject.name, sceneObjectClass.gameObject);
                }
                break;
            case OperationType.DeleteSceneObj:
                for(int i=0;i<info.parma.Length;i++)
                {
                    SceneObjectClass sceneObjectClass = info.parma[i] as SceneObjectClass;
                    GameObjectPool.instance.RemoveTarge(sceneObjectClass.gameObject.name,sceneObjectClass.gameObject);
                    sceneObjectClass.gameObject.SetActive(true);
                    sceneObjectClass.Refresh(false);
                    sceneObjectClass.cell.chunkParent.sceneObjectMgr.AddSceneObject(sceneObjectClass);
                }
                break;
        }

    }



}






