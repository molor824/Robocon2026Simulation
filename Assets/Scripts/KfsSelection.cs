using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KfsSelection : MonoBehaviour
{
    [SerializeField] private GameStateManager _manager;
    [SerializeField] private KfsSpawner _kfsSpawner;
    [SerializeField] private TMP_Text _kfsText;

    private List<Kfs.Type> _orders = new() {
        Kfs.Type.R1,
        Kfs.Type.R1,
        Kfs.Type.R1,
        Kfs.Type.Real,
        Kfs.Type.Real,
        Kfs.Type.Real,
        Kfs.Type.Real,
        Kfs.Type.Fake
    };
    private Button[] _kfsButtons;

    void Start()
    {
        _kfsText.text = _orders[0] switch
        {
            Kfs.Type.R1 => "R1",
            Kfs.Type.Real => "R2",
            _ => "Fake",
        };
        _kfsButtons = new Button[12];
        for (int i = 0; i < _kfsButtons.Length; i++)
        {
            var button = transform.Find($"Button{i}").GetComponent<Button>();
            _kfsButtons[i] = button;

            var index = i;
            button.onClick.AddListener(() =>
            {
                if (_orders.Count == 0)
                {
                    _manager.FinishKfsSelection();
                    return;
                }
                _kfsSpawner.SpawnKfsAt(index, _manager.Team, _orders[0]);
                _orders.RemoveAt(0);
                if (_orders.Count == 0)
                    _manager.FinishKfsSelection();
                else
                    _kfsText.text = _orders[0] switch
                    {
                        Kfs.Type.R1 => "R1",
                        Kfs.Type.Real => "R2",
                        _ => "Fake",
                    };
                button.enabled = false;
            });
        }
    }
}
