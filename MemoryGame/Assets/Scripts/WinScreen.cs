using UnityEngine;
using UnityEngine.UI;
public class WinScreen : MonoBehaviour
{
    [SerializeField]
    private Text resultText;
   public void Set(bool win)
    {
        gameObject.SetActive(true);
        if(win)
        {
            resultText.text = "YOU WON";
        }
        else
        {
            resultText.text = "YOU LOST";
        }
    }

   
}
