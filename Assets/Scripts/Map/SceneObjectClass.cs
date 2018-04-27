using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObjectClass : MonoBehaviour {

	
    public SceneObjectInfo sceneObjectInfo;

    public Vector3 position;

    public Quaternion rotation;

    public HexDirection direction = HexDirection.NE;

    public HexCell cell = null;

    List<Vector2> pointList = new List<Vector2>(); //不规则六边形的边顶点
   // List<List<Vector2>> edgePointList = new List<List<Vector2>>();



    public void SetInfo(Vector3 tPosition, Quaternion tRotation, HexDirection dir,HexCell centerCell)
    {
        pointList.Clear();
        //edgePointList.Clear();

        position = tPosition;
        rotation = tRotation;
        direction = dir;
        cell = centerCell;
        //sceneObjectInfo = 

        Vector3 center = cell.Position;

        EdgeVertices e1 = new EdgeVertices(
            center + HexMetrics.instance.GetFirstSolidCorner(direction),
            center + HexMetrics.instance.GetSecondSolidCorner(direction)
        );
        pointList.Add(new Vector2(HexMetrics.instance.Perturb(center).x, HexMetrics.instance.Perturb(center).z));
        pointList.Add(new Vector2(HexMetrics.instance.Perturb(e1.v1).x, HexMetrics.instance.Perturb(e1.v1).z));
        pointList.Add(new Vector2(HexMetrics.instance.Perturb(e1.v2).x, HexMetrics.instance.Perturb(e1.v2).z));
        pointList.Add(new Vector2(HexMetrics.instance.Perturb(e1.v3).x, HexMetrics.instance.Perturb(e1.v3).z));
        pointList.Add(new Vector2(HexMetrics.instance.Perturb(e1.v4).x, HexMetrics.instance.Perturb(e1.v4).z));

        //EdgeVertices e2 = new EdgeVertices(
        //    cell.GetNeighbor(direction).transform.position + HexMetrics.instance.GetFirstSolidCorner(direction.Opposite()),
        //    cell.GetNeighbor(direction).transform.position + HexMetrics.instance.GetSecondSolidCorner(direction.Opposite())
        //);

        //List<Vector2> tPointList = new List<Vector2>();
        //tPointList.Add(new Vector2(HexMetrics.instance.Perturb(e1.v1).x, HexMetrics.instance.Perturb(e1.v1).z));
        //tPointList.Add(new Vector2(HexMetrics.instance.Perturb(e2.v4).x, HexMetrics.instance.Perturb(e2.v4).z));
        //tPointList.Add(new Vector2(HexMetrics.instance.Perturb(e2.v3).x, HexMetrics.instance.Perturb(e2.v3).z));
        //tPointList.Add(new Vector2(HexMetrics.instance.Perturb(e1.v2).x, HexMetrics.instance.Perturb(e1.v2).z));
        //edgePointList.Add(tPointList);
        //tPointList = new List<Vector2>();
        //tPointList.Add(new Vector2(HexMetrics.instance.Perturb(e1.v2).x, HexMetrics.instance.Perturb(e1.v2).z));
        //tPointList.Add(new Vector2(HexMetrics.instance.Perturb(e2.v3).x, HexMetrics.instance.Perturb(e2.v3).z));
        //tPointList.Add(new Vector2(HexMetrics.instance.Perturb(e2.v2).x, HexMetrics.instance.Perturb(e2.v2).z));
        //tPointList.Add(new Vector2(HexMetrics.instance.Perturb(e1.v3).x, HexMetrics.instance.Perturb(e1.v3).z));
        //edgePointList.Add(tPointList);
        //tPointList = new List<Vector2>();
        //tPointList.Add(new Vector2(HexMetrics.instance.Perturb(e1.v3).x, HexMetrics.instance.Perturb(e1.v3).z));
        //tPointList.Add(new Vector2(HexMetrics.instance.Perturb(e2.v2).x, HexMetrics.instance.Perturb(e2.v2).z));
        //tPointList.Add(new Vector2(HexMetrics.instance.Perturb(e2.v1).x, HexMetrics.instance.Perturb(e2.v1).z));
        //tPointList.Add(new Vector2(HexMetrics.instance.Perturb(e1.v4).x, HexMetrics.instance.Perturb(e1.v4).z));
        //edgePointList.Add(tPointList);
    }

    public void Refresh(bool isInit)
    {
        Vector2 point = new Vector2(position.x, position.z);

        if (!isInit)
        {
            if (ToolClass.instance.IsInside(point, pointList))
            {
                position = new Vector3(position.x, cell.Position.y, position.z);
            }
            else
            {
                transform.GetComponent<BoxCollider>().enabled = false;
                Vector3 worldPosition = transform.parent.TransformPoint(position);
                RaycastHit hit;
                //在斜坡上重新计算y的位置
                if (Physics.Raycast(new Vector3(worldPosition.x, 35, worldPosition.z), Vector3.down, out hit))
                {
                    if (hit.collider.gameObject.tag != "Mesh")
                    {
                        transform.parent.GetComponent<SceneObjectMgr>().MinusSceneObject(this);
                    }
                    position = transform.parent.InverseTransformPoint(new Vector3(worldPosition.x, hit.point.y, worldPosition.z));
                }
                transform.GetComponent<BoxCollider>().enabled = true;
            }
        }
        transform.localPosition = position;
        transform.localRotation = rotation;

        //如果在多边形内
        //if (ToolClass.instance.IsInside(point, pointList))
        //{
        //    position = new Vector3(position.x, cell.Position.y, position.z);
        //}
        //else
        //{

        //    float objectY = cell.transform.position.y;
        //    List<Vector2> tPointList = new List<Vector2>();

        //    for (int i=0;i< edgePointList.Count;i++)
        //    {
        //        if (ToolClass.instance.IsInside(point, edgePointList[i]))
        //        {
        //            tPointList = edgePointList[i];
        //        }
        //    }

        //    double disCell = ToolClass.instance.PointToLine(point, tPointList[0], tPointList[3]);
        //    double disNeighbor = ToolClass.instance.PointToLine(point, tPointList[1], tPointList[2]);
        //    float offest = (float)(disCell/(disCell+disNeighbor));

        //    //如果是阶梯
        //    if (cell.isStepDirection[(int)direction])
        //    {

        //    }
        //    else //如果是斜坡
        //    {
        //        objectY = Mathf.Lerp(cell.transform.position.y, cell.GetNeighbor(direction).transform.position.y, offest+0.2f);
        //    }
        //    position = new Vector3(position.x, objectY, position.z);
        //    Debug.Log(2);


        //if (cell.Elevation!= cell.GetNeighbor(direction).Elevation)
        //{
        //    //如果在斜坡上就直接放入对象池
        //    //transform.parent.GetComponent<SceneObjectMgr>().MinusSceneObject(this);
        //    transform.GetComponent<BoxCollider>().enabled = false;
        //    Vector3 worldPosition = transform.parent.TransformPoint(position);
        //    RaycastHit hit;
        //    //在斜坡上重新计算y的位置
        //    if (Physics.Raycast(new Vector3(worldPosition.x, 35, worldPosition.z), Vector3.down, out hit))
        //    {
        //        if (hit.collider.gameObject.tag != "Mesh")
        //        {
        //            transform.parent.GetComponent<SceneObjectMgr>().MinusSceneObject(this);
        //        }
        //        position = transform.parent.InverseTransformPoint(new Vector3(worldPosition.x, hit.point.y, worldPosition.z));
        //    }
        //    transform.GetComponent<BoxCollider>().enabled = true;
        //}

        //}
    }
}
