Shader "Atto/OutlinePostprocessing"
{
    Properties
    {
        _MainTex("Color (RGB)", 2D) = "white" {}
        _OutlineTex("Outline", 2D) = "white" {}
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
            sampler2D _OutlineTex;
            float4 _MainTex_ST;  // Declarar _MainTex_ST

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 outline = tex2D(_OutlineTex, i.uv);
                return col * outline;
            }
            ENDCG
        }
    }
        FallBack "Diffuse"
}
