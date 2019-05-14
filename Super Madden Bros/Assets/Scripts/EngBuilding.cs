using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EngBuilding : MonoBehaviour
{
    public GameObject music;
    void OnBecameVisible()
    {
        music = GameObject.FindGameObjectWithTag("Player");
        music.GetComponent<Player>().play();
    }
}
