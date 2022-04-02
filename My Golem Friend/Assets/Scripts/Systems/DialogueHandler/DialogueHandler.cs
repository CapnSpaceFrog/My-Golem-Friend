using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public struct DialogueCollection
{
    public string[] Dialogue;
}

[System.Serializable]
public struct BranchingDialogue
{
    public string[] PromptChoices;
    public DialogueCollection[] PostPromptDialogue;
}

[System.Serializable]
public class DialogueInteraction
{
    public string[] MainDialogueString;
    public Sprite[] MainDialogueSpeaker;

    public byte[] PlayerPromptTriggerPoints;

    public BranchingDialogue[] BranchPoints;

    [HideInInspector]
    public bool Exhausted;
}

public class DialogueHandler : MonoBehaviour
{
    [Header("General Dialogue Variables")]
    public GameObject DisplayPanel;

    public RectTransform DisplayPanelTransform;

    public TextMeshProUGUI DisplayTextBox;
    public TextMeshProUGUI CurrentlyTalkingText;

    public float TEXT_DISPLAY_SPEED;

    [Header("Golem Dialogue Variables")]
    public DialogueInteraction[] GolemGenericDialogueInteractions;
    public DialogueInteraction[] GolemStateSpecificInteractions;

    private const int BUTTON_PADDING = 50;
    private const int BUTTON_OFFSET = 45;
    private const int TEXTBOX_PADDING = 100;

    private bool selectedResponse = false;
    private bool canContinue;

    private byte PromptIndex;
    private byte chosenResponse;

    public void OnEnable()
    {
        FPInteract.OnGolemDialogueInteract += StartDialogueInteraction;
    }

    public void StartDialogueInteraction()
    {
        SetDialogueState(true);

        //Call a function that retrieves info from the golem manager and spits out a dialogue interaction\

        StartCoroutine(DialogueInteraction(GetDialogueInteraction()));
    }

    private DialogueInteraction GetDialogueInteraction()
    {
        if (GolemStateSpecificInteractions[GolemManager.GetGolemStateIndex()].Exhausted)
        {
            return GolemGenericDialogueInteractions[UnityEngine.Random.Range(0, GolemGenericDialogueInteractions.Length)];
        }
        else
        {
            return GolemStateSpecificInteractions[GolemManager.GetGolemStateIndex()];
        }
    }

    private void SetDialogueState(bool changeState)
    {
        //Add box animation

        if (changeState)
        {
            InputHandler.EnterDialogueMode();

            DisplayPanel.SetActive(true);
        }
        else
        {
            DisplayPanel.SetActive(false);

            InputHandler.LockCursor();
            InputHandler.ExitDialogueMode();
        }
    }

    private IEnumerator DialogueInteraction(DialogueInteraction currentInteraction)
    {
        string[] displayDialogue = currentInteraction.MainDialogueString;

        PromptIndex = 0;

        for (int i = 0; i < displayDialogue.Length; i++)
        {
            string fullDisplayString = displayDialogue[i];

            if (i == CheckResponseTriggerPoint(i, currentInteraction))
            {
                CurrentlyTalkingText.text = "";
                StartCoroutine(DisplayPrompts(currentInteraction));

                yield return new WaitUntil(() => canContinue == true);

                canContinue = false;

                string[] responseDialogue = currentInteraction.BranchPoints[PromptIndex].PostPromptDialogue[chosenResponse].Dialogue;

                for (int j = 0; j < responseDialogue.Length; j++)
                {
                    fullDisplayString = responseDialogue[j];

                    StartCoroutine(DisplayDialogue(fullDisplayString));

                    yield return new WaitUntil(() => canContinue == true);

                    canContinue = false;
                }

                ++PromptIndex;
            }
            else
            {
                StartCoroutine(DisplayDialogue(fullDisplayString));

                yield return new WaitUntil(() => canContinue == true);

                canContinue = false;
            }
        }

        FinishedDisplaying();
    }

    private int CheckResponseTriggerPoint(int index, DialogueInteraction currentInteraction)
    {
        foreach (int triggerPoint in currentInteraction.PlayerPromptTriggerPoints)
        {
            if (triggerPoint == index)
            {
                return triggerPoint;
            }
        }

        //Returning -1 since it's impossible for the loop index to be a negative number
        return -1;
    }

    private IEnumerator DisplayPrompts(DialogueInteraction currentInteraction)
    {
        DisplayTextBox.text = "";

        GameObject[] buttons = GenerateResponseButtons(currentInteraction);

        //Unlock the cursor for the Player
        InputHandler.UnlockCursor();

        //Wait Until the Player has pressed a button
        yield return new WaitUntil(() => selectedResponse == true);

        selectedResponse = false;

        for (int i = 0; i < buttons.Length; i++)
        {
            Destroy(buttons[i]);
        }

        canContinue = true;
    }

    private IEnumerator DisplayDialogue(string dialogueToDisplay)
    {
        for (int count = 0; count <= dialogueToDisplay.Length; count++)
        {
            string currentText = dialogueToDisplay.Substring(0, count);
            DisplayTextBox.text = currentText;

            yield return new WaitForSeconds(TEXT_DISPLAY_SPEED);
        }

        yield return new WaitUntil(() => Keyboard.current.spaceKey.wasPressedThisFrame);
    }

