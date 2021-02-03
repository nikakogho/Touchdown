using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TeamManager : MonoBehaviour {
    public LayerMask rugbistMask;

    public RugbistData frontData, rightFrontData, rightBackData, leftFrontData, leftBackData;
    public Transform frontSpawn, rightFrontSpawn, rightBackSpawn, leftFrontSpawn, leftBackSpawn;

    public Transform defenseBoundUp, defenseBoundDown;

    private List<Rugbist> rugbists;

    public Rugbist Front => rugbists[0];
    public Rugbist RightFront => rugbists[1];
    public Rugbist LeftFront => rugbists[2];
    public Rugbist RightBack => rugbists[3];
    public Rugbist LeftBack => rugbists[4];
    
    public Rugbist this[int index] => rugbists[index - 1];
    public IEnumerable<Rugbist> AllRugbists => rugbists;

    private Ball ball;

    const float crammedRange = 2;
    const int crammedCount = 3;

    float defenseZMax, defenseZMin;

    public void Init(Color color)
    {
        float z1 = defenseBoundUp.position.z;
        float z2 = defenseBoundDown.position.z;
        if(z1 > z2)
        {
            z1 += z2;
            z2 = z1 - z2;
            z1 -= z2;
        }
        defenseZMin = z1;
        defenseZMax = z2;
        var prefab = GameManager.instance.rugbistPrefab;
        List<KeyValuePair<RugbistData, Transform>> list = new List<KeyValuePair<RugbistData, Transform>>()
        {
            new KeyValuePair<RugbistData, Transform>( frontData, frontSpawn ),
            new KeyValuePair<RugbistData, Transform>( rightFrontData, rightFrontSpawn ),
            new KeyValuePair<RugbistData, Transform>( leftFrontData, leftFrontSpawn ),
            new KeyValuePair<RugbistData, Transform>( rightBackData, rightBackSpawn ),
            new KeyValuePair<RugbistData, Transform>( leftBackData, leftBackSpawn )
        };
        rugbists = new List<Rugbist>();
        foreach(var pair in list)
        {
            var obj = Instantiate(prefab, pair.Value.position, pair.Value.rotation, transform);
            var rugbist = obj.GetComponent<Rugbist>();
            rugbist.Init(this, pair.Key, color, rugbists.Count + 1);
            rugbists.Add(rugbist);
        }
        ball = Ball.instance;
        StartCoroutine(TeamRoutine());
    }

    IEnumerator TeamRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            ReSchedule();
        }
    }

    void ReSchedule()
    {
        for(int i = 3; i < 5; i++)
        {
            var r = rugbists[i];
            if (r == ball.Owner || !r.AiController.enabled) r.AiController.ReleaseFromDefense();
            else r.AiController.Constrain(defenseZMin, defenseZMax);
        }
        if (ball.Owner == null)
        {
            foreach(var r in rugbists) r.AiController.ChaseBall();
        }
        else if(ball.Owner.Team == this)
        {
            Vector3 ownerPos = ball.Owner.transform.position;
            var formation = rugbists.Where(r => r != ball.Owner && r.AiController.enabled).OrderBy(r => Vector3.SqrMagnitude(r.transform.position - ownerPos)).ToArray();
            formation[0].AiController.FollowAtDistance(ball.Owner, 1);
            formation[1].AiController.FollowAtDistance(ball.Owner, 1);
            formation[2].AiController.FollowAtDistance(formation[0], 2);
            if (formation.Length > 3)
            {
                formation[3].AiController.FollowAtDistance(formation[1], 2);
            }
            if (ball.Owner.AiController.enabled)
            {
                var owner = ball.Owner;

                var ai = ball.Owner.AiController;

                var cols = Physics.OverlapSphere(owner.transform.position, crammedRange, rugbistMask);
                List<Rugbist> enemies = new List<Rugbist>();
                foreach(var col in cols)
                {
                    var rug = col.GetComponent<Rugbist>();
                    if (rug.Team != this && !enemies.Contains(rug)) enemies.Add(rug);
                }
                bool crammed = enemies.Count > crammedCount;
                if (crammed)
                {
                    ai.Pass(ai.transform.forward);
                }
                else
                {
                    float z = GameManager.finishZ;
                    if (this == GameManager.instance.TeamA) z = -z;
                    Vector3 destination = ball.Owner.transform.position + Vector3.forward * z;
                    ai.MoveTowards(destination);
                }
            }
        }
        else
        {
            foreach(var r in rugbists) r.AiController.ChaseBall();
        }
    }
}
