using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using TMPro;

// Các lớp bổ trợ để xử lý dữ liệu JSON
[System.Serializable]
public class AuthData
{
    public string username;
    public string password;
}

[System.Serializable]
public class AuthResponse
{
    public string token;
    public string username;
    public string message;
}

public class AuthManager : MonoBehaviour
{
    [Header("API Configuration")]
    public string apiBaseUrl = "http://192.168.100.23:5000/api/auth";

    [Header("UI References")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI messageText;
    
    [Header("Buttons")]
    public GameObject btnSubmitLogin;
    public GameObject btnGoToRegister;
    public GameObject btnSubmitRegister;
    public GameObject btnBackToLogin;

    [Header("Panels")]
    public GameObject loginRegisterPanel;
    public GameObject lobbyPanel;

    private void Start()
    {
        // Khởi tạo trạng thái ban đầu: Hiện Login, ẩn Lobby
        SwitchToLoginState();
        if (lobbyPanel != null) lobbyPanel.SetActive(false);
    }

    // --- HÀM CHUYỂN ĐỔI GIAO DIỆN ---

    public void SwitchToLoginState()
    {
        btnSubmitLogin.SetActive(true);
        btnGoToRegister.SetActive(true);
        
        btnSubmitRegister.SetActive(false);
        btnBackToLogin.SetActive(false);
        
        messageText.text = "LOGIN";
        messageText.color = Color.white;
    }

    public void SwitchToRegisterState()
    {
        btnSubmitLogin.SetActive(false);
        btnGoToRegister.SetActive(false);
        
        btnSubmitRegister.SetActive(true);
        btnBackToLogin.SetActive(true);
        
        messageText.text = "CREATE ACCOUNT";
        messageText.color = Color.white;
    }

    // --- HÀM XỬ LÝ SỰ KIỆN NÚT BẤM ---

    public void OnClick_SubmitRegister()
    {
        StartCoroutine(SendAuthRequest("register"));
    }

    public void OnClick_SubmitLogin()
    {
        StartCoroutine(SendAuthRequest("login"));
    }

    // --- LOGIC KẾT NỐI WEB API ---

    IEnumerator SendAuthRequest(string endpoint)
    {
        string user = usernameInput.text.Trim();
        string pass = passwordInput.text.Trim();

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
        {
            SetMessage("Do not leave blank!", Color.red);
            yield break;
        }

        AuthData data = new AuthData { username = user, password = pass };
        string jsonData = JsonUtility.ToJson(data);

        using (UnityWebRequest request = new UnityWebRequest($"{apiBaseUrl}/{endpoint}", "POST"))
        {
            request.timeout = 7;
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            SetMessage("Connecting to server...", Color.yellow);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                AuthResponse response = JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);

                if (endpoint == "register")
                {
                    SetMessage("Account Created!", Color.green);
                    passwordInput.text = "";
                    SwitchToLoginState();
                }
                else
                {
                    PlayerPrefs.SetString("CurrentPlayerName", response.username);
                    PlayerPrefs.SetString("JWT_Token", response.token);
                    PlayerPrefs.Save();

                    SetMessage("Login Successful!", Color.green);
                    yield return new WaitForSeconds(0.5f);
                    loginRegisterPanel.SetActive(false);
                    lobbyPanel.SetActive(true);
                }
            }
            else
            {
                // --- ĐOẠN SỬA LỖI MESSAGE Ở ĐÂY ---
                string rawResponse = request.downloadHandler.text;

                if (!string.IsNullOrEmpty(rawResponse))
                {
                    try
                    {
                        // Parse JSON để lấy trường "message"
                        AuthResponse errorData = JsonUtility.FromJson<AuthResponse>(rawResponse);
                        SetMessage(errorData.message, Color.red);
                    }
                    catch
                    {
                        // Phòng trường hợp Server trả về lỗi không phải định dạng JSON
                        SetMessage("Server Error: " + request.error, Color.red);
                    }
                }
                else
                {
                    // Trường hợp mất mạng hoặc Server sập hoàn toàn
                    SetMessage("Connection Failed!", Color.red);
                }
            }
        }
    }

    private void SetMessage(string text, Color color)
    {
        messageText.text = text;
        messageText.color = color;
    }
}