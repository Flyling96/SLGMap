using UnityEngine;
using System.Collections.Generic;


public enum MeshClass
{
    terrainMesh,
    roadMesh,
    waterMesh,
    waterEdgeMesh,
}
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour {

    public bool isUseMap = false;//不使用贴图就是指使用颜色
    [SerializeField]
    protected Mesh hexMesh;

    public MeshClass meshClass = MeshClass.terrainMesh;
    List<Vector3> vertices = new List<Vector3>();
    List<Vector3> terrainTypes = new List<Vector3>();
    List<Color> colors = new List<Color>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    MeshCollider meshCollider;

    private void Awake()
    {
        Init();
    }
    bool isInit = false;

    public void Init()
    {
        if (hexMesh == null)
        {
            GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        }
        //else
        //{
        //    if (HexMetrics.instance.isEditor)
        //    {
        //        hexMesh = CopyMesh(hexMesh);
        //        gameObject.GetComponent<MeshFilter>().mesh = hexMesh;
        //    }
        //}

        if (meshCollider == null)
        {
            if (gameObject.GetComponent<MeshCollider>() == null)
            {
                meshCollider = gameObject.AddComponent<MeshCollider>();
            }
            else
            {
                meshCollider = gameObject.GetComponent<MeshCollider>();
            }
        }
        meshCollider.sharedMesh = hexMesh;
        isInit = true;
    }

    Mesh CopyMesh(Mesh origin)
    {
        Mesh newMesh = new Mesh();
        newMesh.vertices = origin.vertices;
        newMesh.triangles = origin.triangles;
        if (isUseMap)
        {
            newMesh.colors = colors.ToArray();
            newMesh.SetUVs(2, terrainTypes);
        }
        else
        {
            newMesh.colors = colors.ToArray();
            newMesh.SetUVs(0, uvs);
        }

        return newMesh;
    }

    public void Triangulate(HexCell[] cells)
    {
        Clear();
        for (int i = 0; i < cells.Length; i++)
        {
            Triangulate(cells[i]);
        }
        Draw();
    }

    //不同的mesh类对此类进行重写
    public virtual void TrangulateByMeshClass(HexCell[] cells)
    {
        if(!isInit)
        {
            Init();
        }
    }

    public void Clear()
    {
        terrainTypes.Clear();
        if(hexMesh!=null)hexMesh.Clear();
        vertices.Clear();
        colors.Clear();
        triangles.Clear();
        uvs.Clear();
    }

    public void Draw()
    {
        hexMesh.vertices = vertices.ToArray();
        hexMesh.triangles = triangles.ToArray();
        if(isUseMap)
        {
            hexMesh.colors = colors.ToArray();
            hexMesh.SetUVs(2, terrainTypes); 
        }
        else
        {
            hexMesh.colors = colors.ToArray();
            hexMesh.SetUVs(0, uvs);
        }
        hexMesh.RecalculateNormals();//mesh重新计算法线
        hexMesh.RecalculateBounds();
        gameObject.GetComponent<MeshFilter>().mesh = hexMesh;
    }


    //绘制六边形
	public void Triangulate (HexCell cell) {
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
			Triangulate(d, cell);
		}
	}

	void Triangulate (HexDirection direction, HexCell cell) {
        if(cell == null)
        {
            return;
        }
		Vector3 center = cell.LocalPosition;
        //Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(direction);   //形成面的内六边形顶点
        //Vector3 v2 = center + HexMetrics.GetSecondSolidCorner(direction);

        EdgeVertices e = new EdgeVertices(
            center + HexMetrics.instance.GetFirstSolidCorner(direction),
            center + HexMetrics.instance.GetSecondSolidCorner(direction)
        );

        //为了增强不规则性，添加顶点
        //Vector3 e1 = Vector3.Lerp(v1, v2, 1f / 3f);
        //Vector3 e2 = Vector3.Lerp(v1, v2, 2f / 3f);

        //      AddTriangle(center, v1, e1);
        //      AddTriangleColor(cell.color);
        //      AddTriangle(center, e1, e2);
        //      AddTriangleColor(cell.color);
        //      AddTriangle(center, e2, v2);
        //      AddTriangleColor(cell.color);

        //      //绘制内三角
        //      //AddTriangle(center, v1, v2);
        ////AddTriangleColor(cell.color);

        //      //只绘制半边
        //if (direction <= HexDirection.SE) {
        //	TriangulateConnection(direction, cell, v1, v2,e1,e2);
        //}
        TriangulateEdgeFan(center, e, cell.color);

        if (direction <= HexDirection.SE)
        {
            TriangulateConnection(direction, cell, e);
        }
    }

    /// <summary>
    /// 绘制外梯形
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="cell"></param>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
     void TriangulateConnection (
		HexDirection direction, HexCell cell, EdgeVertices e1
    ) {
		HexCell neighbor = cell.GetNeighbor(direction);
		if (neighbor == null) {
			return;
		}

        Vector3 bridge = HexMetrics.instance.GetBridge(direction);
        bridge.y = neighbor.LocalPosition.y - cell.LocalPosition.y;
        EdgeVertices e2 = new EdgeVertices( e1.v1 + bridge,e1.v4 + bridge);


        //绘制阶梯
        if (cell.GetEdgeType(cell.isStepDirection[(int)direction],direction) == HexEdgeType.Step) {
            TriangulateEdgeTerraces(e1, cell, e2,  neighbor);
        }
        //绘制平面或者陡坡
		else {
            TriangulateEdgeStrip(e1,cell.color, e2, neighbor.color);
        }

        //绘制中间三角
		HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
		if (direction <= HexDirection.E && nextNeighbor != null) {
			Vector3 v5 = e1.v4 + HexMetrics.instance.GetBridge(direction.Next());//获取桥对面的点
			v5.y = nextNeighbor.LocalPosition.y;//设置桥对面的点的y

			if (cell.Elevation <= neighbor.Elevation) {
				if (cell.Elevation <= nextNeighbor.Elevation) {
					TriangulateCorner(e1.v4, cell, e2.v4, neighbor, v5, nextNeighbor);
				}
				else {
					TriangulateCorner(v5, nextNeighbor, e1.v4, cell, e2.v4, neighbor);
				}
			}
			else if (neighbor.Elevation <= nextNeighbor.Elevation) {
				TriangulateCorner(e2.v4, neighbor, v5, nextNeighbor, e1.v4, cell);
			}
			else {
				TriangulateCorner(v5, nextNeighbor, e1.v4, cell, e2.v4, neighbor);
			}
		}
	}

    //绘制中间三角区域
	void TriangulateCorner (
		Vector3 bottom, HexCell bottomCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell
	) {
		HexEdgeType leftEdgeType = bottomCell.GetEdgeType(bottomCell.isStepDirection[(int)HexCoordinates.GetDirection(bottomCell, leftCell)],leftCell);
		HexEdgeType rightEdgeType = bottomCell.GetEdgeType(bottomCell.isStepDirection[(int)HexCoordinates.GetDirection(bottomCell, rightCell)], rightCell);

        //left-right-other
		if (leftEdgeType == HexEdgeType.Step) {
            //SSF 
            if (rightEdgeType == HexEdgeType.Step) {
				TriangulateCornerTerraces(
					bottom, bottomCell, left, leftCell, right, rightCell
				);
			}
            //SFS
			else if (rightEdgeType == HexEdgeType.Flat && leftCell.GetEdgeType(leftCell.isStepDirection[(int)HexCoordinates.GetDirection(leftCell, rightCell)], rightCell) == HexEdgeType.Step) {
				TriangulateCornerTerraces(
					left, leftCell, right, rightCell, bottom, bottomCell
				);
			}
            //SCS SSC SCC 
			else {
				TriangulateCornerTerracesCliff(
					bottom, bottomCell, left, leftCell, right, rightCell
				);
			}
		}
		else if (rightEdgeType == HexEdgeType.Step) {
            //FSS
			if (leftEdgeType == HexEdgeType.Flat && leftCell.GetEdgeType(leftCell.isStepDirection[(int)HexCoordinates.GetDirection(leftCell, rightCell)], rightCell) == HexEdgeType.Step) {
				TriangulateCornerTerraces(
					right, rightCell, bottom, bottomCell, left, leftCell
				);
			}
            //CSC CSS
			else {
				TriangulateCornerCliffTerraces(
					bottom, bottomCell, left, leftCell, right, rightCell
				);
			}
		}
		else if (leftCell.GetEdgeType(leftCell.isStepDirection[(int)HexCoordinates.GetDirection(leftCell, rightCell)],rightCell) == HexEdgeType.Step) {
            //CCS 
			if (leftCell.Elevation < rightCell.Elevation) {
				TriangulateCornerCliffTerraces(
					right, rightCell, bottom, bottomCell, left, leftCell
				);
			}
			else {
				TriangulateCornerTerracesCliff(
					left, leftCell, right, rightCell, bottom, bottomCell
				);
			}
		}
        //CCF CCC FCC CFC 
		else {
			AddTriangle(bottom, left, right);
			AddTriangleColor(bottomCell.color, leftCell.color,  rightCell.color);
		}
	}

    //绘制阶梯
    void TriangulateEdgeTerraces(
        EdgeVertices begin, HexCell beginCell,
        EdgeVertices end, HexCell endCell
    )
    {
        EdgeVertices e2 = EdgeVertices.TerraceLerp(begin, end, 1);
        Color c2 = HexMetrics.instance.TerraceLerp(beginCell.color, endCell.color, 1);

        TriangulateEdgeStrip(begin, beginCell.color, e2, c2);

        for (int i = 2; i < HexMetrics.instance.terraceSteps; i++)
        {
            EdgeVertices e1 = e2;
            Color c1 = c2;
            e2 = EdgeVertices.TerraceLerp(begin, end, i);
            c2 = HexMetrics.instance.TerraceLerp(beginCell.color, endCell.color, i);
            TriangulateEdgeStrip(e1, c1, e2, c2);
        }

        TriangulateEdgeStrip(e2, c2, end, endCell.color);
    }

    //有出现平面的情况
    void TriangulateCornerTerraces (
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell
	) {
		Vector3 v3 = HexMetrics.instance.TerraceLerp(begin, left, 1);
		Vector3 v4 = HexMetrics.instance.TerraceLerp(begin, right, 1);
		Color c3 = HexMetrics.instance.TerraceLerp(beginCell.color, leftCell.color, 1);
		Color c4 = HexMetrics.instance.TerraceLerp(beginCell.color, rightCell.color, 1);

		AddTriangle(begin, v3, v4);
		AddTriangleColor(beginCell.color, c3, c4);

		for (int i = 2; i < HexMetrics.instance.terraceSteps; i++) {
			Vector3 v1 = v3;
			Vector3 v2 = v4;
			Color c1 = c3;
			Color c2 = c4;
			v3 = HexMetrics.instance.TerraceLerp(begin, left, i);
			v4 = HexMetrics.instance.TerraceLerp(begin, right, i);
			c3 = HexMetrics.instance.TerraceLerp(beginCell.color, leftCell.color, i);
			c4 = HexMetrics.instance.TerraceLerp(beginCell.color, rightCell.color, i);
			AddQuad(v1, v2, v3, v4);
			AddQuadColor(c1, c2, c3, c4);
		}

		AddQuad(v3, v4, left, right);
		AddQuadColor(c3, c4, leftCell.color, rightCell.color);
	}

    //有出现陡坡的情况
    //left为s,right为c
	void TriangulateCornerTerracesCliff (
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell
	) {
        float b = Mathf.Abs(1f / (rightCell.Elevation - beginCell.Elevation));
        Vector3 boundary = Vector3.Lerp(Perturb(begin), Perturb(right), b);
        Color boundaryColor = Color.Lerp(beginCell.color, rightCell.color, b);

		TriangulateBoundaryTriangle(
			begin, beginCell, left, leftCell, boundary, boundaryColor
		);

        //判断left和right之间的类型关系
        if (leftCell.GetEdgeType(leftCell.isStepDirection[(int)HexCoordinates.GetDirection(leftCell, rightCell)],rightCell) == HexEdgeType.Step) {
			TriangulateBoundaryTriangle(
				left, leftCell, right, rightCell, boundary, boundaryColor
			);
		}
		else {
            AddTriangleUnPerturb(Perturb(left), Perturb(right), boundary);
            AddTriangleColor(leftCell.color, rightCell.color, boundaryColor);
        }
	}

    //left为c，right为s 与上一种情况对称
	void TriangulateCornerCliffTerraces (
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell
	) {
		float b = Mathf.Abs( 1f / (leftCell.Elevation - beginCell.Elevation));
		Vector3 boundary = Vector3.Lerp(Perturb(begin), Perturb(left), b);
		Color boundaryColor = Color.Lerp(beginCell.color, leftCell.color, b);

		TriangulateBoundaryTriangle(
			right, rightCell, begin, beginCell, boundary, boundaryColor
		);

        //判断left和right之间的类型关系
		if (leftCell.GetEdgeType(leftCell.isStepDirection[(int)HexCoordinates.GetDirection(leftCell, rightCell)],rightCell) == HexEdgeType.Step) {
			TriangulateBoundaryTriangle(
				left, leftCell, right, rightCell, boundary, boundaryColor
			);
		}
		else {
            AddTriangleUnPerturb(Perturb(left), Perturb(right), boundary);
            AddTriangleColor(leftCell.color, rightCell.color, boundaryColor);
        }
	}

    void TriangulateBoundaryTriangle(
        Vector3 begin, HexCell beginCell,
        Vector3 left, HexCell leftCell,
        Vector3 boundary, Color boundaryColor
    )
    {
        Vector3 v2 = Perturb(HexMetrics.instance.TerraceLerp(begin, left, 1));
        Color c2 = HexMetrics.instance.TerraceLerp(beginCell.color, leftCell.color, 1);

        AddTriangleUnPerturb(Perturb(begin), v2, boundary);
        AddTriangleColor(beginCell.color, c2, boundaryColor);

        for (int i = 2; i < HexMetrics.instance.terraceSteps; i++)
        {
            Vector3 v1 = v2;
            Color c1 = c2;
            v2 = Perturb(HexMetrics.instance.TerraceLerp(begin, left, i));
            c2 = HexMetrics.instance.TerraceLerp(beginCell.color, leftCell.color, i);
            AddTriangleUnPerturb(v1, v2, boundary);
            AddTriangleColor(c1, c2, boundaryColor);
        }

        AddTriangleUnPerturb(v2, Perturb(left), boundary);
        AddTriangleColor(c2, leftCell.color, boundaryColor);
    }



    // 添加三角形
    protected void AddTriangle (Vector3 v1, Vector3 v2, Vector3 v3) {
		int vertexIndex = vertices.Count;
        vertices.Add(Perturb(v1));
        vertices.Add(Perturb(v2));
        vertices.Add(Perturb(v3));
        triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
	}

    //添加没有被微扰的三角形
    protected void AddTriangleUnPerturb(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    // 添加三角形三点的颜色
    protected void AddTriangleColor (Color color) {
		colors.Add(color);
		colors.Add(color);
		colors.Add(color);
	}


    // 添加三角形三点的颜色
    protected void AddTriangleColor (Color c1, Color c2, Color c3) {
		colors.Add(c1);
		colors.Add(c2);
		colors.Add(c3);
	}


    // 添加四边形
    protected void AddQuad (Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) {
		int vertexIndex = vertices.Count;
		vertices.Add(Perturb(v1));
		vertices.Add(Perturb(v2));
		vertices.Add(Perturb(v3));
		vertices.Add(Perturb(v4));
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 3);
	}

    // 添加四边形四个点的颜色
    protected void AddQuadColor (Color c1, Color c2) {
		colors.Add(c1);
		colors.Add(c1);
		colors.Add(c2);
		colors.Add(c2);
	}


    // 添加四边形四个点的颜色
    protected void AddQuadColor (Color c1, Color c2, Color c3, Color c4) {
		colors.Add(c1);
		colors.Add(c2);
		colors.Add(c3);
		colors.Add(c4);
	}


    //对地图进行不规则处理
    public Vector3 Perturb(Vector3 position)
    {
        //用世界坐标进行取样
        Vector3 newPostion =  transform.TransformPoint(position);
        Vector4 sample = HexMetrics.instance.SampleNoise(newPostion);
        position.x += (sample.x * 2f - 1f)* HexMetrics.instance.cellPerturbStrength;
        // position.y += (sample.y * 2f - 1f) * HexMetrics.cellPerturbStrength;  //不对y坐标进行微扰，改做对阶梯高度进行微扰
        position.z += (sample.z * 2f - 1f) * HexMetrics.instance.cellPerturbStrength;
        return position;
    }

    //不规则的边内六边形的三角化
    void TriangulateEdgeFan(Vector3 center, EdgeVertices edge, Color color)
    {
        AddTriangle(center, edge.v1, edge.v2);
        AddTriangleColor(color);
        AddTriangle(center, edge.v2, edge.v3);
        AddTriangleColor(color);
        AddTriangle(center, edge.v3, edge.v4);
        AddTriangleColor(color);
    }

    //规则的边内六边形三角化
    protected void TriangulateEdgeUnPerturb(Vector3 center, EdgeVertices edge, Color color)
    {
        AddTriangleUnPerturb(center, edge.v1, edge.v2);
        AddTriangleColor(color);
        AddTriangleUnPerturb(center, edge.v2, edge.v3);
        AddTriangleColor(color);
        AddTriangleUnPerturb(center, edge.v3, edge.v4);
        AddTriangleColor(color);
    }

    //不规则边的外梯形绘制
    void TriangulateEdgeStrip(
    EdgeVertices e1, Color c1,
    EdgeVertices e2, Color c2
)
    {
        AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
        AddQuadColor(c1, c2);
        AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
        AddQuadColor(c1, c2);
        AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
        AddQuadColor(c1, c2);
    }

    //设置法线
    public void AddTriangleUV(Vector2 uv1, Vector2 uv2, Vector3 uv3)
    {
        uvs.Add(uv1);
        uvs.Add(uv2);
        uvs.Add(uv3);
    }
    public void AddQuadUV(Vector2 uv1, Vector2 uv2, Vector3 uv3, Vector3 uv4)
    {
        uvs.Add(uv1);
        uvs.Add(uv2);
        uvs.Add(uv3);
        uvs.Add(uv4);
    }
    public void AddQuadUV(float uMin, float uMax, float vMin, float vMax)
    {
        uvs.Add(new Vector2(uMin, vMin));
        uvs.Add(new Vector2(uMax, vMin));
        uvs.Add(new Vector2(uMin, vMax));
        uvs.Add(new Vector2(uMax, vMax));
    }


    //设置地形类型
    public void AddTriangleTerrain(Vector3 types)
    {
        terrainTypes.Add(types);
        terrainTypes.Add(types);
        terrainTypes.Add(types);
    }

    public void AddQuadTerrain(Vector3 types)
    {
        terrainTypes.Add(types);
        terrainTypes.Add(types);
        terrainTypes.Add(types);
        terrainTypes.Add(types);
    }

}