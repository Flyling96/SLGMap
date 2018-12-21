Shader "Custom/Terrain" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Terrain Texture Array", 2DArray) = "white" {}
	//地形纹理
		_AlbedoMap0("Albedo Map 0",2D) = "white"{}
		_AlbedoMap1("Albedo Map 1",2D) = "white"{}
		_AlbedoMap2("Albedo Map 2",2D) = "white"{}
		_AlbedoMap3("Albedo Map 3",2D) = "white"{}
		_AlbedoMap4("Albedo Map 4",2D) = "white"{}
		_AlbedoMap5("Albedo Map 5",2D) = "white"{}

	//地形法线纹理
	//	_BumpMap0("Bump Map 0",2D) = "white"{}
	//	_BumpMap1("Bump Map 1",2D) = "white"{}
	//	_BumpMap2("Bump Map 2",2D) = "white"{}
	//	_BumpMap3("Bump Map 3",2D) = "white"{}
	//	_BumpMap4("Bump Map 4",2D) = "white"{}
	//	_BumpMap5("Bump Map 5",2D) = "white"{}


	//地形附加纹理(用一张权重的图的RGBA来对应，所以最多有四个)
		//_AddMap0（"Add Map 0",2D） = "white"{}
		//_AddMap1（"Add Map 1",2D） = "white"{}
		//_AddMap2（"Add Map 2",2D） = "white"{}
		//_AddMap3（"Add Map 3",2D） = "white"{}
		//_WeightMap("Weight Map",2D) = "white"{}


		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_GridTex ("Grid Texture", 2D)= "white" {}
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM

#pragma surface surf Standard fullforwardshadows 
#pragma vertex vert
#pragma target 3.5

#pragma multi_compile TERRAIN_LAYERS_1 TERRAIN_LAYERS_2 TERRAIN_LAYERS_3 TERRAIN_LAYERS_4 TERRAIN_LAYERS_5 TERRAIN_LAYERS_6

		UNITY_DECLARE_TEX2DARRAY(_MainTex);

	struct Input {
		float4 color : COLOR;
		float3 worldPos;
		float3 terrain;
	};


	void vert(inout appdata_full v, out Input data) {
		UNITY_INITIALIZE_OUTPUT(Input, data);
		data.terrain = v.texcoord2.xyz;
	}

	half _Glossiness;
	half _Metallic;
	fixed4 _Color;
	sampler2D _GridTex;

	sampler2D _AlbedoMap0;
	sampler2D _AlbedoMap1;
	sampler2D _AlbedoMap2;
	sampler2D _AlbedoMap3;
	sampler2D _AlbedoMap4;
	sampler2D _AlbedoMap5;


	float4 GetTerrainColorByArray(Input IN, int index) {
		//float BlendWeightsArray[3];
		//BlendWeightsArray[0] = (int)IN.terrain.x;
		//BlendWeightsArray[1] = (int)IN.terrain.y;
		//BlendWeightsArray[2] = (int)IN.terrain.z;
		//{ IN.terrain.x, IN.terrain.y, IN.terrain.z };//BlendWeightsArray[index]
		float3 uvw = float3(IN.worldPos.xz * 0.02, IN.terrain[index]);
		float4 c = UNITY_SAMPLE_TEX2DARRAY(_MainTex, uvw);
		return c * IN.color[index];
	}

	half4 GetTerrainColor(Input IN, int index) {

		float2 uv = float2(IN.worldPos.xz * 0.02);
		half4 c;
		if (IN.terrain[index] == 0)
		{
			c = tex2D(_AlbedoMap0, uv);
		}
		else if (IN.terrain[index] == 1)
		{
			c = tex2D(_AlbedoMap1, uv);
		}
		else if (IN.terrain[index] == 2)
		{
			c = tex2D(_AlbedoMap2, uv);
		}
		else if (IN.terrain[index] == 3)
		{
			c = tex2D(_AlbedoMap3, uv);
		}
		else if (IN.terrain[index] == 4)
		{
			c = tex2D(_AlbedoMap4, uv);
		}
		else if (IN.terrain[index] == 5)
		{
			c = tex2D(_AlbedoMap5, uv);
		}
		else
		{
			c = half4(0, 0, 0, 0);
		}

		return c * IN.color[index];
	}

	void surf(Input IN, inout SurfaceOutputStandard o) {
		fixed4 c =
			GetTerrainColor(IN, 0) +
			GetTerrainColor(IN, 1) +
			GetTerrainColor(IN, 2);

		float2 gridUV = IN.worldPos.xz;
		gridUV.x *= 1 / (4 * 8.66025404);
		gridUV.y *= 1 / (2 * 15.0);
		fixed4 grid = tex2D(_GridTex, gridUV);

		o.Albedo = c.rgb * grid * _Color;
		//o.Albedo = IN.color;
		o.Metallic = _Metallic;
		o.Smoothness = _Glossiness;
		o.Alpha = c.a;
	}
	ENDCG
	}
		FallBack "Diffuse"
}