// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/Game/Sphere" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_ColorStrength("Color Strength", Range(1, 100)) = 1
		_EmissionColor ("Emission Color", Color) = (1,1,1,1)
		_EmissionTex ("Emission (RGB)", 2D) = "white" {}
		_EmissionStrength("Emission Strength", Range(0, 100)) = 1
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_LerpSpeed("Lerp Speed", Range(0.0, 1.0)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex, _EmissionTex;

		struct Input {
			float2 uv_MainTex;
			float2 uv_EmissionTex;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color, _EmissionColor;
		half _ColorStrength, _EmissionStrength;
		half _LerpSpeed, _Inter;

		uniform fixed4 GLOBALmultiply_Color;
		uniform half GLOBALmultiply_Strength;
		uniform int GLOBAL_changeColor;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color * _ColorStrength;

			

			// Emission
			fixed4 e = tex2D(_EmissionTex, IN.uv_EmissionTex) * (_EmissionColor * _EmissionStrength);
			if(GLOBAL_changeColor > 0) {
				//_Inter += _Time.y * _LerpSpeed;
				e = tex2D(_EmissionTex, IN.uv_EmissionTex) * (GLOBALmultiply_Color * 1);
				//e = lerp(e, tex2D(_EmissionTex, IN.uv_EmissionTex) * (GLOBALmultiply_Color * GLOBALmultiply_Strength), _Inter);
			}
			o.Emission = e;

			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
