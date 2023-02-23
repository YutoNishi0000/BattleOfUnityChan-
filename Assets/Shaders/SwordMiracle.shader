Shader "ExampleSurfaceShader"
{
    Properties{
        _MainTex("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        CGPROGRAM

        #pragma surface surf Standard vertex:vert

        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
            // 頂点シェーダから受け渡す変数を定義する
            float3 rimColor;
        };

        sampler2D _MainTex;

        // 第二引数にInputを指定する
        void vert(inout appdata_full v, out Input o) {
            // UNITY_INITIALIZE_OUTPUTでInputを初期化
            UNITY_INITIALIZE_OUTPUT(Input, o);
            // Inputの変数に値を代入
            o.rimColor = float3(1, 0, 0);
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
            o.Metallic = 1;
            o.Smoothness = 0.5;
            o.Alpha = 0.1;
            // IN.rimColorを使う
            o.Emission = (1 - dot(IN.viewDir, o.Normal)) * IN.rimColor;
        }

        ENDCG
    }
}