using System.Collections;
using UnityEngine;
using _Scripts;
using UnityEngine;
using gs = StateManager.GameState;
using h = _Scripts.Helper;
using timer = _Scripts.Timer;
using Random = UnityEngine.Random;

public partial class GameManager : MonoBehaviour {
    // Set up methods
    private partial void Awake() {
        if (instance == null) instance = this;
        sm = StateManager.Get();
        // t = new timer();
        // indefT = new timer();
    }
    
    private partial void Start() {
        ui = UIController.instance;
        am = ActionManager.instance;
        sm.SetState(gs.Tutorial);
    }
    
    private partial void Test() {
        int numShells = 2;
        while (numShells < 9) {
            for (int i = 0; i < 5; i++) {
                NewLoad(numShells);
            }

            numShells++;
            Debug.Log("--------------------------------------------------");
        }
    }
    
    // Dialogue helper methods
    private partial string ShellsToString(int numRounds) {
        if (numRounds == 1) return "1 round";
        return $"{numRounds} rounds";
    }

    private partial string SayLoad() {
        string str = "\"";
        if (load.numLive == 1) str += "1 <color=\"red\">live</color> round. ";
        else str += $"{load.numLive} <color=\"red\">live</color> rounds. ";

        int numBlanks = load.numTotal - load.numLive;
        if (numBlanks == 1) str += "1 <color=\"blue\">blank</color>.";
        else str += $"{numBlanks} <color=\"blue\">blanks</color>.";

        str += "\"";
        return str;
    }
    
    // State transition function
    private partial void HandleStateChange(gs newState) {
        Debug.Log("New state: " + newState);
        state = newState;

        switch (state) {
            case gs.Tutorial:
                StartTutorial();
                break;
            case gs.Fire:
                break;
            //todo gun anim goes here
            case gs.PlayerTurn:
                StartCoroutine(PlayerTurn());
                break;
            case gs.DealerTurn:
                DealerTurn();
                break;
            case gs.GameWon:
                GameWon();
                break;
            default:
                break;
        }
    }
    
    private partial void StartTutorial() {
        // getItems()
        NewLoad(3, 1);
        SetHealth(false, 2);
        SetHealth(true, 2);
        StartCoroutine(TutorialStartDialogue());
        
    }

    private partial void StartLoad() {
        sm.SetState(gs.Load);
        // getItems()
        NewLoad();
        Random.Range(2, 5);
        SetHealth(false, 2);
        SetHealth(true, 2);
    }
    
    private partial IEnumerator TutorialStartDialogue() {
        sm.SetState(gs.Dialogue);
        yield return new WaitForSecondsRealtime(stdDelay);
        if (roundNum == 0) {
            ui.LogText(
                $"A panel in the table flips over to reveal its other side. Attached to it are three shotgun shells. " +
                "One is <color=\"red\">red</color>. The others are <color=\"blue\">blue</color>.");
            yield return new WaitForSecondsRealtime(stdDelay * 1.5f);
            ui.LogText("The dealer speaks.");
            yield return new WaitForSecondsRealtime(stdDelay / 2);
        }
        
        ui.LogText(SayLoad());
        yield return new WaitForSecondsRealtime(stdDelay * 1.5f);
        
        ui.LogText("The dealer starts to load the shells into the shotgun without looking.");
        yield return new WaitForSecondsRealtime(stdDelay);
        
        if (roundNum == 0) {
            ui.LogText(
                h.RandTwo(1, 2)
                    ? "\"I insert the rounds in an unknown sequence.\"" : 
                    "\"They go into the chamber in an unknown order.\""); //todo make it so it wont repeat this.
            yield return new WaitForSecondsRealtime(stdDelay);
        }
        
        ui.LogText("The dealer pumps the shotgun, sets it down on the table, and slides it to you.");
        yield return new WaitForSecondsRealtime(stdDelay);
        
        ui.LogText("\"Your turn.\"");
        yield return new WaitForSecondsRealtime(stdDelay/2);
        
        sm.SetState(gs.PlayerTurn);
    }
    
    public partial void NewLoad(int numShells = -1, int numLive = -1) {
        sm.SetState(gs.Load);
        load = new Load(numShells, numLive);
        Debug.Log(SayLoad());
        // string msg = $"Reloaded shotgun. {load.numLive} live shells, {load.numTotal - load.numLive} blanks";
        // db.LogText(msg);
    }

    private partial IEnumerator PlayerTurn() {
        if (isCuffed) {
            ui.LogText("You try to reach out to grab the shotgun only to realize you're still cuffed. " +
                       "Your turn is skipped.");
            sm.SetState(gs.DealerTurn);
            isCuffed = false;
        }

        while (playerHP > 0 && state != gs.Fire && load.numTotal > 0) {
            yield return null;
            //todo find way to pause indefinitely
        }
    }

    // private partial void UseItem() {
    //     sm.SetState(gs.UseItem);
    //     //todo play anim here
    //     
    // }

    private partial void DealerTurn() {
        am.Shoot(true);
        sm.SetState(gs.PlayerTurn);
    }

    private partial void GameWon() {
        
    }
    
    public partial void Fire(bool isTargetDealer) {
        sm.SetState(gs.Fire);
        bool isLive = load.UseChamber();
        Debug.Log($"Pulled the trigger, isLive = {isLive}, {load.numTotal} shells left, {load.numLive} live.");
        
        if (isLive) ui.LogText("<b>BANG!</b>");
        else ui.LogText("<i>click</i>");

        bool isShooterDealer = (state == gs.DealerTurn);

        if (load.numTotal <= 0) {
            sm.SetState(gs.Load);
            return;
        }
        if (isShooterDealer == isTargetDealer) {
            if (isShooterDealer) sm.SetState(gs.DealerTurn);
            else sm.SetState(gs.PlayerTurn);
            return;
        }
        
        if (isShooterDealer) sm.SetState(gs.PlayerTurn);
        else sm.SetState(gs.DealerTurn);
        
        if (isLive) UpdateHealth(isTargetDealer);
    }

    public partial void UpdateHealth(bool targetIsDealer, int amount = -1) {
        if (targetIsDealer) dealerHP += amount;
        else playerHP += amount;
        
        string subject = targetIsDealer ? "The dealer now has" : "You now have";
        int subjectHP = targetIsDealer ? dealerHP : playerHP;
        ui.LogText($"{subject} {subjectHP} lives."); //todo lives or charges?
    }
    
    public partial void SetHealth(bool targetIsDealer, int amount = -1) {
        if (amount == -1) amount = Random.Range(2, 5);
        
        if (targetIsDealer) dealerHP = amount;
        else playerHP = amount;
        
        string subject = targetIsDealer ? "The dealer now has" : "You now have";
        int subjectHP = targetIsDealer ? dealerHP : playerHP;
        ui.LogText($"{subject} {subjectHP} lives."); //todo lives or charges?
    }
}
