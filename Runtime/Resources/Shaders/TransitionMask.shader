Shader "Atto/TransitionMask"
{
    Properties
    {
        _MaskTex("Mask Texture", 2D) = "white" {}
        _Threshold("Threshold", Range(0, 1)) = 0
        _Reverse("Reverse", Range(0, 1)) = 0
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" }
            LOD 200

            Pass
            {
                Blend SrcAlpha OneMinusSrcAlpha
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    float2 texcoord : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                sampler2D _MaskTex;
                float _Threshold;
                fixed _Reverse;

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.texcoord;
                    return o;
                }

                half4 frag(v2f i) : SV_Target
                {
                    //half4 mainColor = tex2D(_MainTex, i.uv);
                    half4 mainColor = 0;
                    half maskValue = tex2D(_MaskTex, i.uv).r;
                    mainColor.a = maskValue + (_Threshold - 0.5) * 2;
                    mainColor.a = mainColor.a * (1 - _Reverse) + (1 - mainColor.a) * _Reverse;

                    return mainColor;
                }
                ENDCG
            }
        }
            FallBack "Transparent/VertexLit"
}
