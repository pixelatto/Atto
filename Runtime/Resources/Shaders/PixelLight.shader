Shader "Atto/PixelLight"
{
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _Color("Main Color", Color) = (1,1,1,1)
        _Radius("Radius", Float) = 5.0
        _Brightness("Brightness", Float) = 1.0
    }
        SubShader
        {
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" "PreviewType" = "Plane" "CanUseSpriteAtlas" = "True" }

            LOD 200
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Back
            Lighting Off
            ZWrite Off

            Pass
            {
                CGPROGRAM

                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    float4 color : COLOR;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float3 localPos : TEXCOORD1; // Pass the local position
                    float4 color : COLOR;
                };

                sampler2D _MainTex;
                float _Radius;
                float _Brightness;
                float4 _MainTex_ST;
                fixed4 _Color;

                v2f vert(appdata_t IN)
                {
                    v2f OUT;
                    OUT.uv = IN.uv;
                    OUT.vertex = UnityObjectToClipPos(IN.vertex);
                    OUT.localPos = IN.vertex.xyz;
                    OUT.color = IN.color;
                    return OUT;
                }

                fixed4 frag(v2f IN) : SV_Target
                {
                    float darkness = length(IN.localPos / _Radius);
                    float lightness = saturate(1.0 - darkness);
                    float alpha = saturate(lightness * _Brightness);

                    fixed4 texColor = tex2D(_MainTex, IN.uv) * _Color;
                    texColor.a *= alpha;

                    return texColor;
                }
                ENDCG
            }
        }
}
