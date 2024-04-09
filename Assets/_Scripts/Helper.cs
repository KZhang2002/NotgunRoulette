using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts {
    public class Helper {
        public static IEnumerator Delay(float delay) {
            yield return new WaitForSeconds(delay);
        }

        // Rng for an event with two outcomes
        public static Boolean RandTwo(int numer, int denom) {
            int num = Random.Range(0, denom);
            return (num < numer);
        }
    }
}