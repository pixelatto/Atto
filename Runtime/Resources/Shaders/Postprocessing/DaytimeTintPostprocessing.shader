Shader "Atto/DaytimeTintPostprocessing"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _ColorRamp("Color Ramp", 2D) = "white" {}
        _Daytime("DayTime", Float) = 0.5
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
                sampler2D _ColorRamp;
                float _Daytime;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv); // Sample the texture color
                    fixed4 colorRamp = tex2D(_ColorRamp, float2(_Daytime, 0.5)); // Use _Value to sample from the middle of the ramp texture
                    return col * colorRamp; // Modulate texture color with color ramp based on _Value
                }
                ENDCG
            }
        }
            FallBack "Diffuse"
}
