Shader "Atto/GaussianBlurPostprocessing"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _BlurSize("Blur Size", Float) = 1.0
        _TextureSize("Texture Size", Vector) = (1,1,0,0)
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
            float _BlurSize;
            float4 _TextureSize;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float4 col = 0;
                float2 offset = float2(_BlurSize, _BlurSize) / _TextureSize.xy;

                // Sample the surrounding pixels
                for (int x = -2; x <= 2; x++)
                {
                    for (int y = -2; y <= 2; y++)
                    {
                        col += tex2D(_MainTex, uv + float2(x, y) * offset) * 1.0 / 25.0; // simple box blur
                    }
                }

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
