Shader "Atto/LightsPostprocessing"
{
    Properties
    {
        _MainTex("Color (RGB)", 2D) = "white" {}
        _LuminosityTex("Luminosity", 2D) = "white" {}
        _Gamma("Gamma", float) = 1
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

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
            sampler2D _LuminosityTex;
            float4 _MainTex_ST;
            float4 _LuminosityTex_ST;
            float _Gamma;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 lum = tex2D(_LuminosityTex, i.uv);
                return col * pow(lum , _Gamma);
            }
            ENDCG
        }
    }
        FallBack "Diffuse"
}
