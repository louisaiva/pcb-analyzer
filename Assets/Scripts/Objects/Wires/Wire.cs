using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Wire : MonoBehaviour
{
    [SerializeField] private SpriteRenderer line_prefab;
    [SerializeField] private List<SpriteRenderer> lines;
    [SerializeField] private Color connected_color;
    [SerializeField] private Color disconnected_color;
    [SerializeField] private bool connected = false;
    [SerializeField] private bool first_check = false;
    public bool Connected => connected;
    public List<RailCross> crosses = new List<RailCross>();


    [Header("Logs")]
    [SerializeField] private bool log = false;


    ///
    //
    ///  CONNECTION UPDATE
    //
    ///


    private void Update() 
    {
        if (start == null || end == null) { return; }
        update_connection();
    }
    private void update_connection()
    {
        if (start.component.IsDamaged) { disconnect(); return; }
        if (end.component.IsDamaged) { disconnect(); return; }
        foreach (RailCross cross in crosses)
        {
            if (cross.Rocked) { disconnect(); return; }
        }

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
        if (connected && first_check) { return; }
        foreach (SpriteRenderer line in lines)
        {
            line.color = connected_color;
            line.sortingOrder = 0;
        }
        connected = true;
        first_check = true;
        if (log) { Debug.Log($"Wire {name} is now connected !"); }
    }
    private void disconnect()
    {
        if (!connected && first_check) { return; }
        foreach (SpriteRenderer line in lines)
        {
            line.color = disconnected_color;
            line.sortingOrder = -1;
        }
        connected = false;
        first_check = true;
        if (log) { Debug.Log($"Wire {name} is now disconnected :/"); }
    }



    ///
    //
    ///  BUILDING THE WIRE
    //
    ///
    private Weld start = null;
    private Weld end = null;

    public void Clear()
    {
        foreach (SpriteRenderer line in lines)
        {
            Destroy(line.gameObject);
        }
        lines.Clear();
    }
    public void BuildWire(Weld start, List<Vector2> path, Weld end, List<RailCross> crosses)
    {
        Clear();
        this.start = start;
        this.end = end;
        this.crosses = crosses;

        lines.Add(create_line(start.world_position, path[0]));

        for (int i=0; i<path.Count-1; i++)
        {
            lines.Add(create_line(path[i],path[i+1]));
        }

        lines.Add(create_line(path.LastOrDefault(), end.world_position));
    }
    private SpriteRenderer create_line(Vector2 start, Vector2 finish)
    {
        SpriteRenderer line = Instantiate(line_prefab, transform);

        Vector2 direction = finish - start;
        float distance = direction.magnitude;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float scaled_dist = distance * 24f;

        line.transform.position = start;
        if (scaled_dist > 1f) { line.transform.position = start - direction.normalized * 0.5f * (1f/24f); }

        // Rotate to face the finish point
        line.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Scale along the sprite's local X axis
        line.transform.localScale = (scaled_dist > 1f) ? new Vector3(scaled_dist + 1f, 1f, 1f) : new Vector3(scaled_dist, 1f, 1f);
        return line;
    }

    private void OnDrawGizmos()
    {
        if (lines.Count == 0) { return; }

        Gizmos.color = Color.blue;
        float radius = 0.03f;
        foreach (SpriteRenderer line in lines)
        {
            Gizmos.DrawSphere(line.transform.position, radius);
            Gizmos.DrawSphere(line.transform.position, radius);
        }
    }
}