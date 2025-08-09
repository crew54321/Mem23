

public interface ICard
{
    int cardId { get; }
    void Initialize(int id, UnityEngine.Sprite front, UnityEngine.Sprite back,IGamemanager gamemanager);
    void OnClick();
}
