Shader "VStar/Unlit/ScreenClipTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScreenClip("ScreenClip", Vector) = (-1, -1, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct v2f 
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            v2f vert (appdata_base v)
            {
                v2f o;
                o.uv = v.texcoord;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos (o.pos);
                return o;
            }

            sampler2D _MainTex;
			fixed4 _ScreenClip;
            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 screenPos = i.screenPos.xy / i.screenPos.w;

                if(_ScreenClip.x > -1 && _ScreenClip.y > -1)
                {
                    clip(screenPos.x - _ScreenClip.x); //左
                    clip(_ScreenClip.z - screenPos.x); //右
                    clip(_ScreenClip.w - screenPos.y); //上
                    clip(screenPos.y - _ScreenClip.y); //下
                }

                // i.screenPos 在 [0,1] 区间，乘以_ScreenParams就是真实的屏幕坐标了
                //screenPos.xy *= _ScreenParams.xy;

                return fixed4(screenPos.xy, 1, 1);
            }
            ENDCG
        }
    }
}
