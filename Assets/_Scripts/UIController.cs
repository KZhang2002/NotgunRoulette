using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using gs = StateManager.GameState;
using h = _Scripts.Helper;

public class UIController : MonoBehaviour {
    public static UIController instance;

    private StateManager sm;

    [SerializeField] private string startText = "Placeholder";
    [SerializeField] private string stateTextChildName = "StateText";
    private TextMeshProUGUI stateText;

    [SerializeField] private string logTextChildName = "LogText";
    [SerializeField] private int logTextBufferSize = 1000;
    private TextMeshProUGUI log;
    private string logText;
    private string logTextFull;
    private Queue<Tuple<string, float>> logQueue = new Queue<Tuple<string, float>>();
    private Queue<IEnumerator> corQueue = new Queue<IEnumerator>();
    private int corsRunning = 0;

    void Awake() {
        if (instance == null) instance = this;
        sm = StateManager.Get();
        //sm.OnStateChange += HandleStateChange;

        stateText = transform.Find(stateTextChildName).GetComponent<TextMeshProUGUI>();
        if (!stateText) Debug.Log("No state text child found.");
        else Debug.Log("State text child found.");

        log = transform.Find(logTextChildName).GetComponent<TextMeshProUGUI>();
        logText = "";
        if (!log) Debug.Log("No log text child found.");
        else Debug.Log("Log text child found.");

        log.text = logText;
    }

    void Start() {
        //StartCoroutine(ExecuteLogQueue());
        StartCoroutine(ManageCoroutines());
    }

    private void OnEnable() {
        sm.OnStateChange += HandleStateChange;
    }

    private void OnDisable() {
        sm.OnStateChange -= HandleStateChange;
    }

    public void DisableBtns() {
        Button[] buttons = GetComponentsInChildren<Button>();
        foreach (Button btn in buttons) {
            btn.interactable = false;
        }
    }

    public void EnableBtns() {
        Button[] buttons = GetComponentsInChildren<Button>();
        foreach (Button btn in buttons) {
            btn.interactable = true;
        }
    }

    private void HandleStateChange(gs newState) {
        stateText.text = newState.ToString();
        logTextFull += "\n\n State changed to: " + newState;

        switch (newState) {
            case gs.PlayerTurn:
                EnableBtns();
                break;
            default:
                DisableBtns();
                break;
        }
    }

    public void LogText(string[] msg) {
        foreach (string str in msg) {
            logQueue.Enqueue(Tuple.Create(str, 1f));
        }
    }

    public void LogText(string msg) {
        logQueue.Enqueue(Tuple.Create(msg, 1f));
    }

    public void Speak(gs state) {
        corQueue.Enqueue(ExecuteLogQueue(state));
    }

    public void Speak() {
        corQueue.Enqueue(ExecuteLogQueue());
    }

    public IEnumerator ExecuteLogQueue(gs state) {
        sm.SetState(gs.Dialogue);
        yield return new WaitForSeconds(1f);
        while (logQueue.Count > 0) {
            var tpl = logQueue.Dequeue();
            string msg = tpl.Item1;
            logText += "\n\n" + msg;
            logTextFull += "\n\n" + msg;

            if (logText.Length > 1000) {
                int charactersToRemove = logText.Length - 1000;
                logText = logText.Substring(charactersToRemove);
            }

            Debug.Log("DIALOGUE: " + msg);

            log.text = logText;
            yield return new WaitForSeconds(tpl.Item2);
        }

        sm.SetState(state);
    }

    public IEnumerator ExecuteLogQueue() {
        sm.SetState(gs.Dialogue);
        yield return new WaitForSeconds(1f);
        while (logQueue.Count > 0) {
            var tpl = logQueue.Dequeue();
            string msg = tpl.Item1;
            logText += "\n\n" + msg;
            logTextFull += "\n\n" + msg;

            if (logText.Length > 1000) {
                int charactersToRemove = logText.Length - 1000;
                logText = logText.Substring(charactersToRemove);
            }

            Debug.Log("DIALOGUE: " + msg);

            log.text = logText;
            yield return new WaitForSeconds(tpl.Item2);
        }
    }

    IEnumerator ManageCoroutines() {
        while (true) {
            // If there are coroutines in the queue and no coroutine is running, start the next one
            if (corQueue.Count > 0) {
                StartCoroutine(corQueue.Dequeue());
            }

            yield return null;
        }
    }

    void OnApplicationQuit() {
        Debug.Log("Starting to write log.");
        string folderName = "SavedData";
        string currentTime = DateTime.Now.ToString("MMMM dd, HH,mm,ss");
        string fileName = currentTime + ".txt";
        string folderPath = Path.Combine(Application.dataPath, folderName); // Path to the folder within Assets
        string filePath = Path.Combine(folderPath, fileName); // Path to the file

        // Create folder if not there
        if (!Directory.Exists(folderPath)) {
            Directory.CreateDirectory(folderPath);
        }

        // Write log
        using (StreamWriter file = new StreamWriter(filePath)) {
            file.Write(logTextFull);
        }

        Debug.Log("Log saved at: " + filePath);
    }

    void Update() {
        // stateText = transform.Find(stateTextChildName).GetComponent<TextMeshPro>();
        //
        // if (!stateText) {
        //     Debug.Log("No state text child found.");
        // }
        // else {
        //     Debug.Log(stateText.text);
        // }
    }
}