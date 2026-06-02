Shader "PasserCard/UI/TableBackdrop"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (0.06, 0.09, 0.11, 1)
        _AccentColor ("Accent Color", Color) = (0.22, 0.28, 0.32, 1)
        _NoiseScale ("Noise Scale", Float) = 18
        _ScrollSpeed ("Scroll Speed", Float) = 0.08
        _VignettePower ("Vignette Power", Float) = 1.35
        _PulseSpeed ("Pulse Speed", Float) = 0.6
        _Shimmer ("Shimmer", Float) = 0
        _CrackPulse ("Crack Pulse", Float) = 0
        _TimeOffset ("Time Offset", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            fixed4 _BaseColor;
            fixed4 _AccentColor;
            float _NoiseScale;
            float _ScrollSpeed;
            float _VignettePower;
            float _PulseSpeed;
            float _Shimmer;
            float _CrackPulse;
            float _TimeOffset;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            float hash21(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float a = hash21(i);
                float b = hash21(i + float2(1, 0));
                float c = hash21(i + float2(0, 1));
                float d = hash21(i + float2(1, 1));
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float t = _Time.y + _TimeOffset;
                float2 uv = i.uv;

                float n = noise(uv * _NoiseScale + float2(t * _ScrollSpeed, -t * _ScrollSpeed * 0.7));
                float n2 = noise(uv * (_NoiseScale * 1.7) - float2(t * 0.03, t * 0.05));

                float3 col = lerp(_BaseColor.rgb, _AccentColor.rgb, n * 0.22 + n2 * 0.08);

                if (_Shimmer > 0.5)
                {
                    float shimmer = sin((uv.y + t * 0.35) * 40.0) * 0.5 + 0.5;
                    col += _AccentColor.rgb * shimmer * 0.08;
                }

                if (_CrackPulse > 0.5)
                {
                    float crack = abs(sin(uv.x * 22.0 + t * _PulseSpeed * 2.0) * sin(uv.y * 18.0 - t * 1.3));
                    col += _AccentColor.rgb * crack * 0.12;
                }

                float pulse = sin(t * _PulseSpeed) * 0.5 + 0.5;
                col = lerp(col, col * (0.92 + pulse * 0.08), 0.35);

                float2 c = uv - 0.5;
                float vig = 1.0 - dot(c, c) * _VignettePower;
                col *= saturate(vig);

                return fixed4(col, 1) * i.color;
            }
            ENDCG
        }
    }
}
