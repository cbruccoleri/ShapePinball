using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimatedObstacle : MonoBehaviour
{
    [SerializeField]
    protected int _baseScore = 100;

    [SerializeField]
    protected int _pointMultiplier = 2;

    public int PointValue => _pointMultiplier * _baseScore;

    public int PointMultiplier { 
        get => _pointMultiplier; 
        
        private set
        {
            if (value > 1) {
                _pointMultiplier = value;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        DoAction();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PostCollisionBehavior();
    }

    /// <summary>
    /// Implement the default action for this type of object
    /// </summary>
    protected abstract void DoAction();

    /// <summary>
    /// Implement the post-action behaviors for the object. This can be a power-up or similar.
    /// </summary>
    protected abstract void PostCollisionBehavior();
}
