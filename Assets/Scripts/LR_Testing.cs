using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LR_Testing : MonoBehaviour
{
    [SerializeField] private Transform[] points;
    [SerializeField] private LineMgr line;

    private void Start()
    {
        line.SetUpLine(points);
    }
}
