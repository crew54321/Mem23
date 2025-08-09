using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardBase : MonoBehaviour, ICard
{
    public int cardId { get; protected set; }
    protected bool isFlipped;
    private bool isAnimating = false;
    protected Image imageComponent;
    protected Button button;
    protected Sprite frontSprite;
    protected Sprite backSprite;
    protected IGamemanager gamemanager;
    public void Initialize(int id, Sprite front, Sprite back, IGamemanager manager)
    {
        cardId = id;
        frontSprite = front;
        backSprite = back;
        gamemanager = manager;
        imageComponent = GetComponent<Image>();
        button = GetComponent<Button>();
        ShowBack();
        button.onClick.AddListener(OnClick);
       
    }

    public virtual void OnClick()
    {
        if(!isFlipped && gamemanager.CanFlip())
        {
          
            StartCoroutine(FlipAnimation(true));
            gamemanager.CardRevealed(this);
        }
    }

  
    public IEnumerator FlipBackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(FlipAnimation(false));
      
    }
    public void ShowBack()
    {
        imageComponent.sprite = backSprite;
        isFlipped = false;
    }
    public IEnumerator FlipAnimation(bool showFront)
    {
      
        isAnimating = true;

      
        float duration = 0.15f;
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = new Vector3(0f, startScale.y, startScale.z);

        while (elapsed < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = endScale;

       
        if (showFront)
        {
            imageComponent.sprite = frontSprite;
            isFlipped = true;
        }
        else
        {
            imageComponent.sprite = backSprite;
            isFlipped = false;
        }

       
        elapsed = 0f;
        while (elapsed < duration)
        {
            transform.localScale = Vector3.Lerp(endScale, startScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = startScale;

        isAnimating = false;
    }
}
