// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/CircleLinear"
{
	Properties
	{
		_Color ("Color", Color) = (1,0,0,1)
		_Radius("Radius", Range(0.0,0.5)) = 0.4
		_Fade("Fade", Range(0.0,0.5)) = 0.05
	}
	SubShader
	{
		Pass
		{
			ZWrite Off
			Lighting Off
			Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			fixed4 _Color;
			float _Radius;
			float _Fade;
			fixed _AlphaCutoff;

			struct fragmentInput {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			fragmentInput vert (appdata_base v) {
				fragmentInput o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.uv = v.texcoord.xy - fixed2(0.5,0.5);
				return o;
			}

			// r = radius, d = distance, f = fade
			float antialias (float r, float d, float f) {
				if (d > r)
					return clamp(1.0 - abs(d-r) / f, 0.0, 1.0);
					// return - pow(d-r+0.5*t, 2) / pow(p*t, 2) + 1.0; 
				else
					return 1.0;
			}

			fixed4 frag (fragmentInput i) : SV_Target {
				float distance = sqrt(pow(i.uv.x, 2)  + pow(i.uv.y, 2));
				return  fixed4(_Color.r, _Color.g, _Color.b, 
					_Color.a*antialias(_Radius, distance, _Fade));
			}
		ENDCG
		}
	}
}
