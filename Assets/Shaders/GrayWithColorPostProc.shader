Shader "Unlit/GrayWithColorPostProc"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Intensity ("Intensity", Range(0,1)) = 1
        _TargetColor ("Target Color", Color) = (0,0,1,1)
        _TargetColorDistance("TargetColorDistance",Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Intensity;
            float4 _TargetColor;
            float _TargetColorDistance;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 originalColor = tex2D(_MainTex, i.uv);
                float grayValue = (0.3*originalColor.x + 0.59*originalColor.y + 0.11*originalColor.z);
                float grayColor = float4(grayValue, grayValue, grayValue, originalColor.a);
                
                float4 grayScaledColor = lerp(originalColor, grayColor, _Intensity);

                // Now we have to choose between the gray color and the original color with a lerp
                float colorDistance = distance(originalColor, _TargetColor);
                float4 finalColor = colorDistance < _TargetColorDistance ? grayScaledColor : originalColor;
                return finalColor;
            }
            ENDCG
        }
    }
}
