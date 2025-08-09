using UnityEngine;

public interface IGamemanager
{
    bool CanFlip();
    void CardRevealed(CardBase card);
}
