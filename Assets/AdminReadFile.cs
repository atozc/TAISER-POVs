using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class AdminReadFile : MonoBehaviour
{
    public string dataResults;

    public string newDataResults;

    //Game Parameters
    public string packetSpeed;
    public string badPacketRatio;
    public string MaxPacketNumber;
    public string MeanIntervalRuleChanges;
    public string IntervalDeviation;
    public string AICorrectProbability;
    public string AIRandomSeed;
    public string HumanCorrectProbability;
    public string HumanRandomSeed;
    public string MinHumanAdviceTime;
    public string MaxWaves;
    public string MaxHumanAdviceTime;
    public string MinAIAdviceTime; 
    public string MaxAIAdviceTime;
    public string Penalty;


    void Start()
    {
        StartCoroutine(GetData());
    }

    IEnumerator GetData()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://www.cse.unr.edu/~crystala/taiser/test/data/parameters.csv");
        //www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
            //Debug.Log(results);
        }

        dataResults = www.downloadHandler.text;
   
        //Split data into rows
        string[] splitData = dataResults.Split('\n');
        packetSpeed = splitData[0];
        badPacketRatio = splitData[1];
        MaxPacketNumber = splitData[2];
        MeanIntervalRuleChanges = splitData[3];
        IntervalDeviation = splitData[4]; 
        AICorrectProbability = splitData[5];
        AIRandomSeed = splitData[6];
        HumanCorrectProbability = splitData[7];
        HumanRandomSeed = splitData[8];
        MinHumanAdviceTime = splitData[9];
        MaxWaves = splitData[10];
        MaxHumanAdviceTime = splitData[11];
        MinAIAdviceTime = splitData[12];
        MaxAIAdviceTime = splitData[13];
        Penalty = splitData[14];
    
        //Split Rows to find value of Parameter
        string[] splitPacketSpeed = packetSpeed.Split(',');
        packetSpeed = splitPacketSpeed[1];

        string[] splitBadPacketRatio = badPacketRatio.Split(',');
        badPacketRatio = splitBadPacketRatio[1];

        string[] splitMaxPacketNumber = MaxPacketNumber.Split(',');
        MaxPacketNumber = splitMaxPacketNumber[1];

        string[] splitMeanIntervalRuleChanges = MeanIntervalRuleChanges.Split(',');
        MeanIntervalRuleChanges = splitMeanIntervalRuleChanges[1];

        string[] splitIntervalDeviation = IntervalDeviation.Split(',');
        IntervalDeviation = splitIntervalDeviation[1];

        string[] splitAICorrectProbability = AICorrectProbability.Split(',');
        AICorrectProbability = splitAICorrectProbability[1];

        string[] splitAIRandomSeed = AIRandomSeed.Split(',');
        AIRandomSeed = splitAIRandomSeed[1];

        string[] splitHumanCorrectProbability = HumanCorrectProbability.Split(',');
        HumanCorrectProbability = splitHumanCorrectProbability[1];

        string[] splitHumanRandomSeed = HumanRandomSeed.Split(',');
        HumanRandomSeed = splitHumanRandomSeed[1];

        string[] splitMinHumanAdviceTime = MinHumanAdviceTime.Split(',');
        MinHumanAdviceTime = splitMinHumanAdviceTime[1];

        string[] splitMaxWaves = MaxWaves.Split(',');
        MaxWaves = splitMaxWaves[1];

        string[] splitMaxHumanAdviceTime = MaxHumanAdviceTime.Split(',');
        MaxHumanAdviceTime = splitMaxHumanAdviceTime[1];

        string[] splitMinAIAdviceTime = MinAIAdviceTime.Split(',');
        MinAIAdviceTime = splitMinAIAdviceTime[1];

        string[] splitMaxAIAdviceTime = MaxAIAdviceTime.Split(',');
        MaxAIAdviceTime = splitMaxAIAdviceTime[1];

        string[] splitPenalty = Penalty.Split(',');
        Penalty = splitPenalty[1];
    }
}
