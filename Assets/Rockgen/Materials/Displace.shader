Shader "Displace" {
    //show values to edit in inspector
    Properties {
        _Color ("Tint", Color) = (0, 0, 0, 1)
        _MainTex ("_MainTex", 2D) = "white" {}
        _Displace ("_Displace", 2D) = "gray" {}
        _Displace_Scale ("_Displace_Scale", Float) = 1
        _Smoothness ("Smoothness", Range(0, 1)) = 0
        _Metallic ("Metalness", Range(0, 1)) = 0
    }
    SubShader {
        Tags{ "RenderType"="Opaque" "Queue"="Geometry"}

        CGPROGRAM

        #pragma surface surf Standard fullforwardshadows vertex:vert addshadow
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _Displace;
        fixed4 _Color;

        half _Displace_Scale;
        half _Smoothness;
        half _Metallic;

        struct Input {
            float2 uv_MainTex;
            float2 uv_Displace;
        };

        void vert(inout appdata_full data){
            float4 modifiedPos = data.vertex;
            data.vertex.xyz += data.normal * (tex2Dlod(_Displace, float4(data.texcoord.xy, 0, 0)) - .5) * 2 * _Displace_Scale;
        }

        void surf (Input i, inout SurfaceOutputStandard o) {
            fixed4 col = tex2D(_MainTex, i.uv_MainTex);
            col *= _Color;
            o.Albedo = col.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
            o.Emission = 0;
        }
        ENDCG
    }
    FallBack "Standard"
}