    private void FinishedDisplaying()
    {
        //Close the panel, exit the dialogue state, and update the dialogue struct that it has been used
    }

    private void ResponsePressed(int index)
    {
        chosenResponse = (byte)index;

        InputHandler.LockCursor();
        DisplayPanelTransform.sizeDelta = new Vector2(DisplayPanelTransform.sizeDelta.x, 200);

        selectedResponse = true;
    }

    private GameObject[] GenerateResponseButtons(DialogueInteraction currentInteraction)
    {
        RectTransform buttonTransform = null;
        RectTransform textTransform = null;

        int size = currentInteraction.BranchPoints[PromptIndex].PromptChoices.Length;

        RectTransform[] buttonTransforms = new RectTransform[size];
        GameObject[] buttons = new GameObject[size];

        for (int count = 0; count < size; count++)
        {
            GameObject buttonObject = new GameObject($"Button { count }");
            buttonTransform = buttonObject.AddComponent<RectTransform>();
            buttonObject.AddComponent<CanvasRenderer>();
            Image buttonImage = buttonObject.AddComponent<Image>();
            buttonImage.sprite = Resources.Load<Sprite>("UI/Button");
            buttonImage.type = Image.Type.Sliced;

            Button btn = buttonObject.AddComponent<Button>();

            //For some reason passing count gives us the last value of count before the loop stops executing,
            //but assigning count to an int and then assigning that int fixes the issue.
            int x = count;
            btn.onClick.AddListener(delegate { ResponsePressed(x); });

            HorizontalLayoutGroup buttonLayoutGroup = buttonObject.AddComponent<HorizontalLayoutGroup>();
            buttonLayoutGroup.padding.right = 15;
            buttonLayoutGroup.padding.left = 15;
            buttonLayoutGroup.padding.top = 10;
            buttonLayoutGroup.padding.bottom = 10;
            buttonLayoutGroup.childControlHeight = false;
            buttonLayoutGroup.childControlWidth = false;
            buttonLayoutGroup.childForceExpandHeight = false;
            buttonLayoutGroup.childForceExpandWidth = false;

            buttonTransform.SetParent(DisplayPanelTransform);
            buttonTransform.anchoredPosition = new Vector2(DisplayPanelTransform.anchoredPosition.x, DisplayPanelTransform.anchoredPosition.y - DisplayPanelTransform.position.y);

            GameObject textObject = new GameObject("TMP");
            textTransform = textObject.AddComponent<RectTransform>();
            textTransform.SetParent(buttonTransform);
            textTransform.anchoredPosition = buttonTransform.anchoredPosition;
            TextMeshProUGUI tmp = textObject.AddComponent<TextMeshProUGUI>();

            tmp.fontSizeMax = 22;
            tmp.fontSizeMin = 18;
            tmp.enableAutoSizing = true;

            tmp.rectTransform.sizeDelta = new Vector2((DisplayPanelTransform.sizeDelta.x - TEXTBOX_PADDING), 0);

            ContentSizeFitter textSizeFitter = textObject.AddComponent<ContentSizeFitter>();
            textSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            textSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;


            ContentSizeFitter buttonSizeFitter = buttonObject.AddComponent<ContentSizeFitter>();
            buttonSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            buttonSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            //For some reason the content size fitters / horionztal layout slightly scale up
            //the parent object which makes everything *slightly* bigger than it should
            buttonTransform.localScale = Vector3.one;

            tmp.text = currentInteraction.BranchPoints[PromptIndex].PromptChoices[count];
            tmp.ForceMeshUpdate();

            //Need to force all the rect transforms of the layouts to update so I can pull the correct information
            LayoutRebuilder.ForceRebuildLayoutImmediate(buttonTransform);

            //Apparently the height of the button doesn't include the height of its child, so to properly
            //calculate the max button size we'll need to add the two together
            buttonTransforms[count] = buttonTransform;
            buttons[count] = buttonObject;
        }

        float buttonSpace = buttonTransform.rect.height + textTransform.rect.height;

        float totalButtonSpace = (buttonSpace) + (BUTTON_OFFSET * (buttonTransforms.Length - 1)) + BUTTON_PADDING;

        DisplayPanelTransform.sizeDelta = new Vector2(DisplayPanelTransform.sizeDelta.x, totalButtonSpace);

        buttonTransforms[0].localPosition = new Vector2(0, DisplayPanelTransform.sizeDelta.y - ((buttonSpace / 2) + (BUTTON_PADDING / 2)));

        float lastYPosition = buttonTransforms[0].localPosition.y;

        //starting at 1 since we defaultly set the first button always
        for (int i = 1; i < buttonTransforms.Length; i++)
        {
            buttonTransforms[i].localPosition = new Vector2(0, lastYPosition - BUTTON_OFFSET);

            lastYPosition = buttonTransforms[i].localPosition.y;
        }

        return buttons;
    }

    void OnDisable()
    {
        FPInteract.OnGolemDialogueInteract -= StartDialogueInteraction;
    }
}