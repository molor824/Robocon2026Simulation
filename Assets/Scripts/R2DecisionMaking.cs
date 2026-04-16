using UnityEngine;

[RequireComponent(typeof(R2Movement))]
public class R2DecisionMaking : MonoBehaviour
{
    private R2Movement _movement;
    
    [SerializeField] private GameStateManager _manager;

    void Start()
    {
        _movement = GetComponent<R2Movement>();
    }
}
