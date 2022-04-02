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

    private static GolemState currentGolemState;

    [Header("Golem Variables")]
    public int GrowthDistance;

    public static int GetGolemStateIndex()
    {
        return (int)currentGolemState.ReqItem;
    }

    void Awake()
    {
        currentGolemState = GolemStates[0];
    }

    void OnEnable()
    {
        FPInteract.OnGolemInteract += HandleGolemInteract; 
    }

    void Update()
    {
        if (currentGolemState.IsPrimed)
        {
            float distance = CalculateV3Distance(InstantiatedGolem.transform.position, Player.PlayerTransform.position);

            if (distance > GrowthDistance)
            {
                UIHandler.Instance.UpdateGolemProgressUI(currentGolemState.ReqItem);
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
        else if (currentGolemState.ReqItem == item.ItemType)
        {
            Destroy(item.gameObject);
            PrimeGolemStage();
        }
    }

    private void PrimeGolemStage()
    {
        if (currentGolemState.ReqItem == GolemItemType.Essence)
        {
            SceneLoader.Instance.LoadEndScene();
            return;
        }

        GameObject primedGolem = Instantiate(currentGolemState.PrimedStageModel);
        primedGolem.SetActive(false);
        primedGolem.transform.position = InstantiatedGolem.transform.position;
        InstantiatedGolem.SetActive(false);
        primedGolem.SetActive(true);
        Destroy(InstantiatedGolem);
        InstantiatedGolem = primedGolem;
        currentGolemState.IsPrimed = true;
    }

    private void AdvanceStage()
    {
        currentGolemState = GolemStates[(int)currentGolemState.ReqItem + 1];

        GameObject primedGolem = Instantiate(currentGolemState.DefaultStageModel);

        InstantiatedGolem.SetActive(false);
        Destroy(InstantiatedGolem);
        InstantiatedGolem = primedGolem;

        InstantiatedGolem.transform.position = currentGolemState.WaitingPosition.position;
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