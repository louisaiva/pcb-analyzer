using System;
using System.Collections.Generic;
using UnityEngine;

public class ComponentBank : MonoBehaviour
{
    
    public List<ComponentData> bank = new List<ComponentData>();
    public Component prefab;
    public bool log = false;


    public Component CreateRandomComponent(Transform parent)
    {
        string template = get_random_template();
        // string template = "IC66";
        return CreateComponentFromTemplate(template,parent);
    }

    public Component CreateComponentFromTemplate(string template, Transform parent)
    {
        if (!TryGetTemplate(template, out ComponentData data))
        {
            Debug.LogWarning($"(CompBank) Creating component did not find any template for {template} !!!");
            return null;
        }
        Component comp = Instantiate(prefab);
        comp.FeedData(data);
        comp.transform.SetParent(parent);
        comp.transform.localScale = Vector3.one;
        if (log) { Debug.Log($"(CompBank) Creating component {data.name} of type {data.type_of_component}. Localscale is {comp.transform.localScale}"); }
        return comp;
    }

    public bool TryGetTemplate(string template, out ComponentData out_data)
    {
        out_data = null;
        foreach (ComponentData data in bank)
        {
            if (data.name == template)
            {
                out_data = data;
                return true;
            }
        }
        return false;
    }
    private string get_random_template()
    {
        if (bank == null || bank.Count == 0) { return null; }
        int index = UnityEngine.Random.Range(0,bank.Count);
        return bank[index].name;
    }


}



/// <summary>
/// This data is template based. Means each data can feed multiple instances
/// of the same component. This means that some data is INSTANCE-BASED and
/// WON't BE HERE (put it in Component.cs).
/// ex : the sickness parameter. one instance can be sick be the one next to it does not !
/// so it is an instance based variable and not template based. won't be here
/// </summary>
[Serializable] public class ComponentData
{
    public string name = ""; // IC88, MC-32-X, Rizor 77, Red5-o
    public string type_of_component = ""; // cpu, chip etc
    public Vector2Int size = new Vector2Int();
    public Vector2Int offset = new Vector2Int(); // when we need to move down/up/left/right the sr for any reason
    public Vector2Int bg_size = new Vector2Int();
    public int max_HP = 1;

    public List<WeldData> welds = new List<WeldData>();

    public string GetDetails()
    {
        string debug = "";
        debug += $"\n - name : {name}";
        debug += $"\n - type : {type_of_component}";
        debug += $"\n - max_HP : {max_HP}";
        debug += $"\n - size : {size}";
        debug += $"\n - bg_size : {bg_size}";

        debug += $"\n - Potential weld count : {welds.Count}";
        foreach (WeldData weld in welds)
        {
            debug += $"\n   - " + weld.GetDetails();
        }

        return debug;
    }
    // public Sprite sprite;
    // public List<ConnectionPoint> ???? something like this to store the wire points ?
}