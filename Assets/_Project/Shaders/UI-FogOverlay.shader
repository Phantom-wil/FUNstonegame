Shader "PasserCard/UI/FogOverlay"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Fog Color", Color) = (0.55, 0.62, 0.68, 0.35)
        _Intensity ("Intensity", Range(0, 1)) = 0.5
        _ScrollSpeed ("Scroll Speed", Float) = 0.06
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

            fixed4 _Color;
            float _Intensity;
            float _ScrollSpeed;

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

            fixed4 frag(v2f i) : SV_Target
            {
                float t = _Time.y;
                float2 uv = i.uv;
                float n = hash21(floor((uv + float2(t * _ScrollSpeed, -t * _ScrollSpeed * 0.5)) * 24.0));
                float n2 = hash21(floor((uv * 1.7 - float2(t * 0.02, t * 0.04)) * 18.0));

                float vertical = smoothstep(0.15, 0.85, uv.y);
                float alpha = (n * 0.55 + n2 * 0.45) * vertical * _Intensity * _Color.a;
                return fixed4(_Color.rgb, alpha) * i.color;
            }
            ENDCG
        }
    }
}
