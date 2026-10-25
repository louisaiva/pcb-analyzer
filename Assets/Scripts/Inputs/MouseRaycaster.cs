using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseRaycaster : MonoBehaviour
{

    [SerializeField] private InputActionReference mouse_pos;
    [SerializeField] private InputActionReference mouse_click;
    [SerializeField] private bool log_mouse_pos = false;
    [SerializeField] private bool log_hits = false;



    private ContactFilter2D component_filter = new ContactFilter2D();

    private void Start()
    {
        component_filter.useTriggers = true;
        component_filter.SetLayerMask(LayerMask.GetMask("Components", "Buttons", "Rocks", "Bugs"));
        // Debug.Log(component_filter);
    }

    private void OnEnable()
    {
        mouse_click.action.performed += OnMouseClick;
    }


    private void OnDisable()
    {
        mouse_click.action.performed -= OnMouseClick;
    }

    private void Update()
    {
        // get the pos of the mouse
        Vector3 position_of_mouse_in_screen = mouse_pos.action.ReadValue<Vector2>();
        position_of_mouse_in_screen.z = -Camera.main.transform.position.z;
        if (log_mouse_pos) { Debug.Log($"Position of mouse is : {position_of_mouse_in_screen.x} / {position_of_mouse_in_screen.y}"); }
        // this.transform.position = new Vector3(position_of_mouse.x, position_of_mouse.y, -1f);
        Vector3 position_of_mouse_in_world = Camera.main.ScreenToWorldPoint(position_of_mouse_in_screen);

        // send raycast
        RaycastHit2D[] hits = new RaycastHit2D[20];
        Physics2D.Raycast(position_of_mouse_in_world, Camera.main.transform.forward, component_filter, hits, 10f);

        // check collisions with layers
        for (int i=0;i<hits.Length;i++)
        {
            if (hits[i].collider == null) { continue; }
            if (log_hits) { Debug.Log("Hitted " + hits[i].collider.name); }
            if (try_to_hit_hoverable(hits[i].collider)) { return; }
        }

        BoardController.Instance.OnNothingHovered();
    }

    private bool try_to_hit_hoverable(Collider2D collider)
    {
        if (collider.transform.parent == null) { return false; }
        if (!collider.transform.parent.TryGetComponent(out Hoverable hover)) { return false; }

        // send event to the BoardManager
        BoardController.Instance.OnHover(hover);
        return true;
    }


    private void OnMouseClick(InputAction.CallbackContext context)
    {
        // Debug.Log($"(Mouse) Getting click : {context.ReadValue<float>()}");
        if (context.ReadValue<float>() > .5f) { BoardController.Instance.OnMousePress(); }
        else { BoardController.Instance.OnMouseRelease(); }
    }
}