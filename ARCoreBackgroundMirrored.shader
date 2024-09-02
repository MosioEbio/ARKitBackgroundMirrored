Shader "Unlit/ARKitBackgroundMirrored"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EnvironmentDepth ("Environment Depth", 2D) = "black" {}
        [Toggle] _MirrorHorizontal ("Mirror Horizontal", Float) = 1
        [Toggle] _MirrorVertical ("Mirror Vertical", Float) = 1
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque"
            "Queue"="Background"
            "ForceNoShadowCasting"="True"
        }

        Pass
        {
            Cull Off
            ZWrite On
            ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 texcoord : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _EnvironmentDepth;
            float4x4 _UnityDisplayTransform;
            float _MirrorHorizontal;
            float _MirrorVertical;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                // Apply vertical mirroring if enabled
                float2 texcoord = v.texcoord;
                if (_MirrorVertical > 0.5)
                {
                    texcoord.y = 1.0 - texcoord.y;
                }
                
                // Apply the display transform
                o.texcoord = mul(float3(texcoord, 1.0), _UnityDisplayTransform).xy;
                
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.texcoord;
                
                // Apply horizontal mirroring if enabled
                if (_MirrorHorizontal > 0.5)
                {
                    uv.x = 1.0 - uv.x;
                }
                
                // Sample the camera texture
                fixed4 col = tex2D(_MainTex, uv);
                
                // Optional: Sample the environment depth if needed
                // float depth = tex2D(_EnvironmentDepth, uv).r;
                
                return col;
            }
            ENDCG
        }
    }
}