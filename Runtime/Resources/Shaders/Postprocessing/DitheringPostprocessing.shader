Shader "Atto/DitheringPostprocessing"
{
    Properties
    {
        _MainTex("Base (RGB)", 2D) = "white" {}
        _TileTex("Tile Texture", 2D) = "white" {}
        _DitherTileSize("Dither Tile Size", float) = 4
        _DitheringLevels("Dithering Levels", float) = 8
        _LightLevels("Light Levels", float) = 4
        _ScreenWidth("Screen Width", float) = 128
        _ScreenHeight("Screen Height", float) = 72
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
            sampler2D _TileTex;
            float _DitherTileSize;
            float _DitheringLevels;
            float _LightLevels;
            float _ScreenWidth;
            float _ScreenHeight;

            fixed4 frag(v2f_img i) : SV_Target
            {
                float lightValue = clamp(tex2D(_MainTex, i.uv).r, 0.0001, 0.9999);

                float stepStart = floor(lightValue * _LightLevels) / _LightLevels;
                float stepEnd = stepStart + 1 / _LightLevels;
                float stepSize = stepEnd - stepStart;

                float stepLight = (lightValue - stepStart) / stepSize;

                float ditherAmount = floor(stepLight * _DitheringLevels) / (_DitheringLevels);
                float texelSize = float2(_DitherTileSize, _DitherTileSize);
                float2 screenSize = float2(_ScreenWidth, _ScreenHeight) / texelSize;
                float2 gridCoord = floor(i.uv * screenSize) / screenSize;
                float2 gridCoordFrac = frac(i.uv * screenSize);
                float2 baseUV = float2(ditherAmount, 0);
                fixed4 tileColor = tex2D(_TileTex, baseUV + gridCoordFrac);
                fixed4 color = stepStart + tex2D(_TileTex, baseUV + gridCoordFrac / float2(_DitheringLevels, 1)) * 1 / _LightLevels;

                return color;
            }
        ENDCG
    }
    }
        FallBack "Diffuse"
}
