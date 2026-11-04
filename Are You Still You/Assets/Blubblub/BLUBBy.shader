Shader "Custom/FisheyeShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Distortion ("Distortion (-2.0 to 2.0)", Float) = -1.0
        _Aspect ("Aspect Ratio (Width/Height)", Float) = 1.777
        _Scale ("Zoom/Scale", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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
            float _Distortion;
            float _Aspect;
            float _Scale;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            };

            float2 Distort(float2 p)
            {
                float d = length(p);
                // The core Godot math translated to HLSL syntax
                float z = sqrt(abs(_Distortion + d * d * -_Distortion));
                float r = atan2(d, z) / 3.1415926535;
                float phi = atan2(p.y, p.x);
                
                float2 uv;
                uv.x = r * cos(phi) * (1.0 / _Aspect) + 0.5;
                uv.y = r * sin(phi) + 0.5;
                return uv;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Shift UV center origin to screen middle (-1 to 1)
                float2 xy = i.uv * 2.0 - 1.0;
                
                // Adjust for aspect ratio and custom scale factor
                xy.x *= _Aspect;
                xy *= _Scale;

                // Warp the coordinates
                float2 distortedUV = Distort(xy);

                // Clip textures out-of-bounds to keep things clean
                if (distortedUV.x < 0.0 || distortedUV.x > 1.0 || distortedUV.y < 0.0 || distortedUV.y > 1.0)
                {
                    return fixed4(0,0,0,1); // Black edges
                }

                return tex2D(_MainTex, distortedUV);
            }
            ENDCG
        }
    }
}
