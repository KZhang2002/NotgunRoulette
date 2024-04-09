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
    private UIController db;
    
    // subscription stuff
    public delegate void StateChangeHandler(GameState newState);
    public event StateChangeHandler OnStateChange;
    
    public enum GameState {
        Menu,
        Start,
        Pause,
        Load,
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