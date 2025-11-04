Shader "Custom/GrayscaleShader"
    {
        Properties
        {
            _MainTex ("Texture", 2D) = "white" {}
            _GrayscaleAmount ("Grayscale Amount", Range(0,1)) = 1 // 0 = color, 1 = grayscale
        }
        SubShader
        {
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

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
                    fixed4 color : COLOR; // For sprite renderer's tint color
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    fixed4 color : COLOR;
                };

                sampler2D _MainTex;
                float _GrayscaleAmount;

                v2f vert (appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    o.color = v.color;
                    return o;
                }

                fixed4 frag (v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv) * i.color; // Apply sprite tint
                    float grayscale = dot(col.rgb, float3(0.299, 0.587, 0.114)); // Luminosity method
                    col.rgb = lerp(col.rgb, grayscale.xxx, _GrayscaleAmount); // Blend with grayscale
                    return col;
                }
                ENDCG
            }
        }
    }