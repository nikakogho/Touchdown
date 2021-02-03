using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string menuScene;

    public GameObject rugbistPrefab;

    public GameObject outUI, gameOverUI;
    public Text winnerText;

    public float borderX = 6.3f, borderZ = 11.3f;

    public const float finishZ = 10;

    public int touchdownScore = 5;
    public int throwScore = 2;

    public int endScore = 40;

    public static bool onePlayer = true;

    #region Crowd

    public float crowdScale = 0.5f;
    public GameObject[] crowdPrefabs;
    public Transform[] crowdSpawnParents;
    public string crowdApplauseTrigger = "Cheer";
    public string crowdApplauseName = "Applause";
    public int crowdApplauseCount = 5;

    #endregion

    #region Ball

    public Transform ballSpawnPoint;
    public GameObject ballPrefab;
    public Ball Ball { get; private set; }
    public float ballThrowForce;

    #endregion

    #region Teams

    public static Difficulty difficulty = Difficulty.Normal;
    public Transform teamParent;
    public GameObject teamANormalPrefab, teamAHardPrefab, teamAElitePrefab;
    public GameObject teamBPrefab;
    public Color teamAColor, teamBColor;
    public Transform teamASpawnPoint, teamBSpawnPoint;
    public Text teamAScoreText, teamBScoreText;
    public Text endTeamAScoreText, endTeamBScoreText;

    public TeamManager TeamA { get; private set; }
    public TeamManager TeamB { get; private set; }

    int teamAScore, teamBScore;

    #endregion

    public Transform mainCameraSpot, goalACameraSpot, goalBCameraSpot;

    public Transform targetAThrowFrom, targetBThrowFrom;

    public AudioSource source;
    public AudioClip cheerSound, booSound;

    List<Animator> crowdAnims;

    public static GameManager instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        teamAScoreText.color = teamAColor;
        teamBScoreText.color = teamBColor;
        teamAScore = teamBScore = 0;
        SpawnCrowd();
        SpawnBall();
        SpawnTeams();
        PlayerManager.Init(TeamA, TeamB);
    }

    #region Spawn

    void SpawnCrowd()
    {
        crowdAnims = new List<Animator>();
        foreach(var parent in crowdSpawnParents)
        {
            int count = parent.childCount;
            for(int i = 0; i < count; i++)
            {
                var point = parent.GetChild(i);
                var prefab = crowdPrefabs[Random.Range(0, crowdPrefabs.Length)];
                var obj = Instantiate(prefab, point.position, point.rotation, point);
                obj.transform.localScale *= crowdScale;
                var anim = obj.GetComponent<Animator>();
                crowdAnims.Add(anim);
            }
        }
    }

    void SpawnBall()
    {
        Ball = Instantiate(ballPrefab, ballSpawnPoint.position, ballSpawnPoint.rotation).GetComponent<Ball>();
        Ball.name = "Ball";
    }

    void SpawnTeams()
    {
        GameObject teamAPrefab;
        if (onePlayer)
            switch (difficulty)
            {
                case Difficulty.Normal: teamAPrefab = teamANormalPrefab; break;
                case Difficulty.Hard: teamAPrefab = teamAHardPrefab; break;
                default: teamAPrefab = teamAElitePrefab; break;
            }
        else teamAPrefab = teamBPrefab;
        var teamAObj = Instantiate(teamAPrefab, teamASpawnPoint.position, teamASpawnPoint.rotation, teamParent);
        var teamBObj = Instantiate(teamBPrefab, teamBSpawnPoint.position, teamBSpawnPoint.rotation, teamParent);
        TeamA = teamAObj.GetComponent<TeamManager>();
        TeamB = teamBObj.GetComponent<TeamManager>();
        TeamA.Init(teamAColor);
        TeamB.Init(teamBColor);
    }

    #endregion

    #region End Game

    void GameOver(string winner)
    {
        Time.timeScale = 0;
        gameOverUI.SetActive(true);
        winnerText.color = winner == "Team A" ? teamAColor : teamBColor;
        winnerText.text = winner + " Won!";
        endTeamAScoreText.color = teamAColor;
        endTeamAScoreText.text = teamAScore.ToString();
        endTeamBScoreText.color = teamBColor;
        endTeamBScoreText.text = teamBScore.ToString();
    }

    public IEnumerator OutRoutine()
    {
        outUI.SetActive(true);
        yield return new WaitForSeconds(1);
        outUI.SetActive(false);
    }

    void TriggerCrowd()
    {
        source.Play();
        foreach(var anim in crowdAnims)
        {
            anim.SetTrigger(crowdApplauseTrigger);
            int clip = Random.Range(0, crowdApplauseCount);
            anim.SetFloat(crowdApplauseName, clip);
        }
    }

    void FreshStart()
    {
        TriggerCrowd();
        if(TeamA != null) Destroy(TeamA.gameObject);
        if(TeamB != null) Destroy(TeamB.gameObject);
        if(Ball  != null) Destroy(Ball.gameObject);
        SpawnBall();
        SpawnTeams();
        PlayerManager.Init(TeamA, TeamB);
    }

    void TargetThrow(bool teamAThrows)
    {
        Destroy(TeamA.gameObject);
        Destroy(TeamB.gameObject);
        //to do
        //setup for target throw
        FreshStart();
    }

    void RealTourchdown(bool winnerIsB)
    {
        source.clip = cheerSound;
        if (winnerIsB)
        {
            teamBScore += touchdownScore;
            teamBScoreText.text = teamBScore.ToString();
        }
        else
        {
            teamAScore += touchdownScore;
            teamAScoreText.text = teamAScore.ToString();
        }
        if (teamBScore >= endScore) GameOver("Team B");
        else if (teamAScore >= endScore) GameOver("Team A");
        else TargetThrow(!winnerIsB);
    }

    public void Out()
    {
        source.clip = booSound;
        StartCoroutine(OutRoutine());
        TargetThrow(Ball.LastOwner.Team == TeamB);
    }

    public void Touchdown(bool winnerIsB)
    {
        if (Ball.Owner == null) Out();
        else RealTourchdown(winnerIsB);
    }

    #endregion

    #region End Options

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(menuScene);
    }

    public void Exit()
    {
        Application.Quit();
    }

    #endregion
}

public enum Difficulty { Normal, Hard, Elite }