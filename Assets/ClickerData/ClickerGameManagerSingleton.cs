using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickerGameManagerSingleton : MonoBehaviour
{
    public static ClickerGameManagerSingleton instance { get; private set; }

    [Header ("==== Object references =====")]
    [SerializeField] private TextMeshProUGUI coinCount;

    private SaveDataHandler m_DataHandler;

    private int coins;

    void Awake(){
        if (instance != null && instance != this){
            Destroy(this);
        }
        else{
            instance = this;
        }
    }

    void Start(){
        m_DataHandler = new SaveDataHandler(Path.Combine("Savedata", "Clicker"), "clicker.savedata");
        string saveData = m_DataHandler.Load();
        if (saveData.Length > 0){
            SetCoins(int.Parse(saveData));
        }
        else{
            SetCoins(0);
        }
    }

    public void AddCoin(){
        coins += 1;
        SetCoinCounter();
    }

    public void SetCoins(int cns){
        coins = cns;
        SetCoinCounter();
    }

    public int GetCoinCount(){
        return coins;
    }

    private void SetCoinCounter(){
        coinCount.text = coins.ToString();
    }

    public void ExitGame(){
        SceneManager.LoadScene(0);
    }

    void OnDestroy(){
        m_DataHandler.Save(coins.ToString());
    }
}
