Shader "Atto/DitherLighting"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _NoiseTex("Noise", 2D) = "white" {}
        _LightSteps("Light Steps", Int) = 4
        _PixelsWide("Pixels Wide", Int) = 8
        _PixelsHigh("Pixels High", Int) = 8
        _FillColor("Fill Color", Color) = (1,1,1,1)
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
        sampler2D _NoiseTex;
        float4 _MainTex_ST;
        float4 _LightPoints[10];
        float _LightBrightness[10];
        float _LightRadius[10];
        float4 _Colors[10];
        float4 _FillColor;
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
            float2 pixelSize = float2(1.0 / _PixelsWide, 1.0 / _PixelsHigh);
            i.worldPos.xy = floor(i.worldPos.xy / pixelSize) * pixelSize;

            fixed4 col = tex2D(_MainTex, i.uv);
            float exposure = 0.0;
            float4 color = float4(1, 1, 1, 1);

            for (int j = 0; j < 10; j++)
            {
                float2 lightPos = _LightPoints[j].xy;
                float distToCenterNormalized = distance(i.worldPos.xy, lightPos) / _LightRadius[j];
                float distToBorder = (1 - distToCenterNormalized);
                
                float light = clamp(distToBorder, 0, 1);
                float currentExposure = light * _LightBrightness[j];
                exposure = max(currentExposure, exposure);
                if (currentExposure > 0)
                {
                    color += light * _LightBrightness[j] * _Colors[j];
                }
            }
            color /= exposure;

            float lightness = ceil(exposure * _LightSteps) / (_LightSteps);
            float darkness = 1 - lightness;
            float frac = (lightness - exposure);

            fixed4 dither = frac * round(tex2D(_NoiseTex, i.worldPos.xy / 2));

            col.a = clamp(ceil((darkness + dither) * _LightSteps) / _LightSteps, 0, 1);
            col.rgb = _FillColor;

            return col;
        }
        ENDCG
        }
    }
    FallBack "Diffuse"
}
