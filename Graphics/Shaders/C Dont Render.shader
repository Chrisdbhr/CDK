Shader "CDK/C Dont Render" {
    SubShader {
    Pass {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag

      struct v2f {
          float4 pos : SV_POSITION;
          float4 depth : TEXCOORD0; // Stores depth
      };

      v2f vert ()
      {
          v2f o;
          o.pos = fixed4(0,0,0,0);
          return o;
      }

      fixed4 frag (v2f i) : COLOR0 { return fixed4(0,0,0,0); }
      ENDCG
    }
  }
}