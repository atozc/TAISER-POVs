using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.IO;
using System.Text;
using System.Xml;


// Altered from instrumentMgr.cs from Taiser 

[System.Serializable]
public class Session
{
    public string dayandTime;
    public List<TaiserAdminRecord> records;
}

[System.Serializable]
public class TaiserAdminRecord
{
    public float secondsFromStart;
    public string eventName;
    public List<string> eventModifiers;
}


public class AdminWriteFile : MonoBehaviour
{
    [Header("Input Fields")]
    public InputField userName;
    public InputField packetSpeed;
    public InputField badPacketRatio;
    public InputField MaxPacketNumber;
    public InputField MeanIntervalRuleChanges;
    public InputField IntervalDeviation;
    public InputField AICorrectProbability;
    public InputField AIRandomSeed;
    public InputField HumanCorrectProbability;
    public InputField HumanRandomSeed;
    public InputField MinHumanAdviceTime;
    public InputField MaxWaves;
    public InputField MaxHumanAdviceTime;
    public InputField MinAIAdviceTime;
    public InputField MaxAIAdviceTime;
    public InputField Penalty;
    [Header("Data Strings")]
    public string usernameText;
    public string packetSpeedText;
    public string badPacketRatioText;
    public string MaxPacketNumberText;
    public string MeanIntervalRuleChangesText;
    public string IntervalDeviationText;
    public string AICorrectProbabilityText;
    public string AIRandomSeedText;
    public string HumanCorrectProbabilityText;
    public string HumanRandomSeedText;
    public string MinHumanAdviceTimeText;
    public string MaxWavesText;
    public string MaxHumanAdviceTimeText;
    public string MinAIAdviceTimeText;
    public string MaxAIAdviceTimeText;
    public string PenaltyText;

    // Start is called before the first frame update
    void Start()
    {
        CreateOrFindTaiserAdminFolder();
    }

    public string TaiserAdminFolder;

    public void CreateOrFindTaiserAdminFolder()
    {
        try
        {
            TaiserAdminFolder = System.IO.Path.Combine(Application.persistentDataPath);
            System.IO.Directory.CreateDirectory(TaiserAdminFolder);
        }
        catch (System.Exception e)
        {
            Debug.Log("Cannot create Taiser Admin Directory: " + e.ToString());
        }

    }

    public Session session = new Session();

    public void AddRecord(string eventName, List<string> modifiers)
    {
        TaiserAdminRecord record = new TaiserAdminRecord();
        record.eventName = eventName;
        record.eventModifiers = modifiers;
        record.secondsFromStart = Time.time;// Time.realtimeSinceStartup;
        session.records.Add(record);
    }

    public void AddRecord(string eventName, string modifier = "")
    {
        TaiserAdminRecord record = new TaiserAdminRecord();
        record.eventName = eventName;
        List<string> mods = new List<string>();
        mods.Add(modifier);
        record.eventModifiers = mods;
        record.secondsFromStart = Time.time; // Time.realtimeSinceStartup;
        session.records.Add(record);
    }

    public void Send()
    {
        // Converts user input to text
        usernameText = userName.text;
        packetSpeedText = packetSpeed.text;
        badPacketRatioText = badPacketRatio.text;
        MaxPacketNumberText = MaxPacketNumber.text;
        MeanIntervalRuleChangesText = MeanIntervalRuleChanges.text;
        IntervalDeviationText = IntervalDeviation.text;
        AICorrectProbabilityText = AICorrectProbability.text;
        AIRandomSeedText = AIRandomSeed.text;
        HumanCorrectProbabilityText = HumanCorrectProbability.text;
        HumanRandomSeedText = HumanRandomSeed.text;
        MinHumanAdviceTimeText = MinHumanAdviceTime.text;
        MaxWavesText = MaxWaves.text;
        MaxHumanAdviceTimeText = MaxHumanAdviceTime.text;
        MinAIAdviceTimeText = MinAIAdviceTime.text;
        MaxAIAdviceTimeText = MaxAIAdviceTime.text;
        PenaltyText = Penalty.text;

    session.dayandTime = System.DateTime.Now.ToUniversalTime().ToString();
        string tmp = System.DateTime.Now.ToLocalTime().ToString();

        using (StreamWriter sw = new StreamWriter(File.Open(Path.Combine(TaiserAdminFolder, "parameters.csv"), FileMode.Create), Encoding.UTF8))
        {
            WriteHeader(sw);
            WriteRecords(sw);
        }

        StartCoroutine(Upload());
    }

