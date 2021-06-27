using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StretchingObstacle : AnimatedObstacle
{
    protected enum ScaleAxis { X_Axis, Y_Axis, Z_Axis};

    [SerializeField, Range(0.25f, 2.0f)]
    protected float _maxScaleFactor = 1.0f;

    [SerializeField, Range(0.1f, 2.0f)]
    protected float _stretchFreq = 0.25f;

    [SerializeField]
    private ScaleAxis _scaleAxis = ScaleAxis.X_Axis;

    private float _time = 0;

    private float _maxStretchFreq = 1.0f;
    
    protected override void DoAction()
    {
        _time += Time.deltaTime;
        SetScaleFactor(_time);
    }

    protected override void PostCollisionBehavior()
    {
        GameManager.Instance.UpdateScore(PointValue);
        _stretchFreq *= 1.1f;
        if (_stretchFreq > _maxStretchFreq)
            _stretchFreq = _maxStretchFreq;
    }

    private float SetScaleFactor(float time)
    {
        const float TAU = GameConstants.TAU;
        float scaleFactor = 1.25f + _maxScaleFactor * Mathf.Cos(TAU * _stretchFreq * _time);
        Vector3 objScale = transform.localScale;
        objScale[(int)_scaleAxis] = scaleFactor;
        transform.localScale = objScale;
        return scaleFactor;
    }

    private void Awake()
    {
        _maxStretchFreq = _stretchFreq * 4.0f;
    }

}
