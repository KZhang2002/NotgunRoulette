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
    
    // Load variables
    private int healthLimit;
    private int playerHP;
    private int dealerHP;
    
    // Turn variables
    private bool isSawed = false;
    private bool isCuffed = false;
    
    // 0 is tutorial round, 1 - 2 is normal, 3 is final
    // This is GAME rounds NOT SHOTGUN rounds.
    private int roundNum = 0;
    
    // Timers
    private timer t;
    private timer indefT;
    

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
    private partial void Test();
    
    // Dialogue helper methods
    private partial string ShellsToString(int numRounds);
    private partial string SayLoad();
    
    // State transition function
    private partial void HandleStateChange(gs newState);

    // Phase starters
    private partial void StartTutorial();
    private partial void StartLoad();
    public partial void NewLoad(int numShells = -1, int numLive = -1);

    private partial IEnumerator TutorialStartDialogue();

    private partial IEnumerator PlayerTurn();

    // private partial void UseItem() {
    //     sm.SetState(gs.UseItem);
    //     //todo play anim here
    //     
    // }

    private partial void DealerTurn();

    private partial void GameWon();

    public partial void Fire(bool isTargetDealer);

    public partial void UpdateHealth(bool targetIsDealer, int amount = -1);

    public partial void SetHealth(bool targetIsDealer, int amount);
}