
using System.Collections.Generic;
using UnityEngine;

public class Ping : MonoBehaviour
{


	//material that's applied when doing postprocessing
	[SerializeField]
	public Material PostprocessMaterial;

    private bool manual = false;

    [SerializeField]
    private float waveSpeed;
    [SerializeField]
    private int maxWaveDistance;
    [SerializeField]
    private bool waveActive;
	
    private float waveDistance;

    List<float> waves;

    [SerializeField]
    private AudioClip audio;

    [SerializeField]
    private AudioSource source;

    private PlayerController player;

   

	private void Start()
    {
        player = FindObjectOfType<PlayerController>();
		//get the camera and tell it to render a depth texture
		Camera cam = GetComponent<Camera>();
		cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.Depth;

        waves = new List<float>
        {
            -1,
            -1,
            -1,
            -1,
            -1
        };
        
	}

	void Update(){
		//if the wave is active, make it move away, otherwise reset it
		
        if(Input.GetKeyDown("space"))
        {
            manual = true;
            waveActive = true;
            waveDistance = 0;
            waves.Add(0);
            source.clip = audio;
            source.Play();
            player.pinged();
            
            if(waves.Count > 5)
            {
                waves.RemoveAt(0);
            }

            
            Debug.Log(0 + " " + waves[0] + " \n" +
                      1 + " " + waves[1] + " \n" +
                      2 + " " + waves[2] + " \n" +
                      3 + " " + waves[3] + " \n" +
                      4 + " " + waves[4] + " ");

        }

   


        //Doesn't do anything I think
        

        for(int i = 0; i < waves.Count; i++)
        {
            if(waves[i] != -1)
            {
                waves[i] += waveSpeed * Time.deltaTime;
            }
            if(waves[i] > maxWaveDistance)
            {
                waves.RemoveAt(i);
                waves.Insert(0, -1.0f);
            }
        }

        

      

	}

	//method which is automatically called by unity after the camera is done rendering
	private void OnRenderImage(RenderTexture source, RenderTexture destination){
		
        /*
		//draws the pixels from the source texture to the destination texture
		if(manual || waveDistance < 75)
        {
             PostprocessMaterial.SetFloat("_WaveDistance", waveDistance);
            Graphics.Blit(source, destination, PostprocessMaterial);
            manual = false;
        }
        else
        {
            waveActive = false;
        }
        */

       //draws the pixels from the source texture to the destination texture
		if(waves.Count > 0)
        {
            
            PostprocessMaterial.SetInt("_NumWaves", waves.Count);
            PostprocessMaterial.SetFloatArray("_Waves", waves);
            Graphics.Blit(source, destination, PostprocessMaterial);
            manual = false;
        }
        else
        {
            waveActive = false;
        }
	}
}