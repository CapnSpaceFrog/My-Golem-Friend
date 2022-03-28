using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public struct InvUISlot
{
    public GameObject Object;
    public RectTransform Transform;
    public Image Image;
    public bool Filled;
}

public class UIHandler : MonoBehaviour
{
    public static UIHandler Instance { get; private set; }

    //Organize these god forsaken fucking variables later
    enum Panels
    {
        Inventory,
        Map,
        Recipe,
        Max_Panels
    }

    public Transform MainPanelTransform;

    public GameObject[] PlayerUIPanels;

    struct CurrentPanel
    {
        public GameObject Obj;
        public Panels Type;
    }

    CurrentPanel ActivePanel;

    InvUISlot[] InvUISlots;

    public Sprite[] InvImgSprites;

    [Header("Inv Display Variables")]
    public int Rows;
    public int Colums;
    public Vector2 InvSpriteSize;
    public int xPadding;
    public int yPadding;

    private void Awake()
    {
        Instance = this;

        ActivePanel.Obj = PlayerUIPanels[(int)Panels.Inventory];
        ActivePanel.Type = Panels.Inventory;

        CreateInventoryUISlots();
    }

    private void CreateInventoryUISlots()
    {
        InvUISlots = new InvUISlot[Player.InventorySize];

        GameObject InvPanelRef = PlayerUIPanels[(int)Panels.Inventory];

        int xPositionOffset = 0;
        int yPositionOffset = 0;

        int xSpritePadding = (int)InvSpriteSize.x + xPadding;
        int ySpritePadding = (int)InvSpriteSize.y + yPadding;

        //Display is 0 cenetered, so we divide by 2
        int xStartingPos = ((Colums * xSpritePadding) - xSpritePadding) / -2;

        int yStartingPos = ((Rows * ySpritePadding) - ySpritePadding) / -2;

        int xLimit = Colums * xSpritePadding;
        int yLimit = Rows * ySpritePadding;

        for (int i = 0; i < InvUISlots.Length; i++)
        {
            InvUISlots[i].Object = new GameObject($"Inv Slot #{i}");

            InvUISlots[i].Transform = InvUISlots[i].Object.AddComponent<RectTransform>();
            InvUISlots[i].Image = InvUISlots[i].Object.AddComponent<Image>();
            InvUISlots[i].Image.sprite = null;

            InvUISlots[i].Transform.parent = InvPanelRef.transform;

            InvUISlots[i].Transform.position = InvPanelRef.transform.position;

            InvUISlots[i].Transform.sizeDelta = new Vector2((int)InvSpriteSize.x, (int)InvSpriteSize.y);

            InvUISlots[i].Transform.localScale = Vector3.one;

            InvUISlots[i].Transform.localPosition = new Vector2(xStartingPos + xPositionOffset, yStartingPos + yPositionOffset);
            xPositionOffset = (xPositionOffset + xSpritePadding) % xLimit;

            if (xPositionOffset == 0)
            {
                yPositionOffset = (yPositionOffset + ySpritePadding) % yLimit;
            }
        }
    }

    public InvUISlot GetOpenInvUISlot()
    {
        InvUISlot InvSlot = new InvUISlot();

        for (int i = 0; i < InvUISlots.Length; i++)
        {
            if (InvUISlots[i].Filled == false)
            {
                InvSlot = InvUISlots[i];
                InvUISlots[i].Filled = true;
                break;
            }
        }

        return InvSlot;
    }

    public Sprite GetInvSprite(IngredientType IngType)
    {
        Sprite InvSprite = InvImgSprites[(int)IngType];

        return InvSprite;
    }

    public void RightUIShift()
    {
        ActivePanel.Obj.SetActive(false);

        ActivePanel.Type = (Panels) ( ( (int)ActivePanel.Type + 1) %  ( (int)Panels.Max_Panels) );
        ActivePanel.Obj = PlayerUIPanels[(int)ActivePanel.Type];

        ActivePanel.Obj.SetActive(true);
    }

    public void LeftUIClick()
    {
        ActivePanel.Obj.SetActive(false);

        //Can't use modulus for below zero, so check we're
        //Not about to go negative
        if ( ((int)ActivePanel.Type - 1) < 0 )
        {
            //Artifically overflow the active panel type
            ActivePanel.Type = Panels.Recipe;
        }
        else
        {
            ActivePanel.Type = (Panels) ((int)ActivePanel.Type - 1);
        }

        ActivePanel.Obj = PlayerUIPanels[(int)ActivePanel.Type];

        ActivePanel.Obj.SetActive(true);
    }

}
