Shader "Custom/SplatColor" {
    Properties {
        _MainTex ("Splat Map", 2D) = "white" {}
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
            float2 uv2_MainTex;
        };
        sampler2D _MainTex;
        fixed4 _Color1;
        fixed4 _Color2;
        fixed4 _Color3;
        fixed4 _Color4;
        fixed4 _SpecularColor;
        half _Smoothness;
        void surf (Input IN, inout SurfaceOutputStandardSpecular o) {
            float3 splat = tex2D (_MainTex, IN.uv2_MainTex).rgb;
            float vs4 = clamp((splat.r + splat.g + splat.b - 1) / 2, 0, 1);
            o.Albedo = _Color1 * (splat.r - vs4)
                + _Color2 * (splat.g - vs4)
                + _Color3 * (splat.b - vs4)
                + _Color4 * vs4;
            o.Specular = _SpecularColor;
            o.Smoothness = _Smoothness;
        }
        ENDCG
    }
    Fallback "Diffuse"
}
