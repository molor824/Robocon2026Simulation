using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(R2Movement), typeof(R2GrabKfs))]
public class R2DecisionMaking : MonoBehaviour
{
    private static readonly Ray2D[] _meihuaEnterTargets =
    {
        new(new(3, -3.7f), Vector2.up),
    };
    private static readonly Ray2D[] _meihuaExitTargets =
    {
        // new(new())
        new(new(5.15f, 2.75f), Vector2.up),
        new(new(4.57f, 5.41f), Vector2.left),
        new(new(1.43f, 4.73f), Vector2.left),
    };
    private static readonly int[] _ticCellOrder = {4, 3, 5};
    private static readonly Ray2D[] _ticTarget =
    {
        new(new(0.75f, 4.75f), Vector2.left),
        new(new(0.75f, 4.2f), Vector2.left),
        new(new(0.75f, 5.3f), Vector2.left),
    };

    private InputAction _continueAction;
    private R2Movement _movement;
    private R2GrabKfs _grab;
    private bool _continue = false;
    
    [SerializeField] private GameStateManager _manager;
    [SerializeField] private KfsSpawner _spawner;
    [SerializeField] private TicTacToe _ticTacToe;
    [SerializeField] private CameraStream _cameraStream;
    [SerializeField] private RawImage _rtImage;

    void Start()
    {
        _continueAction = InputSystem.actions.FindAction("Continue");
        _continueAction.started += _ => _continue = true;
        _movement = GetComponent<R2Movement>();
        _grab = GetComponent<R2GrabKfs>();
        StartCoroutine(StateRoutine());
    }
    
    IEnumerator StateRoutine()
    {
        // Setup
        Debug.Log("Setting up");
        yield return new WaitUntil(() => _manager.KfsFinished);
        yield return new WaitUntil(() => _continue);

        _cameraStream.gameObject.SetActive(true);
        _rtImage.gameObject.SetActive(true);

        // Meihua
        Debug.Log("Entering meihua");

        // Enter meihua
        foreach (var target in _meihuaEnterTargets)
            _movement.AddTarget(target);
        
        // Calculate which column to move through to get a block
        // First check, middle
        var navigated = false;
        for (var i = 1; i < 12; i += 3)
        {
            // Ignore spot with multiple kfs
            var kfs = _spawner.AtSpawner(i);
            if (kfs != null && kfs.KfsType == Kfs.Type.Real && kfs.KfsTeam == Kfs.Team.Red)
            {
                // Matching, select middle road, move until you reach right before the kfs
                var position = _spawner.Spawners[i].position;
                // Move to previous position
                var flatPosition = new Vector2(position.x, position.z - 1.2f);

                _movement.AddTarget(new(flatPosition, Vector2.up));
                yield return new WaitUntil(() => _movement.Targets.Count == 0);
                
                _grab.StartGrab(kfs);
                yield return new WaitUntil(() => !_grab.Grabbing);

                // now move until the last then turn right and get out through the right
                position = _spawner.Spawners[10].position;
                flatPosition = new Vector2(position.x, position.z);
                _movement.AddTarget(new(flatPosition, Vector2.right));
                _movement.AddTarget(new(flatPosition + Vector2.right * 1.2f, Vector2.up));
                _movement.AddTarget(new(flatPosition + new Vector2(1.2f, 1.2f), Vector2.up));

                navigated = true;
                break;
            }
        }
        if (!navigated)
        {
            // Second check, first
            for (var i = 0; i < 12; i += 3)
            {
                // Ignore spot with multiple kfs
                var kfs = _spawner.AtSpawner(i);
                if (kfs != null && kfs.KfsType == Kfs.Type.Real && kfs.KfsTeam == Kfs.Team.Red)
                {
                    // Matching, select first road, move until you reach next to kfs
                    // Then turn left
                    var position = _spawner.Spawners[i + 1].position;
                    // Move to previous position
                    var flatPosition = new Vector2(position.x, position.z);

                    _movement.AddTarget(new(flatPosition, Vector2.left));
                    yield return new WaitUntil(() => _movement.Targets.Count == 0);

                    _grab.StartGrab(kfs);
                    yield return new WaitUntil(() => !_grab.Grabbing);

                    // now jump onto the grabbed kfs and continue
                    flatPosition += new Vector2(-1.2f, 0);
                    _movement.AddTarget(new(flatPosition, Vector2.up));
                    position = _spawner.Spawners[9].position;
                    flatPosition = new Vector2(position.x, position.z + 1.2f);
                    _movement.AddTarget(new(flatPosition, Vector2.up));

                    navigated = true;
                    break;
                }
            }
        }
        if (!navigated)
        {
            // Third check, last
            for (var i = 2; i < 12; i += 3)
            {
                var kfs = _spawner.AtSpawner(i);
                // Ignore spot with multiple kfs
                if (kfs != null && kfs.KfsType == Kfs.Type.Real && kfs.KfsTeam == Kfs.Team.Red)
                {
                    // Matching, select last road, move until you reach next to kfs
                    // Then turn right
                    var position = _spawner.Spawners[i - 1].position;
                    // Move to previous position
                    var flatPosition = new Vector2(position.x, position.z);

                    _movement.AddTarget(new(flatPosition, Vector2.right));
                    yield return new WaitUntil(() => _movement.Targets.Count == 0);

                    _grab.StartGrab(kfs);
                    yield return new WaitUntil(() => !_grab.Grabbing);

                    // now jump onto the grabbed kfs and continue
                    flatPosition += new Vector2(1.2f, 0);
                    _movement.AddTarget(new(flatPosition, Vector2.up));
                    position = _spawner.Spawners[11].position;
                    flatPosition = new Vector2(position.x, position.z + 1.2f);
                    _movement.AddTarget(new(flatPosition, Vector2.up));

                    navigated = true;
                    break;
                }
            }
        }

        if (!navigated)
        {
            _manager.RequestRetry();
            yield break;
        }

        foreach (var target in _meihuaExitTargets)
            _movement.AddTarget(target);
        
        yield return new WaitUntil(() => _movement.Targets.Count == 0);

        while (true) {
            var found = false;
            for (int i = 0; i < _ticCellOrder.Length; i++)
            {
                int cellIndex = _ticCellOrder[i];
                var cell = _ticTacToe.Cells[cellIndex];
                if (cell.GetKfs() != null) continue;

                _movement.AddTarget(_ticTarget[i]);
                found = true;
                break;
            }
            if (found) break;

            yield return null;
        }
        yield return new WaitUntil(() => _movement.Targets.Count == 0);
        _grab.StartRelease();
    }
}
