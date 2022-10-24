using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject controlsMenu;

    private Button[] mainMenuButtons;
    private Button[] controlsMenuButtons;

    private int mainMenuSelectedButton;
    private int controlsMenuSelectedButton;
    private float changeButtonTimer = .1f;
    private float timer;
    private float quitDelay = .3f;
    private float quitTimer;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0f;
        quitTimer = 0f;
        mainMenuButtons = mainMenu.GetComponentsInChildren<Button>();
        controlsMenuButtons = controlsMenu.GetComponentsInChildren<Button>();
        mainMenuSelectedButton = 0;
        controlsMenuSelectedButton = 0;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mainMenuButtons[mainMenuSelectedButton].gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        quitTimer += Time.deltaTime;
        if (Input.GetAxisRaw("Dash") > 0 && controlsMenu.activeSelf)
        {
            SwitchMenu();
        }
        if (timer > changeButtonTimer && Input.GetAxisRaw("Horizontal") != 0) {
            timer = 0f;
            int dir = (int)Input.GetAxisRaw("Horizontal");
            Debug.Log(dir);
            if (mainMenu.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(null);
                if (dir < 0 && mainMenuSelectedButton == 0) 
                    mainMenuSelectedButton = mainMenuButtons.Length - 1;
                else 
                    mainMenuSelectedButton = (mainMenuSelectedButton + dir) % mainMenuButtons.Length;
                EventSystem.current.SetSelectedGameObject(mainMenuButtons[mainMenuSelectedButton].gameObject);

            }
            else
            {
                EventSystem.current.SetSelectedGameObject(null);
                if (dir < 0 && controlsMenuSelectedButton == 0) controlsMenuSelectedButton = controlsMenuButtons.Length - 1;
                else controlsMenuSelectedButton = (controlsMenuSelectedButton + dir) % controlsMenuButtons.Length;
                EventSystem.current.SetSelectedGameObject(controlsMenuButtons[controlsMenuSelectedButton].gameObject);
            }
        }
        if (Input.GetAxisRaw("Cancel") > 0 && quitTimer > quitDelay)
        {
            quitTimer = 0f;
            if (controlsMenu.activeSelf) SwitchMenu();
            else Application.Quit();
        }
    }


    public void SwitchMenu()
    {
        mainMenu.SetActive(!mainMenu.activeSelf);
        controlsMenu.SetActive(!controlsMenu.activeSelf);
        mainMenuSelectedButton = 0;
        controlsMenuSelectedButton = 0;
        EventSystem.current.SetSelectedGameObject(null);
        if (mainMenu.activeSelf) 
            EventSystem.current.SetSelectedGameObject(mainMenuButtons[mainMenuSelectedButton].gameObject);
        else 
            EventSystem.current.SetSelectedGameObject(controlsMenuButtons[controlsMenuSelectedButton].gameObject);
    }

    public void StartLevel(int level)
    {
        StartCoroutine(loadLevel(level));
    }

    private IEnumerator loadLevel(int level)
    {
        yield return SceneManager.LoadSceneAsync(level);
    }

}
