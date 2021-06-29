// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Swirls"
{
    Properties 
    {
        [NoScaleOffset] _MainTex    ("Texture", 2D) = "white" {}
        _Loc        ("Location", Vector) = (0, 0, 0, 0)
        _Frequency  ("Frequency", Float) = 0.2
        _MinRadius ("Min Radius", Float) = 11.0
        _MaxRadius ("Max Radius", Float) = 150.0
        _SwirlAngle ("Swirl Angle [deg]", Range(0, 180)) = 60
    }
    SubShader {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            // Compile with debug symbols; remove for release.
            //#pragma enable_d3d11_debug_symbols 

            #include "UnityCG.cginc"

            #define TAU 6.28318530f
            #define PI 3.14159265f

            struct Meshdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv :     TEXCOORD0;
                float3 wpos :   TEXCOORD1; // world position
                float3 scrpos : TEXCOORD2; // screen position
            };

            sampler2D _MainTex;
            float3  _Loc;
            float   _Frequency;
            float   _MinRadius;
            float   _MaxRadius;
            float   _SwirlAngle; // Degrees

            float2 rotate(float2 coords, float angRad)
            {
                float ca = cos(angRad);
                float sa = sin(angRad);
                float2x2 rot2d = { ca, -sa, 
                                   sa,  ca };
                return mul(rot2d, coords);
            }


            //Compute the parameter t that gives the value v in a linear interpolation between a and b.
            inline float inverse_lerp(float a, float b, float v)
            {
                return (v - a) / (b - a);
            }


            v2f vert (Meshdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.wpos = mul(unity_ObjectToWorld, v.vertex);
                o.scrpos = ComputeScreenPos(o.vertex);
                return o;
            }


            fixed4 frag (v2f i) : SV_Target
            {
                //fixed4 colOut;
                float2 coords = i.scrpos * _ScreenParams - _Loc;
                const float len = length(coords.xy);
                const float swirlAngle = _SwirlAngle * PI / 180.0f;

                // swirl effect with frequency shift and blending
                // interpolation parameter
                float t = inverse_lerp(_MinRadius, _MaxRadius, len);
                // swirls                    
                float angRot =  lerp(swirlAngle, 0, t);
                const float2 uvCenter = _Loc / _ScreenParams;
                float2 uvOffset = i.uv - uvCenter;
                float2 rotUv = rotate(uvOffset, angRot);

                // rotate the area between inner and outer radius as a function of time
                float omegaAng = TAU * _Frequency * _Time.y;
                rotUv = rotate(rotUv, omegaAng) + uvCenter;
                float4 colA = tex2D(_MainTex, rotUv);
                
                // frequency-shift the distorted color toward red
                colA *= float4(1.0f, 0.5f, 0.3f, 1.0) * (1.0f - t);
                float4 colB = tex2D(_MainTex, i.uv);

                //// pick the inside color based on the distance from the middle
                // this part implements Anti-Aliasing for the inner radius
                // calculate partial derivatives of the change over one pixel
                float h_pd_len = 0.5f * fwidth(len); // 1/2 change over one pixel
                float tpd = inverse_lerp(
                    _MinRadius - h_pd_len,
                    _MinRadius + h_pd_len,
                    len
                );
                // the line below is clamping to [0, 1]
                float t_step = saturate(tpd);
                // without AA
                // float t_step = step(_MinRadius, len);

                float4 colInside = (1.0f - t_step)*float4(0, 0, 0, 1) + 
                    t_step * lerp(colA, colB, (t*t) );

                // pick the color for the outer area of the effect
                float4 colOutside = tex2D(_MainTex, i.uv);
                float t_step2 = step(len, _MaxRadius);
                return t_step2 * colInside + (1 - t_step2) * colOutside;
            }

            ENDCG
        }
    }
}
