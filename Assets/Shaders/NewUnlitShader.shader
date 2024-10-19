Shader "Unlit/NewUnlitShader"
{
    //show values to edit in inspector
    Properties{
        _MainTex ("main texture", 2D) = "white" {}
        [Header(Wave)]
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

            cbuffer MyBuffer : register(b0)
            {
                float _Waves[5];
            }


            //variables to control the wave/Ping
            float _WaveTrail;
            float4 _WaveColor;
            float _WaveDistance;
            int _NumWaves;

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
          

            //Absolute garbage, but I coded this in such a way that it works
            //I probably cannot remove anything
            //This was a holdover for a different system I was doing
            fixed4 frag(v2f i) : SV_TARGET
            {
                
                float depthRaw = tex2D(_CameraDepthTexture, i.uv).r;

                
                float4 enviroTex = tex2D(_MainTex, i.uv);

                //get depth from depth texture
                float depth =   lerp(.1, .9 , depthRaw) ;

                //linear depth between camera and far clipping plane
                depthRaw = Linear01Depth(depthRaw);

                //depth as distance from camera in units
                depthRaw = depthRaw * _ProjectionParams.z;

                float ping = 0;

                for(int i = 0; i < _NumWaves; i++)
                {
                    
                    //Creates the hard color at the exact point of the wave
                    float waveFront = step(depthRaw, _Waves[i]);

                    //Creates the trail of the wave based on depth
                    float waveTrail = smoothstep(_Waves[i] - _WaveTrail, _Waves[i], depthRaw);
                    float wave = waveFront * waveTrail;
                
                    
                    if(step( depth, _Waves[i]))
                    {
                        //Lerps between black and white
                        //adding to the ping allows every ping to exist simultaneously 
                        ping += lerp(0, _WaveColor, wave);
                    }

                }


                
                /*
                //If I want the ping to pass over the original textures
                //Envirotex is the original texture, cant have it downhere for some reason
                if(ping == 0.0)
                {
                    return enviroTex;
                }

                return lerp(enviroTex, _WaveColor, ping); 

                */
                
            
               
                return ping;
            }


            ENDCG
        }
    }
}