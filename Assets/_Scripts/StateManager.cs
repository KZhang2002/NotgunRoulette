using UnityEngine;

// its a singleton
public class StateManager {
    private static StateManager instance;
    private StateManager() { } // just makes constructor private
    
    public static StateManager Get() {
        if (instance == null) instance = new StateManager();
        return instance;
    }

    private GameState state;
    
    // subscription stuff
    public delegate void StateChangeHandler(GameState newState);
    public event StateChangeHandler OnStateChange;
    
    public enum GameState {
        Menu,
        Start,
        Pause,
        GetItems,
        PlayerTurn,
        DealerTurn,
        Dead,
        GameLost,
        GameWon
    }
    
    public void SetState(GameState newState)
    {
        state = newState;
        OnStateChange?.Invoke(newState);
    }
}

public class Manager : MonoBehaviour {
    private StateManager sm;

    // Start is called before the first frame update
    private void Start() {
        sm = StateManager.Get();
        sm.SetState(StateManager.GameState.Start);
    }

    // Update is called once per frame
    private void Update() { }
}