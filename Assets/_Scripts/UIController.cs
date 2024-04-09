using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using TMPro;
using gs = StateManager.GameState;

public class UIController : MonoBehaviour {
    private static UIController instance;

    private StateManager sm;

    [SerializeField] private string startText = "Placeholder";
    [SerializeField] private string stateTextChildName = "StateText";
    private TextMeshProUGUI stateText;

    [SerializeField] private string logTextChildName = "LogText";
    [SerializeField] private int logTextBufferSize = 1000;
    private TextMeshProUGUI log;
    private string logText;

    void Awake() {
        instance = this;
        sm = StateManager.Get();

        stateText = transform.Find(stateTextChildName).GetComponent<TextMeshProUGUI>();
        if (!stateText) Debug.Log("No state text child found.");
        else Debug.Log("State text child found.");

        log = transform.Find(logTextChildName).GetComponent<TextMeshProUGUI>();
        logText = "";
        if (!log) Debug.Log("No log text child found.");
        else Debug.Log("Log text child found.");

        log.text = logText;
    }

    private void OnEnable() {
        sm.OnStateChange += HandleStateChange;
    }

    private void OnDisable() {
        sm.OnStateChange -= HandleStateChange;
    }

    private void HandleStateChange(gs newState) {
        stateText.text = newState.ToString();
    }

    public void LogText(string msg) {
        logText += "\n\n" + msg;

        Debug.Log("Log: " + logText);
        if (logText.Length > 1000) {
            int charactersToRemove = logText.Length - 1000;
            logText = logText.Substring(charactersToRemove);
        }

        log.text = logText;
    }

    void Start() { }

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