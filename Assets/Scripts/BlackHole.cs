using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : AnimatedObstacle
{
    protected override void DoAction()
    {
        // not doing much, sit there and suck-in
        return;
    }

    protected override void PostCollisionBehavior()
    {
        //TODO: animations and stuff will go here
        return;
    }
}
