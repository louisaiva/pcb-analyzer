using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;

public class Rail : MonoBehaviour
{
    public Vector2Int RailPosition;
    private List<Lane> lanes = new List<Lane>();
    public Axis axis = Axis.Horizontal;
    public bool Horizontal => axis == Axis.Horizontal;

    public void Init(Vector2Int position, Axis axis)
    {
        RailPosition = position;
        this.axis = axis;
        lanes = new List<Lane>(3)
        {
            new Lane(this, -2),
            new Lane(this, 0),
            new Lane(this, 2),
        };
    }


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
    public int pixel_offset = 0;
    public Rail rail = null;
    public Wire wire = null;

    public Lane(Rail rail, int px_offset=0)
    {
        this.rail = rail;
        this.pixel_offset= px_offset;
    }

    public Vector2 GetJonction(Weld weld)
    {
        // we need to find the meeting point between the weld and the lane

        float xpos, ypos;
        if (!rail.Horizontal) // the x of the lane, and the y of the weld
        {
            xpos = rail.transform.position.x + pixel_offset * (1f / 24f);
            ypos = weld.world_position.y;
            return new Vector2(xpos,ypos);
        }

        // horizontal --> // the y of the lane, the x of the weld
        xpos = weld.world_position.x;
        ypos = rail.transform.position.y + pixel_offset * (1f / 24f);
        return new Vector2(xpos, ypos);
    }

}

public enum Axis
{
    Horizontal,
    Vertical
}