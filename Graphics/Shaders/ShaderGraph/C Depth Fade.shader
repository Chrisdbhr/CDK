// Made with Amplify Shader Editor v1.9.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "C Depth Fade"
{
	Properties
	{
		[Header(Refraction)]
		_ChromaticAberration("Chromatic Aberration", Range( 0 , 0.3)) = 0.1
		_Color("Color", Color) = (0,0,0,0)
		_Distance("Distance", Range( 0 , 50)) = 0
		_Specular("Specular", Float) = 0
		_Refraction("Refraction", Range( 0.5 , 1.5)) = 0
		_Gloss("Gloss", Float) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		GrabPass{ }
		CGPROGRAM
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma multi_compile _ALPHAPREMULTIPLY_ON
		#pragma surface surf Lambert alpha:fade keepalpha finalcolor:RefractionF noshadow exclude_path:deferred 
		struct Input
		{
			float4 screenPos;
			float3 worldPos;
		};

		uniform half4 _Color;
		uniform half _Specular;
		uniform half _Gloss;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform half _Distance;
		uniform sampler2D _GrabTexture;
		uniform float _ChromaticAberration;
		uniform half _Refraction;

		inline float4 Refraction( Input i, SurfaceOutput o, float indexOfRefraction, float chomaticAberration ) {
			float3 worldNormal = o.Normal;
			float4 screenPos = i.screenPos;
			#if UNITY_UV_STARTS_AT_TOP
				float scale = -1.0;
			#else
				float scale = 1.0;
			#endif
			float halfPosW = screenPos.w * 0.5;
			screenPos.y = ( screenPos.y - halfPosW ) * _ProjectionParams.x * scale + halfPosW;
			#if SHADER_API_D3D9 || SHADER_API_D3D11
				screenPos.w += 0.00000000001;
			#endif
			float2 projScreenPos = ( screenPos / screenPos.w ).xy;
			float3 worldViewDir = normalize( UnityWorldSpaceViewDir( i.worldPos ) );
			float3 refractionOffset = ( indexOfRefraction - 1.0 ) * mul( UNITY_MATRIX_V, float4( worldNormal, 0.0 ) ) * ( 1.0 - dot( worldNormal, worldViewDir ) );
			float2 cameraRefraction = float2( refractionOffset.x, refractionOffset.y );
			float4 redAlpha = tex2D( _GrabTexture, ( projScreenPos + cameraRefraction ) );
			float green = tex2D( _GrabTexture, ( projScreenPos + ( cameraRefraction * ( 1.0 - chomaticAberration ) ) ) ).g;
			float blue = tex2D( _GrabTexture, ( projScreenPos + ( cameraRefraction * ( 1.0 + chomaticAberration ) ) ) ).b;
			return float4( redAlpha.r, green, blue, redAlpha.a );
		}

		void RefractionF( Input i, SurfaceOutput o, inout half4 color )
		{
			#ifdef UNITY_PASS_FORWARDBASE
			color.rgb = color.rgb + Refraction( i, o, _Refraction, _ChromaticAberration ) * ( 1 - color.a );
			color.a = 1;
			#endif
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Normal = float3(0,0,1);
			o.Emission = _Color.rgb;
			o.Specular = _Specular;
			o.Gloss = _Gloss;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			half4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth1 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			half distanceDepth1 = abs( ( screenDepth1 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Distance ) );
			half clampResult8 = clamp( distanceDepth1 , 0.0 , 1.0 );
			o.Alpha = clampResult8;
			o.Normal = o.Normal + 0.00001 * i.screenPos * i.worldPos;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19100
Node;AmplifyShaderEditor.ClampOpNode;8;-94.71471,269.4829;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-90.99839,79.45834;Inherit;False;Property;_Gloss;Gloss;6;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-92.31549,-6.15212;Inherit;False;Property;_Specular;Specular;4;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-94.94968,172.9713;Inherit;False;Property;_Refraction;Refraction;5;0;Create;True;0;0;0;False;0;False;0;1;0.5;1.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;3;-135.0149,-215.5434;Inherit;False;Property;_Color;Color;2;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.1792436,0.2845175,0.3829996,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;2;227.563,-22.99902;Half;False;True;-1;2;ASEMaterialInspector;0;0;Lambert;C Depth Fade;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;ForwardOnly;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;0;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-700.8077,269.1186;Inherit;False;Property;_Distance;Distance;3;0;Create;True;0;0;0;False;0;False;0;1;0;50;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;1;-393.5765,261.4971;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
WireConnection;8;0;1;0
WireConnection;2;2;3;0
WireConnection;2;3;10;0
WireConnection;2;4;12;0
WireConnection;2;8;11;0
WireConnection;2;9;8;0
WireConnection;1;0;9;0
ASEEND*/
//CHKSM=4AA0BC25444491BE1DEF7221ED96D7C53D3137D6