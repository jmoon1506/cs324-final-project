// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/CircleHyperbolicOccluder"
{
	Properties
	{
		_Color ("Color", Color) = (1,0,0,1)
		_Radius("Radius", Range(0.0,0.5)) = 0.4
		_Smoothness("Smoothness", Range(0.001, 1)) = 0.1
		_AlphaCutoff("Alpha Cutoff", Range(0, 0.5)) = 0.01
	}
	SubShader
	{
		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			Stencil
            {
                Ref 4
                Comp Always
                Pass Replace
            }
		CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			fixed4 _Color;
			float _Radius;
			float _Smoothness;
			float _AlphaCutoff;

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

			// r = radius, d = distance
			float antialias (float r, float d) {
				if (d > r) {
					float dist = 0.5 - r;
					float factor = (-1.0 + sqrt(1 + 4 * _Smoothness)) / 2.0;
					float func = _Smoothness / ((d-r) / dist + factor) - factor;
					return clamp(func, 0.0, 1.0);
				}
				else {
					return 1.0;
				}
			}

			fixed4 frag (fragmentInput i) : SV_Target {
				float distance = sqrt(pow(i.uv.x, 2)  + pow(i.uv.y, 2));
				fixed4 c = fixed4(_Color.r, _Color.g, _Color.b, 
					_Color.a*antialias(_Radius, distance));
				clip(c.a - _AlphaCutoff);
				return c;
			}
		ENDCG
		}
	}
}
