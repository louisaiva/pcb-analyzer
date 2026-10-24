using UnityEngine;

public class PCB_Caller : MonoBehaviour
{
    private SpriteRenderer _sr = null;
    private SpriteRenderer sr
    {
        get
        {
            if (_sr != null) { return _sr; }
            _sr = GetComponent<SpriteRenderer>();
            return _sr;
        }
    }


    public void ChangeLayerOrder(int order)
    {
        if (sr == null) { return; }
        sr.sortingOrder = order;
    }
}