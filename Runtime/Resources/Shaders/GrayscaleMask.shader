Shader "Atto/GrayscaleMask"
{
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        [PerRendererData] _TopThreshold("Top Threshold", Float) = 0.5
        [PerRendererData] _LowThreshold("Low Threshold", Float) = 0
        _Color("Color", Color) = (1,1,1,1)
    }
        SubShader{
            Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
            LOD 100

            Blend SrcAlpha OneMinusSrcAlpha
            AlphaTest Greater 0.01
            ZWrite Off
            Cull Off

            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    float4 color : COLOR;
                };

                struct v2f {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float4 color : COLOR;
                };

                sampler2D _MainTex;
                float _TopThreshold;
                float _LowThreshold;

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    o.color = v.color;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    fixed4 texColor = tex2D(_MainTex, i.uv) - 0.1;
                    float alpha = ((texColor.r <= _TopThreshold) && (texColor.r >= _LowThreshold)) ? 1.0 : 0.0;
                    return fixed4(i.color.rgb, alpha * texColor.a);
                }
                ENDCG
            }
        }
            FallBack "Diffuse"
}


