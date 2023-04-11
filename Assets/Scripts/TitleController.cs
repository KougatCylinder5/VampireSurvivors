using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour
{

    public Camera mainCamera;
    public GameObject startButton;
    public GameObject exitButton;
    public GameObject enterButton;
    public GameObject backButton;

    private Animator controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = mainCamera.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startGame() 
    {
        SceneManager.LoadScene("Isolated Forest");
    }

    public void zoomCharacter()
    {
        controller.SetTrigger("ZoomIn");

        exitButton.SetActive(false);
        startButton.SetActive(false);

        
        StartCoroutine(delayedShowStart());
    }

    public void zoomAway()
    {
        controller.SetTrigger("ZoomOut");

        
        enterButton.SetActive(false);
        backButton.SetActive(false);
        StartCoroutine(delayedShowTitle());
    }

    public void exit()
    {
        Application.Quit();
    }
    
    public IEnumerator delayedShowStart()
    {
        yield return new WaitForSeconds(1);
        backButton.SetActive(true);
        enterButton.SetActive(true);
    }

    public IEnumerator delayedShowTitle()
    {
        yield return new WaitForSeconds(1);
        exitButton.SetActive(true);
        startButton.SetActive(true);

    }
}
