using System;
using System.Numerics;
using _Scripts;
using UnityEngine;
using gs = StateManager.GameState;

public class GameManager : MonoBehaviour {
    private StateManager sm;
    private UIController db;
    private _Scripts.Load load;
    
    private int timesRepeated = 0;

    private int playersHealth;
    private int dealersHealth;

    private int stageNum;
    

    // Start is called before the first frame update
    private void Start() {
        sm = StateManager.Get();
        sm.SetState(gs.Start);
        db = GameObject.FindWithTag("UIController").GetComponent<UIController>();
        NewLoad();
        //InvokeRepeating("Fire", 0f, 1f);
    }

    // Update is called once per frame
    private void Update() {
        
    }

    public void NewLoad(int numShells = -1) {
        sm.SetState(gs.Load);
        load = new Load(numShells);
        Debug.Log($"Reloaded shotgun, {load.numTotal} shells, {load.numLive} live.");
        string msg = $"Reloaded shotgun. {load.numLive} live shells, {load.numTotal - load.numLive} blanks";
        db.LogText(msg);
    }

    public void Fire() {
        sm.SetState(gs.PlayerTurn);
        if (load.numTotal <= 0) {
            NewLoad();
        }
        
        Boolean isLive = load.UseChamber();
        Debug.Log($"Pulled the trigger, isLive = {isLive}, {load.numTotal} shells left, {load.numLive} live.");
        if (isLive) {
            string msg = "<b>BANG!!!</b>";
            Invoke("DelayedFunction", 2f);
            db.LogText(msg);
        }
        else {
            string msg = "<i>CLICK</i>";
            Invoke("DelayedFunction", 2f);
            db.LogText(msg);
        }
    }

    private void Test() {
        int numShells = 2;
        while (numShells < 9) {
            for (int i = 0; i < 5; i++) {
                NewLoad(numShells);
            }
            numShells++;
            Debug.Log("--------------------------------------------------");
        }
    }
}