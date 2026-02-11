using System.Text;
using UnityEngine;

public class ListKfsIndices : MonoBehaviour
{
    Kfs[] _kfss;

    public Kfs GetKfs(int index) => _kfss[index];
    void Start()
    {
        _kfss = new Kfs[Kfs.MaxIndex + 1];

        for (var i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (!child.TryGetComponent(out Kfs kfs)) continue;
            var index = kfs.GetIndex();
            _kfss[index] = kfs;
        }

        var builder = new StringBuilder();

        for (var i = 0; i < _kfss.Length; i++)
        {
            builder.AppendLine($"{i}: {_kfss[i].name}");
        }

        Debug.Log(builder);
    }
}