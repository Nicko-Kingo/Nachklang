
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
    private bool waveActive;
	
    private float pingDistance;
    private float waveDistance;

    [SerializeField]
    private AudioClip audio;

    [SerializeField]
    private AudioSource source;

    private PlayerController player;
	private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        pingDistance = 50;
		//get the camera and tell it to render a depth texture
		Camera cam = GetComponent<Camera>();
		cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.Depth;
	}

	void Update(){
		//if the wave is active, make it move away, otherwise reset it
		
        if(Input.GetKeyDown("space"))
        {
            manual = true;
            pingDistance = 20;
            waveActive = true;
            waveDistance = 0;
            source.clip = audio;
            source.Play();
            player.pinged();
        }

        if(PostprocessMaterial.color.a > 0)
        {
            Color stuff = PostprocessMaterial.color;
            stuff.a -= .1f;
            PostprocessMaterial.color = stuff;
        }

        //Gives the player a little time to see how things look
        if(pingDistance > 0)
        {
            pingDistance -= waveSpeed * .9f *  Time.deltaTime;
        }


        if(waveActive)
        {
            waveDistance += waveSpeed * Time.deltaTime;
        } 
        else 
        {
            waveDistance = 0;
        }

      

	}

	//method which is automatically called by unity after the camera is done rendering
	private void OnRenderImage(RenderTexture source, RenderTexture destination){
		
		//draws the pixels from the source texture to the destination texture
		if(manual || pingDistance > 0)
        {
            PostprocessMaterial.SetFloat("_PingDistance", pingDistance);
             PostprocessMaterial.SetFloat("_WaveDistance", waveDistance);
            Graphics.Blit(source, destination, PostprocessMaterial);
            manual = false;
        }
        else
        {
            waveActive = false;
        }
	}
}