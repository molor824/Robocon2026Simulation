using System.Linq;
using UnityEngine;

public class Spearhead : ClassIndex
{
    [SerializeField] private SpearType Type;

    public override int Index => Kfs.MaxIndex + 1 + (int)Type;
}
public enum SpearType
{
    Spear,
    Hand,
    Fist,
}