using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts {
    public class Helper {
        /// <summary>
        /// Generates a random boolean value based on the probability numer/denom.
        /// </summary>
        /// <param name="numer">The numerator of the probability ratio.</param>
        /// <param name="denom">The denominator of the probability ratio.</param>
        /// <returns>True if the randomly generated value is less than the calculated probability, otherwise false.</returns>
        public static Boolean RollRNG(int numer, int denom) {
            int num = Random.Range(0, denom);
            return (num < numer);
        }
    }

    public class Timer {
        private float timer = 0f;
        private bool isWaiting = false;
        private float waitTime;

        // Update is called once per frame
        // void Update() {
        //     if (isWaiting) {
        //         timer += Time.deltaTime;
        //         if (timer >= waitTime) {
        //             timer = 0f;
        //             isWaiting = false;
        //         }
        //     }
        // }

        public async Task Wait(float seconds) {
            waitTime = seconds;
            isWaiting = true;
            await Task.Delay((int)(seconds * 1000)); // Convert seconds to milliseconds
            isWaiting = false;
        }
    }
}