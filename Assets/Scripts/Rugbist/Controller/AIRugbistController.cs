using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRugbistController : RugbistController {
    protected override bool ActiveByDefault => true;
    public bool Constrained => agent.Defending;

    public void Follow(Rugbist rugbist)
    {
        if (rugbist.Team == base.rugbist.Team) FollowAtDistance(rugbist, teammateDistance);
        else ChaseTarget(rugbist);
    }

    //may do
    //something?

    public void Constrain(float minZ, float maxZ)
    {
        agent.Defend(minZ, maxZ);
    }
}
