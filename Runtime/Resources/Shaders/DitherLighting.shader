Shader "Atto/DitherLighting"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _NoiseTex("Noise", 2D) = "white" {}
        _LightSteps("Light Steps", Int) = 4
        _PixelsWide("Pixels Wide", Int) = 8
        _PixelsHigh("Pixels High", Int) = 8
        _Color("Color", Color) = (1,1,1,1)
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
        float _LightRadius[10];
        float _LightLuminosity[10];
        int _PixelsWide;
        int _PixelsHigh;
        int _LightSteps;
        float4 _Color;

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

            for (int j = 0; j < 10; j++)
            {
                float2 lightPos = _LightPoints[j].xy;
                float dist = distance(i.worldPos.xy, lightPos);
                float value = _LightRadius[j] - dist;
                float light = clamp(value / _LightRadius[j], 0, 1);
                exposure += light * _LightLuminosity[j];
            }

            float lightness = ceil(exposure * _LightSteps) / (_LightSteps);
            float darkness = 1 - lightness;
            float frac = (lightness - exposure);

            fixed4 dither = frac * round(tex2D(_NoiseTex, i.worldPos.xy / 2));

            col.rgb = _Color;
            col.a = clamp(ceil((darkness + dither) * _LightSteps) / _LightSteps, 0, 1);
            return col;
        }
        ENDCG
        }
    }
    FallBack "Diffuse"
}