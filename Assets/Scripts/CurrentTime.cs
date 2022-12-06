using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentTime : MonoBehaviour
{
    public Text currentTimeText;

    public void Update()
    {
        System.DateTime theTime = System.DateTime.Now;
        string time = theTime.ToString("HH:mm:ss");

        currentTimeText.text = time;
    }
}
