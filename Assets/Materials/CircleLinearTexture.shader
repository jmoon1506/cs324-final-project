// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/CircleLinearTexture"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,0.5)
		_Radius("Radius", Range(0.0,0.5)) = 0.4
		_Fade("Fade", Range(0.0,0.5)) = 0.05
	}
	SubShader
	{
		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			// fixed4 _Color;
			// float _Thickness;
			float _Radius;
			float _Fade;
			fixed4 _Color;
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;

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
					return 1.0 - abs(d-r) / f;
					// return - pow(d-r+0.5*t, 2) / pow(p*t, 2) + 1.0; 
				else
					return 1.0;
			}

			fixed4 frag (fragmentInput i) : SV_Target {
				float4 texColor = tex2D(_MainTex, _MainTex_ST.xy * i.uv + fixed2(0.5,0.5) + _MainTex_ST.zw);
				float distance = sqrt(pow(i.uv.x, 2)  + pow(i.uv.y, 2));
				fixed4 c = _Color * fixed4(texColor.r, texColor.g, texColor.b, 
					texColor.a*antialias(_Radius, distance, _Fade));
				return c;
			}
			ENDCG
		}
	}
}
