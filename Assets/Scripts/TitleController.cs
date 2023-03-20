using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
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

        backButton.SetActive(true);
        enterButton.SetActive(true);
        
    }

    public void zoomAway()
    {
        controller.SetTrigger("ZoomOut");

        exitButton.SetActive(true);
        startButton.SetActive(true);

        enterButton.SetActive(false);
        backButton.SetActive(false);
    }

    public void exit()
    {
        Application.Quit();
    }


}
