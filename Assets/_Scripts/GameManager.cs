using System;
using System.Collections;
using System.Numerics;
using _Scripts;
using UnityEngine;
using gs = StateManager.GameState;
using h = _Scripts.Helper;

public class GameManager : MonoBehaviour {
    public static GameManager instance = null;
    
    private StateManager sm;
    private UIController ui;
    private _Scripts.Load load;

    private int timesRepeated = 0;

    private int healthLimit;
    private int playersHealth;
    private int dealersHealth;

    private int stageNum = 0; // 0 is tutorial stage, 1 - 2 is normal, 3 is final

    [SerializeField] private float stdDelay = 4f;

    // allows gameManager to stay persistent across scenes
    public static GameManager Instance {
        get {
            if (instance == null)
                instance = FindObjectOfType<GameManager>();

            if (instance == null) {
                GameObject gObj = new GameObject();
                gObj.name = "GameManager";
                instance = gObj.AddComponent<GameManager>();
                DontDestroyOnLoad(gObj);
            }

            return instance;
        }
    }

    private void Awake() {
        if (instance == null) instance = this;
    }

    // Start is called before the first frame update
    private void Start() {
        sm = StateManager.Get();
        ui = UIController.instance;
        StartGame();
    }

    private void StartGame() {
        sm.SetState(gs.Start);
        // getItems()
        NewLoad(3, 1);
        StartCoroutine(TutorialStartDialogue());
    }

    private IEnumerator TutorialStartDialogue() {
        sm.SetState(gs.Dialogue);
        if (stageNum == 0) {
            yield return new WaitForSeconds(stdDelay);
            ui.LogText(
                $"A panel in the table flips up. In it are three shotgun shells. " +
                "One is <color=\"red\">red</color>. The others are <color=\"blue\">blue</color>.");
            yield return new WaitForSeconds(stdDelay);
            ui.LogText(SayLoad() + " it says.");
        } else {
            yield return new WaitForSeconds(stdDelay);
            ui.LogText(SayLoad());
        }
        yield return new WaitForSeconds(stdDelay);
        ui.LogText("The dealer loads the shells into the shotgun without looking.");
        if (stageNum == 0) {
            yield return new WaitForSeconds(stdDelay);
            ui.LogText(h.RandTwo(1,2) ? "\"I load the rounds in a hidden sequence.\"" : 
                "\"The rounds go into the chamber in an unknown order.\""); //todo make it so it wont repeat this.
        }
        yield return new WaitForSeconds(stdDelay);
        ui.LogText("It pumps the shotgun and then sets it down on the table.");
        
        yield return new WaitForSeconds(stdDelay);
        ui.LogText("\"Your turn.\"");
        
        PlayerTurn();
    }
    
    private string ShellsToString(int numRounds) {
        if (numRounds == 1) return "1 round";
        return $"{numRounds} rounds";
    }

    private string SayLoad() {
        string str = "\"";
        if (load.numLive == 1) str += "1 <color=\"red\">live</color> round. ";
        else str += $"{load.numLive} <color=\"red\">live</color> rounds. ";

        int numBlanks = load.numTotal - load.numLive;
        if (numBlanks == 1) str += "1 <color=\"blue\">blank</color>.";
        else str += $"{numBlanks} <color=\"blue\">blanks</color>.";
        
        str += "\"";
        return str;
    }

    private void PlayerTurn() {
        sm.SetState(gs.PlayerTurn);
        
    }

    public void NewLoad(int numShells = -1, int numLive = -1) {
        sm.SetState(gs.Load);
        load = new Load(numShells, numLive);
        Debug.Log(SayLoad());
        // string msg = $"Reloaded shotgun. {load.numLive} live shells, {load.numTotal - load.numLive} blanks";
        // db.LogText(msg);
    }

    public void Fire() {
        sm.SetState(gs.Fire);
        Boolean isLive = load.UseChamber();
        Debug.Log($"Pulled the trigger, isLive = {isLive}, {load.numTotal} shells left, {load.numLive} live.");
        if (isLive) {
            Invoke("DelayedFunction", 2f);
            ui.LogText("<b>BANG!</b>");
        }
        else {
            Invoke("DelayedFunction", 2f);
            ui.LogText("<i>click</i>");
        }
    }

    public void UpdateHealth() {
        
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