    IEnumerator Upload()
    {
        XmlDocument map = new XmlDocument();
        map.LoadXml("<level></level>");
        byte[] levelData = Encoding.UTF8.GetBytes(MakeHeaderString() + MakeRecords());
        //string fileName = new string(usernameText.ToCharArray()) + session.dayandTime; 
        string fileName = "parameters.csv";
        Debug.Log("FileName: " + fileName);

        WWWForm form = new WWWForm();
        Debug.Log("Created new WWW Form");

        form.AddField("name", "data");
        form.AddField("file", "file");
        form.AddBinaryData("file", levelData, fileName, "text/csv");
        Debug.Log("Binary data added");

        UnityWebRequest www = UnityWebRequest.Post("https://www.cse.unr.edu/~crystala/taiser/test/data/dataUploader.php", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("ERROR: " + www.error);
        }
        else
        {
            Debug.Log("No Errors! File is uploading...");
            if (www.uploadProgress == 1 || www.isDone)
            {
                yield return new WaitForSeconds(5);
                Debug.Log("Form Upload Complete!");
                Debug.Log(www.downloadHandler.text);
            }
        }
    }

    public void WriteHeader(StreamWriter sw)
    {
        sw.WriteLine(MakeHeaderString());
    }

    string eoln = "\r\n";
    public string MakeHeaderString()
    {
        string header = "";
        //header += usernameText + ", " + eoln;
        header += "Packet Speed ," + packetSpeedText + eoln;
        header += "Bad Packet Ratio ," + badPacketRatioText + eoln;
        header += "Max Packet Number ," + MaxPacketNumberText + eoln;
        header += "Mean Interval Rule Chamges ," + MeanIntervalRuleChangesText + eoln;
        header += "Interval Deviation ," + IntervalDeviationText + eoln;
        header += "AI Correct Probabiltiy ," + AICorrectProbabilityText + eoln;
        header += "AI Random Seed ," + AIRandomSeedText + eoln;
        header += "Human Correct Probability ," + HumanCorrectProbabilityText + eoln;
        header += "Human Random Seed ," + HumanRandomSeedText + eoln;
        header += "Min Human Advice Time ," + MinHumanAdviceTimeText + eoln;
        header += "Max Waves ," + MaxWavesText + eoln;
        header += "Max Human Advice Time ," + MaxHumanAdviceTimeText + eoln;
        header += "Min AI Advice Time ," + MinHumanAdviceTimeText + eoln;
        header += "Max AI Advice Time ," + MaxAIAdviceTimeText + eoln;
        header += "Penalty ," + PenaltyText + eoln;
        return header;
    }

    public void WriteRecords(StreamWriter sw)
    {
        sw.WriteLine(MakeRecords());
    }

    public string MakeRecords()
    {
        string lines = "";
        foreach (TaiserAdminRecord tr in session.records)
        {
            string mods = CSVString(tr.eventModifiers);
            lines += tr.secondsFromStart.ToString("0000.0") + ", " + tr.eventName + mods + eoln;
        }
        return lines;

    }

    public string CSVString(List<string> mods)
    {
        string modifiers = "";
        foreach (string mod in mods)
        {
            modifiers += ", " + mod;
        }
        return modifiers;
    }
}
