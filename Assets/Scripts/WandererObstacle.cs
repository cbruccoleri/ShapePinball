using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public class WandererObstacle : AnimatedObstacle
{

    private Rigidbody2D _rigidbody;

    private ConcurrentQueue<Vector3> _actions = new ConcurrentQueue<Vector3>();

    protected override void DoAction()
    {
        // TODO: implement energy-seeking behavior
        RandomNudge();
        return;
    }

    private void RandomNudge()
    {
        const float pAction = 0.002f;
        float randomValue = Random.Range(0.0f, 1.0f);
        if (randomValue < pAction) {
            if (_actions.Count < 10) {
                float forceMag = Random.Range(0.1f, 2.0f);
                Vector3 forceVec = GameManager.RandomDirection() * forceMag;
                _actions.Enqueue(forceVec);
                //Debug.Log($"Nudge Wanderer: {forceVec}");
            }
        }
    }

    protected override void PostCollisionBehavior()
    {
        // nothing to do for now
        return;
    }

    private void FixedUpdate()
    {
        Vector3 forceFromBlackHoles = GameManager.Instance.
            BlackHolesOnlyForceAtLocation(transform.position);
        _rigidbody.AddForce(forceFromBlackHoles, ForceMode2D.Force);
        while (_actions.Count > 0) {
            Vector3 force;
            _actions.TryDequeue(out force);
            _rigidbody.AddForce(force, ForceMode2D.Impulse);
        }
    }


    private void Awake()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
    }

}
