using UnityEngine;

public class Player : MonoBehaviour
{
    public static Transform PlayerTransform;

    public static Rigidbody RB;

    public static CapsuleCollider Collider;

    public static Transform CharacterBody;

    public static Transform Hand;

    public static Inventory<Ingredients> PlayerInventory;

    public int InventorySize;

    public void Awake()
    {
        PlayerTransform = transform;

        RB = GetComponent<Rigidbody>();

        Collider = GetComponent<CapsuleCollider>();

        CharacterBody = transform.Find("Body").transform;

        Hand = CharacterBody.Find("Hand").transform;

        PlayerInventory = new Inventory<Ingredients>(InventorySize);
    }

    public void Update()
    {

    }

    public void FixedUpdate()
    {

    }
}