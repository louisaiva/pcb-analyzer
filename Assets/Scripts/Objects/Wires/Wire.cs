using System.Collections.Generic;
using UnityEngine;

public class Wire : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> lines;
    [SerializeField] private Color connected_color;
    [SerializeField] private Color disconnected_color;
    [SerializeField] private bool connected = false;


    [Header("Logs")]
    [SerializeField] private bool log = false;



    private void Update() 
    {
        // check if all the lines are enabled, if
        update_connection();
    }

    private void update_connection()
    {
        foreach (SpriteRenderer line in lines)
        {
            if (log) { Debug.Log($"Checking line {line.name} : {line.gameObject.activeInHierarchy} - {line.gameObject.activeSelf}"); }
            if (!line.gameObject.activeInHierarchy) { disconnect(); return; }
        }

        connect();
    }


    // CONNECT / DISCONNECT
    private void connect()
    {
        if (connected) { return; }
        foreach (SpriteRenderer line in lines)
        {
            line.color = connected_color;
        }
        connected = true;
        if (log) { Debug.Log($"Wire {name} is now connected !"); }
    }
    private void disconnect()
    {
        if (!connected) { return; }
        foreach (SpriteRenderer line in lines)
        {
            line.color = disconnected_color;
        }
        connected = false;
        if (log) { Debug.Log($"Wire {name} is now disconnected :/"); }
    }
}