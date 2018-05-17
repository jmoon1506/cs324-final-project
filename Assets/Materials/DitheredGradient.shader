// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/DitheredGradient"
{
	Properties
	{
		_Color ("Color", Color) = (1,0,0,1)
		_Radius("Radius", Range(0.0,0.5)) = 0.4
		_Smoothness("Smoothness", Range(0.001, 1)) = 0.1
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
			float _Smoothness;

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

			float hash12(float2 p)
			{
				float3 p3  = frac(float3(p.x, p.y, p.x) * float3(443.8975, 397.2973, 491.1871));
				p3 += dot(p3, p3.yzx + float3(19.19, 19.19, 19.19));
				return frac((p3.x + p3.y) * p3.z);
			}

			fixed4 frag (fragmentInput i) : SV_Target {
				float distance = sqrt(pow(i.uv.x, 2)  + pow(i.uv.y, 2));
				float alpha = antialias(_Radius, distance);
				alpha += hash12(i.uv) / 255.0;

				return fixed4(_Color.r, _Color.g, _Color.b, 
					_Color.a*alpha);
			}
			ENDCG
		}
	}
}
