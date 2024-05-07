Shader "Atto/OutlinePostprocessing"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" }
            LOD 100

            Pass
            {
                ZTest Always Cull Off ZWrite Off
                //Blend SrcAlpha OneMinusSrcAlpha // Para manejar la transparencia

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
                    float4 vertex : SV_POSITION;
                    float2 uv : TEXCOORD0;
                };

                sampler2D _MainTex;
                float4 _OutlineColor;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                float4 frag(v2f i) : SV_Target
                {
                    float2 offsetUp    = float2(0, 1.0 / 72.0);
                    float2 offsetRight = float2(1.0 / 128.0, 0);
                    float2 offsetDown  = float2(0,-1.0 / 72.0);
                    float2 offsetLeft  = float2(-1.0 / 128.0, 0);

                    float4 center = tex2D(_MainTex, i.uv);
                    float4 up   = tex2D(_MainTex, i.uv + offsetUp);
                    float4 down = tex2D(_MainTex, i.uv + offsetDown);
                    float4 left = tex2D(_MainTex, i.uv + offsetLeft);
                    float4 right= tex2D(_MainTex, i.uv + offsetRight);

                    float4 texColor = (up + down + left + right + center);
                    float value = ceil(texColor.r + texColor.g + texColor.b);
                    
                    return 1-value;
                }
                ENDCG
            }
        }
}
