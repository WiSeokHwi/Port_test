Shader "Custom/ConeRangeShader"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" { }
        _Color ("Cone Color", Color) = (1, 1, 1, 1)
        _Radius ("Cone Radius", Float) = 5
        _Angle ("Cone Angle", Float) = 60
        _Height ("Cone Height", Float) = 1
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
            };

            struct v2f
            {
                float4 pos : POSITION;
                float3 normal : NORMAL;
            };

            float _Radius;
            float _Angle;
            float _Height;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = v.vertex;

                // 원뿔의 각도와 반지름을 사용하여 내부의 점들을 계산합니다.
                float angleStep = _Angle / 360.0;
                o.pos.x = _Radius * cos(angleStep);
                o.pos.z = _Radius * sin(angleStep);
                o.pos.y = _Height * v.vertex.y;

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                return half4(1, 0, 0, 1); // 빨간색으로 표시
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
}