using System;
using System.Collections.Generic;
using UnityEngine;

public class Welder : MonoBehaviour
{
    public Wire wire_prefab;
    public Transform wires_parent;

    [SerializeField] private List<Wire> wires;

    [SerializeField] private bool logs = false;


    public void WireBoard(Board board, ComponentsManager componer)
    {
        ClearWires();

        // gather 2 components just to try it (can wire the same component maybe oops)
        Component a = componer.GetRandomComponent();
        Component b = componer.GetRandomComponent();

        WireComponents(a,b,board);
    }
    public bool WireComponents(Component a, Component b, Board board)
    {
        if (logs) { Debug.Log($"(Welder) Wiring 2 components {a.name} && {b.name}"); }

        Weld start = a.GetRandomFreeWeld();
        Weld exit = b.GetRandomFreeWeld();

        if (logs) { Debug.Log($"(Welder) Chosen welds are :   [start] {start.world_position}   && [exit] {exit.world_position}"); }


        Rail start_rail = board.GetWeldRail(start);
        if (!start_rail.HasFreeLane()) { return false; }
        Rail exit_rail = board.GetWeldRail(exit);
        if (!exit_rail.HasFreeLane()) { return false; }

        Lane start_lane = start_rail.GetFreeLane();
        Lane exit_lane = exit_rail.GetFreeLane();

        List<Vector2> path = new List<Vector2>
        {
            start_lane.GetJonction(start),
            exit_lane.GetJonction(exit)
        };

        wires.Add(create_wire(wire_prefab, start, path, exit));
        return true;
    }

    public void ClearWires()
    {
        foreach (Wire wire in wires) { Destroy(wire.gameObject); }
        wires.Clear();
    }


    private Wire create_wire(Wire wire_prefab, Weld start, List<Vector2> path, Weld exit)
    {
        Wire wire = Instantiate(wire_prefab);
        wire.transform.SetParent(wires_parent);
        wire.BuildWire(start, path, exit);
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
