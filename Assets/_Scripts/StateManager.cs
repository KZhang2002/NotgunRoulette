using UnityEngine;

// its a singleton
public class StateManager {
    private static StateManager instance;
    private StateManager() { } // just makes constructor private
    
    public static StateManager Get() {
        if (instance == null) instance = new StateManager();
        return instance;
    }

    public GameState state { get; private set; }
    private UIController ui;
    
    // subscription stuff
    public delegate void StateChangeHandler(GameState newState);
    public event StateChangeHandler OnStateChange;
    
    public enum GameState {
        Tutorial,
        Menu,
        Start,
        Pause,
        Load,
        Dialogue,
        GetItems,
        UseItem,
        Fire,
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
        Debug.Log($"State changed to {newState}");
    }
}