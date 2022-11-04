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
    private void Start()
    {
        if (PlayerPrefs.HasKey("Name"))
        {
            GameObject.Find("Name").GetComponent<TMP_InputField>().text = PlayerPrefs.GetString("Name");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
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

        GameObject.Find("Start").SetActive(false);
        GameObject.Find("Title").SetActive(false);
        GameObject.Find("Leaderboard").SetActive(false);
        GameObject.Find("LBoard").SetActive(false);
        GameObject.Find("Name").SetActive(false);
        GameObject.Find("Load").SetActive(false);
        GameObject.Find("StartCamera").SetActive(false);
        GameObject.Find("Skin").SetActive(false);

        GameObject.Find("LowPolyCharacter1").GetComponent<Animator>().SetTrigger("Fall");
        GameObject.Find("CloudSpawner").GetComponent<CloudSpawner>().GenerateCloud();

        SoundManager.instance.PlaySound("ost", 0.3f, 1, true, true);
    }

    public IEnumerator GetLeaderBoard()
    {
        int leaderboardID = 8155;
        int count = 100;
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

        for (int i = 0; i < 10; i++)
        {
            leaderboardText += (i + 1) + ": " + scores[i].metadata + " (" + scores[i].score + ")\n";
        }

        GameObject.Find("LBoard").GetComponent<TextMeshProUGUI>().text = leaderboardText;

        if (GameManager.instance.memberID == scores[0].member_id)
        {
            GameObject.Find("crown").GetComponent<MeshRenderer>().enabled = true;
        }

    }

    public void LoadNextLeaderboardPage()
    {
        page++;

        if (page > scores.Length / 10)
        {
            page = 0;
        }

        GameObject.Find("LBoard").GetComponent<TextMeshProUGUI>().text = "Loading...";

        string leaderboardText = string.Empty;

        for (int i = page * 10; i < Mathf.Clamp((page + 1) * 10, 0, scores.Length); i++)
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
}
