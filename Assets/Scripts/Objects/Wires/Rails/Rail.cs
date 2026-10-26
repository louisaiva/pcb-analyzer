using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;

public class Rail : RailNode
{
    private List<Lane> lanes = new List<Lane>();
    public Axis axis = Axis.Horizontal;
    public bool Horizontal => axis == Axis.Horizontal;
    public bool BugWasCreatedOnIt = false;

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
    public bool disabled = false;
    public override bool Full => !HasFreeLane() || disabled;
    public bool HasFreeLane()
    {
        for (int i = 0; i < 3; i++)
        {
            if (lanes[i].wire == null) { return true; }
        }
        return false;
    }
    public bool IsLaneFree(int px)
    {
        Lane lane = GetLaneByPX(px);
        if (lane == null) { return false; }
        return lane.wire == null;
    }
    public Lane GetLaneByPX(int px)
    {
        foreach (Lane lane in lanes)
        {
            if (lane.pixel_offset == px) { return lane; }
        }
        return null;
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
        return WireLane(lane, wire);
    }
    public Lane WireLane(Lane lane, Wire wire)
    {
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

    public Vector2 GetJonction(Weld weld) { return GetJonction(weld.world_position); }
    public Vector2 GetJonction(Vector2 world_position)
    {
        // we need to find the meeting point between the weld and the lane

        float xpos, ypos;
        if (!rail.Horizontal) // the x of the lane, and the y of the weld
        {
            xpos = rail.transform.position.x + pixel_offset * (1f / 24f);
            ypos = world_position.y;
            return new Vector2(xpos,ypos);
        }

        // horizontal --> // the y of the lane, the x of the weld
        xpos = world_position.x;
        ypos = rail.transform.position.y + pixel_offset * (1f / 24f);
        return new Vector2(xpos, ypos);
    }
    public Vector2 GetCentralLanePosition()
    {
        float xpos, ypos;
        if (!rail.Horizontal) // the x of the lane, and y of the rail
        {
            xpos = rail.transform.position.x + pixel_offset * (1f / 24f);
            ypos = rail.transform.position.y;
            return new Vector2(xpos, ypos);
        }
        // horizontal --> // the y of the lane, the x of the rail
        xpos = rail.transform.position.x;
        ypos = rail.transform.position.y + pixel_offset * (1f / 24f);
        return new Vector2(xpos, ypos);
    }

}

public enum Axis
{
    Horizontal,
    Vertical
}