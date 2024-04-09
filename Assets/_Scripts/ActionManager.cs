using System;
using System.Collections;
using UnityEngine;

namespace _Scripts {
    public class ActionManager : MonoBehaviour {
        private GameManager gm;
        private UIController db;
        
        private void Awake() {
            
        }

        private void Start() {
            gm = GameManager.instance;
            db = UIController.instance;
        }

        public void Fire(float delay = 0.5f) {
            StartCoroutine(FireCoR(delay));
        }

        private IEnumerator FireCoR(float delay) {
            db.LogText("You raise the gun and slowly pull the trigger.");
            yield return new WaitForSeconds(delay);
            gm.Fire();
        }
        
    }
}