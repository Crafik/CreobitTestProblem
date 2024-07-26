using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LauncherManagerSingleton : MonoBehaviour
{
    public static LauncherManagerSingleton instance { get; private set; }

    [Header ("===== Canvas objects =====")]
    [Header (" === Clicker === ")]
    [SerializeField] private GameObject clickerLoadButton;
    [SerializeField] private GameObject clickerUnloadButton;
    [SerializeField] private GameObject clickerPlayButton;
    [SerializeField] private GameObject clickerProgressBar;

    [Space (5)]
    [Header (" === Runner === ")]
    [SerializeField] private GameObject runnerLoadButton;
    [SerializeField] private GameObject runnerUnloadButton;
    [SerializeField] private GameObject runnerPlayButton;
    [SerializeField] private GameObject runnerProgressBar;

    private GameObject[] clickerObjects;
    private GameObject[] runnerObjects;

    private Slider clickerSlider;
    private Slider runnerSlider;

    private const string clickerDataFilename = "clickerdata_assets_all_079b52499c1517cc9843cbbe9fa82af5.bundle";
    private const string clickerSceneFilename = "clickerdata_scenes_all_00a3d141c3b09b0c97c232812d6473f4.bundle";
    private const string runnerDataFilename = "runnerdata_assets_all_7d01ea8d0d2e6b9e6eef3c539fa4b2bf.bundle";
    private const string runnerSceneFilename = "runnerdata_scenes_all_27cf44a746927d26624a986b7b450b44.bundle";
    private string savePath = Path.Combine("DownloadedData", "StandaloneWindows64");
    private AsyncOperationHandle asyncOperationHandle;

    void Awake(){
        if (instance != null && instance != this){
            Destroy(this);
        }
        else{
            instance = this;
        }

        clickerObjects = new GameObject[]{
            clickerLoadButton,
            clickerUnloadButton,
            clickerPlayButton,
            clickerProgressBar
        };
        runnerObjects = new GameObject[]{
            runnerLoadButton,
            runnerUnloadButton,
            runnerPlayButton,
            runnerProgressBar
        };

        for (int i = 1; i < 3; ++i){
            clickerObjects[i].GetComponent<Button>().interactable = false;
            runnerObjects[i].GetComponent<Button>().interactable = false;
        }
        clickerObjects[3].SetActive(false);
        clickerSlider = clickerObjects[3].GetComponent<Slider>();
        clickerSlider.interactable = false;
        runnerObjects[3].SetActive(false);
        runnerSlider = runnerObjects[3].GetComponent<Slider>();
        runnerSlider.interactable = false;
    }

    void Start(){
        // here check if files is already present
    }

    private void LoadAssets(bool isClicker){

        GameObject loadbutton = isClicker ? clickerObjects[0] : runnerObjects[0];
        loadbutton.GetComponent<Button>().interactable = false;

        string[] filenames = isClicker ?
            new string[]{clickerDataFilename, clickerSceneFilename}:
            new string[]{runnerDataFilename, runnerSceneFilename};
        StartCoroutine(FTPConnection(filenames, isClicker));


        // !!! IMPORTANT !!!
        // finish the logic here and then commit
        // message should probably be like:
        // "built remote assets, added loading logic, updated .gitignore"
    }

    private IEnumerator FTPConnection(string[] filenames, bool isClicker){

        UnityWebRequest request;
        string URL = "https://creobitdata567849549892159.b-cdn.net/StandaloneWindows64/";

        bool successFlag = true;

        foreach (string name in filenames){
            request = UnityWebRequest.Get(URL + name);
            request.downloadHandler = new DownloadHandlerFile(Path.Combine(savePath, name));
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success){
                Debug.LogError($"Error connecting to FTP: {request.error}");
                successFlag = false;
                break;
            }
            else{
                Debug.Log("FTP connection successful");
            }
            request.Dispose();
        }

        if (!successFlag){
            Debug.Log("Clearing files");
            UnloadAssets(isClicker);
        }
        else{
            LoadComplete(isClicker);
        }
    }

    private void LoadComplete(bool isClicker){
        GameObject[] objects = isClicker ? clickerObjects : runnerObjects;
        objects[1].GetComponent<Button>().interactable = true;
        objects[2].GetComponent<Button>().interactable = true;
    }

    private void UnloadAssets(bool isClicker){
        string[] filenames = isClicker ?
            new string[]{clickerDataFilename, clickerSceneFilename}:
            new string[]{runnerDataFilename, runnerSceneFilename};
        foreach(string name in filenames){
            string filepath = Path.Combine(savePath, name);
            if (File.Exists(filepath)){
                File.Delete(filepath);
            }
        }
    }

    public void LoadClicker(){
        LoadAssets(true);
    }

    public void UnloadClicker(){
        UnloadAssets(true);
    }

    public void PlayClicker(){
        Addressables.LoadSceneAsync("ClickerData");
    }

    public void LoadRunner(){
        // here be code
        Debug.Log(Addressables.RuntimePath);
    }

    public void UnloadRunner(){

    }

    public void PlayRunner(){
        SceneManager.LoadScene(2);
    }

    void Update(){
        // if (clickerProgressBar.gameObject.activeSelf){
        //     clickerProgressBar.value = asyncOperationHandle.PercentComplete;
        // }
    }
}
