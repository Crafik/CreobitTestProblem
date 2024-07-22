using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider collision){
        RunnerGameManagerSingleton.instance.PlayerFinished();
    }
}
