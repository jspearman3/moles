using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldManager : MonoBehaviour
{
    MolesClient molesClient;
    // Start is called before the first frame update
    void Start()
    {
        molesClient = GameObject.Find("MolesClient").GetComponent<MolesClient>();
    }

    public void Disconnect()
    {
        molesClient.disconnect();
    }
}
