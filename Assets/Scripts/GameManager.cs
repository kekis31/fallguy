using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private float score;
    private int level;
    
    [SerializeField]
    public Material[] materials;

    private TextMeshProUGUI scoreText;

    string memberID = "404";
    string playerName = "Test";
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (!response.success)
            {
                Debug.Log("error starting LootLocker session");
                
                return;
            }

            memberID = response.player_id.ToString();

            Debug.Log("successfully started LootLocker session");
        });

        scoreText = GameObject.Find("Score").GetComponent<TextMeshProUGUI>();
    }


    // Update is called once per frame
    void Update()
    {
        scoreText.text = ((int)score).ToString();
    }

    public void AddScore(float amount)
    {
        score += amount;
    }

    public void GameOver()
    {
        int leaderboardID = 8043;

        LootLockerSDKManager.SubmitScore(memberID, (int)score, leaderboardID, playerName, (response) =>
        {
            if (response.statusCode == 200)
            {
                Debug.Log("Successful");
            }
            else
            {
                Debug.Log("failed: " + response.Error);
            }
        });

        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AddLevel()
    {
        level++;
    }

    public int GetLevel()
    {
        return level;
    }
}
