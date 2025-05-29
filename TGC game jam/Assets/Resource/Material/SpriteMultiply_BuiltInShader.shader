Shader "Custom/SpriteMultiply_BuiltIn_Adjustable" // Shader名字稍作修改以区分
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture (Base RGB, Base A)", 2D) = "white" {}
        _MultiplyTex ("Multiply Texture (RGB)", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _MultiplyIntensity ("Multiply Intensity", Range(0.0, 1.0)) = 1.0 // 新增：叠底强度控制
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            sampler2D _MultiplyTex;
            fixed4 _Color;
            float _MultiplyIntensity; // 新增：叠底强度变量

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap(OUT.vertex);
                #endif
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 baseColor = tex2D(_MainTex, IN.texcoord);
                fixed4 multiplyColorTex = tex2D(_MultiplyTex, IN.texcoord); // 重命名以区分

                // 计算完全正片叠底的效果
                fixed3 fullMultiplyEffect = baseColor.rgb * multiplyColorTex.rgb;

                // 使用 _MultiplyIntensity 在原始底色和完全叠底效果之间进行插值
                // 当 _MultiplyIntensity = 0, blendedRGB = baseColor.rgb (无叠底)
                // 当 _MultiplyIntensity = 1, blendedRGB = fullMultiplyEffect (完全叠底)
                fixed3 blendedRGB = lerp(baseColor.rgb, fullMultiplyEffect, _MultiplyIntensity);

                // Alpha通道处理：这里我们假设强度控制主要针对颜色，Alpha仍然可以按之前方式处理
                // 或者，如果希望叠底强度也轻微影响Alpha（例如强度低时，叠底图的Alpha贡献也降低），可以调整
                fixed finalAlpha = baseColor.a * multiplyColorTex.a; // 保持原样，或者 baseColor.a * lerp(1.0, multiplyColorTex.a, _MultiplyIntensity)

                fixed4 finalColor = fixed4(blendedRGB, finalAlpha);
                finalColor *= IN.color; // 应用顶点颜色 (已包含材质Tint和Sprite Renderer颜色)

                return finalColor;
            }
            ENDCG
        }
    }
    Fallback "Sprites/Default"
}