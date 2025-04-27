Shader "Custom/ConeRangeShader"
{
    Properties
    {
        _Color ("Color", Color) = (1, 0, 0, 1)
        _Radius ("Radius", Float) = 5.0
        _Angle ("Angle", Float) = 60.0
        _Height ("Height", Float) = 0.1
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
            };

            float _Radius;
            float _Angle;
            float _Height;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = v.vertex;

                // 원뿔 범위의 각도와 반지름 계산
                float angle = atan2(v.vertex.x, v.vertex.z);
                if (abs(angle) > _Angle)
                    o.pos.z = _Height; // 각도를 넘으면 높이를 낮추어 원뿔 안으로 제한

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                return half4(1, 0, 0, 1); // 빨간색으로 원뿔 표시
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}