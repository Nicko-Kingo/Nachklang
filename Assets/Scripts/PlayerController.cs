
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Camera cam;

    [SerializeField]
    private float speed;

    
    [SerializeField]
    private int health;


    public int sensitivity;

    private int pingBuffer;

    private MonsterController monster;

    private float huntTimer;

    private UIController ui;
    private bool isPaused;
    

    // Start is called before the first frame update
    void Start()
    {
        
        isPaused = false;
        monster = FindObjectOfType<MonsterController>();
        cam = FindObjectOfType<Camera>();
        ui = FindObjectOfType<UIController>();

       

        foreach(Transform child in ui.gameObject.transform)
        {
            if(child.gameObject.GetComponent<Button>() != null)
            {
                child.gameObject.SetActive(false);
            }
        }

        
        Cursor.visible = false; 
        Cursor.lockState = CursorLockMode.Locked;
        pingBuffer = 3;
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;


        gameObject.transform.Rotate(Vector3.up * mouseX);


        if(health == 0)
        {
            death();
        }

        if(huntTimer <= 0)
        {
            StopCoroutine(hunting());
            pingBuffer = 2;
            huntTimer = 8f;
        }

        if(Input.GetKeyDown("p"))
        {
            if(isPaused)
            {
                ui.unPause();
                isPaused = false;
            }
            else
            {
                isPaused = true;
                ui.pause();
            }
        }
    }

     public void Move()
    {
    
        Vector3 Movement = new Vector3 (Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    

        Vector3 CorrectedMovement = Movement.x * gameObject.transform.right + Movement.z * gameObject.transform.forward;
        gameObject.transform.position += CorrectedMovement * speed * Time.deltaTime;
    }

    private void death()
    {
        Cursor.visible = true; 
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Lose Screen");
    }

    private void win()
    {
        Cursor.visible = true; 
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Win Screen");
    }

    public void pinged()
    {
        pingBuffer -= 1;


        if(monster.checkHuntStatus())
        {
            huntTimer += 1f;
            monster.speedUp();
        }

        if(pingBuffer == 0)
        {
            Debug.Log("You can be hunted now!!!");
        }

        if(pingBuffer < 0)
        {
            
            if(!monster.checkHuntStatus() && monster.huntRoll())
            {
                huntTimer = 8f;
                StartCoroutine(hunting());
            }
        }
    }

    public int getPingBuffer()
    {
        return pingBuffer;
    }

    public float getSpeed()
    {
        return speed;
    }

    private IEnumerator hunting()
    {
        while(huntTimer > 0)
        {
            Debug.Log(huntTimer);
            yield return new WaitForSeconds(.5f);
            huntTimer -= .5f;
        }
        monster.stopHunt();
    }

    public void damage()
    {
        health -= 1;
        Debug.Log(health);
        StopCoroutine(hunting());
        huntTimer = 8f;
        
    }


    void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.layer == 7)
        {
            win();
        }

        //stop the hunt if there was a hit
    }


}
