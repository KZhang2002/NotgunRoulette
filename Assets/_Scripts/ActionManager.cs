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
            Shoot(true);
        }
        
        public void ShootPlayer() {
            Shoot(false);
        }
        
        public void Shoot(bool isTargetDealer) {
            string msg = "";
            if (sm.state != gs.PlayerTurn && sm.state != gs.DealerTurn) {
                ui.LogText("Error with states! Check action manager!");
            }
            
            if (sm.state == gs.PlayerTurn) {
                msg += "You raise the gun and point it at ";
            } else {
                msg += "The dealer raises the gun and points it at ";
            }

            if (sm.state == gs.PlayerTurn && isTargetDealer) msg += "the dealer.";
            else if (sm.state == gs.PlayerTurn && !isTargetDealer) msg += "yourself.";
            else if (sm.state == gs.DealerTurn && isTargetDealer) msg += "itself.";
            else msg += "you.";
            
            ui.LogText(msg);
            
            StartCoroutine(FireCoR(isTargetDealer));
        }

        private IEnumerator FireCoR(bool targetIsDealer) {
            yield return new WaitForSeconds(1f);
            if (sm.state == gs.PlayerTurn) {
                ui.LogText("You pull the trigger and...");
            } else {
                ui.LogText("The dealer pulls the trigger and...");
            }
            yield return new WaitForSeconds(1f);
            gm.Fire(targetIsDealer);
        }
        
    }
}