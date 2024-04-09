using System.Collections;
using UnityEngine;

namespace _Scripts {
    public class GeneralHelper {
        IEnumerator Delay(float delay)
        {
            Debug.Log("Start of coroutine");
            yield return new WaitForSeconds(delay); // Delay for the specified time
            Debug.Log("End of coroutine after delay");
        }
    }
}