using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BugBehaviour : MonoBehaviour
{
    public List<BugState> all_states = new List<BugState>();
    public BugState current_state = null;
    public Bug bug = null;
    public string current_state_name = "";
    public Component target = null;

    public void Init()
    {
        all_states = new List<BugState>()
        {
            new IdleState(),
            new MoveState(),
            new AttackState(),
        };

        SwitchState("move");
    }

    public void SwitchState(string next_state)
    {
        if (current_state != null)
        {
            current_state.ExitState(this);
        }

        current_state = get_state(next_state);
        if (current_state != null)
        {
            current_state.EnterState(this);
            current_state_name = current_state.name;
        }
    }
    public void UpdateBehaviour()
    {
        if (current_state == null) { return; }
        current_state.UpdateState(this);
    }
    public void ReEnterCurrentState()
    {
        if (current_state != null)
        {
            current_state.EnterState(this);
        }
    }
    public void Stop()
    {
        if (current_state != null) { current_state.ExitState(this); }
        current_state = null;
    }

    // TARGET
    public void SetTarget(Component comp)
    {
        target = comp;
    }
    public BugState get_state(string name)
    {
        foreach (BugState state in all_states)
        {
            if (state.name == name) { return state; }
        }
        return null;
    }
}

public class BugState
{
    public string name = "";
    public bool active = false;

    public BugState(string name)
    {
        this.name = name;
        active = false;
    }

    public virtual void EnterState(BugBehaviour bb)
    {
        active = true;
        // Debug.Log($"{bb.bug.name} is entering state {name}");
    }
    public virtual void UpdateState(BugBehaviour bb) {}
    public virtual void ExitState(BugBehaviour bb)
    {
        active = false;
    }
}

public class IdleState : BugState
{
    public IdleState() : base("idle") {}

    public override async void EnterState(BugBehaviour bb)
    {
        base.EnterState(bb);
        bb.bug.Animator.Play(bb.bug.Tier + "_idle", 1);

        // ? here we can change delay range based on tier
        int delay = UnityEngine.Random.Range(800, 3000);

        await Task.Delay(delay);
        if (!active) { return; }
        bb.SwitchState("move");
    }
}
public class MoveState : BugState
{
    public MoveState() : base("move") { }

    public override async void EnterState(BugBehaviour bb)
    {
        base.EnterState(bb);
        bb.bug.Animator.Play(bb.bug.Tier + "_move", 1);

        // find the closest alive component
        List<Component> potential_targets = BoardController.Instance.Comper.GetAliveComponents();
        if (potential_targets.Count == 0) { bb.SwitchState("idle"); return; }

        potential_targets.Sort((a, b) => (int)(Vector2.Distance(bb.transform.position, a.transform.position) - Vector2.Distance(bb.transform.position, b.transform.position)));

        if (bb.target != null && potential_targets.Count != 1 && UnityEngine.Random.Range(0, 3) != 1)
        {
            potential_targets.Remove(bb.target);
        }

        // we get a random from the 3 closest
        List<Component> selection = new List<Component>();
        selection.Add(potential_targets[0]);
        if (potential_targets.Count > 1) { selection.Add(potential_targets[1]); }
        if (potential_targets.Count > 2) { selection.Add(potential_targets[2]); }

        bb.SetTarget(selection[Random.Range(0, selection.Count)]);
    }
    public override void UpdateState(BugBehaviour bb)
    {
        bb.bug.Animator.Play(bb.bug.Tier + "_move", 1);

        // get target
        Component target = bb.target;
        if (target == null) { bb.SwitchState("idle"); return; }

        // get speed based on tier
        float speed = 1f;
        if (bb.bug.Tier == "fat") { speed = .33f; }

        // check distance from target
        Vector2 distance = target.transform.position - bb.bug.transform.position;
        if (distance.magnitude <= (10f / 24f)) { bb.SwitchState("attack"); return; }
        bb.bug.Renderer.flipX = distance.x < 0f;

        // move to target
        Vector2 movement = distance.normalized * speed * Time.deltaTime;
        bb.bug.transform.position += (Vector3)movement;
    }
}
public class AttackState : BugState
{
    private float attack_cooldown = 0f;
    public AttackState() : base("attack") { }

    public override async void EnterState(BugBehaviour bb)
    {
        base.EnterState(bb);
        if (bb.target == null) { bb.SwitchState("idle"); return; }

        attack_cooldown = 0f;
        bb.bug.Animator.Play(bb.bug.Tier + "_idle", 1);
    }
    public override void UpdateState(BugBehaviour bb)
    {
        if (bb.target == null || bb.target.IsDead) { bb.SwitchState("idle"); return; }
        if (attack_cooldown <= 0f)
        {
            // we attack !
            if (bb.bug.Tier == "bat")
            {
                bat_attack(bb);
                attack_cooldown = 3f;
            }
            else if (bb.bug.Tier == "fat")
            {
                fat_attack(bb);
                attack_cooldown = 4f;
            }
            return;
        }
        attack_cooldown -= Time.deltaTime;
    }
    public void bat_attack(BugBehaviour bb)
    {
        bb.bug.Animator.Play("bat_attack", 1);
        bb.target.Damage();
    }
    public async void fat_attack(BugBehaviour bb)
    {
        bb.bug.Animator.Play("fat_attack", 1);
        bb.target.Damage();

        await Task.Delay(500);

        bb.bug.Animator.Play("fat_attack", 1);
        bb.target.Damage();
    }
}