using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public enum UISlotType
{
    PlayerInv,
    Cauldron
}

public enum MainPanelType
{
    Player,
    IngStorageTable,
    CraftingStation
}

public enum SubPanelType
{
    Recipes,
    Ingredients,
    None
}

public class UIHandler : MonoBehaviour
{
    public static UIHandler Instance { get; private set; }

    [System.Serializable]
    public struct TaggableIcons
    {
        public Image Image;
        public string Tag;
    }

    private struct ActivePanel
    {
        public GameObject Obj;
        public MainPanelType Type;
    }

    [System.Serializable]
    public struct MainPanelCollection
    {
        public GameObject[] MainPanels;
        [HideInInspector]
        public int CurrentIndex;
        public int MaxMainPanels;
        public SubPanelCollection SubPanels;
    }

    //TODO: make this usable with multiple sub panels on the same main panel
    [System.Serializable]
    public class SubPanelCollection
    {
        public GameObject[] Obj;
        public SubPanelType SubType;
        public int CurrentIndex;
        public int MaxSubPanels;
    }
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

    [Header("General Panel Variables")]
    public GameObject CrossHair;
    public GameObject MainPanel;
    public MainPanelCollection[] UIMainPanels;
    public Vector2 InvSpriteSize;
    public int xPadding;
    public int yPadding;
    ActivePanel CurrentMainPanel;
    bool InMenu;

    [Header("Player Inventory Panel Variables")]
    public Sprite[] InvImgSprites;
    public int InvUIRows;
    public int InvUIColumns;

    [Header("Ing Storage Table Panel Variables")]
    public TextMeshProUGUI[] StorageCounters;

    [Header("Cauldron Inventory Panel Variables")]
    public int CauldronInventorySlots;
    public int CauldronUIRows;
    public int CauldronUIColumns;

    [Header("Crafting Recipe Panel Variables")]
    public TaggableIcons[] RecipeIcons;

    [Header("Golem Progress Panel Variables")]
    public Image[] GolemProgressImages;

    private static Dictionary<UISlotType, UISlot[]> UISlots;

