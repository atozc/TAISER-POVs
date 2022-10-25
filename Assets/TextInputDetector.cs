using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextInputDetector : MonoBehaviour
{
    public InputField input;
    public GameObject AdminPanel;
    //public GameObject Panel;
    public Button Button;

    void Start()
    {
        Button btn = Button.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        if (input.text == "admin" | input.text == "Admin" | input.text == "ADMIN")
        {
            Debug.Log("Variation of 'admin' entered as input");
            AdminPanel.SetActive(true);   //Load admin console panel
        }
        else
        {
            //Panel.SetActive(true); //Loads next scene w/o admin
        }
    }
}
