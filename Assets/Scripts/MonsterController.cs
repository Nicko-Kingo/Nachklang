
using System.Collections;

using UnityEngine;


public class MonsterController : MonoBehaviour
{
    private float speed;

    private bool isHunting;
    private bool canHunt;

    private PlayerController player;

    private System.Random rnd;

    [SerializeField]
    private float spawnDistance = 15f;

    private AudioSource chaseAudio;


    [SerializeField]
    private GameObject model;

    private Vector3 currentTarget;


    //waiting to move;
    private int moveTimer;

    private float baseSpeed;

    private UIController ui;

    // Start is called before the first frame update
    void Start()
    {
        chaseAudio = GetComponent<AudioSource>();
        rnd = new System.Random();
        isHunting = false;
        player = FindObjectOfType<PlayerController>();
        speed = player.getSpeed() * .80f;
        baseSpeed = speed;
        ui = FindObjectOfType<UIController>();
        
    }

    // Update is called once per frame
    void Update()
    {

        if(isHunting && moveTimer == 0)
        {
            StopCoroutine(wait());
            move();
        }
        
    }


    //Randomly roll to see if the monster should hunt
    public bool huntRoll()
    {
        int thing = 6 + player.getPingBuffer();
        if(thing < 1)
        {
            thing = 1;
        }
        isHunting = rnd.Next(1, thing) == 1;
        if(isHunting)
        {
            
            Debug.Log("The Monster is Hunting");
            startHunt();
        }

        
        return isHunting;
    }

    public void stopHunting()
    {
        isHunting = false;
        canHunt = false;
    }

    public bool checkHuntStatus()
    {
        return isHunting;
    }

    private void startHunt()
    {
        moveTimer = 1;
        chase();
        spawnMonster();
        chaseAudio.Play();
        //play music
        //start some coroutine
        StartCoroutine(chasing());
        StartCoroutine(wait());
        


    }


    private void spawnMonster()
    {
        
            float random1 = UnityEngine.Random.Range(0f,1f);
            float random2 = UnityEngine.Random.Range(0f,1f);

            Vector3 randomPositionOnScreen = ScreenPositionIntoWorld(
            // example screen center:
            new Vector2(random1, random2),
            // distance into the world from the screen:
            UnityEngine.Random.Range(.25f, spawnDistance)
            );


        

        gameObject.transform.position = new Vector3(randomPositionOnScreen.x, 1f, randomPositionOnScreen.normalized.z * spawnDistance);

        model.GetComponent<MeshRenderer>().enabled = true;

    }

    public void stopHunt()
    {
        Debug.Log("Hunt Stopped");
        chaseAudio.Stop();
        //stop music
        despawnMonster();
        stopHunting();
        StopCoroutine(chasing());
        speed = baseSpeed;
    }

    private void despawnMonster()
    {
        model.GetComponent<MeshRenderer>().enabled = false;
    }

    public void chase()
    {
        currentTarget = player.transform.position;
    }

    private IEnumerator chasing()
    {
        while(isHunting)
        {
            chase();

            yield return new WaitForSeconds(2);
        }
    }

    public void speedUp()
    {
        speed *= 1.30f;
    }

    private void move()
    {
        gameObject.transform.LookAt(currentTarget);
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, currentTarget, speed * Time.deltaTime);
    }

    private IEnumerator wait()
    {
        Debug.Log("wait started");
        yield return new WaitForSeconds(1f);
        Debug.Log("wait finished");


        moveTimer = 0;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Idk man");
        if(collision.gameObject.layer == 6)
        {
            player.damage();
            stopHunt();
            
            ui.hit();

            gameObject.transform.position = new Vector3(99,99,99);
        }

        //stop the hunt if there was a hit
    }


    public static Vector3 ScreenPositionIntoWorld(Vector2 screenPosition, float distance)
    {
      Ray ray = Camera.main.ScreenPointToRay(screenPosition);
      return ray.direction.normalized * distance;
    }

}
