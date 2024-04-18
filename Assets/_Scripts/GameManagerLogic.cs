using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _Scripts;
using UnityEngine;
using gs = StateManager.GameState;
using h = _Scripts.Helper;
using timer = _Scripts.Timer;
using im = _Scripts.ItemManager;
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
        ui.Speak(gs.Tutorial);
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
                PlayerTurn();
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
        ui.Speak(gs.PlayerTurn); // todo disable
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
        ui.Speak(gs.PlayerTurn);
    }
    
    private partial void UseItem() {
        ui.Speak(gs.UseItem);
        //todo play anim here
    }

    private partial void PlayerTurn() {
        wasLastRoundDealers = false;
        if (isPlayerCuffed >= 2) {
            ui.LogText("You try to reach out to grab the shotgun only to realize you're still cuffed. " +
                       "Your turn is skipped.");
            ui.Speak(gs.DealerTurn);
            isPlayerCuffed = 1;
        } else if (isPlayerCuffed >= 1) {
            ui.LogText("You break the cuffs. It's your turn.");
            ui.Speak();
            isPlayerCuffed = 0;
        }
    }

    private partial void DealerTurn() {
        wasLastRoundDealers = true;
        if (isDealerCuffed >= 2) {
            ui.LogText("\"I'm still cuffed. Your turn\".");
            ui.Speak(gs.PlayerTurn);                                                                       
            isDealerCuffed = 1;
        } else if (isDealerCuffed >= 1) {
            ui.LogText("The dealer breaks the cuffs.");
            ui.LogText("\"My turn.\"");
            ui.Speak();
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
            ui.Speak(gs.NewRound);
            // Player turn must be triggered here because unity fuckery
            //ui.Speak(gs.PlayerTurn); 
            return;
        }

        if (playerHP <= 0) {
            ui.LogText("You lost lol.");
            ui.Speak(gs.GameLost);
            return;
        }
        
        if (load.numTotal <= 0) {
            TurnoverDialogue(isShooterDealer, isTargetDealer, isLive);
            Debug.Log("set state to new load from fire()");
            ui.Speak(gs.NewLoad);
            // Player turn must be triggered here because unity fuckery
            //ui.Speak(gs.PlayerTurn);
            return;
        }
        
        if (isShooterDealer == isTargetDealer && !isLive) {
            Debug.Log($"Self shot, shooter is dealer: {isShooterDealer}, target is dealer: {isTargetDealer}");
            if (isShooterDealer) ui.Speak(gs.DealerTurn);
            else ui.Speak(gs.PlayerTurn);
        } else {
            if (isShooterDealer) ui.Speak(gs.PlayerTurn);
            else ui.Speak(gs.DealerTurn);
        }

        TurnoverDialogue(isShooterDealer, isTargetDealer, isLive);
    }

    private partial void TurnoverDialogue(bool isShooterDealer, bool isTargetDealer, bool isLive) {
        ui.LogText("Turnover dialogue here.");
        if (!isTargetDealer && isLive) ui.LogText("");
        
        if (isShooterDealer) ui.LogText("The dealer puts the shotgun down.");
        else ui.LogText("You put the shotgun back on the table.");
        ui.Speak();
    }

    private partial void StartRound() {
        roundNum++;
        if (roundNum == 2) InitHealth(4);
        else InitHealth(6);
        
        playerItems.Clear();
        dealerItems.Clear();
        
        if (roundNum == 2) {
            ui.LogText("\"Let's make this more interesting.\"");
            ui.LogText("\"Draw some items. More items after every load.\"");
            DrawItems(2);
        }
        else DrawItems(4);
        StartLoad();
        
        StartRoundDialogue();
    }

    private partial void StartRoundDialogue() {
        //ui.LogText("NEW ROUND STARTING.");
        ui.LogText("NEW ROUND NEW ROUND NEW ROUND NEW ROUND!");
        ui.Speak();
    }

    private partial void DrawItems(int numItems) {
        if (numItems < 0) {
            numItems = Random.Range(1, 5);
        }

        bool dialogueFlag = false;

        for (int i = 0; i < numItems; i++) {
            if (playerItems.Count < 8) {
                int itemNum = Random.Range(1, 6);
                playerItems.Add(new Item(itemNum));
                ui.LogText("You pull out a " + Item.getUnit((ItemType) itemNum) + ".");
            }
            else if (!dialogueFlag){
                ui.LogText("You pull out a " + Item.getUnit((ItemType) Random.Range(1, 6)) + 
                           ", but your inventory is full.");
                ui.LogText("\"Unfortunate.\"");
                ui.Speak();
                dialogueFlag = true;
            }
            
            if (dealerItems.Count < 8)
                dealerItems.Add(new Item(Random.Range(1, 6)));
        }
    }

    private partial void UseItem(bool isUserDealer, int itemIndex) {
        List<Item> itemList;
        bool actionValid = true;
        if (isUserDealer) itemList = dealerItems;
        else itemList = playerItems;

        var item = itemList[itemIndex];
        
        switch (item.itemType) {
            case ItemType.Beer:
                ejectedShellLive = load.UseChamber();
                break;
            case ItemType.Cigs:
                UpdateHealth(isUserDealer, 1);
                break;
            case ItemType.Cuffs:
                if (isUserDealer) {
                    isPlayerCuffed = 2;
                    ui.LogText("The dealer cuffs you to the table.");
                }
                else {
                    isDealerCuffed = 2;
                    ui.LogText("You hand the cuffs to the dealer, who cuffs himself to the table.");
                }
                break;
            case ItemType.Glass:
                ui.LogText("You pump the shotgun halfway and look inside.");
                string strEnd = load.IsChamberLive() ? "live round." : "blank.";
                ui.LogText("It's a " + strEnd);
                break;
            case ItemType.Saw:
                ui.LogText("You saw the barrel off the shotgun.");
                if (isSawed) actionValid = false;
                else isSawed = true;
                break;
        }
        
        if (actionValid) {
            ui.LogText("Invalid move.");
            ui.Speak();
            return;
        }
        
        itemList[itemIndex] = null;
        
        var itemNamesList = itemList.Select(x => Item.getUnit(x.itemType)).ToArray();
        ui.LogText("You now have the following items: " + String.Join(", ", itemNamesList));
        ui.Speak();
    }

    private partial void StartLoad() {
        StartLoad(-1, -1);
    }
    
    private partial void StartLoad(int numShells, int numLive) {
        Debug.Log("Started new load.");
        // getItems()
        ClearFlags();
        NewLoad(numShells, numLive);
        LoadDialogue();
    }

    private partial void LoadDialogue() {
        ui.LogText("The panel in the table flips over.");
        ui.LogText(SayLoad(true));
        ui.Speak(gs.PlayerTurn);
    }

    public partial void NewLoad() {
        NewLoad(-1, -1);
    }
    
    public partial void NewLoad(int numShells, int numLive) {
        load = null;
        load = new Load(numShells, numLive);
        Debug.Log(SayLoad(false));
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
        ui.Speak();
    }

    public partial void UpdateHealth(bool targetIsDealer, int amount = -1) {
        if (targetIsDealer) dealerHP += amount;
        else playerHP += amount;
        
        string subject = targetIsDealer ? "The dealer now has" : "You now have";
        int subjectHP = targetIsDealer ? dealerHP : playerHP;
        string lifeStr = subjectHP == 1 ? "life" : "lives";
        ui.LogText($"{subject} {subjectHP} {lifeStr}."); //todo lives or charges?
        ui.Speak();
    }
    
    public partial void SetHealth(bool targetIsDealer, int amount = -1) {
        if (amount == -1) amount = Random.Range(2, 5);
        
        if (targetIsDealer) dealerHP = amount;
        else playerHP = amount;
        
        string subject = targetIsDealer ? "The dealer now has" : "You now have";
        int subjectHP = targetIsDealer ? dealerHP : playerHP;
        ui.LogText($"{subject} {subjectHP} lives."); //todo lives or charges?
        ui.Speak();
    }
    
    public partial void InitHealth(int amount) {
        if (amount < 1) amount = Random.Range(2, 5);
        
        dealerHP = amount;
        playerHP = amount;
        healthLimit = amount;
        ui.LogText($"The dealer now has {dealerHP} lives."); //todo lives or charges?
        ui.LogText($"You now have {playerHP} lives."); //todo lives or charges?
        ui.Speak();
    }
}
