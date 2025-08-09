using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class MemoryGameManager : GameManagerBase
{
    [Header("Board")]
    public GameObject cardPrefab;         // prefab containing a concrete Card (CardBase)
    [SerializeField] 
    private GridLayoutGroup gridLayout;     // assign a RectTransform (UI) 
    public int rows = 2;
    public int cols = 4;
    public Vector2 cardSize = Vector2.zero; // UI: pixels, World: units (if zero -> will auto-calc defaults)
    public float spacing = 10f;           // UI: pixels gap, World: units gap

    [Header("Sprites")]
    public Sprite backSprite;
    public List<Sprite> frontSprites;     // must contain at least (rows*cols)/2 unique sprites



    private bool canFlip = true;
    private int score;
    private float timeRemaining = 120f;

    private List<CardBase> cards = new List<CardBase>();
    private List<CardBase> revealedCards = new List<CardBase>();
    private List<int> matchedCardIDs = new List<int>();

    [SerializeField]
    WinScreen winScreen;    
    [SerializeField]
    Timer timer;
    [SerializeField]
    InputField col_Input, row_Input;
    private void Start()
    {
        LoadOrStartNewGame();
    }
    private void RestoreMatchedCards()
    {
        foreach (CardBase card in cards)
        {
            if (matchedCardIDs.Contains(card.cardId))
            {
                StartCoroutine(card.FlipAnimation(true)); 
            }
        }
      
    }

    private void SaveProgress()
    {
        SaveData data = new SaveData
        {
            score = score,
            timeRemaining = timeRemaining,
            matchedCardIDs = new List<int>(matchedCardIDs)
        };
        SaveLoadManager.SaveGame(data);
    }
    private void LoadOrStartNewGame()
    {
        SaveData data = SaveLoadManager.LoadGame();
        if (data != null ) // totalUniqueCards = number of unique pairs in the game
        {
            score = data.score;
            timeRemaining = data.timeRemaining;
            matchedCardIDs = new List<int>(data.matchedCardIDs);
            Debug.Log($"Loaded Game: Score={score}, Time={timeRemaining}");
            
            SetupCards();
            if (matchedCardIDs.Count == cards.Count / 2)
            {
                StartGameAgain();
                return;
            }
            RestoreMatchedCards();
        }
        else
        {
            SetupCards();
        }
    }

    private void SetupCards()
    {
        foreach (Transform child in gridLayout.transform)
            Destroy(child.gameObject);

        cards.Clear();

        // Adjust GridLayout
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = cols;
        gridLayout.cellSize = new Vector2(
            (gridLayout.GetComponent<RectTransform>().rect.width - (cols - 1) * spacing) / cols,
            (gridLayout.GetComponent<RectTransform>().rect.height - (rows - 1) * spacing) / rows
        );
        gridLayout.spacing = new Vector2(spacing, spacing);

        // Create pairs
        List<int> ids = new List<int>();
        for (int i = 0; i < (rows * cols) / 2; i++)
        {
            ids.Add(i);
            ids.Add(i);
        }
        ids = ids.OrderBy(x => UnityEngine.Random.value).ToList();

        foreach (int id in ids)
        {
            GameObject cardObj = Instantiate(cardPrefab, gridLayout.transform);
            Card card = cardObj.GetComponent<Card>();
            card.Initialize(id, frontSprites[id % frontSprites.Count], backSprite, this);
            cards.Add(card);
        }
        timer.StartCoroutine(timer.StartTimer(120, TimerFinished));
    }
   
    void TimerFinished()
    {
        winScreen.Set(false);
    }
    
   
    public override bool CanFlip() => canFlip;

    public override void CardRevealed(CardBase card)
    {
        revealedCards.Add(card);

        if (revealedCards.Count == 2)
        {
            StartCoroutine(CheckMatch());
        }
    }
    public void StartGameAgain()
    {
        // Reset score and tracking lists
       
        if(string.IsNullOrEmpty(col_Input.text) || string.IsNullOrEmpty(row_Input.text))
        {
            return;
        }
        score = 0;
        cols = int.Parse(col_Input.text);
        rows = int.Parse(row_Input.text);
        timer.StopCoroutine();
        timeRemaining = 120;
        matchedCardIDs.Clear();
        revealedCards.Clear();
        cards.Clear();
        winScreen.gameObject.SetActive(false);
        // Reset UI (if you have score text)

        // Destroy all current cards in the scene
        foreach (Transform child in gridLayout.transform) // cardsParent is the GameObject holding all cards
        {
            Destroy(child.gameObject);
        }

        // Shuffle and spawn new cards
        SetupCards();

        
        canFlip = true;

        // Save new empty progress
        SaveProgress();
    }

    void OnAllCardsMatched()
    {
        timer.StopCoroutine();
        winScreen.Set(true);
        Debug.Log("Game Over - All matches found!");
    }
    IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(0.5f);

        if (revealedCards[0].cardId == revealedCards[1].cardId)
        {
            score += 10;
            matchedCardIDs.Add(revealedCards[0].cardId);
            if (matchedCardIDs.Count == cards.Count/2) // totalUniqueCards = number of unique pairs in the game
            {
                Debug.Log("🎉 All cards matched! You win!");
                OnAllCardsMatched(); // Call a method to handle win state
            }
        }
        else
        {
            yield return revealedCards[0].FlipBackAfterDelay(0);
            yield return revealedCards[1].FlipBackAfterDelay(0);
        }

        revealedCards.Clear();
        SaveProgress();

      
        canFlip = true;
    }
}