using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GolemItemType
{
    Core,
    Potion,
    Scroll,
    Necklace,
    Essence
}

public class GolemManager : MonoBehaviour
{
    [System.Serializable]
    public struct GolemState
    {
        public GolemItemType ReqItem;
        public GameObject DefaultStageModel;
        public GameObject PrimedStageModel;
        public Transform WaitingPosition;
        public bool IsPrimed;
    }

    [Header("Golem States")]
    public GameObject InstantiatedGolem;

    public GolemState[] GolemStates;

    public GolemState CurrentGolemState;

    [Header("Golem Variables")]
    public int MinGrowthDistance;

    void Awake()
    {
        CurrentGolemState = GolemStates[0];
    }

    void OnEnable()
    {
        FPInteract.OnGolemInteract += HandleGolemInteract; 
    }

    void Update()
    {
        if (CurrentGolemState.IsPrimed)
        {
            float distance = CalculateV3Distance(InstantiatedGolem.transform.position, Player.PlayerTransform.position);

            if (distance > MinGrowthDistance)
            {
                AdvanceStage();
            }
        }
    }

    private void HandleGolemInteract()
    {
        GolemItem item = FPInteract.HeldObject.GetComponent<GolemItem>();

        if (item == null)
        {
            return;
        }
        else if (CurrentGolemState.ReqItem == item.ItemType)
        {
            Destroy(item.gameObject);
            PrimeGolemStage();
        }
    }

    private void PrimeGolemStage()
    {
        if (CurrentGolemState.ReqItem == GolemItemType.Essence)
        {
            SceneLoader.Instance.LoadEndScene();
            return;
        }

        GameObject primedGolem = Instantiate(CurrentGolemState.PrimedStageModel);
        primedGolem.SetActive(false);
        primedGolem.transform.position = InstantiatedGolem.transform.position;
        InstantiatedGolem.SetActive(false);
        primedGolem.SetActive(true);
        Destroy(InstantiatedGolem);
        InstantiatedGolem = primedGolem;
        CurrentGolemState.IsPrimed = true;
    }

    private void AdvanceStage()
    {
        CurrentGolemState = GolemStates[(int)CurrentGolemState.ReqItem + 1];

        GameObject primedGolem = Instantiate(CurrentGolemState.DefaultStageModel);

        InstantiatedGolem.SetActive(false);
        Destroy(InstantiatedGolem);
        InstantiatedGolem = primedGolem;

        InstantiatedGolem.transform.position = CurrentGolemState.WaitingPosition.position;
    }

    private int CalculateV3Distance(Vector3 startingPoint, Vector3 endPoint)
    {
        int xDis = (int)startingPoint.x - (int)endPoint.x;
        int yDis = (int)startingPoint.y - (int)endPoint.y;
        int zDis = (int)startingPoint.z - (int)endPoint.z;

        xDis *= xDis;
        yDis *= yDis;
        zDis *= zDis;

        int totalDis = xDis + yDis + zDis;

        totalDis = (int)Mathf.Sqrt(totalDis);

        return totalDis;
    }
}