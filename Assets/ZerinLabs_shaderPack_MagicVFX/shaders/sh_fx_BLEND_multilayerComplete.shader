Shader "zerinlabs/sh_fx_BLEND_multilayerComplete"
{
	Properties
	{
		[Header(Global params)]
		_colorTintA("Color Highs", color) = (1.0,0.5,0.0,1.0)
		_colorTintB("Color Lows", color) = (1.0,0.5,0.0,1.0)
		[Space(20)]
		_Col_mix("Mix (color vs. texture)", Range(0,1)) = 0
		[Space(20)]
		_overbright1("Overbright Highs Amount:", Float) = 1
		_overbright2("Overbright Lows Amount:", Float) = 1
		[Toggle] _grayTex("Alpha: Use textures as grayscale", Float) = 1
		[Space(20)]
		_Col_midPoint("Color midpoint:", Range(0,1)) = 0.5
		_Col_thresh("Color smoothness:", Range(0,0.5)) = 0.1
		[Space(20)]
		_alphaPow("Alpha intensity:", Float) = 1

		[Header (Base map)]
		_MainTex("Texture", 2D) = "white" {}
		_Main_speedX("Speed X:", Float) = 0.3
		_Main_speedY("Speed Y:", Float) = 0.3

		[Header(Mask map)]
		_MaskTex("Texture", 2D) = "white" {}
		_Mask_speedX("Speed X:", Float) = 0.3
		_Mask_speedY("Speed Y:", Float) = 0.3
		
		[Header(Distorsion map)]
		_DistorsionAmount("Amount", Float) = 0.05
		_DistorsionRange("Dist. influence (mask map)", Range(0, 1)) = 1
		
		[Space(20)]
		_NormalTex("Texture (Normal map)", 2D) = "white" {}
		_Normal_speedX("Speed X:", Float) = 0.3
		_Normal_speedY("Speed Y:", Float) = 0.3

		[Header(Orientation fade)]
		_NdotVPow("Orientation-fade amount", Float) = 1.0
		[Toggle] _NdotVAmount("Orientation-fade active", Float) = 0
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

			//	LEGEND for BENDING MODES
			//------------------------------------------------------------------------------------------------------
				Blend	 SrcAlpha			OneMinusSrcAlpha	// Traditional transparency
			//	Blend	 One				OneMinusSrcAlpha	// Blend Add (aka premultiplied alpha) NOTES: Premultiply the alpha with the RBG either in the texture, or the shader - and then use an alpha channel to modulate the output alpha. An alpha of 1 will be blend, an alpha of 0 will be add.
			//	Blend	 One				One					// Additive
			//	Blend	 OneMinusDstColor	One					// Soft Additive
			//	Blend	 DstColor			Zero				// Multiplicative
			//	Blend	 DstColor			SrcColor			// 2x Multiplicative

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

				uniform fixed4 _colorTintA;
				uniform fixed4 _colorTintB;

				uniform float _Col_mix;

				uniform float _DistorsionAmount;
				uniform float _DistorsionRange;
				uniform float _overbright1;
				uniform float _overbright2;

				uniform float _Col_midPoint;
				uniform float _Col_thresh;
				uniform float _grayTex;
				uniform float _alphaPow;

				uniform float _NdotVAmount;
				uniform float _NdotVPow;

				struct vertexInput
				{
					float4 vertex : POSITION;
					//float4 tangent : TANGENT;
					float3 normal : NORMAL;
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
					float NdotV : TEXCOORD3;

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

					float3 viewDir = normalize(UNITY_MATRIX_MV[2].xyz);
					float3 normDir = normalize(v.normal);

					float dt = max(dot(viewDir, normDir), 0.0);
					dt = pow(dt, _NdotVPow);

					OUT.NdotV = max(dt, (1.0 - _NdotVAmount));

					return OUT;
				}

				half4 frag(vertexOutput IN) : COLOR
				{
					//texture load (calculate distorsion if applies)
					float4 Normal = tex2D(_NormalTex, IN.normal_uv) * 2.0 - 1.0;
					float4 Main = tex2D(_MainTex, IN.main_uv + (Normal.rg * _DistorsionAmount));
					float4 Mask = tex2D(_MaskTex, IN.mask_uv + (Normal.rg * _DistorsionAmount * _DistorsionRange));

					//Calculate the "alpha" value based on the alpha of the textures (and the R channel of the textures if B/W toggle is enabled)
					float Alpha = lerp(Main.a * Mask.a, Main.r * Mask.r, _grayTex);
					Alpha = Alpha * IN.col.a * _alphaPow;

					//calculate the color gradient based on the "alpha value"
					float lerpMask = saturate(smoothstep(_Col_midPoint - _Col_thresh, _Col_midPoint + _Col_thresh, Alpha));
					float overbright = lerp(_overbright2, _overbright1, lerpMask);
					float4 ColComposite = lerp(_colorTintB, _colorTintA, lerpMask);
					float3 Color = lerp(ColComposite.rgb, Main.rgb, _Col_mix);
					Color = Color * IN.col.rgb * overbright;

					//output composite: RGB + A
					fixed4 Complete = float4(Color, Alpha * IN.NdotV);

					//RETURN output
					return Complete;
				}

				ENDCG

			}//end pass

		}//end subshader

			Fallback "Fx/Flare"
}