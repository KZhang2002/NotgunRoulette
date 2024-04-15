using System;
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
        StartCoroutine(StartGame());
    }
    
    IEnumerator StartGame() {
        yield return new WaitForSeconds(1f);
        sm.SetState(gs.Tutorial);
    }
    
    // private partial void Test() {
    //     int numShells = 2;
    //     while (numShells < 9) {
    //         for (int i = 0; i < 5; i++) {
    //             NewLoad(numShells);
    //         }
    //
    //         numShells++;
    //         Debug.Log("--------------------------------------------------");
    //     }
    // }
    
    // Dialogue helper methods
    private partial string ShellsToString(int numRounds) {
        if (numRounds == 1) return "1 round";
        return $"{numRounds} rounds";
    }

    private partial string SayLoad(bool saidByDealer) {
        string str = "";
        if (saidByDealer) str += "\"";
        if (load.numLive == 1) str += "1 <color=\"red\">live</color> round. ";
        else str += $"{load.numLive} <color=\"red\">live</color> rounds. ";

        int numBlanks = load.numTotal - load.numLive;
        if (numBlanks == 1) str += "1 <color=\"blue\">blank</color>.";
        else str += $"{numBlanks} <color=\"blue\">blanks</color>.";

        if (saidByDealer) str += "\"";
        return str;
    }
    
    // State transition function
    private partial void HandleStateChange(gs newState) {
        Debug.Log("New state: " + newState);
        state = newState;
        
        StopAllCoroutines();

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
            case gs.NewLoad:
                if (roundNum == 1) StartLoad(5, 3);
                else StartLoad();
                break;
            case gs.NewRound:
                StartRound();
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
        TutorialStartDialogue(); //todo reenable
        //sm.SetState(gs.PlayerTurn); // todo disable
    }
    
    private partial void TutorialStartDialogue() {
        ui.LogText(
            "A panel in the table flips over to reveal its other side. Attached to it are three shotgun shells. " +
            "One is <color=\"red\">red</color>. The others are <color=\"blue\">blue</color>.");
        ui.LogText("The dealer speaks.");
        ui.LogText(SayLoad(true));
        ui.LogText("The dealer starts to load the shells into the shotgun without looking.");


        if (turnNum == 1) ui.LogText("\"I insert the rounds in an unknown order.\"");
        else if (turnNum == 2) ui.LogText("\"They enter the chamber in a hidden sequence.\""); //todo make it so it wont repeat this.
        
        
        ui.LogText("The dealer pumps the shotgun, sets it down on the table, and slides it to you.");
        ui.LogText("\"Your turn.\"");
        ui.Speak();
        sm.SetState(gs.PlayerTurn);
    }

    private partial IEnumerator PlayerTurn() {
        wasLastRoundDealers = false;
        if (isPlayerCuffed >= 2) {
            ui.LogText("You try to reach out to grab the shotgun only to realize you're still cuffed. " +
                       "Your turn is skipped.");
            sm.SetState(gs.DealerTurn);
            isPlayerCuffed = 1;
        } else if (isPlayerCuffed >= 1) {
            ui.LogText("You break the cuffs. It's your turn.");
            isPlayerCuffed = 0;
        }

        while (true) {
            //Debug.Log("Playerturn pause");
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
        wasLastRoundDealers = true;
        if (isDealerCuffed >= 2) {
            ui.LogText("\"I'm still cuffed. Your turn\".");
            sm.SetState(gs.PlayerTurn);
            isDealerCuffed = 1;
        } else if (isDealerCuffed >= 1) {
            ui.LogText("The dealer breaks the cuffs.");
            ui.LogText("\"My turn.\"");
            isDealerCuffed = 0;
        }
        // todo placeholder logic, dealer always shoots himself
        am.Shoot(true, true);
        //while (playerHP > 0 && dealerHP > 0 && state != gs.Fire && load.numTotal > 0) yield return null;
    }

    private partial void GameWon() {
        
    }
    
    public partial void Fire(bool isTargetDealer, bool isShooterDealer) {
        bool isLive = load.UseChamber();
        Debug.Log($"Pulled the trigger, isLive = {isLive}, {load.numTotal} shells left, {load.numLive} live.");
        
        if (isLive) ui.LogText("<b>BANG!</b>");
        else ui.LogText("<i>click</i>");

        int dmg = isSawed ? -2 : -1;
        if (isLive) UpdateHealth(isTargetDealer, dmg);
        isSawed = false;

        if (dealerHP <= 0) {
            sm.SetState(gs.NewRound);
            // Player turn must be triggered here because unity fuckery
            sm.SetState(gs.PlayerTurn); 
            return;
        }

        if (playerHP <= 0) {
            sm.SetState(gs.GameLost);
            ui.LogText("You lost lol.");
            return;
        }
        
        if (load.numTotal <= 0) {
            StartCoroutine(TurnoverDialogue(isShooterDealer, isTargetDealer, isLive));
            Debug.Log("set state to new load from fire()");
            sm.SetState(gs.NewLoad);
            // Player turn must be triggered here because unity fuckery
            sm.SetState(gs.PlayerTurn);
            return;
        }
        
        if (isShooterDealer == isTargetDealer && !isLive) {
            Debug.Log($"Self shot, shooter is dealer: {isShooterDealer}, target is dealer: {isTargetDealer}");
            if (isShooterDealer) sm.SetState(gs.DealerTurn);
            else sm.SetState(gs.PlayerTurn);
        } else {
            if (isShooterDealer) sm.SetState(gs.PlayerTurn);
            else sm.SetState(gs.DealerTurn);
        }

        StartCoroutine(TurnoverDialogue(isShooterDealer, isTargetDealer, isLive));
    }

    private partial IEnumerator TurnoverDialogue(bool isShooterDealer, bool isTargetDealer, bool isLive) {
        yield return new WaitForSeconds(stdDelay);
        ui.LogText("Turnover dialogue here.");
        if (!isTargetDealer && isLive) ui.LogText("");
        
        if (isShooterDealer) ui.LogText("The dealer puts the shotgun down.");
        else ui.LogText("You put the shotgun back on the table.");
        yield return new WaitForSeconds(stdDelay);
    }

    private partial void StartRound() {
        InitHealth();
        StartLoad();
        ui.LogText(SayLoad(true)); // todo remove line
        Array.Clear(playerItems, 0, playerItems.Length);
        Array.Clear(dealerItems, 0, dealerItems.Length);
        StartCoroutine(StartRoundDialogue());
    }

    private partial IEnumerator StartRoundDialogue() {
        yield return new WaitForSeconds(stdDelay);
        ui.LogText("Start round dialogue here.");
    }

    private partial void StartLoad() {
        StartLoad(-1, -1);
    }
    
    private partial void StartLoad(int numShells, int numLive) {
        Debug.Log("Started new load.");
        // getItems()
        ClearFlags();
        NewLoad(numShells, numLive);
        sm.SetState(gs.PlayerTurn);
    }

    private partial IEnumerator LoadDialogue() {
        ui.LogText("The panel in the table flips over.");
        yield return new WaitForSeconds(stdDelay);
        ui.LogText(SayLoad(true));
        yield return new WaitForSeconds(stdDelay);
    }

    public partial void NewLoad() {
        NewLoad(-1, -1);
    }
    
    public partial void NewLoad(int numShells, int numLive) {
        load = null;
        load = new Load(numShells, numLive);
        Debug.Log(SayLoad(false));
        StartCoroutine(LoadDialogue());
        // string msg = $"Reloaded shotgun. {load.numLive} live shells, {load.numTotal - load.numLive} blanks";
        // db.LogText(msg);
    }

    public partial void ClearFlags() {
        if (isSawed) ui.LogText("The length of the barrel that was sawed off phases back into existence.");
        if (isPlayerCuffed > 0) ui.LogText("You snap the handcuffs off your wrist.");  
        if (isDealerCuffed > 0) ui.LogText("The dealer snaps the handcuffs off his wrists.");
        isSawed = false;
        isPlayerCuffed = 0;
        isDealerCuffed = 0;
    }

    public partial void UpdateHealth(bool targetIsDealer, int amount = -1) {
        if (targetIsDealer) dealerHP += amount;
        else playerHP += amount;
        
        string subject = targetIsDealer ? "The dealer now has" : "You now have";
        int subjectHP = targetIsDealer ? dealerHP : playerHP;
        string lifeStr = subjectHP == 1 ? "life" : "lives";
        ui.LogText($"{subject} {subjectHP} {lifeStr}."); //todo lives or charges?
    }
    
    public partial void SetHealth(bool targetIsDealer, int amount = -1) {
        if (amount == -1) amount = Random.Range(2, 5);
        
        if (targetIsDealer) dealerHP = amount;
        else playerHP = amount;
        
        string subject = targetIsDealer ? "The dealer now has" : "You now have";
        int subjectHP = targetIsDealer ? dealerHP : playerHP;
        ui.LogText($"{subject} {subjectHP} lives."); //todo lives or charges?
    }
    
    public partial void InitHealth() {
        int amount = Random.Range(2, 5);
        
        dealerHP = amount;
        playerHP = amount;
        ui.LogText($"The dealer now has {dealerHP} lives."); //todo lives or charges?
        ui.LogText($"You now have {playerHP} lives."); //todo lives or charges?
    }
}
