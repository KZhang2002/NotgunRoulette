using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugUIController : MonoBehaviour {
    private static DebugUIController instance;
    [SerializeField] private string startText = "Placeholder";
    [SerializeField] private string stateTextChildName = "StateText";
    private TextMeshProUGUI stateText;
    private StateManager sm;

    void Awake() {
        instance = this;
        stateText = transform.Find(stateTextChildName).GetComponent<TextMeshProUGUI>();
        sm = StateManager.Get();

        if (!stateText) {
            Debug.Log("No state text child found.");
        }
        else {
            Debug.Log(stateText.text);
        }
            
    }

    private void OnEnable() {
        sm.OnStateChange += HandleStateChange;
    }
    
    private void OnDisable()
    {
        sm.OnStateChange -= HandleStateChange;
    }
    
    private void HandleStateChange(StateManager.GameState newState)
    {
        stateText.text = "Game State: " + newState.ToString();
    }

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
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