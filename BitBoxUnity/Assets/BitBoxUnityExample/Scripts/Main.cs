using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour
{
    int ButtonCount = 2;
    static GUILayoutOption[] ButtonLayoutOption;

    void OnGUI()
    {
        GUILayout.BeginVertical();
        if (GUILayout.Button("ECHO WEB", GetOption()))
        {
            ClientNetwork.Instance.Send_CS_ECHO_WEB_REQ("hello world web");
        }

        if (GUILayout.Button("ECHO APP", GetOption()))
        {
            ClientNetwork.Instance.Send_CS_ECHO_APP_REQ("hello world app");
        }

        if (GUILayout.Button("Send twice ECHO APP", GetOption()))
        {
            ClientNetwork.Instance.Send_CS_ECHO_APP_REQ_TWICE("hello world app");
        }
        GUILayout.EndVertical();
    }

    GUILayoutOption[] GetOption()
    {
        if (ButtonLayoutOption == null)
            ButtonLayoutOption = new GUILayoutOption[] { GUILayout.MaxHeight(Screen.height / ButtonCount), GUILayout.MaxWidth(Screen.width) };
        return ButtonLayoutOption;
    }
}