Shader "CDK/C Dont Render" {
    SubShader {
    Pass {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag

      struct v2f {
          float4 pos : SV_POSITION;
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