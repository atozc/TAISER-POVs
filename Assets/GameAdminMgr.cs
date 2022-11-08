using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAdminMgr : MonoBehaviour
{
    public enum Parameters {
    packetSpeed,
    badPacketRatio,
    MaxPacketNumber,
    MeanIntervalRuleChanges,
    IntervalDeviation,
    AICorrectProbability,
    AIRandomSeed,
    HumanCorrectProbability,
    HumanRandomSeed,
    MinHumanAdviceTime,
    MaxWaves,
    MaxHumanAdviceTime,
    MinAIAdviceTime,
    MaxAIAdviceTime,
    Penalty
    }

    public static GameAdminMgr inst;

    public float GetTuningParameterValue;

    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
