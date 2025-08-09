using UnityEngine;

public abstract class GameManagerBase : MonoBehaviour,IGamemanager
{
    public abstract bool CanFlip();

    

    public abstract void CardRevealed(CardBase card);
}
