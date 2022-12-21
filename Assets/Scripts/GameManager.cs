using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    private TextMeshProUGUI comboText;

    [HideInInspector]
    public string memberID = "404";
    string playerName = "Player";

    [HideInInspector]
    public List<GameObject> clouds = new List<GameObject>();
    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public bool playerPassed, playerScored;
    [HideInInspector]
    public bool gameStarted;

    int combo;
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

            StartCoroutine(GameObject.Find("StartManager").GetComponent<StartManager>().GetLeaderBoard());

            Debug.Log("successfully started LootLocker session");
        });

        scoreText = GameObject.Find("Score").GetComponent<TextMeshProUGUI>();
        comboText = GameObject.Find("Combo").GetComponent<TextMeshProUGUI>();

        player = GameObject.FindGameObjectWithTag("Player");
    }


    // Update is called once per frame
    void Update()
    {
        if (!gameStarted)
        {
            return;
        }

        // Score text
        scoreText.text = ((int)score).ToString();
        scoreText.fontSize = (score / 100000 * 30) + 60;

        if (clouds.Count != 0)
        {
            foreach (GameObject cloud in clouds)
            {
                if (cloud != null)
                {
                    MeshRenderer cmr = cloud.GetComponent<MeshRenderer>();

                    if (cloud.GetComponent<CloudBehaviour>().fake == false)
                    {
                        cmr.material.color = new Color(cmr.material.color.r,
                            cmr.material.color.g,
                            cmr.material.color.b,
                            Mathf.Clamp01((player.transform.position.y / cloud.transform.position.y) - 0.25f));
                    }
                    else
                    {
                        cmr.material.color = new Color(cmr.material.color.r,
                            cmr.material.color.g,
                            cmr.material.color.b,
                            Mathf.Clamp(player.transform.position.y / cloud.transform.position.y - 0.25f, 0, 0.04f));
                    }
                }
            }
        }

        if (player.transform.position.y <= -75 && !playerPassed)
        {
            foreach (GameObject cloud in clouds)
            {
                if (cloud != null)
                {
                    cloud.GetComponent<CloudBehaviour>().Invoke("ShowCloud", Random.Range(0, 0.5f));
                }
            }

            playerPassed = true;
        }

        if (player.transform.position.y <= -150 && !playerScored)
        {
            if (player.GetComponent<PlayerControl>().dashing)
            {
                combo++;
                
                AddScore(150 + Mathf.Clamp((combo - 1) * 10, 0, 100));

                scoreText.GetComponent<Animator>().SetTrigger("Big");
                comboText.GetComponent<Animator>().SetTrigger("Show");

                comboText.text = "Combo! (x" + combo + ")";

            }
            else
            {
                AddScore(50);

                combo = 0;

                scoreText.GetComponent<Animator>().SetTrigger("Big");
            }

            player.transform.Find("CloudParticle").GetComponent<ParticleSystem>().Play();

            SoundManager.instance.PlaySound("Score");
            playerScored = true;
        }
    }

    public void AddScore(float amount)
    {
        score += amount;
    }

    public void GameOver()
    {
        SoundManager.instance.StopAllSounds();
        SoundManager.instance.PlaySound("explosion", 1.5f, 1, false, false);

        GameObject.Find("VirtualJoystick").GetComponent<Image>().enabled = false;

        if (Random.Range(1, 100) == 100)
        {
            SoundManager.instance.PlaySound("gameover", 1, 1, false, true);
        }

        GameObject.Find("GameOver").GetComponent<Animator>().SetTrigger("Fade");
        GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>().text = "Score: " + (int)score;

        int leaderboardID = 9925;

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

        
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AddLevel()
    {
        level++;
    }

    public int GetLevel()
    {
        return level;
    }

    public void UpdateName()
    {
        string[] forbidden_names = { "nigger", "nigga", "fuck", "shit" };

        string newName = GameObject.Find("Name").GetComponent<TMP_InputField>().text;

        foreach (string name in forbidden_names)
        {
            if (newName.ToLower().Contains(name))
            {
                return;
            }
        }

        PlayerPrefs.SetString("Name", newName);

        print("Name updated");

        playerName = newName;
    }
}
