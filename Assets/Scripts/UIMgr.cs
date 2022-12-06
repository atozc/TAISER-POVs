using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMgr : MonoBehaviour
{
    public void OnTargetClick()
    {
        SceneManager.LoadScene(4);
    }
}
