using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Rugbist))]
public class MoveAgent : MonoBehaviour {
    private float speed;

    //Rugbist rugbist;
    //Rugbist targetRugbist = null;
    Transform target = null;
    float keepDistance;
    Vector3? destination = null;
    Vector3 dir;
    Rigidbody rb;
    bool stopped = true;
    bool stopIfMoved = false;
    Vector3 lastPos;

    const float approachDist = 0.1f;
    const float lift = 0.1f;

    public bool Defending { get; private set; }
    float minZ, maxZ;

    public void Init(Rugbist rugbist, float speed)
    {
        rb = GetComponent<Rigidbody>();
        //this.rugbist = rugbist;
        this.speed = speed;
    }

    public void Stop()
    {
        destination = null;
        target = null;
        stopped = true;
    }
    
    public void MoveTowards(Vector3 destination)
    {
        this.destination = destination;
        target = null;
        dir = (destination - transform.position).normalized;
        stopped = false;
        transform.LookAt(destination);
    }

    #region Defense

    public void Defend(float minZ, float maxZ)
    {
        Defending = true;
        this.minZ = minZ;
        this.maxZ = maxZ;
    }

    public void ReleaseFromDefense()
    {
        Defending = false;
    }

    #endregion

    #region Chase

    public void ChaseWhileUnmoved(Transform target)
    {
        //targetRugbist = null;
        this.target = target;
        destination = null;
        stopped = false;
        stopIfMoved = true;
        lastPos = target.position;
        keepDistance = approachDist;
    }

    public void ChaseTarget(Rugbist target)
    {
        //targetRugbist = target;
        this.target = target.transform;
        destination = null;
        stopped = false;
        stopIfMoved = false;
        keepDistance = approachDist;
    }

    public void ChaseTarget(Rugbist target, float keepDistance)
    {
        ChaseTarget(target);
        this.keepDistance = keepDistance;
    }

    #endregion

    public void Collapse(Vector3 dir)
    {
        dir.Normalize();
        rb.isKinematic = false;
        rb.angularVelocity = transform.right * lift;
        rb.velocity = dir + Vector3.up * lift;
    }

    public void Recover()
    {
        rb.isKinematic = true;
        transform.rotation = Quaternion.identity;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    void FixedUpdate()
    {
        if (stopped) return;

        if(target == null)
        {
            Vector3 moveDir = dir;
            if (Defending)
            {
                if (transform.position.z > maxZ) moveDir.z = -1;
                else if (transform.position.z < minZ) moveDir.z = 1;
                moveDir.Normalize();
            }
            moveDir *= speed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + moveDir);
            if (Vector3.SqrMagnitude(destination.Value - transform.position) < approachDist)
            {
                //might do
                //some kind of alert
                destination = null;
                stopped = true;
            }
        }
        else
        {
            if (stopIfMoved)
            {
                if(target.position != lastPos)
                {
                    target = null;
                    stopped = true;
                    return;
                }
            }
            transform.LookAt(target);
            if(Vector3.SqrMagnitude(target.position - transform.position) > Mathf.Pow(keepDistance, 2))
            {
                Vector3 move = (target.position - transform.position).normalized;
                if (Defending)
                {
                    if (transform.position.z > maxZ) move.z = -1;
                    else if (transform.position.z < minZ) move.z = 1;
                    move.Normalize();
                }
                move *= speed * Time.fixedDeltaTime;
                rb.MovePosition(rb.position + move);
            }
        }
    }
}
