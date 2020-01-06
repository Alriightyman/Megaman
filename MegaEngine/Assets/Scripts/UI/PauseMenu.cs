using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public Transform Selector;
    public Transform ResumeSelectionPos;
    public Transform QuitSeleectionPos;
    public GameObject PauseMenuObj;
    private float lastInput = 0f;
    public bool IsPaused { get; set; }
    int selection = 0; // 0 - resume, 1 - quit

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool pausedPressed = Input.GetButtonDown("Pause");
        if(pausedPressed && !IsPaused)
        {
            Pause();
        }
        else if(pausedPressed && IsPaused)
        {
            Unpause();
        }

        if(IsPaused)
        {
            HandleInput();
            UpdateSelector();
            ExecuteSelection();
        }
    }

    void Pause()
    {
        Time.timeScale = 0;
        GameEngine.Player.IsPlayerInactive = true;
        IsPaused = true;
        PauseMenuObj.SetActive(true);
    }

    void Unpause()
    {
        Time.timeScale = 1;
        GameEngine.Player.IsPlayerInactive = false;
        IsPaused = false;
        PauseMenuObj.SetActive(false);
    }

    private void ExecuteSelection()
    {
        if(Input.GetButtonDown("Submit"))
        {
            if(selection == 0)
            {
                Unpause();
            }
            else
            {
                Application.Quit();
            }
        }
    }

    private void UpdateSelector()
    {
        if(selection == 0)
        {
            Selector.position = ResumeSelectionPos.position;
        }
        else if(selection == 1)
        {
            Selector.position = QuitSeleectionPos.position;
        }

    }

    void HandleInput()
    {
        var input = Input.GetAxis("Vertical");
       
        if(input != 0 && lastInput != input)
        {
            selection = selection == 0 ? 1 : 0;
        }

        lastInput = input;
    }

}
