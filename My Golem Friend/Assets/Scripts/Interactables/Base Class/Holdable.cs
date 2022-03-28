using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holdable : Interactable
{
    private Rigidbody RB;

    [Header("Held Variables")]
    public Vector3 HeldScale;
    public Vector3 HeldRotation;
    public Vector3 HeldPosition;
    public float THROW_FORCE;

    public override void Awake()
    {
        base.Awake();

        RB = GetComponent<Rigidbody>();

        RB.constraints = RigidbodyConstraints.FreezeAll;
    }

    //for debugging the held scale, rotation, and position of the objects
    void Update()
    {
        transform.localScale = HeldScale;
        transform.transform.localEulerAngles = HeldRotation;
        transform.localPosition = HeldPosition;
    }

    public void PickedUp()
    {
        Collider.enabled = false;

        RB.constraints = RigidbodyConstraints.FreezeAll;

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
}
