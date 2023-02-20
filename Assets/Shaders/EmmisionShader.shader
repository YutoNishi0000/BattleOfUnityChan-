Shader "Custom/EmmisionShader"
{
    Properties
    {
        _EmissionColor("Emission Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _MainTex("Base(RGB)", 2D) = "white"{}
        _EmissionTex("Emission Texture", 2D) = "white"{}
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Lambert
        
        half4 _EmissionColor;
        sampler2D _MainTex;
        sampler2D _EmissionTex;
        float4 _MyEmissionColor;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            half4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
            o.Emission = _EmissionColor;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
