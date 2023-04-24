// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Egged/Unlit/Background"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color1 ("Color1", Color) = (.25, .5, .5, 1)
		_Color2 ("Color2", Color) = (.25, .5, .5, 1)
	}
	SubShader
	{
		Tags
		{
			"RenderType"="Background"
			"PreviewType"="Skybox"
		}
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 scrPos : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color1;
			float4 _Color2;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.scrPos = o.vertex;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float2 screenPosition = (i.scrPos.xy/i.scrPos.w) * _ScreenParams.xy + _ScreenParams.xy;
				fixed4 col = _Color1;
				if (screenPosition.x % 8 < 4 == screenPosition.y % 8 < 4)
				{
					col = _Color2;
				}
				return col;
			}
			ENDCG
		}
	}
}
