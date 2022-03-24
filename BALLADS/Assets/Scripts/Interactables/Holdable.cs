using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holdable : Interactable
{
    private Rigidbody RB;

    public bool Exhausted;

    [Header("Held Variables")]
    public Vector3 HeldScale;
    public Vector3 HeldRotation;
    public Vector3 HeldPosition;
    public float THROW_FORCE;

    GameObject InteractIcon;

    public Vector3 IconThreshold;
    Vector3 directionToPlayer;

    public override void Awake()
    {
        base.Awake();

        CreateIcon();

        RB = GetComponent<Rigidbody>();

        RB.constraints = RigidbodyConstraints.FreezeAll;
    }

    void CreateIcon()
    {
        InteractIcon = Instantiate(Resources.Load<GameObject>("UI/Interactable Icon"));
        InteractIcon.transform.parent = transform;
        InteractIcon.transform.position = transform.position;
        InteractIcon.transform.localPosition = new Vector3(0, GetComponent<Collider>().bounds.size.y + 0.15f, 0);
        InteractIcon.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load<Material>("UI/Icon Material");
    }

    void Update()
    {
        if (!Exhausted)
        {
            UpdatetIconRotation();
        }
    }

    public void PickedUp()
    {
        Collider.enabled = false;

        RB.constraints = RigidbodyConstraints.FreezeAll;

        InteractIcon.SetActive(false);

        transform.parent = Player.Hand;

        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = HeldRotation;

        //Scale down the transform of this obj
        transform.localScale = HeldScale;

        FPInteract.HeldObject = this;
    }

    public void Drop()
    {
        Collider.enabled = true;

        FPInteract.HeldObject = null;

        transform.parent = null;

        RB.constraints = RigidbodyConstraints.None;

    }

    public void Throw()
    {
        Collider.enabled = true;

        FPInteract.HeldObject = null;

        transform.parent = null;

        RB.constraints = RigidbodyConstraints.None;

        RB.AddForce(Camera.main.transform.forward * THROW_FORCE, ForceMode.Impulse);
    }

    private void UpdatetIconRotation()
    {
        directionToPlayer = Camera.main.transform.position - transform.position;

        Quaternion rotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);
        InteractIcon.transform.rotation = rotation;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, directionToPlayer);
    }
}
