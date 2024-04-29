Shader "Atto/ColorRampPostProcessing"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _RampTex("Color Ramp", 2D) = "white" {}
        _Daytime("Daytime", float) = 0
    }
        SubShader
    {
        // Tags necesarios para el post-procesamiento
        Tags { "Queue" = "Overlay" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _RampTex;
            float4 _MainTex_TexelSize;
            float _Daytime;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 color = tex2D(_MainTex, i.uv);
                float gray = dot(color.rgb, float3(0.299, 0.587, 0.114)); // Conversión a escala de grises
                float4 rampColor = tex2D(_RampTex, float2(gray, _Daytime)); // Mapeo usando la rampa
                return rampColor;
            }
            ENDCG
        }
    }
        FallBack "Diffuse"
}
