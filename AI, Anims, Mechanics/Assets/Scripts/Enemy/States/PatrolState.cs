using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : BaseState
{
    public int waypointIndex; //tracks which waypoint currently being targeted
    public float waitTimer;
    public override void Enter() {}

    public override void Perform() {
        PatrolCycle();
    }

    public override void Exit() {}

    public void PatrolCycle()
    {
        //implement patrol logic.
        if(enemy.Agent.remainingDistance < 0.2f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer > 3) //time between next movement
            {
                if (waypointIndex < enemy.path.waypoints.Count - 1)
                    waypointIndex++;
                else
                    //fail safe
                    waypointIndex = 0;
                enemy.Agent.SetDestination(enemy.path.waypoints[waypointIndex].position);
                waitTimer = 0;
            }
        }
    }
}
