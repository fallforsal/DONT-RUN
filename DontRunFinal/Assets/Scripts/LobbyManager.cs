using UnityEngine;
using Mirror;
using Mirror.Discovery; 
using TMPro; // THIẾU DÒNG NÀY LÀ LỖI ĐỎ CẢ ĐÀN

public class LobbyManager : MonoBehaviour
{
    [Header("Object NetworkManager")]
    public NetworkDiscovery networkDiscovery;
    
    [Header("Manual Connection")]
    public TMP_InputField ipInputField;

    // Phải là PUBLIC void thì Unity mới thấy trong danh sách OnClick
    public void OnClick_JoinManual()
    {
        string targetIP = ipInputField.text.Trim();
        
        if (string.IsNullOrEmpty(targetIP))
        {
            targetIP = "localhost"; 
        }

        // Gán IP thủ công vào NetworkManager
        NetworkManager.singleton.networkAddress = targetIP;
        NetworkManager.singleton.StartClient();

        gameObject.SetActive(false);
    }

    public void OnClick_HostGame()
    {
        NetworkManager.singleton.StartHost(); 
        networkDiscovery.AdvertiseServer(); 
        gameObject.SetActive(false); 
    }

    public void OnClick_FindAndJoin()
    {
        networkDiscovery.OnServerFound.AddListener(ConnectToHost);
        networkDiscovery.StartDiscovery(); 
    }

    private void ConnectToHost(ServerResponse info)
    {
        networkDiscovery.StopDiscovery(); 
        NetworkManager.singleton.StartClient(info.uri); 
        gameObject.SetActive(false); 
    }
}