using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseRaycaster : MonoBehaviour
{

    [SerializeField] private InputActionReference mouse_pos;
    [SerializeField] private bool log_mouse_pos = false;
    [SerializeField] private bool log_hits = false;


    [SerializeField] private InputActionReference mouse_click;


    private ContactFilter2D component_filter = new ContactFilter2D();

    private void Start()
    {
        component_filter.useTriggers = true;
        component_filter.SetLayerMask(LayerMask.GetMask("Components", "Buttons"));
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
            if (try_to_hit_component(hits[i].collider)) { return; }
            if (try_to_hit_button(hits[i].collider)) { return; }
        }

        BoardController.Instance.OnNothingHovered();
    }

    private bool try_to_hit_component(Collider2D collider)
    {
        if (collider.transform.parent == null) { return false; }
        if (!collider.transform.parent.TryGetComponent(out Component hitted_component)) { return false; }

        // send event to the BoardManager
        BoardController.Instance.OnComponentHover(hitted_component);
        return true;
    }

    private bool try_to_hit_button(Collider2D collider)
    {
        if (collider.transform.parent == null) { return false; }
        if (!collider.transform.parent.TryGetComponent(out Button hitted_button)) { return false; }

        // send event to the BoardManager
        BoardController.Instance.OnButtonHover(hitted_button);
        return true;
    }



    private void OnMouseClick(InputAction.CallbackContext context)
    {
        BoardController.Instance.OnMouseClick();
    }
}