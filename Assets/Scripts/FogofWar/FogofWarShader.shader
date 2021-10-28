/*
Shader "Custom/FogofWarShader"
Shader "Custom/FogofWarShader"
{
    Properties
    {
         _Color("Main Color", Color) = (1,1,1,1)
         _MainTex("Base (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "LightMode" = "ForwardBase" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert

        fixed4 _Color;
        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            half4 baseColor = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = baseColor.rgb;
            o.Alpha = _Color.a - baseColor.g;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
*/
/*
Shader "Custom/FogofWarShader"
{
    Properties
    {
         _Color("Main Color", Color) = (1,1,1,1)
         _MainTex("Base (RGB)", 2D) = "white" {}
    }
        SubShader
    {
        Tags {  "queue" = "transparent" "RenderType" = "Transparent" "LightMode" = "ForwardBase" }
        Blend SrcAlpha OneMinusSrcAlpha
        Lighting Off
        LOD 200

        CGPROGRAM
        #pragma surface surf NoLighting noambient alpha:fade
       // #pragma surface surf Lambert fullforwardshadows alpha
        //#pragma surface surf Lambert vertex:vert alpha:fade

        fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, float aten)
        {
            fixed4 color;
            color.rgb = s.Albedo;
            color.a = s.Alpha;
            return color;
        }

        fixed4 _Color;
        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            half4 baseColor = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = _Color.rgb * baseColor.b;
            o.Alpha = _Color.a - baseColor.g;
        }
        ENDCG
    }
        FallBack "Diffuse"
}
*/





 Shader "Custom/FogofWarShader"
{
    Properties
    {
         _Color("Main Color", Color) = (1,1,1,1)
         _MainTex("Base (RGB)", 2D) = "white" {}
    }
        SubShader
    {
        Tags { "RenderType" = "Transparent" "LightMode" = "ForwardBase" }
        Blend SrcAlpha OneMinusSrcAlpha
        Lighting Off
        LOD 200

        CGPROGRAM
        #pragma surface surf NoLighting noambient alpha
        //#pragma target 3.0

        fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, float aten)
        {
            fixed4 color;
            color.rgb = s.Albedo;
            color.a = s.Alpha;
            return color;
        }

        fixed4 _Color;
        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            half4 baseColor = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = _Color.rgb * baseColor.b;
            o.Alpha = _Color.a - baseColor.g;
            //o.Alpha =0;
        }
        ENDCG
    }
        FallBack "Diffuse"
}


























/*
Shader "Custom/FogofWarShader"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        //_Glossiness ("Smoothness", Range(0,1)) = 0.5
        //_Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "LightMode"="ForwardBase" }
        Blend SrcAlpha OneMinusSrcAlpha
        Lighting Off
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf NoLighting noambient

        fixed4 LightingNoLighting(SurfaceOutput s, fixed3 ligthDir, float aten)
        {
            fixed4 color;
            color.rgb = s.Albedo;
            color.a = s.Alpha;
            return color;
        }

        // Use shader model 3.0 target, to get nicer looking lighting
        //#pragma target 3.0

        //sampler2D _MainTex;
        fixed4 _Color;
        sampler2D _MainTex;
        struct Input
        {
            float2 uv_MainTex;
        };

        //half _Glossiness;
        //half _Metallic;
        

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        //UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        //UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutput o)
        {
            half4 baseColor = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = _Color.rgb * baseColor.b;
            o.Alpha = _Color.a - baseColor.g;
            // Albedo comes from a texture tinted by color
            //fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
           // o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            //o.Metallic = _Metallic;
            //o.Smoothness = _Glossiness;
            //o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
*/