Shader "CDK/C Dont Render" {
    SubShader
    {
        Tags {"Queue" = "Geometry-1" }
 
        Lighting Off
 
        Pass
 
        {
            ZWrite Off
            ColorMask 0    
        }
    }
}