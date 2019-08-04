Shader "Andrew/FogOfWar"
{
    Properties
    {
		_Color ("Color", Color) = (1,1,1,1)
		_VisionRadius ("Vision Radius", Float) = 5.0
		_Radius ("Radius", Float) = 6.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        LOD 100

        Pass
		{
			ZWrite Off
			ZTest Always
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct vertexInput {
				float4 vertex : POSITION;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float3 worldPos : TEXCOORD0;
			};

			v2f vert(vertexInput i) {
				v2f o;
				o.vertex = UnityObjectToClipPos(i.vertex);
				o.worldPos = mul(unity_ObjectToWorld, i.vertex);
				return o;
			}

			#define POINTS_SIZE 1000

			fixed4 _Color;
			float _VisionRadius;
			float _Radius;
			// Points are represented as x and z.
			float _Points[POINTS_SIZE];
			int _Index = 0;

            fixed4 frag (v2f input) : SV_Target
            {
				// Find the closest point to the current one.
				float min_distance = 1.#INF;
				for (uint i = 0; i < _Index; i += 2) {
					float cur_distance = distance(float3(_Points[i], 0.0, _Points[i + 1]), input.worldPos);
					if (cur_distance < min_distance)
						min_distance = cur_distance;
				}
				
				// If distance is very small -> return.
				if (min_distance < _VisionRadius)
					discard;
				// If distance is medium lerp the color.    
				half f = (min_distance - _Radius) / (_Radius - _VisionRadius);
				
				return lerp(fixed4(_Color.xyz, 0.0), _Color, f);
            }

            ENDCG
        }
    }
}
