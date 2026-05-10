using System.Collections.Generic;
using UnityEngine;

public class TicTacToe : MonoBehaviour
{
    private TicTacCell[] _cells;

    public IReadOnlyList<TicTacCell> Cells => _cells;

    void Start()
    {
        _cells = new TicTacCell[9];
        for (int i = 0; i < 9;)
        {
            var child = transform.GetChild(i);
            if (!child.TryGetComponent(out TicTacCell collider)) continue;
            _cells[i] = collider;
            i++;
        }
    }
}
