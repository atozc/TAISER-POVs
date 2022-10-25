using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class AdminReadFile : MonoBehaviour
{
    public string dataResults;

    public string newDataResults;

    public string packetSpeed;
    public string badPacketRatio;

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

        string[] splitData = dataResults.Split('\n');
        packetSpeed = splitData[0];
        badPacketRatio = splitData[1];

        string[] splitPacketSpeed = packetSpeed.Split(',');
        packetSpeed = splitPacketSpeed[1];

        string[] splitBadPacketRatio = badPacketRatio.Split(',');
        badPacketRatio = splitBadPacketRatio[1];
    }
}
