// Made with Amplify Shader Editor v1.9.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "C Triplanar Mapping"
{
	Properties
	{
		_Tilling("Tilling", Vector) = (1,1,0,0)
		_Specular("Specular", Float) = 0
		_Gloss("Gloss", Float) = 0
		_Tint("Tint", Color) = (1,1,1,1)
		[Toggle]_UseStochastic("Use Stochastic", Float) = 0
		_Texture("Texture", 2D) = "gray" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Off
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform float _UseStochastic;
		uniform sampler2D _Texture;
		uniform float2 _Tilling;
		uniform float4 _Tint;
		uniform float _Specular;
		uniform float _Gloss;


		inline float4 TriplanarSampling1( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );
			yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
			zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
		}


		void StochasticTiling( float2 UV, out float2 UV1, out float2 UV2, out float2 UV3, out float W1, out float W2, out float W3 )
		{
			float2 vertex1, vertex2, vertex3;
			// Scaling of the input
			float2 uv = UV * 3.464; // 2 * sqrt (3)
			// Skew input space into simplex triangle grid
			const float2x2 gridToSkewedGrid = float2x2( 1.0, 0.0, -0.57735027, 1.15470054 );
			float2 skewedCoord = mul( gridToSkewedGrid, uv );
			// Compute local triangle vertex IDs and local barycentric coordinates
			int2 baseId = int2( floor( skewedCoord ) );
			float3 temp = float3( frac( skewedCoord ), 0 );
			temp.z = 1.0 - temp.x - temp.y;
			if ( temp.z > 0.0 )
			{
				W1 = temp.z;
				W2 = temp.y;
				W3 = temp.x;
				vertex1 = baseId;
				vertex2 = baseId + int2( 0, 1 );
				vertex3 = baseId + int2( 1, 0 );
			}
			else
			{
				W1 = -temp.z;
				W2 = 1.0 - temp.y;
				W3 = 1.0 - temp.x;
				vertex1 = baseId + int2( 1, 1 );
				vertex2 = baseId + int2( 1, 0 );
				vertex3 = baseId + int2( 0, 1 );
			}
			UV1 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex1 ) ) * 43758.5453 );
			UV2 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex2 ) ) * 43758.5453 );
			UV3 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex3 ) ) * 43758.5453 );
			return;
		}


		void TriplanarWeights( float3 WorldNormal, out float W0, out float W1, out float W2 )
		{
			half3 weights = max( abs( WorldNormal.xyz ), 0.000001 );
			weights /= ( weights.x + weights.y + weights.z ).xxx;
			W0 = weights.x;
			W1 = weights.y;
			W2 = weights.z;
			return;
		}


		void surf( Input i , inout SurfaceOutput o )
		{
			o.Normal = float3(0,0,1);
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float4 triplanar1 = TriplanarSampling1( _Texture, ase_worldPos, ase_worldNormal, 1.0, _Tilling, 1.0, 0 );
			float localStochasticTiling53_g1 = ( 0.0 );
			float2 temp_output_104_0_g1 = _Tilling;
			float3 temp_output_80_0_g1 = ase_worldPos;
			float2 Triplanar_UV050_g1 = ( temp_output_104_0_g1 * (temp_output_80_0_g1).zy );
			float2 UV53_g1 = Triplanar_UV050_g1;
			float2 UV153_g1 = float2( 0,0 );
			float2 UV253_g1 = float2( 0,0 );
			float2 UV353_g1 = float2( 0,0 );
			float W153_g1 = 0.0;
			float W253_g1 = 0.0;
			float W353_g1 = 0.0;
			StochasticTiling( UV53_g1 , UV153_g1 , UV253_g1 , UV353_g1 , W153_g1 , W253_g1 , W353_g1 );
			float2 temp_output_57_0_g1 = ddx( Triplanar_UV050_g1 );
			float2 temp_output_58_0_g1 = ddy( Triplanar_UV050_g1 );
			float localTriplanarWeights108_g1 = ( 0.0 );
			float3 WorldNormal108_g1 = ase_worldNormal;
			float W0108_g1 = 0.0;
			float W1108_g1 = 0.0;
			float W2108_g1 = 0.0;
			TriplanarWeights( WorldNormal108_g1 , W0108_g1 , W1108_g1 , W2108_g1 );
			float localStochasticTiling83_g1 = ( 0.0 );
			float2 Triplanar_UV164_g1 = ( temp_output_104_0_g1 * (temp_output_80_0_g1).zx );
			float2 UV83_g1 = Triplanar_UV164_g1;
			float2 UV183_g1 = float2( 0,0 );
			float2 UV283_g1 = float2( 0,0 );
			float2 UV383_g1 = float2( 0,0 );
			float W183_g1 = 0.0;
			float W283_g1 = 0.0;
			float W383_g1 = 0.0;
			StochasticTiling( UV83_g1 , UV183_g1 , UV283_g1 , UV383_g1 , W183_g1 , W283_g1 , W383_g1 );
			float2 temp_output_86_0_g1 = ddx( Triplanar_UV164_g1 );
			float2 temp_output_92_0_g1 = ddy( Triplanar_UV164_g1 );
			float localStochasticTiling117_g1 = ( 0.0 );
			float2 Triplanar_UV271_g1 = ( temp_output_104_0_g1 * (temp_output_80_0_g1).xy );
			float2 UV117_g1 = Triplanar_UV271_g1;
			float2 UV1117_g1 = float2( 0,0 );
			float2 UV2117_g1 = float2( 0,0 );
			float2 UV3117_g1 = float2( 0,0 );
			float W1117_g1 = 0.0;
			float W2117_g1 = 0.0;
			float W3117_g1 = 0.0;
			StochasticTiling( UV117_g1 , UV1117_g1 , UV2117_g1 , UV3117_g1 , W1117_g1 , W2117_g1 , W3117_g1 );
			float2 temp_output_107_0_g1 = ddx( Triplanar_UV271_g1 );
			float2 temp_output_110_0_g1 = ddy( Triplanar_UV271_g1 );
			float4 Output_Triplanar295_g1 = ( ( ( ( tex2D( _Texture, UV153_g1, temp_output_57_0_g1, temp_output_58_0_g1 ) * W153_g1 ) + ( tex2D( _Texture, UV253_g1, temp_output_57_0_g1, temp_output_58_0_g1 ) * W253_g1 ) + ( tex2D( _Texture, UV353_g1, temp_output_57_0_g1, temp_output_58_0_g1 ) * W353_g1 ) ) * W0108_g1 ) + ( W1108_g1 * ( ( tex2D( _Texture, UV183_g1, temp_output_86_0_g1, temp_output_92_0_g1 ) * W183_g1 ) + ( tex2D( _Texture, UV283_g1, temp_output_86_0_g1, temp_output_92_0_g1 ) * W283_g1 ) + ( tex2D( _Texture, UV383_g1, temp_output_86_0_g1, temp_output_92_0_g1 ) * W383_g1 ) ) ) + ( W2108_g1 * ( ( tex2D( _Texture, UV1117_g1, temp_output_107_0_g1, temp_output_110_0_g1 ) * W1117_g1 ) + ( tex2D( _Texture, UV2117_g1, temp_output_107_0_g1, temp_output_110_0_g1 ) * W2117_g1 ) + ( tex2D( _Texture, UV3117_g1, temp_output_107_0_g1, temp_output_110_0_g1 ) * W3117_g1 ) ) ) );
			o.Albedo = ( (( _UseStochastic )?( Output_Triplanar295_g1 ):( triplanar1 )) * _Tint ).rgb;
			o.Specular = _Specular;
			o.Gloss = _Gloss;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Lambert keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float4 tSpace0 : TEXCOORD1;
				float4 tSpace1 : TEXCOORD2;
				float4 tSpace2 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19100
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;8;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Lambert;C Triplanar Mapping;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-355.1411,282.319;Inherit;False;Property;_Gloss;Gloss;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-361.6411,186.1189;Inherit;False;Property;_Specular;Specular;1;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TriplanarNode;1;-1569.697,-298.4382;Inherit;True;Spherical;World;False;Top Texture;_TopTexture;white;0;None;Mid Texture 0;_MidTexture0;white;-1;None;Bot Texture 0;_BotTexture0;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;16;-1548.7,42.93499;Inherit;False;Procedural Sample;-1;;1;f5379ff72769e2b4495e5ce2f004d8d4;2,157,2,315,2;7;82;SAMPLER2D;0;False;158;SAMPLER2DARRAY;0;False;183;FLOAT;0;False;5;FLOAT2;0,0;False;80;FLOAT3;0,0,0;False;104;FLOAT2;1,1;False;74;SAMPLERSTATE;0;False;5;COLOR;0;FLOAT;32;FLOAT;33;FLOAT;34;FLOAT;35
Node;AmplifyShaderEditor.Vector2Node;10;-1798.111,-96.04721;Inherit;False;Property;_Tilling;Tilling;0;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TexturePropertyNode;21;-2225.788,-41.7213;Inherit;True;Property;_Texture;Texture;5;0;Create;True;0;0;0;False;0;False;None;58b6422504be0f3458860655315bd776;False;gray;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.ToggleSwitchNode;31;-1100.086,-136.6216;Inherit;False;Property;_UseStochastic;Use Stochastic;4;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-405.7031,-123.4888;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;15;-707.2907,-37.49837;Inherit;False;Property;_Tint;Tint;3;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
WireConnection;8;0;13;0
WireConnection;8;3;11;0
WireConnection;8;4;12;0
WireConnection;1;0;21;0
WireConnection;1;3;10;0
WireConnection;16;82;21;0
WireConnection;16;104;10;0
WireConnection;31;0;1;0
WireConnection;31;1;16;0
WireConnection;13;0;31;0
WireConnection;13;1;15;0
ASEEND*/
//CHKSM=BB25F83F21E610BDE8B4A012DFF19A3B8BCAE6E6