using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }

    #region InspectorProperties
    [SerializeField]
    private List<GameObject> _attractors = new List<GameObject>();

    [SerializeField]
    private bool _debugMode = false;

    [SerializeField]
    private GameObject _player;
    #endregion

    private bool _gameOver = false;

    public bool GameOver {
        get => _gameOver;
        private set => _gameOver = value;
    }

    public void GameIsOver()
    {
        if (!_debugMode) {
            GameOver = true;
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

    public int score {
        get => _score; 
        private set => _score = value > 0 ? value : _score;
    }

    public void UpdateScore(int pointsToAdd)
    {
        score += pointsToAdd;
        Debug.Log($"Score: {score}");
    }


    private void Awake()
    {
        // Implement singleton
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        }
        else {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // InitializeUI
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: update UI, etc.
    }
}
