﻿using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour
{
    void Start()
    {
        ClientNetwork client = ClientNetwork.Instance;
    }

    void OnGUI()
    {
        if (GUILayout.Button("ECHO WEB"))
        {
            ClientNetwork.Instance.Send_CS_ECHO_WEB_REQ("hello world");
        }
    }
}
