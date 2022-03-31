using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemManager : MonoBehaviour
{
    [System.Serializable]
    public struct GolemState
    {
        public GameObject StageModel;
        public GameObject ItemReqToGrow;
    }

    public GolemState[] GolemStates;

    private void AdvanceStage()
    {

    }
}
