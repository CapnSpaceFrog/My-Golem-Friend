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

        for (int i = 0; i < InvUISlots.Length; i++)
        {
            InvUISlots[i].Object = new GameObject($"Inv Slot #{i}");

            InvUISlots[i].Transform = InvUISlots[i].Object.AddComponent<RectTransform>();
            InvUISlots[i].Image = InvUISlots[i].Object.AddComponent<Image>();
            InvUISlots[i].Image.sprite = null;

            InvUISlots[i].Transform.parent = InvPanelRef.transform;

            InvUISlots[i].Transform.position = InvPanelRef.transform.position;

            InvUISlots[i].Transform.sizeDelta = new Vector2(160, 160);

            InvUISlots[i].Transform.localScale = Vector3.one;

            InvUISlots[i].Transform.localPosition = new Vector2(-384 + xPositionOffset, -288 + yPositionOffset);
            xPositionOffset = (xPositionOffset + 192) % 960;

            if (xPositionOffset == 0)
            {
                yPositionOffset = (yPositionOffset + 192) % 768;
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
