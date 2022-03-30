using TMPro;
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
    bool InSystemPanel;
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

    private void Awake()
    {
        Instance = this;

        //When we call this function will need to change when we start swapping scenes
        InvUISlots = CreateUISlots(UIPanelCollection[(int)PanelType.Player].Panels[0].transform,
            Player.InventorySize, InvUIRows, InvUIColumns);
        CauldronUISlots = CreateUISlots(UIPanelCollection[(int)PanelType.CraftingStation].Panels[0].transform,
            CauldronInventorySlots, CauldronUIRows, CauldronUIColumns);
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

    //TODO: Put all the panels into one group and use the index of the panel type to find our panels
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

    public void FillInvUISlot(StorableIngredient ingredient)
    {
        for (int i = 0; i < InvUISlots.Length; i++)
        {
            if (InvUISlots[i].Filled == false)
            {
                InvUISlots[i].Filled = true;
                InvUISlots[i].Ing = ingredient;
                InvUISlots[i].Image.sprite = InvImgSprites[(int)ingredient.Type];
                InvUISlots[i].Btn.onClick.AddListener(delegate { PlayerInvUISlotClick(InvUISlots[i]); });
                InvUISlots[i].Btn.interactable = true;
                Debug.Log("Added InvUISlot");
                break;
            }
        }
    }

    public void EmptyInvUISlot(StorableIngredient ingredient)
    {
        for (int i = 0; i < InvUISlots.Length; i++)
        {
            if (InvUISlots[i].Ing == ingredient)
            {
                InvUISlots[i].Ing = null;
                InvUISlots[i].Filled = false;
                InvUISlots[i].Image.sprite = Resources.Load<Sprite>("InvImg/Empty");
                InvUISlots[i].Btn.onClick.RemoveAllListeners();
                InvUISlots[i].Btn.interactable = false;
                Debug.Log("Emptied InvUISlot");
                break;
            }
        }
    }

    public void IngTableStoreAllBtnPress()
    {
        Player.Inv.StoreIngredientsToTable();
    }

    private void PlayerInvUISlotClick(UISlot UISlot)
    {
        WorldObjectManager.Instance.InstantiateHoldableIngredient(UISlot.Ing);
        Player.Inv.RemoveIngredient(UISlot.Ing);

        ToggleUIPanel();
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
