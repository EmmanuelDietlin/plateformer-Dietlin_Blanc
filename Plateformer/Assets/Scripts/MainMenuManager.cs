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
    private float changeButtonTimer = .3f;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0f;
        mainMenuButtons = mainMenu.GetComponentsInChildren<Button>();
        controlsMenuButtons = controlsMenu.GetComponentsInChildren<Button>();
        Debug.Log(mainMenuButtons.Length);
        Debug.Log(controlsMenuButtons.Length);
        mainMenuSelectedButton = 0;
        controlsMenuSelectedButton = 0;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mainMenuButtons[mainMenuSelectedButton].gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (Input.GetAxisRaw("Dash") > 0 && controlsMenu.activeSelf)
        {
            SwitchMenu();
        }
        if (Input.GetAxisRaw("Horizontal") != 0 && timer > changeButtonTimer) {
            timer = 0f;
            if (mainMenu.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(null);
                mainMenuSelectedButton = (mainMenuSelectedButton + 1) % mainMenuButtons.Length;
                EventSystem.current.SetSelectedGameObject(mainMenuButtons[mainMenuSelectedButton].gameObject);

            }
            else
            {
                EventSystem.current.SetSelectedGameObject(null);
                controlsMenuSelectedButton = (controlsMenuSelectedButton + 1) % controlsMenuButtons.Length;
                EventSystem.current.SetSelectedGameObject(controlsMenuButtons[controlsMenuSelectedButton].gameObject);
            }
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
