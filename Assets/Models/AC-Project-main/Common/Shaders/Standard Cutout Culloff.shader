// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Karim/Standard Cutout Culloff"
{
	Properties
	{
		_Color("Tint", Color) = (1,1,1,1)
		_MainTex("Albedo", 2D) = "white" {}
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		[NoScaleOffset]_MetallicGlossMap("Metallic and smoothness", 2D) = "white" {}
		_Metallic("Metallness", Range( 0 , 1)) = 0
		_Glossiness("Smoothness", Range( 0 , 1)) = 0
		[NoScaleOffset][Normal]_BumpMap("Normal Map", 2D) = "bump" {}
		_BumpScale("Normal Map Value", Float) = 1
		[NoScaleOffset]_OcclusionMap("Ambient Occlusion", 2D) = "white" {}
		[HDR]_EmissionColor("Emission Color", Color) = (0,0,0,1)
		[HDR][NoScaleOffset]_EmissionMap("Emission map", 2D) = "black" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			half ASEVFace : VFACE;
		};

		uniform sampler2D _BumpMap;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _BumpScale;
		SamplerState sampler_BumpMap;
		uniform float4 _Color;
		uniform float4 _EmissionColor;
		uniform sampler2D _EmissionMap;
		uniform float _Metallic;
		uniform sampler2D _MetallicGlossMap;
		SamplerState sampler_MetallicGlossMap;
		uniform float _Glossiness;
		uniform sampler2D _OcclusionMap;
		SamplerState sampler_OcclusionMap;
		SamplerState sampler_MainTex;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float2 UV89 = uv_MainTex;
			float3 tex2DNode3 = UnpackScaleNormal( tex2D( _BumpMap, UV89 ), _BumpScale );
			float3 appendResult101 = (float3(tex2DNode3.r , tex2DNode3.g , ( tex2DNode3.b * -1.0 )));
			float3 switchResult99 = (((i.ASEVFace>0)?(tex2DNode3):(appendResult101)));
			o.Normal = switchResult99;
			float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
			float4 Albedo87 = ( _Color * tex2DNode1 );
			o.Albedo = Albedo87.rgb;
			o.Emission = ( _EmissionColor * tex2D( _EmissionMap, UV89 ) ).rgb;
			float4 tex2DNode2 = tex2D( _MetallicGlossMap, UV89 );
			o.Metallic = ( _Metallic * tex2DNode2.r );
			o.Smoothness = ( tex2DNode2.a * _Glossiness );
			o.Occlusion = tex2D( _OcclusionMap, UV89 ).r;
			o.Alpha = 1;
			float Opacity103 = tex2DNode1.a;
			clip( Opacity103 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18500
255;73;833;343;1318.764;794.0991;5.138001;True;False
Node;AmplifyShaderEditor.CommentaryNode;94;-304.9816,-744.1874;Inherit;False;496.9896;214;Comment;2;85;89;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;85;-254.9816,-694.1874;Inherit;False;0;1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;89;-31.99207,-679.2154;Inherit;False;UV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;97;-1606.718,310.8551;Inherit;False;980.093;381.0392;Comment;6;3;39;91;99;100;101;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;37;-338.2242,-498.1736;Inherit;False;549.617;453.5422;Albedo;3;6;1;7;;0.8603413,1,0,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;91;-1529.139,361.8234;Inherit;False;89;UV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;39;-1556.718,431.9333;Inherit;False;Property;_BumpScale;Normal Map Value;7;0;Create;False;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-288.2241,-274.6317;Inherit;True;Property;_MainTex;Albedo;1;0;Create;False;0;0;False;0;False;-1;None;3e220214573c08e439baf9434718d971;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-1349.25,360.8551;Inherit;True;Property;_BumpMap;Normal Map;6;2;[NoScaleOffset];[Normal];Create;False;0;0;False;0;False;-1;None;53e8369d749b7ae4ab85679f2554ddcd;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;6;-209.9778,-448.1739;Inherit;False;Property;_Color;Tint;0;0;Create;False;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;49.39336,-345.3033;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;66;-361.9698,713.4469;Inherit;False;569.687;446.153;Emission;3;4;40;41;;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;92;-545.4869,940.0148;Inherit;False;89;UV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;90;-528.6539,133.3616;Inherit;False;89;UV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;100;-1060.722,581.1722;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;38;-371.8424,-14.74366;Inherit;False;583.3253;422.6915;Metallic and smoothness;5;33;34;2;35;36;;1,0.7949257,0.04245281,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;101;-935.3692,548.1301;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;4;-311.9691,929.5999;Inherit;True;Property;_EmissionMap;Emission map;10;2;[HDR];[NoScaleOffset];Create;False;0;0;False;0;False;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;33;-301.7635,35.25634;Inherit;False;Property;_Metallic;Metallness;4;0;Create;False;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-302.1277,291.9479;Inherit;False;Property;_Glossiness;Smoothness;5;0;Create;False;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;40;-234.9527,763.4469;Inherit;False;Property;_EmissionColor;Emission Color;9;1;[HDR];Create;False;0;0;False;0;False;0,0,0,1;0,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-321.8423,105.6752;Inherit;True;Property;_MetallicGlossMap;Metallic and smoothness;3;1;[NoScaleOffset];Create;False;0;0;False;0;False;-1;None;a4a7acc62ba909f4e8f2074e91a0df43;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;87;212.4617,-331.2039;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;93;-373.9791,1247.329;Inherit;False;89;UV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;103;18.70343,-143.1874;Inherit;False;Opacity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;36.48336,242.8885;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;49.48336,72.58842;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SwitchByFaceNode;99;-806.7844,370.8106;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;45.71786,823.4745;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;104;867.6378,391.4011;Inherit;False;103;Opacity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;5;-58.58876,1175.329;Inherit;True;Property;_OcclusionMap;Ambient Occlusion;8;1;[NoScaleOffset];Create;False;0;0;False;0;False;-1;None;f8e77a4f4616a2041b54eb8804f8feae;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;86;321.46,501.945;Inherit;False;87;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1099.46,109.6602;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Karim/Standard Cutout Culloff;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;2;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;89;0;85;0
WireConnection;3;1;91;0
WireConnection;3;5;39;0
WireConnection;7;0;6;0
WireConnection;7;1;1;0
WireConnection;100;0;3;3
WireConnection;101;0;3;1
WireConnection;101;1;3;2
WireConnection;101;2;100;0
WireConnection;4;1;92;0
WireConnection;2;1;90;0
WireConnection;87;0;7;0
WireConnection;103;0;1;4
WireConnection;36;0;2;4
WireConnection;36;1;34;0
WireConnection;35;0;33;0
WireConnection;35;1;2;1
WireConnection;99;0;3;0
WireConnection;99;1;101;0
WireConnection;41;0;40;0
WireConnection;41;1;4;0
WireConnection;5;1;93;0
WireConnection;0;0;86;0
WireConnection;0;1;99;0
WireConnection;0;2;41;0
WireConnection;0;3;35;0
WireConnection;0;4;36;0
WireConnection;0;5;5;1
WireConnection;0;10;104;0
ASEEND*/
//CHKSM=4CF8D8BF6B038B3A918AE11AB23FA20350F22C39