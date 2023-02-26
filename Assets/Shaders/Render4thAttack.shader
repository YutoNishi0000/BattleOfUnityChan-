Shader "Unlit/Render4thAttack"
{
	Properties
	{
		_EmissionColor("Emission Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Blend("Blend", Range(0.0,1.0)) = 0.5
		_PointX("PointX", float) = 0
		_PointZ("PointZ", float) = 0
		_MainTex("Base(RGB)", 2D) = "white"{}
		_EmissionTex("Emission Texture", 2D) = "white"{}
		_Side("Side", float) = 0
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
			float _Side;

			struct Input
			{
				float3 worldPos;
				float2 uv_MainTex;
			};

			float rectangle(float2 p, float2 size) {
				return max(abs(p.x) - size.x, abs(p.y) - size.y);
			}

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				float dist = ellipse(fixed3(0, 0, -2), IN.worldPos, 5);//distance(fixed3(0, 0, -2), IN.worldPos);
				float radius1 = 12;
				float radius2 = 9;
				half4 main = tex2D(_MainTex, IN.uv_MainTex);
				half4 sub = fixed4(1, 0, 0, 1);

				if (_Side == 0)
				{
					o.Albedo = main.rgb;
					o.Alpha = main.a;
				}
				else if (_Side == 1)
				{
					if (radius1 < dist)
					{
						o.Albedo = main * (1 - _Blend) + sub * _Blend;
					}
					else
					{
						o.Albedo = main.rgb;
						o.Alpha = main.a;
					}
				}
			}
			ENDCG
		}
			FallBack "Diffuse"
}
