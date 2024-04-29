Shader "Atto/NoisePostprocessing"
{
    Properties
    {
        _MainTex("Base (RGB)", 2D) = "white" {}
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _ScreenWidth("Screen Width", float) = 128
        _ScreenHeight("Screen Height", float) = 72
        _NoiseAmount("Noise Amount", float) = 0.5
        _NoiseSpeed("Noise Speed", float) = 0.1
        _NoiseScale("Noise Scale", float) = 1
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float _ScreenWidth;
            float _ScreenHeight;
            float _NoiseAmount;
            float _NoiseSpeed;
            float _NoiseScale;

            fixed4 frag(v2f_img i) : SV_Target
            {
                float2 screenSize = float2(_ScreenWidth, _ScreenHeight);
                float2 gridCoord = floor(i.uv * screenSize) / screenSize;

                float4 noise = tex2D(_MainTex, i.uv) - tex2D(_NoiseTex, gridCoord * _NoiseScale + _Time * _NoiseSpeed * float2(1, 0))  * _NoiseAmount;
                return noise;
            }
        ENDCG
    }
    }
        FallBack "Diffuse"
}
