Shader "Common/Vertex Coloured Surf"
{
	Properties
	{
	}
    SubShader
    {
		Tags { "RenderType" = "Opaque" }
		CGPROGRAM
			#pragma surface surf Lambert
			struct Input
			{
			  float4 color : COLOR;
			};

	  		sampler2D _MainTex;
			  
			void surf (Input IN, inout SurfaceOutput o)
			{	  
				o.Albedo = IN.color.rgb;
			}
		ENDCG
    }
    Fallback "Diffuse"
}