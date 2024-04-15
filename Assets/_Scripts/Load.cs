using System;
using System.Numerics;
using UnityEngine;
using Random = UnityEngine.Random;
using h = _Scripts.Helper;

namespace _Scripts {
    public class Load {
        //public Vector<Boolean> Shells { get; private set; }
        public int numLive { get; private set; }
        public int numTotal { get; private set; }
        private int shellIsLive = -1;

        public Load(int numShells = -1, int liveNum = -1) {
            numTotal = numShells < 0 ?  Random.Range(2, 9) : numShells; // 2 - 8 shells
            if (liveNum > -1) numLive = liveNum;
            else {
                if (liveNum == 5 || liveNum == 7) {
                    // 50 50 chance of live rounds being one greater vs one less
                    Boolean moreLive = h.RollRNG(1,2); 
                    numLive = moreLive ? numTotal / 2 : numTotal - numTotal / 2;
                } else numLive = numTotal / 2;
            }
            Debug.Log($"Total: {numTotal}, Live: {numLive}, Blank: {numTotal - numLive}");
        }

        public bool IsChamberLive() {
            if (shellIsLive == -1) {
                bool isLive = h.RollRNG(numLive, numTotal);
                shellIsLive = isLive ? 1 : 0;
            }
            return shellIsLive == 1;
        }

        public bool UseChamber() {
            bool isLive;
            if (shellIsLive == -1) isLive = IsChamberLive();
            
            if (shellIsLive == 1) numLive--; // handle shell amounts
            numTotal--;
            
            isLive = shellIsLive == 1;
            shellIsLive = -1; // reset shell state

            return isLive;
        }
    }
}