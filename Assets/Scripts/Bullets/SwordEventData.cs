using UnityEngine;

[System.Serializable]
public class SwordEventData
{
    public bool isSharp;
    public int location;

    public SwordEventData(bool isSharp, int location)
    {
        this.isSharp = isSharp;
        this.location = location;
    }
}
