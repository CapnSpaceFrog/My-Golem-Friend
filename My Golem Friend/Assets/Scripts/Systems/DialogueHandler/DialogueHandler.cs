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
    public string[] Speaker;
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
    public string[] MainDialogueSpeaker;

    public byte[] PlayerPromptTriggerPoints;

    public BranchingDialogue[] BranchPoints;

    public GolemItemType StateReq;

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
    public TMP_FontAsset FontAsset;

    [Header("Golem Dialogue Variables")]
    public DialogueInteraction[] GolemGenericDialogueInteractions;
    public DialogueInteraction[] GolemStateSpecificInteractions;

    private const int BUTTON_PADDING = 50;
    private const int BUTTON_OFFSET = 85;
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
        if (DictateDialogueInteraction())
        {
            SetDialogueState(true);
            StartCoroutine(DialogueInteraction(GolemStateSpecificInteractions[GolemManager.GetGolemStateIndex()]));
        }
        else
        {
            Debug.Log((GolemItemType)GolemManager.GetGolemStateIndex());
            DialogueInteraction possibleInteraction = GetGenericDialogueFromState((GolemItemType)GolemManager.GetGolemStateIndex());
            Debug.Log(possibleInteraction);
            if (possibleInteraction == null)
                return;

            SetDialogueState(true);
            StartCoroutine(DialogueInteraction(possibleInteraction));
        }
    }

    private DialogueInteraction GetGenericDialogueFromState(GolemItemType type)
    {
        foreach (DialogueInteraction interaction in GolemGenericDialogueInteractions)
        {
            if (interaction.Exhausted == true)
                continue;

            if (interaction.StateReq == type)
                return interaction;
        }

        return null;
    }

    private bool DictateDialogueInteraction()
    {
        if (GolemStateSpecificInteractions[GolemManager.GetGolemStateIndex()].Exhausted)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void SetDialogueState(bool changeState)
    {
        //Add box animation
        if (changeState)
        {
            UIHandler.Instance.CrossHair.SetActive(false);
            InputHandler.EnterDialogueMode();

            DisplayPanel.SetActive(true);
        }
        else
        {
            DisplayPanel.SetActive(false);

            InputHandler.ExitDialogueMode();
            UIHandler.Instance.CrossHair.SetActive(true);
        }
    }

    private IEnumerator DialogueInteraction(DialogueInteraction currentInteraction)
    {
        string[] mainDialogue = currentInteraction.MainDialogueString;
        string[] mainSpeaker = currentInteraction.MainDialogueSpeaker;

        PromptIndex = 0;

        for (int i = 0; i < mainDialogue.Length; i++)
        {
            string fullDisplayString = mainDialogue[i];
            string speaker = mainSpeaker[i];

            if (i == CheckResponseTriggerPoint(i, currentInteraction))
            {
                CurrentlyTalkingText.text = "";
                StartCoroutine(DisplayPrompts(currentInteraction));

                yield return new WaitUntil(() => canContinue == true);

                canContinue = false;

                string[] responseDialogue = currentInteraction.BranchPoints[PromptIndex].PostPromptDialogue[chosenResponse].Dialogue;
                string[] currentSpeaker = currentInteraction.BranchPoints[PromptIndex].PostPromptDialogue[chosenResponse].Speaker;

                for (int j = 0; j < responseDialogue.Length; j++)
                {
                    CurrentlyTalkingText.text = currentSpeaker[j];
                    fullDisplayString = responseDialogue[j];

                    StartCoroutine(DisplayDialogue(fullDisplayString));

                    yield return new WaitUntil(() => canContinue == true);

                    canContinue = false;
                }

                ++PromptIndex;
            }
            else
            {
                CurrentlyTalkingText.text = speaker;
                StartCoroutine(DisplayDialogue(fullDisplayString));

                yield return new WaitUntil(() => canContinue == true);

                canContinue = false;
            }
        }

        currentInteraction.Exhausted = true;
        SetDialogueState(false);
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

        canContinue = true;
    }

    private void ResponsePressed(int index)
    {
        chosenResponse = (byte)index;

        InputHandler.LockCursor();
        DisplayPanelTransform.sizeDelta = new Vector2(DisplayPanelTransform.sizeDelta.x, 300);

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
            buttonImage.sprite = Resources.Load<Sprite>("UIPanels/Dialogue/PromptButton");
            buttonImage.type = Image.Type.Sliced;

            Button btn = buttonObject.AddComponent<Button>();

            //For some reason passing count gives us the last value of count before the loop stops executing,
            //but assigning count to an int and then assigning that int fixes the issue.
            int x = count;
            btn.onClick.AddListener(delegate { ResponsePressed(x); });

            HorizontalLayoutGroup buttonLayoutGroup = buttonObject.AddComponent<HorizontalLayoutGroup>();
            buttonLayoutGroup.padding.right = 15;
            buttonLayoutGroup.padding.left = 15;
            buttonLayoutGroup.padding.top = 20;
            buttonLayoutGroup.padding.bottom = 20;
            buttonLayoutGroup.childControlHeight = false;
            buttonLayoutGroup.childControlWidth = false;
            buttonLayoutGroup.childForceExpandHeight = false;
            buttonLayoutGroup.childForceExpandWidth = false;

            buttonTransform.SetParent(DisplayPanelTransform);
            buttonTransform.anchoredPosition = new Vector2(DisplayPanelTransform.anchoredPosition.x, DisplayPanelTransform.anchoredPosition.y);
            // - DisplayPanelTransform.position.y

            GameObject textObject = new GameObject("TMP");
            textTransform = textObject.AddComponent<RectTransform>();
            textTransform.SetParent(buttonTransform);
            textTransform.anchoredPosition = buttonTransform.anchoredPosition;
            TextMeshProUGUI tmp = textObject.AddComponent<TextMeshProUGUI>();

            tmp.font = FontAsset;
            tmp.color = new Color32(111, 88, 69, 255);

            tmp.fontSizeMax = 26;
            tmp.fontSizeMin = 20;
            tmp.enableAutoSizing = true;
            tmp.characterSpacing = 15;

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

        buttonTransforms[0].localPosition = new Vector2(0, (DisplayPanelTransform.sizeDelta.y / 2) - ((buttonSpace / 2) + (BUTTON_PADDING / 2)) );

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