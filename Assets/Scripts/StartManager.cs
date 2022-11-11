using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LootLocker.Requests;
using TMPro;

public class StartManager : MonoBehaviour
{
    LootLockerLeaderboardMember[] scores;
    int page;

    public Material[] playerSkins;
    int skin;

    bool leaderboardOpen;
    int ranksPerPage = 20;
    private void Start()
    {
        if (PlayerPrefs.HasKey("Name"))
        {
            GameObject.Find("Name").GetComponent<TMP_InputField>().text = PlayerPrefs.GetString("Name");
        }
    }

    void Update()
    {
        float touchY = 0.5f;
        if (Input.touchCount > 0 && !leaderboardOpen)
        {
            touchY = Input.GetTouch(0).position.y / Screen.height;
        }

        if (Input.GetKeyDown(KeyCode.Return) || touchY < 0.25f)
        {
            StartGame();

            Destroy(gameObject);
        }
    }

    public void StartGame()
    {
        GameManager.instance.gameStarted = true;

        GameObject.Find("StartCloud").transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
        GameObject.Find("StartCloud").GetComponent<ParticleSystem>().Play();

        GameObject.Find("Start").GetComponent<Animator>().SetTrigger("Start");
        GameObject.Find("Title").SetActive(false);
        GameObject.Find("Leaderboard Button").SetActive(false);
        GameObject.Find("StartCamera").SetActive(false);
        GameObject.Find("Skin").SetActive(false);

        GameObject.Find("LowPolyCharacter1").GetComponent<Animator>().SetTrigger("Fall");

        StartCoroutine(GameObject.Find("CloudSpawner").GetComponent<CloudSpawner>().GenerateCloud());

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>().Dive();

        SoundManager.instance.PlaySound("ost", 0.3f, 1, true, true);
    }

    public void OpenMobileKeyboard()
    {
        TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false, "", 10);
    }

    public IEnumerator GetLeaderBoard()
    {
        int leaderboardID = 8155;
        int count = 1000;
        int after = 0;

        scores = null;

        LootLockerSDKManager.GetScoreList(leaderboardID, count, after, (response) =>
        {
            if (response.statusCode == 200)
            {
                scores = response.items;

                Debug.Log("Successful");
            }
            else
            {
                Debug.Log("failed: " + response.Error);
            }
        });

        while (true)
        {
            if (scores != null)
            {
                break;
            }

            yield return new WaitForSeconds(0.1f);
        }

        string leaderboardText = string.Empty;

        for (int i = 0; i < ranksPerPage; i++)
        {
            leaderboardText += (i + 1) + ": " + scores[i].metadata + " (" + scores[i].score + ")\n";
        }

        if (GameObject.Find("LBoard") != null)
            GameObject.Find("LBoard").GetComponent<TextMeshProUGUI>().text = leaderboardText;

        if (GameManager.instance.memberID == scores[0].member_id)
        {
            GameObject.Find("crown").GetComponent<MeshRenderer>().enabled = true;
        }

    }

    public void LoadNextLeaderboardPage()
    {
        page++;

        if (page > scores.Length / ranksPerPage)
        {
            page = 0;
        }

        GameObject.Find("LBoard").GetComponent<TextMeshProUGUI>().text = "Loading...";

        string leaderboardText = string.Empty;

        for (int i = page * ranksPerPage; i < Mathf.Clamp((page + 1) * ranksPerPage, 0, scores.Length); i++)
        {
            leaderboardText += (i + 1) + ": " + scores[i].metadata + " (" + scores[i].score + ")\n";
        }

        GameObject.Find("LBoard").GetComponent<TextMeshProUGUI>().text = leaderboardText;
    }

    public void ChangePlayerSkin()
    {
        skin++;

        if (skin > playerSkins.Length - 1)
        {
            skin = 0;
        }

        GameObject.Find("Player").transform.Find("LowPolyCharacter1/Player").GetComponent<SkinnedMeshRenderer>().material = playerSkins[skin];
    }

    public void OpenLeaderboard()
    {
        leaderboardOpen = true;
        GameObject.Find("LeaderboardPanel").GetComponent<Animator>().SetBool("Open", true);
    }

    public void CloseLeaderboard()
    {
        Invoke(nameof(SetLeaderboardFalse), 0.5f);
        GameObject.Find("LeaderboardPanel").GetComponent<Animator>().SetBool("Open", false);
    }

    void SetLeaderboardFalse()
    {
        leaderboardOpen = false;
    }
}
