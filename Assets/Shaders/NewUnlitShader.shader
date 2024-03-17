Shader "Unlit/NewUnlitShader"
{
    //show values to edit in inspector
    Properties{
        [HideInInspector]_MainTex ("Texture", 2D) = "white" {}
        [Header(Wave)]
        _PingDistance ("Distance from player", float) = 10
        _WaveDistance ("Distance of wave", float) = 10
        _WaveTrail ("Length of the trail", Range(0,8)) = 1
        _WaveColor ("Color", Color) = (1,0,0,1)
    }

    SubShader{
        // markers that specify that we don't need culling
        // or reading/writing to the depth buffer
        Cull Off
        ZWrite Off
        ZTest Always

        Pass{
            CGPROGRAM
            //include useful shader functions
            #include "UnityCG.cginc"

            //define vertex and fragment shader
            #pragma vertex vert
            #pragma fragment frag

            //texture and transforms of the texture
            sampler2D _MainTex;

            //variables to control the wave
            float _PingDistance;
            float _WaveTrail;
            float4 _WaveColor;
            float _WaveDistance;

            //the object data that's put into the vertex shader
            struct appdata{
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            //the data that's used to generate fragments and can be read by the fragment shader
            struct v2f{
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            //the vertex shader
            v2f vert(appdata v){
                v2f o;
                //convert the vertex positions from object space to clip space so they can be rendered
                o.position = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            //the fragment shader
            sampler2D _CameraDepthTexture;

            //the fragment shader
            fixed4 frag(v2f i) : SV_TARGET
            {
                float depthRaw = tex2D(_CameraDepthTexture, i.uv).r;
                //get depth from depth texture
                float depth =   lerp(.1, .9 , depthRaw) ;
                //linear depth between camera and far clipping plane
                depthRaw = Linear01Depth(depthRaw);
                //depth as distance from camera in units
                depthRaw = depthRaw * _ProjectionParams.z;

                float waveFront = step(depthRaw, _WaveDistance);
                float waveTrail = smoothstep(_WaveDistance - _WaveTrail, _WaveDistance, depthRaw);
                float wave = waveFront * waveTrail;
                
                if(step( depth * 40, _PingDistance))
                {
                    float thing = lerp(0, _WaveColor, wave);
                    return thing;
                }

                if(depthRaw >= _ProjectionParams.z)
                    return 0;
            
                //return depth;
                return 0;
            }


            ENDCG
        }
    }
}