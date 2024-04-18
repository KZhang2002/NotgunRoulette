using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotScript : MonoBehaviour {
    private int slotNum = -1;
    private Image img;

    private Color originalColor;
    // Start is called before the first frame update

    private void Awake() {
        int.TryParse(name, out slotNum); 
        img = GetComponent<Image>();
        originalColor = img.color;
        img.color = Color.clear;
    }

    void Start() {
        
    }

    // Update is called once per frame
    void Update() { }
}