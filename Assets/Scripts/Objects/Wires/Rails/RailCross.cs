using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RailCross : RailNode
{
    public async void Init(Vector2Int position)
    {
        RailPosition = position;
    }


    private List<Vector2Int> offsets = new List<Vector2Int>()
    {
        (Vector2Int.up + Vector2Int.right) * 2,
        Vector2Int.right * 2,
        (Vector2Int.down + Vector2Int.right) * 2,
        Vector2Int.down * 2,
        (Vector2Int.down + Vector2Int.left) * 2,
        Vector2Int.left * 2,
        (Vector2Int.up + Vector2Int.left) * 2,
        Vector2Int.up * 2,
    };
    private List<Vector2> Get8Positions()
    {
        List<Vector2> positions = new List<Vector2>();
        foreach (Vector2Int offset in offsets)
        {
            positions.Add(((Vector2)transform.position) + offset * new Vector2(1f/24f,1f/24f));
        }
        return positions;
    }

    public Vector2 GetClosestCrossPosition(Lane lane)
    {
        Vector2 lane_center = lane.GetCentralLanePosition();
        List<Vector2> positions = Get8Positions();

        float min_distance = float.MaxValue;
        Vector2 best_pos = new Vector2(-888f,-888f);

        foreach (Vector2 pos in positions)
        {
            float distance = Vector2.Distance(lane_center, pos);
            if (distance > min_distance) { continue; }
            best_pos = pos;
            min_distance = distance;
        }
        if (best_pos == new Vector2(-888f, -888f))
        {
            Debug.Log($"(RailCross) Cross located in [{RailPosition}] could not get the jonction with lane [{lane.rail.RailPosition}]. Our 8 positions are {string.Join("   /   ", positions)}");
        }
        return best_pos;
    }
}