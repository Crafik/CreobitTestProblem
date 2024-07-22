using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LauncherManagerSingleton : MonoBehaviour
{
    public static LauncherManagerSingleton instance { get; private set; }

    void Awake(){
        if (instance != null && instance != this){
            Destroy(this);
        }
        else{
            instance = this;
        }
    }

    // well, time to learn CloudContentDelivery

    public void PlayClicker(){
        SceneManager.LoadScene(1);
    }

    public void PlayRunner(){
        SceneManager.LoadScene(2);
    }
}
