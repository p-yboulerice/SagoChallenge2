Shader "BugBuilder/Drawing/DrawingSurfaceStencilled" {
    Properties {
        [HideInInspector]_MainTex ("Base (RGB)", 2D) = "white" {}
    }
    Category {
       Lighting Off
       ZWrite On
       Cull Back
       SubShader {
            Tags { "Queue"="Geometry+1" }
            Pass {
                Stencil {
                    ReadMask 1
                    Ref 1
                    Comp Equal
                }   
               SetTexture [_MainTex] {}
            }
        } 
    }
}