// Made with Amplify Shader Editor v1.9.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "C Area Of Effect"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Color("Color", Color) = (1,0.3160377,0.3160377,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog vertex:vertexDataFunc 
		struct Input
		{
			float4 screenPosition;
		};

		uniform half4 _Color;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _Cutoff = 0.5;


		inline float Dither4x4Bayer( int x, int y )
		{
			const float dither[ 16 ] = {
				 1,  9,  3, 11,
				13,  5, 15,  7,
				 4, 12,  2, 10,
				16,  8, 14,  6 };
			int r = y * 4 + x;
			return dither[r] / 16; // same # of instructions as pre-dividing due to compiler magic
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float4 ase_screenPos = ComputeScreenPos( UnityObjectToClipPos( v.vertex ) );
			o.screenPosition = ase_screenPos;
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Emission = _Color.rgb;
			float4 ase_screenPos = i.screenPosition;
			half4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth1 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			half distanceDepth1 = abs( ( screenDepth1 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( 1.0 ) );
			half2 clipScreen5 = ase_screenPosNorm.xy * _ScreenParams.xy;
			half dither5 = Dither4x4Bayer( fmod(clipScreen5.x, 4), fmod(clipScreen5.y, 4) );
			half clampResult8 = clamp( ( ( 1.0 - distanceDepth1 ) * dither5 ) , 0.0 , 1.0 );
			o.Alpha = clampResult8;
			clip( clampResult8 - _Cutoff );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19100
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;2;227.563,-22.99902;Half;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;C Area Of Effect;False;False;False;False;True;True;True;True;True;True;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;False;0;True;Opaque;;AlphaTest;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;0;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-283.4333,261.7524;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;8;-34.26857,233.7562;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;9;-466.808,257.5528;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DitheringNode;5;-538.1983,386.3351;Inherit;False;0;False;4;0;FLOAT;0;False;1;SAMPLER2D;;False;2;FLOAT4;0,0,0,0;False;3;SAMPLERSTATE;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;3;-322.8036,-135.9523;Inherit;False;Property;_Color;Color;1;0;Create;True;0;0;0;False;0;False;1,0.3160377,0.3160377,0;1,0.3160376,0.3160376,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DepthFade;1;-725.6079,241.7548;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
WireConnection;2;2;3;0
WireConnection;2;9;8;0
WireConnection;2;10;8;0
WireConnection;6;0;9;0
WireConnection;6;1;5;0
WireConnection;8;0;6;0
WireConnection;9;0;1;0
ASEEND*/
//CHKSM=DC1DCE19A67E68EDD4E0A360FC734E4FFE20A5EA