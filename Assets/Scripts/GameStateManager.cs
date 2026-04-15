using TMPro;
using UnityEditor.SearchService;
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
    
    private GameState _state = new GameState.Setup();
    private InputAction _yesAction;

    public Kfs.Team Team => _team;

    void Start()
    {
        _yesAction = InputSystem.actions.FindAction("Yes");
        _yesAction.performed += (ctx) =>
        {
            switch (_state)
            {
                case GameState.Finished:
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    break;
                case GameState.Retry:
                    RequestOngoing();
                    break;
                case GameState.Ongoing:
                    RequestRetry();
                    break;
            }
        };
    }

    public void RequestRetry()
    {
        _state = new GameState.Retry();
        Time.timeScale = 0;
        _stateNotifier.text = "Y to retry";
    }
    public void RequestSetup()
    {
        _kfsSelection.gameObject.SetActive(true);
        _state = new GameState.Setup();
        Time.timeScale = 1;
        _stateNotifier.text = "Setup";
    }
    public void RequestFinished()
    {
        _state = new GameState.Finished();
        Time.timeScale = 0;
        _stateNotifier.text = "Finished. Y to restart";
    }
    public void RequestOngoing()
    {
        _kfsSelection.gameObject.SetActive(false);
        _r2Robot.position = _r2RetryZone.position;
        _state = new GameState.Ongoing();
        Time.timeScale = 1;
        _stateNotifier.text = "";
    }

    void Update()
    {
        switch (_state)
        {
            case GameState.Ongoing ongoing:
                ongoing.Timer -= Time.deltaTime;
                _timerText.text = $"{Mathf.FloorToInt(ongoing.Timer)}";
                if (ongoing.Timer <= 0)
                    RequestFinished();
                break;
        }
    }
}
public abstract class GameState
{
    public class Setup : GameState {}
    public class Ongoing : GameState
    {
        public float Timer = 3 * 60;
    }
    public class Retry : GameState {}
    public class Finished : GameState {}
}
