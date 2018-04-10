using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObjectClass : MonoBehaviour {

	
    public SceneObjectInfo sceneObjectInfo;

    public Vector3 position;

    public Quaternion rotation;

    public HexDirection direction;

    public HexCell cell = null;


    public void SetInfo(Vector3 tPosition, Quaternion tRotation, HexDirection dir,HexCell centerCell)
    {
        position = tPosition;
        rotation = tRotation;
        direction = dir;
        cell = centerCell;
    }

    public void Refresh()
    {
        Vector2 point = new Vector2(position.x, position.z);
        List<Vector2> pointList = new List<Vector2>();
        Vector3 center = cell.Position;
        EdgeVertices e = new EdgeVertices(
            center + HexMetrics.instance.GetFirstSolidCorner(direction),
            center + HexMetrics.instance.GetSecondSolidCorner(direction)
        );
        pointList.Add(new Vector2(HexMetrics.instance.Perturb(center).x, HexMetrics.instance.Perturb(center).z));
        pointList.Add(new Vector2(HexMetrics.instance.Perturb(e.v1).x, HexMetrics.instance.Perturb(e.v1).z));
        pointList.Add(new Vector2(HexMetrics.instance.Perturb(e.v2).x, HexMetrics.instance.Perturb(e.v2).z));
        pointList.Add(new Vector2(HexMetrics.instance.Perturb(e.v3).x, HexMetrics.instance.Perturb(e.v3).z));
        pointList.Add(new Vector2(HexMetrics.instance.Perturb(e.v4).x, HexMetrics.instance.Perturb(e.v4).z));

        Debug.Log(point);
        for (int i=0;i<pointList.Count;i++)
        {
            Debug.Log(pointList[i]);
        }
        //如果在多边形内
        if (ToolClass.instance.IsInside(point, pointList))
        {
            Debug.Log(1);
        }
        else
        {
            Debug.Log(2);
        }

    }
}
