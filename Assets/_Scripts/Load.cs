using System;
using System.Numerics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts {
    public class Load {
        //public Vector<Boolean> Shells { get; private set; }
        public int numLive { get; private set; }
        public int numTotal { get; private set; }
        private int shellIsLive = -1;

        public Load(int numShells = -1) {
            // min shells 2, max shells 8
            numTotal = numShells < 0 ?  Random.Range(2, 9) : numShells;
            numLive = numTotal / 2;
            Debug.Log($"Total: {numTotal}, Live: {numLive}, Blank: {numTotal - numLive}");
        }

        public Boolean IsChamberLive() {
            if (shellIsLive == -1) {
                if (Random.Range(0, numTotal) + 1 <= numLive) {
                    shellIsLive = 1;
                }
                else {
                    shellIsLive = 0;
                }
            }

            return shellIsLive == 1;
        }

        public Boolean UseChamber() {
            Boolean isLive;
            if (shellIsLive == -1) isLive = IsChamberLive();
            
            // handle shell amounts
            if (shellIsLive == 1) numLive--;
            numTotal--;
            
            isLive = shellIsLive == 1;
            shellIsLive = -1; // reset shell state

            return isLive;
        }
    }
}