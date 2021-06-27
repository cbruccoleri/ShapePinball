using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    static readonly int MOUSE_BUTTON_LEFT = 0;

    [SerializeField]
    private ParticleSystem _particleBounce;

    // True if the user clicked on the screen
    private bool _userClicked = false;
    
    // Save the position where the user clicked
    private Vector3 _worldPosClicked;

    // Rigidbody component of this object
    private Rigidbody2D _rigidBody;


    private void Awake()
    {
        _rigidBody = gameObject.GetComponent<Rigidbody2D>();
    }


    // Start is called before the first frame update
    void Start()
    {
        // give the player a nudge to get it going
        float fx = UnityEngine.Random.Range(2.0f, 5.0f);
        float fy = UnityEngine.Random.Range(2.0f, 5.0f);
        _rigidBody.AddForce(new Vector2(fx, fy), ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(MOUSE_BUTTON_LEFT)) {
            _worldPosClicked = Camera.main.
                ScreenToWorldPoint(Input.mousePosition);
            _worldPosClicked.z = transform.position.z;
            _userClicked = true;
        }
    }

    private void FixedUpdate()
    {
        if (_userClicked) {
            HandleUserClick();
            _userClicked = false;
        }
        ApplyForces();
    }


    private void ApplyForces()
    {
        Vector3 attractionForce = GameManager.Instance.
            GetAttractionForceAtLocation(transform.position);
        _rigidBody.AddForce(attractionForce, ForceMode2D.Force);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D contactPoint = collision.GetContact(0);
        _particleBounce.transform.position = contactPoint.point;
        _particleBounce.Play();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameManager.Instance.GameIsOver();
    }

    private void HandleUserClick()
    {
        Vector3 vecForce = (_worldPosClicked - transform.position).normalized * 5.0f;
        _rigidBody.AddForce(vecForce, ForceMode2D.Impulse);
    }
}
