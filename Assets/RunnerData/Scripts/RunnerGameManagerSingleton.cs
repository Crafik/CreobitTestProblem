using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

public class RunnerGameManagerSingleton : MonoBehaviour
{
    public static RunnerGameManagerSingleton instance { get; private set; }

    [Header ("===== Object references =====")]
    [SerializeField] private TextMeshProUGUI bestTimeDisplay;
    [SerializeField] private TextMeshProUGUI currentTimeDisplay;

    [Space (5)]
    [SerializeField] private TextMeshProUGUI mainTextDisplay;

    [Space (5)]
    [SerializeField] private PlayerController m_player;

    private bool firstTimeFlag;
    private Vector3 startPos;
    private SaveDataHandler dataHandler;
    private float bestTime;
    private float currentTime;

    private Controls m_controls;

    [HideInInspector] public bool isStarted;

    // need to make start, finish, timer and save load of lowest time

    void Awake(){
        if (instance != null && instance != this){
            Destroy(this);
        }
        else{
            instance = this;
        }

        firstTimeFlag = true;
        startPos = m_player.gameObject.transform.position;
        isStarted = false;
        
        m_controls = new Controls();
        dataHandler = new SaveDataHandler(Path.Combine("Savedata", "Runner"), "runner.savedata");
        string saveData = dataHandler.Load();
        if (saveData.Length > 0){
            if(!float.TryParse(saveData, out bestTime)){
                bestTime = 0f;
            }
        }
        else{
            bestTime = 0f;
        }
        UpdateDisplay(ref bestTimeDisplay, bestTime);
        mainTextDisplay.enabled = true;
        mainTextDisplay.text = "Press 'Enter' to start!\nPress 'Escape' to exit to launcher";

        currentTime = 0f;
    }

    void OnEnable(){
        m_controls.Enable();

        m_controls.General.Start.performed += StartGame;
        m_controls.General.Exit.performed += ExitGame;
    }

    void OnDisable(){
        m_controls.Disable();

        m_controls.General.Start.performed -= StartGame;
        m_controls.General.Exit.performed -= ExitGame;
    }

    void StartGame(InputAction.CallbackContext ctx){
        if (firstTimeFlag){
            firstTimeFlag = false;
        }
        else{
            m_player.SetPlayerPosition(startPos);
        }
        isStarted = true;
        m_player.SetPlayerActive();
        mainTextDisplay.enabled = false;
        currentTime = 0f;
    }

    public void PlayerFinished(){
        isStarted = false;
        m_player.SetPlayerActive(false);
        mainTextDisplay.enabled = true;
        mainTextDisplay.text = "";
        if (currentTime < bestTime || bestTime == 0f){
            bestTime = currentTime;
            UpdateDisplay(ref bestTimeDisplay, bestTime);
            dataHandler.Save(bestTime.ToString());
            mainTextDisplay.text = "New record! -= " + bestTimeDisplay.text + "=-\n";
        }
        mainTextDisplay.text += "Press 'Enter' to try again!\nPress 'Escape' to exit to launcher";
    }

    private void UpdateDisplay(ref TextMeshProUGUI v_display, float v_time){
        string buf = Mathf.FloorToInt(v_time / 60f).ToString();
        buf += ':' + Mathf.FloorToInt(v_time % 60f).ToString("D2");
        buf += '.' + Mathf.FloorToInt(v_time % 1 * 100f).ToString("D2");

        v_display.text = buf;
    }

    void ExitGame(InputAction.CallbackContext ctx){
        SceneManager.LoadScene(0);
        // there probably should be something else also
    }

    void Update(){
        if (isStarted){
            currentTime += Time.deltaTime;
            UpdateDisplay(ref currentTimeDisplay, currentTime);
        }
    }
}
