Shader "Hidden/PixelizePostprocessing"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Width("Width", Int) = 128
        _Height("Height", Int) = 72
    }
        SubShader
        {
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
                float4 _MainTex_TexelSize;
                int _Width;
                int _Height;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    float2 pixelSize = float2(_Width, _Height);
                    float2 pixelUv = floor(i.uv * pixelSize) / pixelSize + 0.5f / pixelSize;
                    return tex2D(_MainTex, pixelUv);
                }
                ENDCG
            }
        }
            FallBack "Diffuse"
}
