using UnityEngine;
using Mirror;

public class InGameMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseMenuPanel;
    public GameObject lobbyPanel;

    // THÊM DÒNG NÀY: Biến static để báo cho toàn bộ game biết Menu đang mở hay đóng
    public static bool isMenuOpen = false;

    void Start()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        isMenuOpen = false; // Đảm bảo lúc mới vào game là false
    }

    void Update()
    {
        if (NetworkClient.isConnected || NetworkServer.active)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (pauseMenuPanel.activeSelf) ResumeGame();
                else ToggleMenu();
            }
        }
    }

    public void ToggleMenu()
    {
        pauseMenuPanel.SetActive(true);
        isMenuOpen = true; // Đánh dấu là đã mở menu
    }

    public void ResumeGame()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        isMenuOpen = false; // Đánh dấu là đã đóng menu
    }

    public void LeaveRoom()
    {
        if (NetworkServer.active && NetworkClient.isConnected) NetworkManager.singleton.StopHost();
        else if (NetworkClient.isConnected) NetworkManager.singleton.StopClient();

        pauseMenuPanel.SetActive(false);
        isMenuOpen = false; // Reset lại biến này

        if (lobbyPanel != null) lobbyPanel.SetActive(true);
    }

    public void QuitGame()
    {
        LeaveRoom();
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}