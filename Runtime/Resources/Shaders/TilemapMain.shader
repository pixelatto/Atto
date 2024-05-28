Shader "Atto/TilemapMain"
{
    Properties
    {
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _MainTex("Main Texture", 2D) = "white" {}
        _VariantsMain("Green Channel", 2D) = "white" {}
        _VariantsSec("Blue Channel", 2D) = "white" {}
        _VariantsThird("Red Channel", 2D) = "white" {}
        _NoiseTex("Noise Texture", 2D) = "white" {}
    }
        SubShader
    {
        Tags { "RenderType" = "Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

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
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _VariantsMain;
            sampler2D _VariantsSec;
            sampler2D _VariantsThird;
            sampler2D _NoiseTex;
            float4 _OutlineColor;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float3 worldPos = i.worldPos;

                float noiseValue = tex2D(_NoiseTex, worldPos.xy/256).r;
                int spriteIndex = (int)(noiseValue * 16); // Asume que hay 16 sprites (4x4 en una textura 32x32)
                uint spriteX = spriteIndex % (uint)4;
                uint spriteY = spriteIndex / (uint)4;
                float2 spriteUV = frac(worldPos.xy) + float2(spriteX, spriteY);

                fixed4 texMain = tex2D(_VariantsMain, spriteUV / 4);
                fixed4 texSec = tex2D(_VariantsSec, spriteUV / 4);
                fixed4 texThird = tex2D(_VariantsThird, spriteUV / 4);

                fixed4 result;

                result.rgb = col.g * texMain.rgb + col.b * texSec.rgb + col.r * texThird.rgb;
                result.a = col.a;

                return result;
            }

            ENDCG
        }
    }
        FallBack "Diffuse"
}
