Shader "Unlit/FogOfWar"
{
    Properties
    {
		_Color ("Color", Color) = (1,1,1,1)
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

            #include "UnityCG.cginc"

			struct vertexInput 
			{
				float4 vertex : POSITION;
			};

            struct v2f
            {
                float4 vertex : SV_POSITION;
				float3 worldPos : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (vertexInput i)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(i.vertex);
				o.worldPos = mul(unity_ObjectToWorld, i.vertex);
				return o;
            }

			fixed4 _Color;

            fixed4 frag (v2f i) : SV_Target
            {
				if (distance(float3(0, 0, 0), i.worldPos) < 1.0) {
					discard;
				}
                return _Color;
            }

            ENDCG
        }
    }
}
