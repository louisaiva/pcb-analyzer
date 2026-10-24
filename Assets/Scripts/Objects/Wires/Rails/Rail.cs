using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rail : MonoBehaviour
{
    public Vector2Int position;
    private List<Lane> lanes = new List<Lane>(3) { new Lane(-2), new Lane(0), new Lane(2)};


    // POSITION HANDLING
    public Vector2 GetWorldRightOffseted(int px_offset)
    {
        float xpos = transform.position.x + PCB.cell_size / 2f * (1f / 24f);
        float ypos = transform.position.y + px_offset * (1f / 24f);
        return new Vector2(xpos, ypos);
    }
    public Vector2 GetWorldLeftOffseted(int px_offset)
    {
        float xpos = transform.position.x - PCB.cell_size / 2f * (1f / 24f);
        float ypos = transform.position.y + px_offset * (1f / 24f);
        return new Vector2(xpos, ypos);
    }
    public Vector2 GetWorldUpOffseted(int px_offset)
    {
        float xpos = transform.position.x + px_offset * (1f / 24f);
        float ypos = transform.position.y + PCB.cell_size / 2f * (1f / 24f);
        return new Vector2(xpos, ypos);
    }
    public Vector2 GetWorldDownOffseted(int px_offset)
    {
        float xpos = transform.position.x + px_offset * (1f / 24f);
        float ypos = transform.position.y - PCB.cell_size / 2f * (1f / 24f);
        return new Vector2(xpos, ypos);
    }


    // LANES HANDLING
    public bool HasFreeLane()
    {
        for (int i = 0; i < 3; i++)
        {
            if (lanes[i].wire == null) { return true; }
        }
        return false;
    }
    public Lane GetFreeLane()
    {
        List<Lane> free_lanes = lanes.Where(l => l.wire == null).ToList();
        if (free_lanes.Count == 0) { return null; }
        return free_lanes[UnityEngine.Random.Range(0,free_lanes.Count)];
    }
    public Lane WireFreeLane(Wire wire)
    {
        Lane lane = GetFreeLane();
        if (lane == null) { Debug.LogError($"(Rail) {name} has no free lanes to wire {wire.name} !!"); return null; }
        lane.wire = wire;
        return lane;
    }
    public Lane GetWiredLane(Wire wire)
    {
        if (wire == null) { return null; }
        foreach (Lane lane in lanes)
        {
            if (lane.wire == wire) { return lane; }
        }
        return null;
    }
}

public class Lane
{
    public Lane(int px_offset=0)
    {
        this.pixel_offset= px_offset;
    }
    public int pixel_offset = 0;
    public Wire wire = null;
}