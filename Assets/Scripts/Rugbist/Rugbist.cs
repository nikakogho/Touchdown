using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MoveAgent))]
[RequireComponent(typeof(AIRugbistController))]
[RequireComponent(typeof(PlayerRugbistController))]
public class Rugbist : MonoBehaviour {
    public Transform ballHandle;

    public RugbistData Data { get; private set; }
    public TeamManager Team { get; private set; }
    public MoveAgent Agent { get; private set; }

    public bool IsInit { get; private set; } = false;

    private Renderer[] rends;

    public AIRugbistController AiController { get; private set; }
    private PlayerRugbistController playerController;

    private Ball ball;
    private Collider triggerCol;

    public bool Collapsed { get; private set; }

    string GetNumberText(int number)
    {
        if (GameManager.onePlayer)
        {
            if (Team == GameManager.instance.TeamB) return number.ToString();
            if (number == 5) return "0";
            return (number + 5).ToString();
        }
        else if (Team == GameManager.instance.TeamA) return number.ToString();
        if (number == 5) return "0";
        return (number + 5).ToString();
    }
    
    public void Init(TeamManager team, RugbistData data, Color color, int number)
    {
        Team = team;
        foreach (var text in GetComponentsInChildren<Text>()) text.text = GetNumberText(number);
        Data = data;
        AiController = GetComponent<AIRugbistController>();
        playerController = GetComponent<PlayerRugbistController>();
        Agent = GetComponent<MoveAgent>();
        ball = Ball.instance;
        Agent.Init(this, data.speed);
        rends = GetComponentsInChildren<Renderer>();
        foreach (var rend in rends) rend.material.color = color;
        triggerCol = GetComponents<Collider>().Where(col => col.isTrigger).First();
        IsInit = true;
    }

    public void PlayerTakesOver()
    {
        AiController.enabled = false;
        playerController.enabled = true;
        Agent.Stop();
    }

    public void FreedFromPlayer()
    {
        playerController.enabled = false;
        AiController.enabled = true;
        Agent.Stop();
    }

    #region Fall

    void Collapse(Vector3 dir)
    {
        gameObject.tag = "Untagged";
        triggerCol.enabled = false;
        Agent.Collapse(dir);
        Collapsed = true;
    }

    void Recover()
    {
        triggerCol.enabled = true;
        Agent.Recover();
        gameObject.tag = "Rugbist";
        Collapsed = false;
    }

    IEnumerator FallRoutine(Vector3 dir, float fallenTime)
    {
        Collapse(dir);
        yield return new WaitForSeconds(fallenTime);
        Recover();
    }

    #endregion

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            if (ball.Owner == null) ball.SetOwner(this);
            else if (ball.Owner.Team != Team) ball.Owner.AiController.Drop();
        }
        else if(other.CompareTag("Rugbist"))
        {
            var rugbist = other.GetComponent<Rugbist>();
            if(rugbist.Team == Team)
            {
                if (ball.Owner == this) ball.SetOwner(rugbist);
            }
            else
            {
                //to do
                /*
                bool weaker = rugbist.Data.attackStrength > Data.defenseStrength;
                if (weaker && ball.Owner == this) ball.SetOwner(rugbist);
                var toThrow = weaker ? this : rugbist;
                Vector3 dir = (transform.position - other.transform.position) * (weaker ? 1 : -1);
                toThrow.StartCoroutine(toThrow.FallRoutine(dir, toThrow.Data.fallenTime));
                */
            }
        }
    }
}
