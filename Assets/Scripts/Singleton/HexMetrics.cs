using UnityEngine;

public class HexMetrics: Singleton<HexMetrics>
{
    public int chunkWidth = 5;
    public int chunkHeight = 5;

    public static Texture2D noiseSource;//噪声图片

    public float cellPerturbStrength = 3f;//微扰程度

    public float outerRadius = 10f;//外径

	public float innerRadius = 8;//内径

	public float solidFactor = 0.75f;

	public float blendFactor = 0.25f;

	public float elevationStep = 5f;

	public int terracesPerStep = 2;//台阶数

    public float waterOffset = -0.5f;//同高度下，水面相对陆面的偏移


	public int terraceSteps = 2 * 2 + 1;//台阶面数

	public float horizontalTerraceStepSize = 1f / 5;

	public float verticalTerraceStepSize = 1f / (5 + 1);

    //偏移中心点的各点的向量
    Vector3[] corners = null;


    private void Awake()
    {
        innerRadius = outerRadius * 0.866025404f;
        blendFactor = 1f - solidFactor;
        terraceSteps = terracesPerStep * 2 + 1;
        horizontalTerraceStepSize = 1f / terraceSteps;
        verticalTerraceStepSize = 1f / (terracesPerStep + 1);
        corners = new Vector3[]{
            new Vector3(0f, 0f, outerRadius),
		    new Vector3(innerRadius, 0f, 0.5f * outerRadius),
		    new Vector3(innerRadius, 0f, -0.5f * outerRadius),
		    new Vector3(0f, 0f, -outerRadius),
		    new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
		    new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
		    new Vector3(0f, 0f, outerRadius)
        };
    }

    public  Vector3 GetFirstCorner (HexDirection direction) {
		return corners[(int)direction];
	}

	public  Vector3 GetSecondCorner (HexDirection direction) {
		return corners[(int)direction + 1];
	}

	public  Vector3 GetFirstSolidCorner (HexDirection direction) {
		return corners[(int)direction] * solidFactor;
	}

	public  Vector3 GetSecondSolidCorner (HexDirection direction) {
		return corners[(int)direction + 1] * solidFactor;
	}


	public Vector3 GetBridge (HexDirection direction) {
		return (corners[(int)direction] + corners[(int)direction + 1]) *
			blendFactor;
	}


    // 获取a到b之间的差值位置
	public Vector3 TerraceLerp (Vector3 a, Vector3 b, int step) {
		float h = step * horizontalTerraceStepSize;
		a.x += (b.x - a.x) * h;
		a.z += (b.z - a.z) * h;
		float v = ((step + 1) / 2) * verticalTerraceStepSize;
		a.y += (b.y - a.y) * v;
		return a;
	}

	public Color TerraceLerp (Color a, Color b, int step) {
		float h = step * horizontalTerraceStepSize;
		return Color.Lerp(a, b, h);
	}

	public HexEdgeType GetEdgeType (int elevation1, int elevation2,bool isStep) {
		if (elevation1 == elevation2) {
			return HexEdgeType.Flat;
		}
		int delta = elevation2 - elevation1;
		if (delta == 1 || delta == -1) {
            if (isStep)
            {
                return HexEdgeType.Step;
            }
            else
            {
                return HexEdgeType.Slope;
            }
		}
		return HexEdgeType.Cliff;
	}

    public const float elevationPerturbStrength = 1.5f;//y轴上的微扰

    public const float noiseScale = 0.003f;
    //获取正交化坐标系下像素颜色
    public Vector4 SampleNoise(Vector3 position)
    {
        return noiseSource.GetPixelBilinear(position.x* noiseScale, position.z* noiseScale);
    }

    public Color splatColorR = new Color(1, 0, 0);
    public Color splatColorG = new Color(0, 1, 0);
    public Color splatColorB = new Color(0, 0, 1);
}