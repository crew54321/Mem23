using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField]
    Text remainingTimeText;
    
   public IEnumerator StartTimer(int totalTimer,Action timerFinished)
    {
        while (totalTimer > 0)
        {
            remainingTimeText.text = totalTimer.ToString();
            yield return new WaitForSeconds(1);
            totalTimer--;
        }
        if(timerFinished!=null)
        {
            timerFinished.Invoke();
        }
            yield break;
        

    }

    public void StopCoroutine()
    {
        StopAllCoroutines();
    }

}
