using System;
using UnityEngine;

public class Welder : MonoBehaviour
{
    
}

public class Weld
{
    public Component component;
    public Vector2 position;
    public Vector2Int exit_direction;
}
[Serializable] public class WeldData
{
    public Vector2Int pixel;
    public Vector2Int exit;


    public string GetDetails()
    {
        string debug = $"    pixel : {pixel}           /////       exit : {exit}";
        return debug;
    }
}