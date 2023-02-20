// Made with Amplify Shader Editor v1.9.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "C Two Moving Textures"
{
	Properties
	{
		_Color("Color", Color) = (0.179244,0.2845179,0.383,0)
		_WaveSpeed1("Wave Speed 1", Float) = 0.05
		_WaveSpeed2("Wave Speed 2", Float) = 0.02
		_BaseMap("BaseMap", 2D) = "white" {}
		_Tilling("Tilling", Vector) = (1,1,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform half4 _Color;
		uniform sampler2D _BaseMap;
		uniform half2 _Tilling;
		uniform half _WaveSpeed1;
		uniform half _WaveSpeed2;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			half2 temp_cast_0 = (( _Time.y * _WaveSpeed1 )).xx;
			float2 uv_TexCoord10 = i.uv_texcoord * _Tilling + temp_cast_0;
			half2 temp_cast_1 = (( _Time.y * _WaveSpeed2 )).xx;
			float2 uv_TexCoord17 = i.uv_texcoord * _Tilling + temp_cast_1;
			o.Emission = ( _Color + ( tex2D( _BaseMap, uv_TexCoord10 ) * tex2D( _BaseMap, uv_TexCoord17 ) ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19100
Node;AmplifyShaderEditor.TextureCoordinatesNode;17;-1385.537,690.6964;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-1795.373,344.0218;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-1779.812,556.6847;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-1954.437,563.6009;Inherit;False;Property;_WaveSpeed2;Wave Speed 2;2;0;Create;True;0;0;0;False;0;False;0.02;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;7;-2199.656,426.4934;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;8;-1968.269,351.2892;Inherit;False;Property;_WaveSpeed1;Wave Speed 1;1;0;Create;True;0;0;0;False;0;False;0.05;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;10;-1433.685,184.6906;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;26;-1594.45,424.9681;Inherit;False;Property;_Tilling;Tilling;4;0;Create;True;0;0;0;False;0;False;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SamplerNode;16;-943.9139,551.2545;Inherit;True;Property;_TextureSample2;Texture Sample 0;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-562.4217,395.6274;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;14;-924.8256,225.4118;Inherit;True;Property;_TextureSample0;Texture Sample 0;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;13;-1221.271,385.367;Inherit;True;Property;_BaseMap;BaseMap;3;0;Create;True;0;0;0;False;0;False;61c0b9c0523734e0e91bc6043c72a490;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleAddOpNode;22;226.6832,232.6266;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;2;-45.18208,-72.71419;Inherit;False;Property;_Color;Color;0;0;Create;True;0;0;0;False;0;False;0.179244,0.2845179,0.383,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;604.6181,155.0214;Half;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;C Two Moving Textures;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;17;0;26;0
WireConnection;17;1;12;0
WireConnection;11;0;7;2
WireConnection;11;1;8;0
WireConnection;12;0;7;2
WireConnection;12;1;9;0
WireConnection;10;0;26;0
WireConnection;10;1;11;0
WireConnection;16;0;13;0
WireConnection;16;1;17;0
WireConnection;19;0;14;0
WireConnection;19;1;16;0
WireConnection;14;0;13;0
WireConnection;14;1;10;0
WireConnection;22;0;2;0
WireConnection;22;1;19;0
WireConnection;0;2;22;0
ASEEND*/
//CHKSM=A800E8465AF0C76F2E249E555150542B8DF56A89