// Made with Amplify Shader Editor v1.9.3.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Mat_rocks"
{
	Properties
	{
		_low_defaultMat_MaskMap("low_defaultMat_MaskMap", 2D) = "white" {}
		_low_defaultMat_Normal("low_defaultMat_Normal", 2D) = "bump" {}
		_low_defaultMat_BaseMap("low_defaultMat_BaseMap", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _low_defaultMat_Normal;
		uniform float4 _low_defaultMat_Normal_ST;
		uniform sampler2D _low_defaultMat_BaseMap;
		uniform float4 _low_defaultMat_BaseMap_ST;
		uniform sampler2D _low_defaultMat_MaskMap;
		uniform float4 _low_defaultMat_MaskMap_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_low_defaultMat_Normal = i.uv_texcoord * _low_defaultMat_Normal_ST.xy + _low_defaultMat_Normal_ST.zw;
			o.Normal = UnpackNormal( tex2D( _low_defaultMat_Normal, uv_low_defaultMat_Normal ) );
			float2 uv_low_defaultMat_BaseMap = i.uv_texcoord * _low_defaultMat_BaseMap_ST.xy + _low_defaultMat_BaseMap_ST.zw;
			o.Albedo = tex2D( _low_defaultMat_BaseMap, uv_low_defaultMat_BaseMap ).rgb;
			float2 uv_low_defaultMat_MaskMap = i.uv_texcoord * _low_defaultMat_MaskMap_ST.xy + _low_defaultMat_MaskMap_ST.zw;
			float4 tex2DNode1 = tex2D( _low_defaultMat_MaskMap, uv_low_defaultMat_MaskMap );
			o.Smoothness = tex2DNode1.a;
			o.Occlusion = tex2DNode1.g;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19303
Node;AmplifyShaderEditor.SamplerNode;1;-768,48;Inherit;True;Property;_low_defaultMat_MaskMap;low_defaultMat_MaskMap;0;0;Create;True;0;0;0;False;0;False;-1;91c2021a9564f6145a9445fd793f8d1a;91c2021a9564f6145a9445fd793f8d1a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-768,-160;Inherit;True;Property;_low_defaultMat_Normal;low_defaultMat_Normal;1;0;Create;True;0;0;0;False;0;False;-1;d9441e13902c8724fb4f4a286840e0d2;d9441e13902c8724fb4f4a286840e0d2;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-784,-352;Inherit;True;Property;_low_defaultMat_BaseMap;low_defaultMat_BaseMap;2;0;Create;True;0;0;0;False;0;False;-1;7293dcd34325a0b4d82e9f6ab6686612;7293dcd34325a0b4d82e9f6ab6686612;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Mat_rocks;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;0;0;3;0
WireConnection;0;1;2;0
WireConnection;0;4;1;4
WireConnection;0;5;1;2
ASEEND*/
//CHKSM=9EDDAAA1CEC8272D897AF5091874DBCE85990160