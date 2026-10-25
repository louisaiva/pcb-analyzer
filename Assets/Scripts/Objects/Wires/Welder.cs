using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Welder : MonoBehaviour
{
    [Range(0,10), SerializeField] private int wires_to_generate = 1;
    public Wire wire_prefab;
    public Transform wires_parent;
    public RailPathfinder Railer = null;

    [SerializeField] private List<Wire> wires;

    [SerializeField] private bool logs = false;
    [SerializeField] private bool log_lanes_assignement = false;


    public void ClearWires()
    {
        foreach (Wire wire in wires) { Destroy(wire.gameObject); }
        wires.Clear();
        Railer = null;
    }
    public void WireBoard(Board board, ComponentsManager componer, int max_iterations = 100)
    {
        ClearWires();

        Railer = new RailPathfinder(board.GetBoardRailNodes());

        for (int i=0;i< wires_to_generate;i++)
        {
            int iterations = 0;
            while (iterations < max_iterations)
            {
                if (Wire2RandomComponents(board, componer)) { break; }
                iterations++;
            }
            if (iterations == max_iterations) { break; }
        }
    }
    public bool Wire2RandomComponents(Board board, ComponentsManager componer)
    {

        // gather 2 components just to try it (can wire the same component maybe oops)
        Component a = componer.GetRandomComponent();
        Component b = componer.GetRandomComponent();

        return WireComponents(a,b,board);
    }
    public bool WireComponents(Component a, Component b, Board board)
    {
        if (logs) { Debug.Log($"(Welder) Wiring 2 components {a.name} && {b.name}"); }

        // Weld start = a.GetRandomFreeWeld();
        Weld start = a.GetClosestFreeWeldFrom(b);
        Weld end = b.GetClosestFreeWeldFrom(a);
        if (start == null || end == null) { return false; }

        if (logs) { Debug.Log($"(Welder) Chosen welds are :   [start] {start.world_position}   && [exit] {end.world_position}"); }


        Rail start_rail = board.GetWeldRail(start);
        if (!start_rail.HasFreeLane()) { return false; }
        Rail end_rail = board.GetWeldRail(end);
        if (!end_rail.HasFreeLane()) { return false; }

        // launch the circuitery
        List<RailNode> node_path = Railer.FindPath(start_rail, end_rail);
        if (node_path.Count == 0) { return false; }

        /* Lane start_lane = start_rail.GetFreeLane();
        Lane end_lane = end_rail.GetFreeLane();

        List<Vector2> path = new List<Vector2>
        {
            start_lane.GetJonction(start),
            end_lane.GetJonction(end)
        }; */
        // List<Vector2> path = node_path.ConvertAll(n => n.WorldPosition);

        Wire wire = create_wire(wire_prefab);
        start_rail.WireFreeLane(wire);
        List<Vector2> path = wire_lanes(wire, start.world_position, end.world_position, node_path);
        
        wire.BuildWire(start, path, end);
        wires.Add(wire);
        start.used = true;
        end.used = true;
        return true;
    }

    private List<Vector2> wire_lanes(Wire wire, Vector2 start, Vector2 end, List<RailNode> node_path)
    {
        List<Vector2> path = new List<Vector2>() { start };

        for (int i=0; i<node_path.Count-1;i+=2)
        {
            Rail a = (Rail)node_path[i];
            RailCross cross = (RailCross)node_path[i+1];
            Rail b = (Rail)node_path[i+2];

            if (log_lanes_assignement) { Debug.Log($"(Welder) Assigning lanes between : [{a.RailPosition}] ->> [{cross.RailPosition}] ->> [{b.RailPosition}]"); }

            // get the start lane
            Lane start_lane = a.GetWiredLane(wire);
            path.Add(start_lane.GetJonction(path.LastOrDefault()));

            // check next rail axis
            if (b.Horizontal != a.Horizontal)
            {
                b.WireFreeLane(wire); // we don't care about the lane it choses it will work anyway :)
                continue;
            }

            // we check if the same lane is free
            if (b.IsLaneFree(start_lane.pixel_offset))
            {
                b.WireLane(b.GetLaneByPX(start_lane.pixel_offset), wire);
                continue;
            }

            // else we have a problem oh no we need to make the cross change lanes
            if (log_lanes_assignement) { Debug.LogWarning($"(Welder) Changing lane is mandatory :///"); }
            path.Add(cross.GetClosestCrossPosition(start_lane));
            Lane next_lane = b.WireFreeLane(wire); // we get what we have, i mean yeah it is life
            path.Add(cross.GetClosestCrossPosition(next_lane));
        }

        Lane last_lane = ((Rail)node_path.LastOrDefault()).GetWiredLane(wire);
        path.Add(last_lane.GetJonction(path.LastOrDefault()));
        path.Add(last_lane.GetJonction(end));

        return path;
    }

    private Wire create_wire(Wire wire_prefab)
    {
        Wire wire = Instantiate(wire_prefab);
        wire.transform.SetParent(wires_parent);
        
        return wire;
    }



}






public class Weld
{
    public Component component;
    public Vector2Int RailPosition => component.RailPosition;
    public Vector2 world_position;
    public Vector2Int exit_direction;
    public bool used = false;
}
[Serializable] public class WeldData
{
    public Vector2 pixel;
    public Vector2Int exit;


    public string GetDetails()
    {
        string debug = $"    pixel : {pixel}           /////       exit : {exit}";
        return debug;
    }
}
