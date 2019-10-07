Shader "Custom/ColorShader" {
    Properties {
        _Color("Color", Color) = (1.000000,1.000000,1.000000,1.000000)
    }
    SubShader {
        Tags 
    { 
        "RenderType"="Opaque"
    }
        LOD 150

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Lambert noforwardadd

        float4 _Color;

        struct Input {
            half color : COLOR;
        };

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

        
        void surf (Input IN, inout SurfaceOutput o) {
            o.Albedo = _Color;
            o.Emission = _Color;
        }
        ENDCG
    }
    FallBack "Diffuse"  
}