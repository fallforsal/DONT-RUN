using UnityEngine;
using Mirror;
using Unity.Cinemachine; // Nhớ dùng đúng namespace của Cinemachine 3.x

public class LocalCameraSetup : NetworkBehaviour
{
    private CinemachineCamera cam;

    public override void OnStartLocalPlayer()
    {
        cam = FindAnyObjectByType<CinemachineCamera>();
        SetCameraTarget(this.transform); // Mặc định bám theo chính mình
    }

    // Cung cấp hàm public để GameOverManager có thể gọi
    public void SetCameraTarget(Transform target)
    {
        if (cam != null && target != null)
        {
            cam.Target.TrackingTarget = target;
            cam.Follow = target;
            Debug.Log($"[Camera] Đã chuyển mục tiêu sang: {target.name}");
        }
    }
}