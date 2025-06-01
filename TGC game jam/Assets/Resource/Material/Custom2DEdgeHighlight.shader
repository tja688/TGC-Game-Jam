Shader "Custom/2DEdgeHighlight"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] _OutlineEnabled ("Enable Outline", Float) = 1
        _OutlineColor ("Outline Color", Color) = (1,0,0,1)
        _OutlineThickness ("Outline Thickness", Range(0, 0.1)) = 0.01
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1) // For sprite flipping
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
        Blend One OneMinusSrcAlpha // Normal alpha blending

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing // For batching
            #pragma multi_compile _ PIXELSNAP_ON // For pixel snap
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA // For ETC1 support with external alpha

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID // For instancing
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float2 screenPos : TEXCOORD1; // For outline calculation based on screen space
                UNITY_VERTEX_OUTPUT_STEREO // For stereo rendering
            };

            fixed4 _Color;
            fixed4 _OutlineColor;
            float _OutlineThickness;
            float _OutlineEnabled;
            float4 _MainTex_TexelSize; // Contains 1/width, 1/height, width, height

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(IN); // For instancing
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT); // For stereo rendering

                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color; // Apply tint
                OUT.screenPos = ComputeScreenPos(OUT.vertex).xy / ComputeScreenPos(OUT.vertex).w; // Normalized screen coordinates
                return OUT;
            }

            sampler2D _MainTex;
            sampler2D _AlphaTex; // For ETC1 external alpha
            float _EnableExternalAlpha; // For ETC1 external alpha

            fixed4 SampleSpriteTexture(float2 uv)
            {
                fixed4 color = tex2D(_MainTex, uv);

#if ETC1_EXTERNAL_ALPHA
                // If material has "_AlphaTex" property defined, use it as an alpha channel.
                // Used by ETC1 textures with external alpha.
                color.a = tex2D(_AlphaTex, uv).r;
#endif //ETC1_EXTERNAL_ALPHA

                return color;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 mainColor = SampleSpriteTexture(IN.texcoord) * IN.color;

                if (_OutlineEnabled > 0.5)
                {
                    // Calculate outline based on alpha difference in neighboring pixels
                    float2 texelSize = _MainTex_TexelSize.xy; // 1/width, 1/height
                    float outlineAlpha = 0.0;
                    float currentAlpha = mainColor.a;

                    // Sample surrounding pixels
                    // More samples give smoother but more expensive outlines
                    // This is a simple cross sampling, you can expand to a box or circle for better quality
                    float2 offsets[4] =
                    {
                        float2(-texelSize.x * _OutlineThickness * 100, 0), // Left (multiplied by 100 to make thickness more intuitive)
                        float2(texelSize.x * _OutlineThickness * 100, 0),  // Right
                        float2(0, -texelSize.y * _OutlineThickness * 100), // Bottom
                        float2(0, texelSize.y * _OutlineThickness * 100)   // Top
                    };

                    // A more robust way to detect edges is to check the alpha of neighboring pixels.
                    // If the current pixel is opaque and a neighbor is transparent (or less opaque), it's an edge.
                    for (int i = 0; i < 4; i++)
                    {
                        float2 sampleCoord = IN.texcoord + offsets[i];
                        float neighborAlpha = SampleSpriteTexture(sampleCoord).a;
                        // If current pixel has alpha and neighbor has less or no alpha, it's an edge towards the neighbor
                        if (currentAlpha > 0.01 && neighborAlpha < currentAlpha * 0.5) {
                             outlineAlpha = 1.0;
                             break;
                        }
                        // Alternative: if current pixel is transparent and neighbor is opaque, it's also an edge (inner outline for holes)
                        // if (currentAlpha < 0.5 && neighborAlpha > 0.5) {
                        //    outlineAlpha = 1.0;
                        //    break;
                        // }
                    }


                    // If the current pixel itself is almost transparent, but an opaque pixel is nearby,
                    // it means this pixel should be part of the outline.
                    if (currentAlpha < 0.1) // If the current pixel is transparent
                    {
                        float maxNeighborAlpha = 0.0;
                        for (int j = 0; j < 4; j++)
                        {
                             maxNeighborAlpha = max(maxNeighborAlpha, SampleSpriteTexture(IN.texcoord + offsets[j] * 2.0).a); // Sample a bit further for transparent pixels
                        }
                         if (maxNeighborAlpha > 0.5) // If there's an opaque neighbor
                         {
                            // This pixel should be outline color
                            return fixed4(_OutlineColor.rgb, _OutlineColor.a * IN.color.a);
                         }
                         else
                         {
                            // Otherwise, it's just transparent
                            return fixed4(0,0,0,0);
                         }
                    }
                    else
                    {
                        // If current pixel is opaque, check if it's an edge pixel
                        float isEdge = 0.0;
                        for (int k = 0; k < 4; k++)
                        {
                            if (SampleSpriteTexture(IN.texcoord + offsets[k]).a < 0.1) // If any neighbor is transparent
                            {
                                isEdge = 1.0;
                                break;
                            }
                        }
                        if (isEdge > 0.5)
                        {
                             return fixed4(_OutlineColor.rgb, _OutlineColor.a * mainColor.a); // Draw outline, preserve original alpha for blending with outline
                        }
                    }
                }
                return mainColor; // Return original color if no outline or not an edge
            }
            ENDCG
        }
    }    
}