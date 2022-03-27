using UnityEngine;

public class Player : MonoBehaviour
{
    public static Transform PlayerTransform;

    public static Rigidbody RB;

    public static CapsuleCollider Collider;

    public static Transform CharacterBody;

    public static Transform Hand;

    public static Inventory PlayerInventory;

    //12 in this case represents the number of ingredients we have in our game
    private int InventorySize = 12;

    public void Awake()
    {
        PlayerTransform = transform;

        RB = GetComponent<Rigidbody>();

        Collider = GetComponent<CapsuleCollider>();

        CharacterBody = transform.Find("Body").transform;

        Hand = CharacterBody.Find("Hand").transform;

        PlayerInventory = new Inventory(InventorySize);
    }

    public void Update()
    {

    }

    public void FixedUpdate()
    {

    }
}