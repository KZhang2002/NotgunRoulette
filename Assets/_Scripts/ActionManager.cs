using System;
using System.Collections;
using UnityEngine;
using gs = StateManager.GameState;

namespace _Scripts {
    public class ActionManager : MonoBehaviour {
        public static ActionManager instance;
        private GameManager gm;
        private StateManager sm;
        private UIController ui;
        
        private void Awake() {
            if (instance == null) instance = this;
        }

        private void Start() {
            gm = GameManager.instance;
            sm = StateManager.Get();
            ui = UIController.instance;
        }

        public void ShootDealer() {
            Shoot(true, false);
        }
        
        public void ShootPlayer() {
            Shoot(false, false);
        }
        
        public void Shoot(bool isTargetDealer, bool isShooterDealer) {
            sm.SetState(gs.Fire);
            string msg = "";
            // if (sm.state != gs.PlayerTurn && sm.state != gs.DealerTurn) {
            //     ui.LogText("Error with states! Check action manager!");
            // }
            
            if (!isShooterDealer) {
                msg += "You raise the gun and point it at ";
            } else {
                msg += "The dealer raises the gun and points it at ";
            }
    
            if (isShooterDealer && isTargetDealer) msg += "himself.";
            else if (!isShooterDealer && isTargetDealer) msg += "the dealer.";
            else if (isShooterDealer && !isTargetDealer) msg += "you.";
            else msg += "yourself.";
            
            ui.LogText(msg);
            
            FireCoR(isTargetDealer, isShooterDealer);
        }

        private void FireCoR(bool targetIsDealer, bool isShooterDealer) {
            if (!isShooterDealer) {
                ui.LogText("You pull the trigger and...");
            } else {
                ui.LogText("The dealer pulls the trigger and...");
            }
            ui.Speak();
            gm.Fire(targetIsDealer, isShooterDealer);
        }
        
    }
}