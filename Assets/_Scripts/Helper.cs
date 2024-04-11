using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts {
    public class Helper {
        // Rng for an event with two outcomes
        public static Boolean RandTwo(int numer, int denom) {
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