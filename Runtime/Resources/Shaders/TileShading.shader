Shader "Custom/TileShading"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _LightPoints("Light Points", Vector) = (0,0,0,0)
        _LightRadius("Light Radius", Float) = 0.1
        _Luminosity("Luminosity", Float) = 400
        _LightSteps("Light Steps", Int) = 4
        _PixelsWide("Pixels Wide", Int) = 32
        _PixelsHigh("Pixels High", Int) = 32
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
            LOD 100
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            Pass
            {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

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
            float4 _MainTex_ST;
            float4 _LightPoints[10];
            float _LightRadius;
            float _Luminosity;
            int _PixelsWide;
            int _PixelsHigh;
            int _LightSteps;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Calculando el tamaño de cada pixel grande en el mundo
                float2 pixelSize = float2(1.0 / _PixelsWide, 1.0 / _PixelsHigh);

                // Coordenadas del mundo ajustadas para la pixelación
                i.worldPos.xy = floor(i.worldPos.xy / pixelSize) * pixelSize;

                fixed4 col = tex2D(_MainTex, i.uv);
                float visibility = 0.0;

                for (int j = 0; j < 10; j++)
                {
                    float2 lightPos = _LightPoints[j].xy;
                    float dist = distance(i.worldPos.xy, lightPos);
                    float value = _LightRadius - dist;
                    float light = clamp(value / _LightRadius, 0, 1);
                    visibility += light * _Luminosity;
                }

                float lightIntensity = ceil(visibility * _LightSteps) / (_LightSteps);

                col.rgb = 0;
                col.a = clamp(1 - lightIntensity, 0, 1);
                return col;
            }
            ENDCG
            }
        }
            FallBack "Diffuse"
}
