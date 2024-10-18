using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    // Start is called before the first frame update
    public void quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_WEBPLAYER
        Application.OpenURL(webplayerQuitURL);
        #else
        Application.Quit();
        #endif
    }

    public void play()
    {
        if(Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        SceneManager.LoadScene("MainScene");
        
    }

    public void backToStart()
    {
        if(Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        SceneManager.LoadScene("Start Screen");
    }

    public void tutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void pause()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
        foreach(Transform child in gameObject.transform)
        {
            if(child.gameObject.GetComponent<Button>() != null)
            {
                child.gameObject.SetActive(true);
            }
        }
    }

    public void unPause()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        foreach(Transform child in gameObject.transform)
        {
            if(child.gameObject.GetComponent<Button>() != null)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
    public void hit()
    {
        RawImage img = FindObjectOfType<RawImage>();

        img.color = new Color(1,1,1,1);

        StartCoroutine(fadeOut(img));
    }

    private IEnumerator fadeOut(RawImage img)
    {
        for (float i = 1; i >= 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                img.color = new Color(1, 1, 1, i);
                yield return null;
            }
        StopCoroutine(fadeOut(img));
    }
}
