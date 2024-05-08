Shader "Atto/MotionBlurPostprocessing"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _CameraMotionVectorsTexture("Motion Vectors", 2D) = "white" {}
        _BlurAmount("Blur Amount", Range(0.0, 0.92)) = 0.5
    }
        SubShader
        {
            // No culling or depth
            Cull Off ZWrite Off ZTest Always

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
                sampler2D _CameraMotionVectorsTexture;
                float _BlurAmount;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    float2 uv = i.uv;
                    fixed4 col = tex2D(_MainTex, uv);
                    float2 motion = tex2D(_CameraMotionVectorsTexture, uv).xy;
                    motion *= _BlurAmount;

                    if (length(motion) > 0.01) // ajustar la sensibilidad según sea necesario
                    {
                        int numSamples = 10;
                        for (int j = 0; j < numSamples; j++)
                        {
                            float p = (j / (float)(numSamples - 1) - 0.5) * 2.0;
                            col += tex2D(_MainTex, uv + motion * p);
                        }
                        col /= numSamples;
                    }

                    return col;
                }
                ENDCG
            }
        }
        Fallback "Diffuse"
}
