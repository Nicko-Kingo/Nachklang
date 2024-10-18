
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
	
    private float waveDistance;

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
	}

	void Update(){
		//if the wave is active, make it move away, otherwise reset it
		
        if(Input.GetKeyDown("space"))
        {
            manual = true;
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
	}
}