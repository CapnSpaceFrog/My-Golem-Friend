using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct HoldableProperty
{
    public Vector3 HeldRotation;
    public Vector3 HeldPositionOffset;
    public float ThrowForce;
    public float DropForce;
}

public enum HoldableType
{
    Ingredient,
    GolemPart,
    QuestItem
}

public class Holdable : Interactable
{
    protected Rigidbody RB;

    [Header("Held Variables")]
    public HoldableProperty HoldableProperties;
    public HoldableType HoldableType;

    public override void Awake()
    {
        base.Awake();

        RB = GetComponent<Rigidbody>();

        RB.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void SetHeldTransform()
    {
        transform.transform.localEulerAngles = HoldableProperties.HeldRotation;
        transform.localPosition = Vector3.zero + HoldableProperties.HeldPositionOffset;
    }

    public void AddToHand()
    {
        Collider.enabled = false;

        RB.constraints = RigidbodyConstraints.FreezeAll;

        transform.parent = Player.Hand;

        SetHeldTransform();

        FPInteract.HeldObject = this;
    }

    public void DropFromHand()
    {
        Collider.enabled = true;

        FPInteract.HeldObject = null;

        transform.parent = null;

        RB.constraints = RigidbodyConstraints.None;

        RB.AddForce(Camera.main.transform.forward * HoldableProperties.DropForce, ForceMode.Impulse);
    }

    public void ThrowFromHand()
    {
        Collider.enabled = true;

        FPInteract.HeldObject = null;

        transform.parent = null;

        RB.constraints = RigidbodyConstraints.None;

        RB.AddForce(Camera.main.transform.forward * HoldableProperties.ThrowForce, ForceMode.Impulse);
    }
}
