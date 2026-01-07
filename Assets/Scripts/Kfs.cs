using System;
using UnityEngine;

public class Kfs : MonoBehaviour
{
    public enum Type
    {
        Real,
        Fake,
        R1,
    }
    public enum Team
    {
        Red,
        Blue,
    }

    public Type KfsType;
    public Team KfsTeam;
    public int KfsIndex;

    // Used for classifying kfss
    // Indices:
    // 0-14 Red Real
    // 15-29 Red Fake
    // 30 Red R1
    // 31-45 Blue Real
    // 46-60 Blue Fake
    // 61 Blue R1
    public int GetIndex()
    {
        var index = KfsIndex;
        if (KfsTeam == Team.Blue)
            index += 31;
        if (KfsType == Type.Fake)
            index += 15;
        else if (KfsType == Type.R1)
            index++;
        return index;
    }

}
