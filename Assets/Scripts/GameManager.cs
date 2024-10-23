using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        CreatePlayer();
    }

    private void CreatePlayer()
    {
        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);
        PhotonNetwork.Instantiate("Player",
                                points[idx].position,
                                points[idx].rotation,
                                0);
    }
}
