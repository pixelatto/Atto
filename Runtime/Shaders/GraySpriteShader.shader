Shader "Custom/GraySpriteShader"
{
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _Threshold("Threshold", Float) = 0.5
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
                float _Threshold;

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    o.color = v.color;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    fixed4 texColor = tex2D(_MainTex, i.uv);
                    float grayScale = texColor.r * texColor.a; // Assume texture is greyscale (R=G=B)

                    // Calculate alpha based on the threshold

                    float alpha = grayScale <= _Threshold ? 1.0 : 0.0;
                    return fixed4(i.color.rgb, alpha * texColor.a);
                }
                ENDCG
            }
        }
            FallBack "Diffuse"
}
