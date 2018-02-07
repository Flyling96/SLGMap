using UnityEngine;

public static class HexMetrics {

    public const int chunkWidth = 5;
    public const int chunkHeight = 5;

    public static Texture2D noiseSource;//噪声图片

    public const float cellPerturbStrength = 3f;//微扰程度

    public const float outerRadius = 10f;//外径

	public const float innerRadius = outerRadius * 0.866025404f;//内径

	public const float solidFactor = 0.75f;

	public const float blendFactor = 1f - solidFactor;

	public const float elevationStep = 5f;

	public const int terracesPerStep = 2;//台阶数

	public const int terraceSteps = terracesPerStep * 2 + 1;//台阶面数

	public const float horizontalTerraceStepSize = 1f / terraceSteps;

	public const float verticalTerraceStepSize = 1f / (terracesPerStep + 1);

    //偏移中心点的各点的向量
	static Vector3[] corners = {
		new Vector3(0f, 0f, outerRadius),
		new Vector3(innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(0f, 0f, -outerRadius),
		new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(0f, 0f, outerRadius)
	};

	public static Vector3 GetFirstCorner (HexDirection direction) {
		return corners[(int)direction];
	}

	public static Vector3 GetSecondCorner (HexDirection direction) {
		return corners[(int)direction + 1];
	}

	public static Vector3 GetFirstSolidCorner (HexDirection direction) {
		return corners[(int)direction] * solidFactor;
	}

	public static Vector3 GetSecondSolidCorner (HexDirection direction) {
		return corners[(int)direction + 1] * solidFactor;
	}


	public static Vector3 GetBridge (HexDirection direction) {
		return (corners[(int)direction] + corners[(int)direction + 1]) *
			blendFactor;
	}


    // 获取a到b之间的差值位置
	public static Vector3 TerraceLerp (Vector3 a, Vector3 b, int step) {
		float h = step * HexMetrics.horizontalTerraceStepSize;
		a.x += (b.x - a.x) * h;
		a.z += (b.z - a.z) * h;
		float v = ((step + 1) / 2) * HexMetrics.verticalTerraceStepSize;
		a.y += (b.y - a.y) * v;
		return a;
	}

	public static Color TerraceLerp (Color a, Color b, int step) {
		float h = step * HexMetrics.horizontalTerraceStepSize;
		return Color.Lerp(a, b, h);
	}

	public static HexEdgeType GetEdgeType (int elevation1, int elevation2,bool isStep) {
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
    public static Vector4 SampleNoise(Vector3 position)
    {
        return noiseSource.GetPixelBilinear(position.x* noiseScale, position.z* noiseScale);
    }
}