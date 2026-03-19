using UnityEngine;

public class LobbyUIBridge : MonoBehaviour
{
    public static PlayerIdentity localPlayerIdentity;

    public void OnClickReadyButton()
    {
        if (localPlayerIdentity != null)
        {
            localPlayerIdentity.ToggleReady();
        }
        else
        {
            Debug.LogError("Chưa tìm thấy Local Player Identity!");
        }
    }
}