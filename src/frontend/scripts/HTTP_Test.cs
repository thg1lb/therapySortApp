using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class HTTP_Test : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(TestConnection());
    }

    IEnumerator TestConnection()
    {
        string url = "http://127.0.0.1:5000/"; // local host

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Connected! Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.Log("Error: " + request.error);
        }
    }
}

