using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public struct UISlot
{
    public GameObject Object;
    public RectTransform Transform;
    public Image Image;
    public Button Btn;
    public bool Filled;
    public StorableIngredient Ing;
    public UISlotType UIType;
}

public enum UISlotType
{
    PlayerInv,
    Cauldron
}

public enum PanelType
{
    Player,
    IngStorageTable,
    CraftingStation
}

public class UIHandler : MonoBehaviour
{
    public static UIHandler Instance { get; private set; }

    [Header("General Panel Variables")]
    public GameObject MainPanel;
    public PanelCollection[] UIPanelCollection;
    public Vector2 InvSpriteSize;
    public int xPadding;
    public int yPadding;
    ActivePanel CurrentPanel;
    bool InMenu;

    private struct ActivePanel
    {
        public GameObject Obj;
        public PanelType Type;
    }

    [System.Serializable]
    public struct PanelCollection
    {
        public GameObject[] Panels;
        [HideInInspector]
        public int CurrentIndex;
        public int MaxPanels;
    }

    [Header("Player Inventory Panel Variables")]
    public Sprite[] InvImgSprites;
    public int InvUIRows;
    public int InvUIColumns;
    UISlot[] InvUISlots;

    [Header("Ing Storage Table Panel Variables")]
    public TextMeshProUGUI[] StorageCounters;

    [Header("Crafting Station Panel Variables")]
    public int CauldronInventorySlots;
    public int CauldronUIRows;
    public int CauldronUIColumns;
    UISlot[] CauldronUISlots;

    private static Dictionary<UISlotType, UISlot[]> UISlots;

    private void Awake()
    {
        Instance = this;

        UISlots = new Dictionary<UISlotType, UISlot[]>()
        {
            { UISlotType.PlayerInv, CreateUISlots(UIPanelCollection[(int)PanelType.Player].Panels[0].transform,
            Player.InventorySize, InvUIRows, InvUIColumns) },
            { UISlotType.Cauldron, CreateUISlots(UIPanelCollection[(int)PanelType.CraftingStation].Panels[0].transform,
            CauldronInventorySlots, CauldronUIRows, CauldronUIColumns) }
        };
    }

    private UISlot[] CreateUISlots(Transform parentPanel, int numberOfSlots, int rows, int columns)
    {
        UISlot[] newSlots = new UISlot[numberOfSlots];

        Sprite emptyInv = Resources.Load<Sprite>("InvImg/Empty");

        int xPositionOffset = 0;
        int yPositionOffset = 0;

        int xSpritePadding = (int)InvSpriteSize.x + xPadding;
        int ySpritePadding = (int)InvSpriteSize.y + yPadding;

        //Display is 0 cenetered, so we divide by 2
        int xStartingPos = ((columns * xSpritePadding) - xSpritePadding) / -2;

        int yStartingPos = ((rows * ySpritePadding) - ySpritePadding) / -2;

        int xLimit = columns * xSpritePadding;
        int yLimit = rows * ySpritePadding;

        for (int i = 0; i < newSlots.Length; i++)
        {
            newSlots[i].Object = new GameObject($"UI Inv Slot #{i}");

            newSlots[i].Transform = newSlots[i].Object.AddComponent<RectTransform>();
            newSlots[i].Image = newSlots[i].Object.AddComponent<Image>();
            newSlots[i].Image.sprite = emptyInv;

            newSlots[i].Btn = newSlots[i].Object.AddComponent<Button>();
            newSlots[i].Btn.transition = Selectable.Transition.None;
            newSlots[i].Btn.interactable = false;

            newSlots[i].Transform.parent = parentPanel;

            newSlots[i].Transform.position = parentPanel.position;

            newSlots[i].Transform.sizeDelta = new Vector2((int)InvSpriteSize.x, (int)InvSpriteSize.y);

            newSlots[i].Transform.localScale = Vector3.one;

            newSlots[i].Transform.localPosition = new Vector2(xStartingPos + xPositionOffset, yStartingPos + yPositionOffset);
            xPositionOffset = (xPositionOffset + xSpritePadding) % xLimit;

            if (xPositionOffset == 0)
            {
                yPositionOffset = (yPositionOffset + ySpritePadding) % yLimit;
            }
        }

        return newSlots;
    }

    private void OnEnable()
    {
        InputHandler.OnMenuInput += OnMenuInput;
        FPInteract.OnIngStorageTableInteract += HandleIngStorageTableInteract;
        FPInteract.OnCraftingStationInteract += HandleCraftingStationInteract;
    }

    private void OnMenuInput()
    {
        CurrentPanel.Type = PanelType.Player;

        ToggleUIPanel();
    }

