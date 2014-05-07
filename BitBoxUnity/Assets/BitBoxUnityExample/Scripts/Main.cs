using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour
{
    void Start()
    {
        ClientNetwork client = ClientNetwork.Instance;
    }

    void OnGUI()
    {
        if (GUILayout.Button("TEST"))
        {
            ClientNetwork.Instance.Send_CS_TEST_REQ("hello world");
        }
    }
}
