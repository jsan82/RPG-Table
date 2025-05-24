using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private GameObject serverButton;
    [SerializeField] private GameObject hostButton;
    [SerializeField] private GameObject clientButton;

    private void Awake()
    {
        serverButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartServer();
            }
        );
        
        hostButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartHost();
            }
        );
        
        clientButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartClient();
            }
        );
    }
}