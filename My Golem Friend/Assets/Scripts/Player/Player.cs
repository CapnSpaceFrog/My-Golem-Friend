using UnityEngine;

public class Player : MonoBehaviour
{
    public static Transform PlayerTransform;

    public static Rigidbody RB;

    public static CapsuleCollider Collider;

    public static Transform CharacterBody;

    public static Transform Hand;

    public static Inventory Inv;

    //20 slots of Player inventory available
    public readonly static int InventorySize = 15;

    public void Awake()
    {
        PlayerTransform = transform;

        RB = GetComponent<Rigidbody>();

        Collider = GetComponent<CapsuleCollider>();

        CharacterBody = transform.Find("Body").transform;

        Hand = CharacterBody.Find("Hand").transform;

        Inv = new Inventory(InventorySize);
    }

    public void Update()
    {

    }

    public void FixedUpdate()
    {

    }
}