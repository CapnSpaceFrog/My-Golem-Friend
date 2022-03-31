using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemManager : MonoBehaviour
{
    [System.Serializable]
    public struct GolemState
    {
        public GameObject DefaultStageModel;
        public GameObject ItemReqToGrow;
        public GameObject PrimedStageModel;
        public bool IsPrimed;
        public Vector3 DefaultPosition;
    }

    public GolemState[] GolemStates;

    public GolemState CurrentGolemState;

    public float MinGrowthDistance;

    private GameObject InstantiatedGolem;

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
        Debug.Log(CalculateV3Distance(transform.position, Player.PlayerTransform.position));

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
        if (FPInteract.HeldObject != null)
        {
            return;
        }
        else if (CurrentGolemState.ItemReqToGrow == FPInteract.HeldObject.gameObject)
        {
            PrimeGolemStage();
        }
    }

    private void PrimeGolemStage()
    {
        CurrentGolemState.IsPrimed = true;
    }

    private void AdvanceStage()
    {

    }

    private float CalculateV3Distance(Vector3 startingPoint, Vector3 endPoint)
    {
        float xDis = startingPoint.x - endPoint.x;
        float yDis = startingPoint.y - endPoint.y;
        float zDis = startingPoint.z - endPoint.z;

        xDis *= xDis;
        yDis *= yDis;
        zDis *= zDis;

        float totalDis = xDis + yDis + zDis;

        totalDis = Mathf.Sqrt(totalDis);

        return totalDis;
    }
}
