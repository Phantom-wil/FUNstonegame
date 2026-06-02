Shader "PasserCard/UI/TablePlayArea"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (0.12, 0.16, 0.19, 0.55)
        _AccentColor ("Accent", Color) = (0.22, 0.28, 0.32, 1)
        _Softness ("Edge Softness", Float) = 0.18
        _Pulse ("Pulse", Float) = 0
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
            fixed4 _AccentColor;
            float _Softness;
            float _Pulse;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 p = (i.uv - 0.5) * float2(1.0, 1.35);
                float dist = dot(p, p);
                float alpha = 1.0 - smoothstep(0.12, 0.12 + _Softness, dist);
                alpha *= _Color.a * (0.85 + _Pulse * 0.15);

                float3 col = lerp(_Color.rgb, _AccentColor.rgb, 0.25 + _Pulse * 0.2);
                return fixed4(col, alpha) * i.color;
            }
            ENDCG
        }
    }
}
