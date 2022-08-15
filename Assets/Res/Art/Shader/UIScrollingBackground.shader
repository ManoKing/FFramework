Shader "Unlit/UIScrollingBackground"
{
    Properties
    {
        _MainTex("BaseTexture(RGB)", 2D) = "white" {}
        _ScrollXBaseSpeed("BaseSpeed", Float) = 1.0
    }
        SubShader
        {
            //如果是UI就需要下面这些东西
            Tags
            {
                "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "PreviewType" = "Plane"
                "CanUseSpriteAtlas" = "True"
            }

            Lighting Off
            ZWrite Off
            ZTest[unity_GUIZTestMode]
            Blend SrcAlpha OneMinusSrcAlpha
            //到此结束   如果不是UI可以删掉这段
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
                float4 _MainTex_ST;

                float _ScrollXBaseSpeed;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex) + frac(float2(_ScrollXBaseSpeed, 0) * _Time.y);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {

                    // sample the texture
                    fixed4 col = tex2D(_MainTex, i.uv);

                    return col;
                }
                ENDCG
            }
        }
}

