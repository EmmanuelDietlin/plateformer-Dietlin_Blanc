using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject controlsMenu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchMenu()
    {
        mainMenu.SetActive(!mainMenu.activeSelf);
        controlsMenu.SetActive(!controlsMenu.activeSelf);
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
