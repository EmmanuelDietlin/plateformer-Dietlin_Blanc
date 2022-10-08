using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    [SerializeField] private GameObject modificationMenu;
    // Start is called before the first frame update
    void Start()
    {
        modificationMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCheckBoxChange()
    {
        modificationMenu.SetActive(!(modificationMenu.activeSelf));
    }
}
