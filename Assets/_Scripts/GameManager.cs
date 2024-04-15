using System;
using System.Collections;
using System.Numerics;
using _Scripts;
using UnityEngine;
using gs = StateManager.GameState;
using h = _Scripts.Helper;
using timer = _Scripts.Timer;
using Random = UnityEngine.Random;

public partial class GameManager : MonoBehaviour {
    public static GameManager instance = null;

    private StateManager sm;
    private UIController ui;
    private ActionManager am;
    private _Scripts.Load load;
    
    private gs state;

    private int timesRepeated = 0;

    // Round variables
    private Item[] playerItems;
    private Item[] dealerItems;
    private Boolean wasLastRoundDealers;
    
    // Load variables
    private int healthLimit;
    private int playerHP;
    private int dealerHP;
    
    // Turn variables
    private bool isSawed = false;
    private int isPlayerCuffed = 0; // 2 = just cuffed, 1 = cuffed last round, 
    private int isDealerCuffed = 0; // 0 = not cuffed
    
    // Anim variables
    private bool lastRoundLive = false;
    
    // 1 is tutorial, 2 is normal, 3 is final
    // This is GAME rounds NOT SHOTGUN rounds.
    private int roundNum = 1;
    private int turnNum = 1;
    
    // Timers
    // private timer t;
    // private timer indefT;
    

    [SerializeField] private float stdDelay = 4f;
    
    // Set up methods
    public static GameManager Instance {
        get {
            if (instance == null)
                instance = FindObjectOfType<GameManager>();

            if (instance == null) {
                GameObject gObj = new GameObject();
                gObj.name = "GameManager";
                instance = gObj.AddComponent<GameManager>();
                DontDestroyOnLoad(gObj); // allows gameManager to stay persistent across scenes
            }

            return instance;
        }
    }
    private void OnEnable() { sm.OnStateChange += HandleStateChange; }
    private void OnDisable() { sm.OnStateChange -= HandleStateChange; }
    private partial void Awake();
    
    // Driver
    private partial void Start();
    
    // Unit tests
    //private partial void Test();
    
    // Dialogue helper methods
    private partial string ShellsToString(int numRounds);
    private partial string SayLoad(bool saidByDealer);
    
    // State transition function
    private partial void HandleStateChange(gs newState);

    // Phase starters
    private partial void StartTutorial();

    private partial void TutorialStartDialogue();

    private partial void PlayerTurn();

    // private partial void UseItem() {
    //     sm.SetState(gs.UseItem);
    //     //todo play anim here
    //     
    // }

    private partial void DealerTurn();

    public partial void Fire(bool isTargetDealer, bool isShooterDealer);

    private partial void TurnoverDialogue(bool isShooterDealer, bool isTargetDealer, bool isLive);

    private partial void StartRound();

    private partial void StartRoundDialogue();

    public partial void NewLoad();
    
    public partial void NewLoad(int numShells, int numLive);
    
    private partial void StartLoad();

    private partial void StartLoad(int numShells, int numLive);

    private partial void LoadDialogue();
    
    private partial void GameWon();

    public partial void ClearFlags();

    public partial void UpdateHealth(bool targetIsDealer, int amount = -1);

    public partial void SetHealth(bool targetIsDealer, int amount);

    public partial void InitHealth();
}