using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{

    [Header("Points")]
    public List<Transform> respanwPoint = new List<Transform>();

    private void Awake()
    {
        Transform[] point = GetComponentsInChildren<Transform>();

        respanwPoint = new List<Transform>();
        for (int i = 1; i < point.Length; i++)
        {
            respanwPoint.Add(point[i]);
        }
    }
}
