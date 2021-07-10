using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class GameManager : MonoBehaviour
{

    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }

    /// <summary>
    /// Calculate the force of attraction (or repulsion) using a squared distance field calculation.
    /// </summary>
    /// <param name="pos"></param>  The position of the body being attracted or repulsed.
    /// <param name="bodyPos"></param>  The position of the body attracting or repulsing
    /// <param name="forceConstant"></param>  A force constant multiplier, default 1.0f
    /// <returns></returns>
    public static Vector3 SquaredDistanceForce(Vector3 pos, Vector3 bodyPos, float forceConstant = 1.0f)
    {
        const float MIN_DIST = 1.0f;
        Vector3 vecToBody = (bodyPos - pos);
        float distance = vecToBody.magnitude;
        if (distance < MIN_DIST) {
            // gotta have this or else the computer gets swallowed by the math black hole
            distance = MIN_DIST;
        }
        return vecToBody.normalized * forceConstant / (distance * distance);
    }


    public static Vector3 RandomDirection()
    {
        // not all directions are equally likely, but good enough
        return new Vector3(
            Random.Range(-1.0f, 1.0f),
            Random.Range(-1.0f, 1.0f),
            0.0f).normalized;
    }


    #region InspectorProperties
    // ENCAPSULATION
    [SerializeField]
    private bool _debugMode = false;

    [SerializeField]
    private GameObject _player;

    [SerializeField]
    private GameObject _obstacleGroup;

    [SerializeField]
    private GameObject _ripplePlane;

    [SerializeField, Range(0.0f, 5.0f)]
    private float _rippleDuration = 2.0f;

    #endregion

    private List<GameObject> _attractors = new List<GameObject>();

    private List<GameObject> _blackHoles = new List<GameObject>();

    private bool _gameOver = false;

    private bool _pause = false;

    // ENCAPSULATION
    public bool IsPaused {
        get => _pause;
        private set => _pause = value;
    }

    public bool GameOver {
        get => _gameOver;
        private set => _gameOver = value;
    }

    #region MouseRippleHandling

    private Queue<GameObject> _queueRipples = new Queue<GameObject>();

    public GameObject GetRippleAtLocation(Vector3 pos)
    {
        GameObject objInstance;

        if (_queueRipples.Count > 0) { // get object from the queue
            objInstance = _queueRipples.Dequeue();
        }
        else {
            objInstance = GameObject.Instantiate(_ripplePlane, _ripplePlane.transform);
        }
        objInstance.transform.position = pos;
        objInstance.GetComponent<RipplePlaneControl>().ActivateFromPool(_rippleDuration);
        return objInstance;
    }


    public void ReturnToPool(GameObject objInstance)
    {
        _queueRipples.Enqueue(objInstance);
    }
    #endregion

    public void GameIsOver()
    {
        if (!_debugMode) {
            GameOver = true;
            //PauseGame();
            _player.SetActive(false);
        }
        else {
            Debug.Log("You crossed the event horizon...");
        }
    }

    public List<GameObject> Attractors { 
        get => _attractors;
        private set => _attractors = value;
    }

    private int _score = 0;

    private bool _drawVectorField;

    public int score {
        get => _score; 
        private set => _score = value > 0 ? value : _score;
    }

    public void UpdateScore(int pointsToAdd)
    {
        score += pointsToAdd;
        Debug.Log($"Score: {score}");
    }

    // ABSTRACTION
    public Vector3 GetAttractionForceAtLocation(Vector3 position)
    {
        Vector3 vecForce = Vector3.zero;
        foreach (var body in _attractors) {
            vecForce += GameManager.SquaredDistanceForce(
                position, 
                body.transform.position, 
                body.GetComponent<AnimatedObstacle>().AttractionForce
            );
            //if (_debugMode) { // Note: Gizmos must be enabled
            //    Debug.DrawRay(position, vecForce);
            //}
        }
        return vecForce;
    }

    // ABSTRACTION
    public Vector3 BlackHolesOnlyForceAtLocation(Vector3 position)
    {
        Vector3 vecForce = Vector3.zero;
        foreach (var body in _blackHoles) {
            vecForce += GameManager.SquaredDistanceForce(
                position,
                body.transform.position,
                body.GetComponent<AnimatedObstacle>().AttractionForce
            );
        }
        return vecForce;
    }


    void Awake()
    {
        // Implement singleton
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        }
        else {
            _instance = this;
        }
        // subscribe to receive events from pooled objects
        RipplePlaneControl.OnDeactivate += ReturnToPool;
    }

    private void OnDestroy()
    {
        // unsubscribe to receive events from pooled objects
        RipplePlaneControl.OnDeactivate -= ReturnToPool;
    }

    // Start is called before the first frame update
    void Start()
    {
        PopulateAttractors();
        // InitializeUI
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: update UI, etc.
        if (Input.GetKeyDown(KeyCode.Pause)) {
            TogglePause();
        }
        if (Input.GetKeyDown(KeyCode.F1)) {
            ToggleDrawVectorField();
        }
    }

    private void TogglePause()
    {
        IsPaused = !IsPaused;
        if (IsPaused) {
            PauseGame();
        }
        else {
            RestartGameFromPause();
        }
    }

    private void PauseGame()
    {
        // TODO: other actions here
        Time.timeScale = 0;
    }

    private void RestartGameFromPause()
    {
        // TODO: other actions here
        Time.timeScale = 1.0f;
    }

    /// <summary>
    /// Make a list of all attractors in the scene
    /// </summary>
    private void PopulateAttractors()
    {
        Transform transform = _obstacleGroup.transform;
        for (int i = 0; i < transform.childCount; ++i) {
            _attractors.Add(transform.GetChild(i).gameObject);
        }
        // Add all the black holes to the list
        _blackHoles.AddRange(GameObject.FindGameObjectsWithTag("BlackHole"));
    }


    private void OnDrawGizmos()
    {
        if (_drawVectorField) {
            DrawVectorField();
        }
    }

    private void ToggleDrawVectorField()
    {
        _drawVectorField = !_drawVectorField;
    }

    private void DrawVectorField()
    {
        const float MinX = -20.0f, MaxX = 20.0f;
        const float MinY = -10.0f, MaxY = 10.0f;
        Vector3 cubeSize = new Vector3( 0.1f, 0.1f, 0.1f );
        const float forceScaleFactor = 0.2f;

        for (float x = MinX; x <= MaxX; x += 1.0f) {
            for (float y = MinY; y <= MaxY; y += 1.0f) {
                Vector2 position = new Vector2(x, y);
                Vector3 forceField = forceScaleFactor * GetAttractionForceAtLocation(position);
                Debug.DrawRay(position, forceField, Color.red);
                Gizmos.DrawCube(position, cubeSize);
            }
        }
    }
}
