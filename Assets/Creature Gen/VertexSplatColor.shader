Shader "Custom/VertexSplatColor" {
    Properties {
        _Color1 ("Color 1", Color) = (1.0,1.0,1.0,1)
        _Color2 ("Color 2", Color) = (1.0,1.0,1.0,1)
        _Color3 ("Color 3", Color) = (1.0,1.0,1.0,1)
        _Color4 ("Color 4", Color) = (1.0,1.0,1.0,1)
        _SpecularColor ("Specular", Color) = (0.0,0.0,0.0,1)
        _Smoothness("Smoothness", Range(0.0, 1.0)) = 1.0
    }
    SubShader {
        Tags { "RenderType" = "Opaque" }
        CGPROGRAM
        #pragma surface surf StandardSpecular fullforwardshadows
        #pragma require interpolators15
        struct Input {
			float3 vertColor : COLOR;
        };
        fixed4 _Color1;
        fixed4 _Color2;
        fixed4 _Color3;
        fixed4 _Color4;
        fixed4 _SpecularColor;
        half _Smoothness;
        void surf (Input IN, inout SurfaceOutputStandardSpecular o) {
            float vs4 = (IN.vertColor.r + IN.vertColor.g + IN.vertColor.b - 1) / 2;
            o.Albedo = _Color1 * (IN.vertColor.r - vs4)
                + _Color2 * (IN.vertColor.g - vs4)
                + _Color3 * (IN.vertColor.b - vs4)
                + _Color4 * vs4;
            o.Specular = _SpecularColor;
            o.Smoothness = _Smoothness;
        }
        ENDCG
    }
    Fallback "Diffuse"
}