using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    [SerializeField] private int _score = 0;
    [SerializeField] private Kfs.Team _team;
    [SerializeField] private TMP_Text _stateNotifier;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private Transform _r2Robot;
    [SerializeField] private Transform _r2RetryZone;
    [SerializeField] private KfsSelection _kfsSelection;
    [SerializeField] private float _competitionTimer = 3 * 60;
    
    private InputAction _yesAction;

    private bool _yesPressed, _kfsFinished, _retryRequested;

    public Kfs.Team Team => _team;
    public bool KfsFinished => _kfsFinished;

    void Start()
    {
        _yesAction = InputSystem.actions.FindAction("Yes");
        _yesAction.performed += ctx => _yesPressed = true;

        StartCoroutine(RunStates());
    }

    IEnumerator RunStates()
    {
        // Setup
        _stateNotifier.text = "Setup";
        yield return new WaitUntil(() => _kfsFinished);

        while (true) {
            // Ongoing
            _kfsSelection.gameObject.SetActive(false);
            _r2Robot.position = _r2RetryZone.position;
            _stateNotifier.text = "";
            
            var timer = _competitionTimer;
            while (timer > 0)
            {
                if (_retryRequested) break;

                _timerText.text = $"{Mathf.FloorToInt(timer)}";
                timer -= Time.deltaTime;
                yield return null;
            }

            Time.timeScale = 0;
            if (timer <= 0) _timerText.text = "";

            if (_retryRequested)
            {
                _stateNotifier.text = "Y to retry.";
                yield return new WaitUntil(() => _yesPressed);
                _yesPressed = false;
            }
            else
            {
                _stateNotifier.text = "Finished. Y to restart";
                yield return new WaitUntil(() => _yesPressed);
                _yesPressed = false;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    public void RequestRetry() => _retryRequested = true;
    public void FinishKfsSelection() => _kfsFinished = true;
}
