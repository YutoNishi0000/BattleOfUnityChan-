Shader "Custom/RenderCircle"
{
	Properties
	{
		_EmissionColor("Emission Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Blend("Blend", Range(0.0,1.0)) = 0.5
		_PointX("PointX", float) = 0
		_PointZ("PointZ", float) = 0
		_MainTex("Base(RGB)", 2D) = "white"{}
		_EmissionTex("Emission Texture", 2D) = "white"{}
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard 
		#pragma target 3.0

		sampler2D _MainTex;
		float _Blend;
		fixed3 pos;
		float _PointX;
		float _PointZ;

		struct Input 
		{
			float3 worldPos;
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o) 
		{
			pos = fixed3(_PointX, 0, _PointZ);
			float dist = distance(fixed3(pos), IN.worldPos);
			float radius = 4;
			half4 main = tex2D(_MainTex, IN.uv_MainTex);
			half4 sub = fixed4(1, 0, 0, 1);

			if (radius < dist)
			{
				o.Albedo = main.rgb;
				o.Alpha = main.a;
			}
			else
			{
				o.Albedo = main * (1 - _Blend) + sub * _Blend;
			}
		}
		ENDCG
	}
	FallBack "Diffuse"
}