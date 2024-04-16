
using System.Collections;

using UnityEngine;
using UnityEngine.UIElements;


public class MonsterController : MonoBehaviour
{
    private float speed;

    private bool isHunting;
    private bool canHunt;

    private PlayerController player;

    private System.Random rnd;

    [SerializeField]
    private float spawnDistance;

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
        //speed = 0;
        baseSpeed = speed;
        ui = FindObjectOfType<UIController>();
        gameObject.GetComponent<CapsuleCollider>().enabled = false;

        
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
        moveTimer = 2;
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
        
        Vector3 playerPos = player.transform.position;
        Vector3 playerDirection = player.transform.forward;
        float distance = Random.Range(1f, spawnDistance);

        Vector3 spawn = playerPos + playerDirection*distance;

        spawn.x += Random.Range(-distance/1.3f, distance/1.3f);
        spawn.y = 0.75f;

        gameObject.transform.position = spawn;

        model.GetComponent<MeshRenderer>().enabled = true;
        Debug.Log(gameObject.transform.position.ToString());

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
        gameObject.GetComponent<CapsuleCollider>().enabled = false;

        
    }

    private void despawnMonster()
    {
        model.GetComponent<MeshRenderer>().enabled = false;
        gameObject.transform.position = new Vector3(99,99,99);
    }

    public void chase(bool pinged = false)
    {
        if(pinged || player.GetComponent<Rigidbody>().velocity.magnitude > 0)
        {
            currentTarget = player.transform.position;
        }
        else //choose a random point near the player
        {
            Vector3 pos = player.transform.position;
            Vector3 Newpos = new Vector3(Random.Range(pos.x-1.5f, pos.x+1.5f),0.75f,Random.Range(pos.z-1.5f,pos.z+1.5f));
            currentTarget = Newpos;
        }
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
        chaseAudio.pitch *= 1.30f;
    }

    private void move()
    {
        gameObject.transform.LookAt(currentTarget);
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, currentTarget, speed * Time.deltaTime);
    }

    private IEnumerator wait()
    {
        Debug.Log("wait started");
        yield return new WaitForSeconds(2f);

        Debug.Log("wait finished");

        gameObject.GetComponent<CapsuleCollider>().enabled = true;


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

        }

        //stop the hunt if there was a hit
    }


    public static Vector3 ScreenPositionIntoWorld(Vector2 screenPosition, float distance)
    {
      Ray ray = Camera.main.ScreenPointToRay(screenPosition);
      return ray.direction.normalized * distance;
    }

}
