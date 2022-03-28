using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HoldableProperty
{
    public Vector3 HeldScale;
    public Vector3 HeldRotation;
    public Vector3 HeldPositionOffset;
    public float THROW_FORCE;
}

public class Holdable : Interactable
{
    private static float DROP_FORCE = 1.25f;

    private Rigidbody RB;

    [Header("Held Variables")]
    public HoldableProperty HoldableProperties;

    public override void Awake()
    {
        base.Awake();

        RB = GetComponent<Rigidbody>();

        RB.constraints = RigidbodyConstraints.FreezeAll;
    }

    //for debugging the held scale, rotation, and position of the objects
    void Update()
    {
        transform.localScale = HoldableProperties.HeldScale;
        transform.transform.localEulerAngles = HoldableProperties.HeldRotation;
        transform.localPosition = HoldableProperties.HeldPositionOffset;
    }

    public void PickedUp()
    {
        Collider.enabled = false;

        RB.constraints = RigidbodyConstraints.FreezeAll;

        transform.parent = Player.Hand;

        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = HoldableProperties.HeldRotation;

        //Scale down the transform of this obj
        transform.localScale = HoldableProperties.HeldScale;

        FPInteract.HeldObject = this;
    }

    public void Drop()
    {
        Collider.enabled = true;

        FPInteract.HeldObject = null;

        transform.parent = null;

        RB.constraints = RigidbodyConstraints.None;

        RB.AddForce(Camera.main.transform.forward * DROP_FORCE, ForceMode.Impulse);
    }

    public void Throw()
    {
        Collider.enabled = true;

        FPInteract.HeldObject = null;

        transform.parent = null;

        RB.constraints = RigidbodyConstraints.None;

        RB.AddForce(Camera.main.transform.forward * HoldableProperties.THROW_FORCE, ForceMode.Impulse);
    }
}
