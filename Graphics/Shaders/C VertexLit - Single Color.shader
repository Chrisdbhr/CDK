// From VLukianenko https://discussions.unity.com/t/mobile-diffuse-with-color/585257#post-3530533
Shader "CDK/C VertexLit - Single color" {
Properties {
    _Color ("Color", color) = (1,1,1,1)
}
 
SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 80
 
    // Non-lightmapped
    Pass {
        Tags { "LightMode" = "Vertex" }
 
        Material {
            Diffuse [_Color]
            Ambient [_Color]
        }
        Lighting On
        SetTexture [_MainTex] {
            constantColor [_Color]
            Combine primary DOUBLE, constant // UNITY_OPAQUE_ALPHA_FFP
        }
    }
 
    // Lightmapped, encoded as dLDR
    Pass {
        Tags { "LightMode" = "VertexLM" }
 
        BindChannels {
            Bind "Vertex", vertex
            Bind "normal", normal
            Bind "texcoord1", texcoord0 // lightmap uses 2nd uv
        }
 
        SetTexture [unity_Lightmap] {
            matrix [unity_LightmapMatrix]
            combine texture
        }
        SetTexture [_MainTex] {
            constantColor [_Color]
            combine previous, constant // UNITY_OPAQUE_ALPHA_FFP
        }
    }
 
    // Lightmapped, encoded as RGBM
    Pass {
        Tags { "LightMode" = "VertexLMRGBM" }
 
        BindChannels {
            Bind "Vertex", vertex
            Bind "normal", normal
            Bind "texcoord1", texcoord0 // lightmap uses 2nd uv
            Bind "texcoord", texcoord1 // main uses 1st uv
        }
 
        SetTexture [unity_Lightmap] {
            matrix [unity_LightmapMatrix]
            combine texture * texture alpha DOUBLE
        }
        SetTexture [_MainTex] {
            constantColor [_Color]
            combine previous QUAD, constant // UNITY_OPAQUE_ALPHA_FFP
        }
    }
 
    // Pass to render object as a shadow caster
    Pass
    {
        Name "ShadowCaster"
        Tags { "LightMode" = "ShadowCaster" }
 
        ZWrite On ZTest LEqual Cull Off
 
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0
        #pragma multi_compile_shadowcaster
        #include "UnityCG.cginc"
 
        struct v2f {
            V2F_SHADOW_CASTER;
            UNITY_VERTEX_OUTPUT_STEREO
        };
 
        v2f vert( appdata_base v )
        {
            v2f o;
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
            TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
            return o;
        }
 
        float4 frag( v2f i ) : SV_Target
        {
            SHADOW_CASTER_FRAGMENT(i)
        }
        ENDCG
    }
}
    Fallback "VertexLit"
}