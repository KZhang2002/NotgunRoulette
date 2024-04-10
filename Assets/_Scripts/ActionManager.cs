using System;
using System.Collections;
using UnityEngine;
using gs = StateManager.GameState;

namespace _Scripts {
    public class ActionManager : MonoBehaviour {
        private GameManager gm;
        private StateManager sm;
        private UIController db;
        
        private void Awake() {
            
        }

        private void Start() {
            gm = GameManager.instance;
            sm = StateManager.Get();
            db = UIController.instance;
        }
        
        public void ShootDealer() {
            string msg = "";
            if (sm.state == gs.PlayerTurn) {
                msg += "You raise the gun and point it at the dealer.";
                db.LogText(msg);
                db.LogText("You pull the trigger and...");
            } else if (sm.state == gs.DealerTurn) {
                msg += "The dealer raises the gun and points it at itself.";
                db.LogText(msg);
                db.LogText("The dealer pulls the trigger and...");
            } else {
                db.LogText("Error with states when shooting dealer.");
            }
            
            StartCoroutine(FireCoR(true));
        }
        
        public void ShootPlayer() {
            StartCoroutine(FireCoR(false));
        }

        private IEnumerator FireCoR(bool targetIsDealer, float delay = 0.5f) {
            yield return new WaitForSeconds(delay);
            gm.Fire(targetIsDealer);
        }
        
    }
}