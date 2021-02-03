using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Rugbist))]
public abstract class RugbistController : MonoBehaviour {
    protected abstract bool ActiveByDefault { get; }

    protected Rugbist rugbist;
    protected MoveAgent agent;
    protected Ball ball;

    protected float minX, maxX, minZ, maxZ;

    protected GameManager manager;

    protected const float teammateDistance = 0.5f;

    const float throwForce = 10;

    void Awake()
    {
        ball = Ball.instance;
        rugbist = GetComponent<Rugbist>();
        agent = GetComponent<MoveAgent>();
        if (!ActiveByDefault) enabled = false;
    }

    protected virtual void Start()
    {
        manager = GameManager.instance;
        minX = -manager.borderX;
        maxX = manager.borderX;
        minZ = -manager.borderZ;
        maxZ = manager.borderZ;
    }

    public void MoveTowards(Vector3 destination)
    {
        if (!enabled) return;
        agent.MoveTowards(destination);
    }

    public void ChaseTarget(Rugbist target)
    {
        if (!enabled) return;
        agent.ChaseTarget(target);
    }

    public void FollowAtDistance(Rugbist target, float distance)
    {
        if (!enabled) return;
        agent.ChaseTarget(target, distance);
    }

    public void ChaseBall()
    {
        if (!enabled) return;

        var owner = ball.Owner;

        if (owner == null) agent.MoveTowards(ball.transform.position);
        else if (owner.Team == rugbist.Team) agent.ChaseTarget(owner, teammateDistance);
        else agent.ChaseTarget(owner);
    }

    public void Kick()
    {
        //to do
    }

    public void Throw(Vector3 direction)
    {
        if (ball.Owner != rugbist) return;
        ball.Throw(direction * throwForce);
    }

    float GetAngle(Vector3 direction, Rugbist rugbist)
    {
        return Vector3.Angle(direction, rugbist.transform.position - rugbist.transform.position);
    }

    public void Pass(Vector3 direction)
    {
        if (ball.Owner != rugbist) return;
        var options = rugbist.Team.AllRugbists.Where(r => r != rugbist)
            .OrderBy(r => GetAngle(direction, r));
        var first = options.Where(r => GetAngle(direction, r) <= rugbist.Data.passAngle);
        Rugbist passTo = first.FirstOrDefault();
        if (passTo == null) Throw(direction);
        else
        {
            transform.LookAt(passTo.transform);
            Throw(direction);
        }
    }

    public void Drop()
    {
        if (ball.Owner != rugbist) return;
        ball.Release();
    }

    protected virtual void FixedUpdate()
    {
        float x = Mathf.Clamp(transform.position.x, minX, maxX);
        float y = Mathf.Clamp(transform.position.y, 0, rugbist.Collapsed ? 10 : 0);
        float z = Mathf.Clamp(transform.position.z, minZ, maxZ);
        transform.position = new Vector3(x, y, z);
    }

    public void ReleaseFromDefense()
    {
        agent.ReleaseFromDefense();
    }
}
