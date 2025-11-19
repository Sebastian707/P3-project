Shader "UI/Blur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Range(0, 100)) = 2
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _BlurSize;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 offset = _BlurSize / _ScreenParams.xy; // pixel offset
                fixed4 col = tex2D(_MainTex, uv) * 0.36;
                col += tex2D(_MainTex, uv + float2(offset.x, 0)) * 0.16;
                col += tex2D(_MainTex, uv - float2(offset.x, 0)) * 0.16;
                col += tex2D(_MainTex, uv + float2(0, offset.y)) * 0.16;
                col += tex2D(_MainTex, uv - float2(0, offset.y)) * 0.16;
                return col;
            }
            ENDCG
        }
    }
}
