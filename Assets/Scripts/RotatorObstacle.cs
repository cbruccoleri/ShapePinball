using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INHERITANCE
public class RotatorObstacle : AnimatedObstacle
{
    #region InspectorProperties
    // Color when hit
    [SerializeField]
    private Color _hitColor = new Color (1.0f, 0.2f, 0.0f, 1.0f);

    [SerializeField]
    private float _timerHitDuration = 8.0f;

    // Rotation speed for this object
    [SerializeField]
    protected float _rotationSpeed = -9; // deg/sec
    #endregion

    // helper property to remember the original color
    private Color _originalColor;

    // reference to the SpriteRenderer component to change the color
    private SpriteRenderer _spriteRenderer;

    // True if the hit timer is running
    private bool _hasBeenHit = false;

    // Current value of the Hit Timer
    private float _timerHit = 0;

    // Current Rotation angle
    protected float _rotationAngle = 0;

    protected override void DoAction()
    {
        _rotationAngle += _rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, _rotationAngle);
        if (_hasBeenHit) {
            _timerHit -= Time.deltaTime;
            if (_timerHit <= 0) {
                _hasBeenHit = false;
            }
            float tParam = Mathf.InverseLerp(0, _timerHitDuration, _timerHit) ;
            _spriteRenderer.color = Color.Lerp(_originalColor, _hitColor, tParam);
        }
    }
    
    // POLYMORPHISM
    protected override void PostCollisionBehavior()
    {
        GameManager.Instance.UpdateScore(PointValue);
        _spriteRenderer.color = _hitColor;
        _hasBeenHit = true;
        _timerHit = _timerHitDuration;
        _rotationSpeed *= 1.1f; // TODO: find balance and set maximum speed
    }

    private void Awake()
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _originalColor = _spriteRenderer.color;
        _rotationAngle = Random.Range(0, 360.0f);
    }

}
