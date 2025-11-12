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
}
