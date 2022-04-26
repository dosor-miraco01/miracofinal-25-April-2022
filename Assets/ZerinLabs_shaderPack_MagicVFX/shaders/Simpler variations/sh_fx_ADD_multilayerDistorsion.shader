// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "zerinlabs/sh_fx_ADD_multilayerDistorsion" 
{
	Properties
	{
		_colorTint("Tint", color) = (1.0,0.5,0.0,1.0)
		_overbright("Overbright amount:", Float) = 1

		_MainTex("Base map", 2D) = "white" {}
		_Main_speedX("Base speed X:", Float) = 0.3
		_Main_speedY("Base speed Y:", Float) = 0.3

		_MaskTex("Mask map", 2D) = "white" {}
		_Mask_speedX("Mask speed X:", Float) = 0.3
		_Mask_speedY("Mask speed Y:", Float) = 0.3

		_NormalTex("Normal map", 2D) = "white" {}
		_Normal_speedX("Normal speed X:", Float) = 0.3
		_Normal_speedY("Normal speed Y:", Float) = 0.3
		
		_DistorsionAmount("Distorsion amount:", Float) = 0.05
		_DistorsionRange("Mask dist. amount:", Range(0, 1)) = 1
	}

		SubShader
		{

			Tags
			{
				"Queue" = "Transparent"
				"RenderType" = "Transparent"
			}

			LOD 200

			Cull Off //two sided
			Lighting Off
			ZWrite Off

			//Blending mode----------------------------------------------
			//Blend SrcAlpha OneMinusSrcAlpha			// Traditional transparency
			//Blend One OneMinusSrcAlpha				// Premultiplied transparency
			Blend One One								// Additive
			//Blend OneMinusDstColor One				// Soft Additive
			//Blend DstColor Zero						// Multiplicative
			//Blend DstColor SrcColor					// 2x Multiplicative

			Pass
			{
				CGPROGRAM

				//#pragma surface surf Standard alphatest:_Cutoff vertex:vert addshadow
				#pragma vertex vert 
				#pragma fragment frag

				#include "UnityCG.cginc"

				uniform sampler2D _MainTex;
				uniform sampler2D _MaskTex;
				uniform sampler2D _NormalTex;

				uniform float _Main_speedX;
				uniform float _Main_speedY;
				uniform float _Mask_speedX;
				uniform float _Mask_speedY;
				uniform float _Normal_speedX;
				uniform float _Normal_speedY;

				uniform float4 _MainTex_ST;
				uniform float4 _MaskTex_ST;
				uniform float4 _NormalTex_ST;

				uniform fixed4 _colorTint;

				uniform float _DistorsionAmount;
				uniform float _DistorsionRange;
				uniform float _overbright;

				struct vertexInput
				{
					float4 vertex : POSITION;
					//float4 tangent : TANGENT;
					//float3 normal : NORMAL;
					float4 texcoord0 : TEXCOORD0;
					float4 texcoord1 : TEXCOORD1;
					float4 texcoord2 : TEXCOORD2;
					fixed4 color : COLOR;
					//half4 texcoord2 : TEXCOORD2;
					//half4 texcoord3 : TEXCOORD3;
					//half4 texcoord4 : TEXCOORD4;
					//half4 texcoord5 : TEXCOORD5;
				};

				struct vertexOutput
				{
					float4 pos : SV_POSITION;
					float2 main_uv : TEXCOORD0;
					float2 mask_uv : TEXCOORD1;
					float2 normal_uv : TEXCOORD2;

					fixed4 col : COLOR;
				};

				

				vertexOutput vert(vertexInput v)
				{
					vertexOutput OUT;
					OUT.pos = UnityObjectToClipPos(v.vertex);

					float2 mn_uv = TRANSFORM_TEX(v.texcoord0, _MainTex);
					float2 mk_uv = TRANSFORM_TEX(v.texcoord0, _MaskTex);
					float2 nm_uv = TRANSFORM_TEX(v.texcoord0, _NormalTex);

					OUT.main_uv = mn_uv + float2(_Main_speedX, _Main_speedY) * _Time.x;
					OUT.mask_uv = mk_uv + float2(_Mask_speedX, _Mask_speedY) * _Time.x;
					OUT.normal_uv = nm_uv + float2(_Normal_speedX, _Normal_speedY) * _Time.x;

					OUT.col = v.color;

					return OUT;
				}

				half4 frag(vertexOutput IN) : COLOR
				{
					float4 Normal = tex2D(_NormalTex, IN.normal_uv) * 2.0 - 1.0;
					float4 Main = tex2D(_MainTex, IN.main_uv + (Normal.rg * _DistorsionAmount));
					float4 Mask = tex2D(_MaskTex, IN.mask_uv + (Normal.rg * _DistorsionAmount * _DistorsionRange));
					
					float Alpha = Mask.a * Main.a * _colorTint.a * IN.col.a;

					float3 Color = Main.rgb * Mask.rgb * _colorTint.rgb * IN.col.rgb * _overbright * Alpha;

					fixed4 Complete = float4(Color, Alpha);

					return Complete;
				}

				ENDCG

			}//end pass

		}//end subshader

		Fallback "Fx/Flare"
}