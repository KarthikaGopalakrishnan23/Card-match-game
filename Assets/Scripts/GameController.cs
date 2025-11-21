using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [Header("Game Settings")]
    public int totalPairs;
    private int matchedPairs = 0;
    private bool gameFinished = false;
    private bool inputLocked = false;

    [Header("Game Over Settings")]
    public float gameTime = 60f;
    public int maxMistakes = 10;

    private float remainingTime;
    private int mistakeCount = 0;

    [Header("Score / UI")]
    public TMP_Text timerText;
    public TMP_Text turnsText;
    public TMP_Text matchScoreText;
    public TMP_Text scoreText;  // To display score

    private int turns = 0;
    private int score = 0;  // New score variable

    [Header("UI Panels")]
    public GameObject winPanel;
    public GameObject gameOverPanel;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip flipsound;
    public AudioClip matchSound;
    public AudioClip mismatchSound;
    public AudioClip gameCompleteSound;
    public AudioClip gameOverSound;

    private Card firstSelectedCard;
    private Card secondSelectedCard;
    private Card[] allCards;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        remainingTime = gameTime;

        if (winPanel) winPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);

        UpdateUI();

        allCards = FindObjectsOfType<Card>();
    }

    void Update()
    {
        if (gameFinished) return;

        remainingTime -= Time.deltaTime;
        UpdateUI();

        if (remainingTime <= 0)
        {
            GameOver();
        }
    }

    public void SelectCard(Card card)
    {
        if (gameFinished || inputLocked) return;

        if (firstSelectedCard == null)
        {
            firstSelectedCard = card;
        }
        else if (secondSelectedCard == null)
        {
            secondSelectedCard = card;

            turns++;
            UpdateUI();

            StartCoroutine(CheckMatch());
        }
    }

    IEnumerator CheckMatch()
    {
        inputLocked = true;

        yield return new WaitForSeconds(0.5f);

        if (firstSelectedCard.cardId == secondSelectedCard.cardId)
        {
            firstSelectedCard.SetMatched();
            secondSelectedCard.SetMatched();

            PlaySound(matchSound);

            matchedPairs++;
            UpdateScore(true); // Update score for a correct match
            UpdateUI();

            if (matchedPairs >= totalPairs)
            {
                WinGame();
            }
        }
        else
        {
            PlaySound(mismatchSound);

            mistakeCount++;
            UpdateScore(false); // Update score for a mismatch

            if (mistakeCount >= maxMistakes)
            {
                GameOver();
            }

            StartCoroutine(firstSelectedCard.FlipBack());
            StartCoroutine(secondSelectedCard.FlipBack());
        }

        firstSelectedCard = null;
        secondSelectedCard = null;
        inputLocked = false;
    }

    void WinGame()
    {
        gameFinished = true;

        PlaySound(gameCompleteSound);

        if (winPanel) winPanel.SetActive(true);
        DisableAllCards();
    }

    void GameOver()
    {
        if (gameFinished) return;

        gameFinished = true;

        PlaySound(gameOverSound);

        if (gameOverPanel) gameOverPanel.SetActive(true);
        DisableAllCards();
    }

    void DisableAllCards()
    {
        foreach (Card c in allCards)
        {
            // Disable the script so OnPointerClick no longer runs
            c.enabled = false;

            // Disable image raycasts so Unity stops sending clicks
            if (c.frontImage) c.frontImage.raycastTarget = false;
            if (c.backImage) c.backImage.raycastTarget = false;
        }
    }

    public void PlayFlipSound()
    {
        PlaySound(flipsound);
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void UpdateUI()
    {
        if (timerText)
        {
            timerText.text = "Time: " + Mathf.Ceil(remainingTime).ToString() + " secs";
        }

        if (turnsText)
        {
            turnsText.text = "Turns: " + turns.ToString();
        }

        if (matchScoreText)
        {
            matchScoreText.text = "Matches: " + matchedPairs + "/" + totalPairs;
        }

        if (scoreText)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    void UpdateScore(bool isMatch)
    {
        if (isMatch)
        {
           
            score += 100;
        }
        else
        {
          
            score -= 50;
        }

       
        score += Mathf.CeilToInt(remainingTime) * 10;

       
        score -= turns * 10;

     
        score -= mistakeCount * 50;

       
        score = Mathf.Max(score, 0);
    }
    


public void SaveGame()
{
    SaveData data = new SaveData();

    data.score = score;
    data.remainingTime = remainingTime;
    data.turns = turns;
    data.mistakeCount = mistakeCount;
    data.matchedPairs = matchedPairs;
    data.gameFinished = gameFinished;

    string json = JsonUtility.ToJson(data);
    PlayerPrefs.SetString("SaveFile", json);
    PlayerPrefs.Save();

    Debug.Log("Game Saved: " + json);
}

public void LoadGame()
{
    if (!PlayerPrefs.HasKey("SaveFile"))
    {
        Debug.Log("No save file found.");
        return;
    }

    string json = PlayerPrefs.GetString("SaveFile");
    SaveData data = JsonUtility.FromJson<SaveData>(json);

    score = data.score;
    remainingTime = data.remainingTime;
    turns = data.turns;
    mistakeCount = data.mistakeCount;
    matchedPairs = data.matchedPairs;
    gameFinished = data.gameFinished;

    Debug.Log("Game Loaded: " + json);

    UpdateUI();
}

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnApplicationQuit()
    {
        SaveGame();
    }   
}
