Shader "Atto/TilemapSurfaces"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _MainColor("MainColor", Color) = (1,1,1,1)
        _SecColor("SecColor", Color) = (1,1,1,1)
        _Opacity("Opacity", Float) = 1
    }
        SubShader
        {
            Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
            LOD 100

            Blend SrcAlpha OneMinusSrcAlpha
            AlphaTest Greater 0.5
            Cull Off
            Lighting On

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata_t
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
                float4 _MainTex_ST;
                float4 _MainColor;
                float4 _SecColor;
                float _Opacity;

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv);
                    fixed4 o = col.b* _MainColor + col.g * _SecColor;
                    o.a = col.a * _Opacity;
                    return o;
                }
                ENDCG
            }
        }
            FallBack "Diffuse"
}