    private void Awake()
    {
        Instance = this;

        UISlots = new Dictionary<UISlotType, UISlot[]>()
        {
            { UISlotType.PlayerInv, CreateUISlots(UIMainPanels[(int)MainPanelType.Player].MainPanels[0].transform,
            Player.InventorySize, InvUIRows, InvUIColumns) },
            { UISlotType.Cauldron, CreateUISlots(UIMainPanels[(int)MainPanelType.CraftingStation].MainPanels[0].transform,
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

            newSlots[i].Transform.SetParent(parentPanel, false);

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
        CurrentMainPanel.Type = MainPanelType.Player;

        ToggleUIPanel();
    }

    private void ToggleUIPanel()
    {
        if (InMenu)
        {
            MainPanel.SetActive(false);
            CurrentMainPanel.Obj.SetActive(false);

            InMenu = false;
            InputHandler.ExitUIMode();
            CrossHair.SetActive(true);
        }
        else
        {
            CurrentMainPanel.Obj = UIMainPanels[(int)CurrentMainPanel.Type].MainPanels[0];
            UIMainPanels[(int)CurrentMainPanel.Type].CurrentIndex = 0;

            CurrentMainPanel.Obj.SetActive(true);
            MainPanel.SetActive(true);

            InMenu = true;
            InputHandler.EnterUIMode();
            CrossHair.SetActive(false);
        }
    }

    private void HandleIngStorageTableInteract()
    {
        //Default to the first System UI Panel
        CurrentMainPanel.Type = MainPanelType.IngStorageTable;

        ToggleUIPanel();
    }

    private void HandleCraftingStationInteract()
    {
        CurrentMainPanel.Type = MainPanelType.CraftingStation;

        ToggleUIPanel();
    }

    public void RightUIPanelClick()
    {
        CurrentMainPanel.Obj.SetActive(false);

        UIMainPanels[(int)CurrentMainPanel.Type].CurrentIndex = ++UIMainPanels[(int)CurrentMainPanel.Type].CurrentIndex 
            % UIMainPanels[(int)CurrentMainPanel.Type].MaxMainPanels;

        CurrentMainPanel.Obj = UIMainPanels[(int)CurrentMainPanel.Type].MainPanels[UIMainPanels[(int)CurrentMainPanel.Type].CurrentIndex];

        CurrentMainPanel.Obj.SetActive(true);
    }

    public void LeftUIPanelClick()
    {
        CurrentMainPanel.Obj.SetActive(false);

        if (UIMainPanels[(int)CurrentMainPanel.Type].CurrentIndex - 1 < 0)
        {
            UIMainPanels[(int)CurrentMainPanel.Type].CurrentIndex = UIMainPanels[(int)CurrentMainPanel.Type].MaxMainPanels;
        }

        CurrentMainPanel.Obj = UIMainPanels[(int)CurrentMainPanel.Type].MainPanels[--UIMainPanels[(int)CurrentMainPanel.Type].CurrentIndex];

        CurrentMainPanel.Obj.SetActive(true);
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

    public void SubMenuRightClick()
    {
        SubPanelCollection CurrentSubPanels = UIMainPanels[(int)CurrentMainPanel.Type].SubPanels;

        CurrentSubPanels.Obj[CurrentSubPanels.CurrentIndex].SetActive(false);

        CurrentSubPanels.CurrentIndex = ++CurrentSubPanels.CurrentIndex % CurrentSubPanels.MaxSubPanels;

        CurrentSubPanels.Obj[CurrentSubPanels.CurrentIndex].SetActive(true);
    }

    public void SubMenuLeftClick()
    {
        SubPanelCollection CurrentSubPanels = UIMainPanels[(int)CurrentMainPanel.Type].SubPanels;

        CurrentSubPanels.Obj[CurrentSubPanels.CurrentIndex].SetActive(false);

        if (CurrentSubPanels.CurrentIndex - 1 < 0)
        {
            CurrentSubPanels.CurrentIndex = CurrentSubPanels.MaxSubPanels;
        }

        CurrentSubPanels.CurrentIndex = --CurrentSubPanels.CurrentIndex;

        CurrentSubPanels.Obj[CurrentSubPanels.CurrentIndex].SetActive(true);
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
    }

    public void UpdateStorageTableIngCounters()
    {
        for (int i = 0; i < StorageCounters.Length; i++)
        {
            StorageCounters[i].text = $"x{IngredientTableManager.StoredIngredients[i]}";
        }
    }

    public void UpdateUnlockedRecipeUI(string recipeToUnlock)
    {
        
        foreach(TaggableIcons icon in RecipeIcons)
        {
            if (icon.Tag == recipeToUnlock)
            {
                //The tag of the icon must match the proper sprite in the resources file else this will fail
                icon.Image.sprite = Resources.Load<Sprite>($"UIPanels/Recipes/{icon.Tag}");
                return;
            }
        }
    }

    public void UpdateGolemProgressUI(GolemItemType type)
    {
        //switch (type)
        //{
        //    case GolemItemType.Core:
        //        GolemProgressImages[0].sprite = Resources.Load<Sprite>($"UIPanels/GolemProgress/{type}");
        //        break;

        //    case GolemItemType.Potion:
        //        GolemProgressImages[1].sprite = Resources.Load<Sprite>($"UIPanels/GolemProgress/{type}");
        //        break;

        //    case GolemItemType.Scroll:
        //        GolemProgressImages[2].sprite = Resources.Load<Sprite>($"UIPanels/GolemProgress/{type}");
        //        break;

        //    case GolemItemType.Necklace:
        //        GolemProgressImages[3].sprite = Resources.Load<Sprite>($"UIPanels/GolemProgress/{type}");
        //        break;

        //    case GolemItemType.Essence:
        //        GolemProgressImages[0].sprite = Resources.Load<Sprite>($"UIPanels/GolemProgress/{type}");
        //        break;
        //}
    }

    public void IngTableStoreAllBtnPress()
    {
        Player.Inv.StoreIngredientsToTable(UISlotType.PlayerInv);
        WorldObjectManager.PurgeFlaggedObjects();
    }

    public void MoveIngFromCauldronToTableBtnPress()
    {
        CraftingHandler.Instance.EmptyCauldronToTable();
    }

    public void MoveIngFromCauldronToPlayerInvBtnPress()
    {
        CraftingHandler.Instance.EmptyIngToPlayerInv();
    }

    private void OnDisable()
    {
        InputHandler.OnMenuInput -= OnMenuInput;
        FPInteract.OnIngStorageTableInteract -= HandleIngStorageTableInteract;
        FPInteract.OnCraftingStationInteract -= HandleCraftingStationInteract;
    }
}