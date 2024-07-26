using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LauncherManagerSingleton : MonoBehaviour
{
    public static LauncherManagerSingleton instance { get; private set; }

    [Header ("===== Canvas objects =====")]
    [Header (" === Clicker === ")]
    [SerializeField] private GameObject clickerLoadButton;
    [SerializeField] private GameObject clickerUnloadButton;
    [SerializeField] private GameObject clickerPlayButton;
    [SerializeField] private GameObject clickerStatusBar;

    [Space (5)]
    [Header (" === Runner === ")]
    [SerializeField] private GameObject runnerLoadButton;
    [SerializeField] private GameObject runnerUnloadButton;
    [SerializeField] private GameObject runnerPlayButton;
    [SerializeField] private GameObject runnerStatusBar;

    private GameObject[] clickerObjects;
    private GameObject[] runnerObjects;

    private Slider clickerSlider;
    private Slider runnerSlider;

    // not the best solution, but will do for now
    private readonly string clickerDataFilename = "clickerdata_assets_all_079b52499c1517cc9843cbbe9fa82af5.bundle";
    private readonly string clickerSceneFilename = "clickerdata_scenes_all_fb6f6d6f9c0a9cc0ae35763afd2f3a41.bundle";
    private readonly string runnerDataFilename = "runnerdata_assets_all_7d01ea8d0d2e6b9e6eef3c539fa4b2bf.bundle";
    private readonly string runnerSceneFilename = "runnerdata_scenes_all_107e6492109e50364dc3397f4f8af202.bundle";
    private readonly string savePath = Path.Combine("DownloadedData", "StandaloneWindows64");

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
            clickerStatusBar
        };
        runnerObjects = new GameObject[]{
            runnerLoadButton,
            runnerUnloadButton,
            runnerPlayButton,
            runnerStatusBar
        };
    }

    void Start(){
        // here check if files is already present
        bool existsFlag = true;
        foreach (string name in new string[]{clickerDataFilename, clickerSceneFilename}){
            if (!File.Exists(Path.Combine(savePath, name))){
                existsFlag = false;
                UnloadAssets(true);
                break;
            }
        }
        SetGameLoaded(true, existsFlag);

        existsFlag = true;
        foreach (string name in new string[]{runnerDataFilename, runnerSceneFilename}){
            if (!File.Exists(Path.Combine(savePath, name))){
                existsFlag = false;
                UnloadAssets(false);
                break;
            }
        }
        SetGameLoaded(false, existsFlag);
    }

    private void LoadAssets(bool isClicker){

        GameObject loadbutton = isClicker ? clickerObjects[0] : runnerObjects[0];
        loadbutton.GetComponent<Button>().interactable = false;

        (isClicker ? clickerObjects[3] : runnerObjects[3]).SetActive(true);

        string[] filenames = isClicker ?
            new string[]{clickerDataFilename, clickerSceneFilename}:
            new string[]{runnerDataFilename, runnerSceneFilename};
        StartCoroutine(FTPConnection(filenames, isClicker));

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
                successFlag = false;
                break;
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
        SetGameLoaded(isClicker, true);
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
        SetGameLoaded(isClicker, false);
    }

    private void SetGameLoaded(bool isClicker, bool isLoaded){
        GameObject[] objects = isClicker ? clickerObjects : runnerObjects;
        objects[0].GetComponent<Button>().interactable = !isLoaded;
        objects[1].GetComponent<Button>().interactable = isLoaded;
        objects[2].GetComponent<Button>().interactable = isLoaded;
        objects[3].SetActive(false);
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
        LoadAssets(false);
    }

    public void UnloadRunner(){
        UnloadAssets(false);
    }

    public void PlayRunner(){
        Addressables.LoadSceneAsync("RunnerData");
    }
}
