Shader "Atto/PalettePostprocessing"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _LUT("Lookup Texture", 2D) = "white" {}
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
            sampler2D _LUT;
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float4 texColor = tex2D(_MainTex, i.uv);

                float blueIndex = floor(texColor.b * 32) / 32;
                float3 mappedColor = tex2D(_LUT, float2(blueIndex + texColor.g/32, texColor.r)).rgb;
                return half4(mappedColor, texColor.a);
            }
            ENDCG
        }
    }
        FallBack "Diffuse"
}
