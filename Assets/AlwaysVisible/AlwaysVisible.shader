Shader "Andrew/Visible/AlwaysVisible"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0

		_VisibilityColor ("Visibility Color", Color) = (1,1,1,1)
		_OutlineScaleFactor ("Outline Scale Factor", Range(1, 1.2)) = 0.02
		_OutlineColor ("Outline Color", Color) = (1,1,1,1)
    }
    SubShader
	{
		Pass
		{
			ZTest Always
			ZWrite Off
			Lighting Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			fixed4 _VisibilityColor;
			float _OutlineScaleFactor;
			fixed4 _OutlineColor;

			float4 vert(float4 vertex : POSITION) : SV_POSITION 
			{
				return UnityObjectToClipPos(vertex * _OutlineScaleFactor);
			}

			fixed4 frag(float4 i : SV_POSITION) : SV_Target
			{
				return _OutlineColor;
			}
			ENDCG
		}

		Pass
		{
			ZTest Always
			ZWrite Off
			Lighting Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
			};

			fixed4 _VisibilityColor;

			v2f vert(appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return _VisibilityColor;
			}
			ENDCG
		}


		Tags { "Queue" = "Transparent" }
		LOD 200

		ZTest LEqual
		ZWrite On

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
    FallBack "Diffuse"
}