    private void ToggleUIPanel()
    {
        if (InMenu)
        {
            MainPanel.SetActive(false);
            CurrentPanel.Obj.SetActive(false);

            InMenu = false;
            InputHandler.ExitUIMode();
        }
        else
        {
            CurrentPanel.Obj = UIPanelCollection[(int)CurrentPanel.Type].Panels[0];
            UIPanelCollection[(int)CurrentPanel.Type].CurrentIndex = 0;

            CurrentPanel.Obj.SetActive(true);
            MainPanel.SetActive(true);

            InMenu = true;
            InputHandler.EnterUIMode();
        }
    }

    private void HandleIngStorageTableInteract()
    {
        //Default to the first System UI Panel
        CurrentPanel.Type = PanelType.IngStorageTable;

        ToggleUIPanel();
    }

    private void HandleCraftingStationInteract()
    {
        CurrentPanel.Type = PanelType.CraftingStation;

        ToggleUIPanel();
    }

    public void RightUIPanelClick()
    {
        CurrentPanel.Obj.SetActive(false);

        UIPanelCollection[(int)CurrentPanel.Type].CurrentIndex = ++UIPanelCollection[(int)CurrentPanel.Type].CurrentIndex 
            % UIPanelCollection[(int)CurrentPanel.Type].MaxPanels;

        CurrentPanel.Obj = UIPanelCollection[(int)CurrentPanel.Type].Panels[UIPanelCollection[(int)CurrentPanel.Type].CurrentIndex];

        CurrentPanel.Obj.SetActive(true);
    }

    public void LeftUIPanelClick()
    {
        CurrentPanel.Obj.SetActive(false);

        if (UIPanelCollection[(int)CurrentPanel.Type].CurrentIndex - 1 < 0)
        {
            UIPanelCollection[(int)CurrentPanel.Type].CurrentIndex = UIPanelCollection[(int)CurrentPanel.Type].MaxPanels;
        }

        CurrentPanel.Obj = UIPanelCollection[(int)CurrentPanel.Type].Panels[--UIPanelCollection[(int)CurrentPanel.Type].CurrentIndex];

        CurrentPanel.Obj.SetActive(true);
    }

    public void FillUISlot(StorableIngredient ing, UISlotType slotType)
    {
        UISlot[] slots = UISlots[slotType];

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].Filled == false)
            {
                slots[i].Filled = true;
                slots[i].Ing = ing;
                slots[i].Image.sprite = InvImgSprites[(int)ing.Type];
                slots[i].Btn.onClick.AddListener(delegate { PlayerUISlotClick(slots[i]); });
                slots[i].Btn.interactable = true;
                slots[i].UIType = slotType;
                break;
            }
        }
    }

    public void EmptyUISlot(StorableIngredient ing, UISlotType slotType)
    {
        UISlot[] slots = UISlots[slotType];

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].Ing == ing)
            {
                slots[i].Ing = null;
                slots[i].Filled = false;
                slots[i].Image.sprite = Resources.Load<Sprite>("InvImg/Empty");
                slots[i].Btn.onClick.RemoveAllListeners();
                slots[i].Btn.interactable = false;
                break;
            }
        }
    }

    public void IngTableStoreAllBtnPress()
    {
        Player.Inv.StoreIngredientsToTable();
    }

    private void PlayerUISlotClick(UISlot UISlot)
    {
        switch(UISlot.UIType)
        {
            case UISlotType.PlayerInv:
                WorldObjectManager.Instance.InstantiateHoldableIngredient(UISlot.Ing);
                Player.Inv.RemoveIngredient(UISlot.Ing, UISlotType.PlayerInv);
                ToggleUIPanel();
                break;

            case UISlotType.Cauldron:
                CraftingHandler.Instance.RemoveIngFromMix(UISlot.Ing);
                Player.Inv.AddIngredient(UISlot.Ing, UISlotType.PlayerInv);
                EmptyUISlot(UISlot.Ing, UISlot.UIType);
                break;
        }
        

        //1 is equal to removing 1 ingredient from the cauldron mix
        
    }

    public void UpdateIngStorageTableCounters()
    {
        for (int i = 0; i < StorageCounters.Length; i++)
        {
            StorageCounters[i].text = $"{(IngredientType)i} x{IngredientTableManager.StoredIngredients[i]}";
        }
    }

    private void OnDisable()
    {
        InputHandler.OnMenuInput -= OnMenuInput;
        FPInteract.OnIngStorageTableInteract -= HandleIngStorageTableInteract;
        FPInteract.OnCraftingStationInteract -= HandleCraftingStationInteract;
    }
}
