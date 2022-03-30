using TMPro;
using UnityEngine.UI;
using UnityEngine;

public struct InvUISlot
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
    System
}

public class UIHandler : MonoBehaviour
{
    public static UIHandler Instance { get; private set; }

    [Header("General Panel Variables")]
    public GameObject MainPanel;
    public PanelCollection PlayerPanels;
    public PanelCollection SystemPanels;
    bool InSystemPanel;
    GameObject CurrentPanel;

    [System.Serializable]
    public struct PanelCollection
    {
        public GameObject[] Panels;
        [HideInInspector]
        public int CurrentIndex;
        public int MaxPanels;
    }

    [Header("Inventory Panel Variables")]
    public Sprite[] InvImgSprites;
    public int Rows;
    public int Colums;
    public Vector2 InvSpriteSize;
    public int xPadding;
    public int yPadding;
    InvUISlot[] InvUISlots;

    [Header("Ingredient Storage Table Variables")]
    public TextMeshProUGUI[] StorageCounters; 
    

    private void Awake()
    {
        Instance = this;

        //When we call this function will need to change when we start swapping scenes
        CreateInventoryUISlots();
    }

    private void CreateInventoryUISlots()
    {
        InvUISlots = new InvUISlot[Player.InventorySize];

        Transform InvPanelTransform = PlayerPanels.Panels[0].transform;
        Sprite emptyInv = Resources.Load<Sprite>("InvImg/Empty");

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
            InvUISlots[i].Image.sprite = emptyInv;

            InvUISlots[i].Btn = InvUISlots[i].Object.AddComponent<Button>();
            InvUISlots[i].Btn.transition = Selectable.Transition.None;
            InvUISlots[i].Btn.interactable = false;

            InvUISlots[i].Transform.parent = InvPanelTransform;

            InvUISlots[i].Transform.position = InvPanelTransform.position;

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

    private void OnEnable()
    {
        InputHandler.OnMenuInput += OnMenuInput;
        FPInteract.OnStorageTableInteract += HandleStorageTableInteract;
    }

    //TODO: Break this out to more digesitble methods that have
    //more direct control over what behavior to do
    private void OnMenuInput()
    {
        //If we received input and are in the system panel, back out of it
        if (InSystemPanel)
        {
            InSystemPanel = false;
            MainPanel.SetActive(false);
            CurrentPanel.SetActive(false);

            InputHandler.ExitUIMode();
        }
        //If we aren't in the system panel, back out of it
        else if (MainPanel.activeInHierarchy == true)
        {
            MainPanel.SetActive(false);
            CurrentPanel.SetActive(false);

            InputHandler.ExitUIMode();
        }
        //We aren't in any menu screen, and we didnt interact with the table,
        //So enable the player inv panel
        else
        {
            CurrentPanel = PlayerPanels.Panels[0];
            PlayerPanels.CurrentIndex = 0;

            MainPanel.SetActive(true);
            CurrentPanel.SetActive(true);

            InputHandler.EnterUIMode();
        }
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
                InvUISlots[i].Btn.onClick.AddListener(delegate { OnInvUISlotClick(InvUISlots[i]); });
                InvUISlots[i].Btn.interactable = true;
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
                break;
            }
        }
    }

    private void HandleStorageTableInteract()
    {
        //Default to the first System UI Panel
        CurrentPanel = SystemPanels.Panels[0];
        SystemPanels.CurrentIndex = 0;

        //Lock the Player controls
        InputHandler.EnterUIMode();

        //Display the UI Panels
        CurrentPanel.SetActive(true);
        MainPanel.SetActive(true);

        InSystemPanel = true;
    }

    //Seperate these functions into sub functions depending on which menu the Player is in
    public void RightUIPanelClick()
    {
        if (InSystemPanel)
        {
            CurrentPanel.SetActive(false);

            CurrentPanel = SystemPanels.Panels[(++SystemPanels.CurrentIndex) % SystemPanels.MaxPanels];

            CurrentPanel.SetActive(true);
        }
        else
        {
            CurrentPanel.SetActive(false);

            CurrentPanel = PlayerPanels.Panels[(++PlayerPanels.CurrentIndex) % PlayerPanels.MaxPanels];

            CurrentPanel.SetActive(true);
        }
    }

    public void LeftUIPanelClick()
    {
        if (InSystemPanel)
        {
            CurrentPanel.SetActive(false);

            if (SystemPanels.CurrentIndex - 1 < 0)
            {
                SystemPanels.CurrentIndex = SystemPanels.MaxPanels;
            }

            CurrentPanel = SystemPanels.Panels[--SystemPanels.CurrentIndex];

            CurrentPanel.SetActive(true);
        }
        else
        {
            CurrentPanel.SetActive(false);

            if (PlayerPanels.CurrentIndex - 1 < 0)
            {
                PlayerPanels.CurrentIndex = PlayerPanels.MaxPanels;
            }

            CurrentPanel = PlayerPanels.Panels[--PlayerPanels.CurrentIndex];

            CurrentPanel.SetActive(true);
        }
    }

    public void StoreAllButtonPress()
    {
        Player.Inv.StoreIngredientsToTable();
    }

    private void OnInvUISlotClick(InvUISlot UISlot)
    {
        WorldObjectManager.Instance.InstantiateHoldableIngredient(UISlot.Ing);
        Player.Inv.RemoveIngredient(UISlot.Ing);

        OnMenuInput();
    }

    public void UpdateStoredIngCounters()
    {
        for (int i = 0; i < StorageCounters.Length; i++)
        {
            StorageCounters[i].text = $"{(IngredientType)i} x{IngredientTableManager.StoredIngredients[i]}";
        }
    }

    private void OnDisable()
    {
        InputHandler.OnMenuInput -= OnMenuInput;
        FPInteract.OnStorageTableInteract -= HandleStorageTableInteract;
    }
}
