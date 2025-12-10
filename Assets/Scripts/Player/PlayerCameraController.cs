using UnityEngine;
public class PlayerCameraController : MonoBehaviour
{
    private void LateUpdate()
    {
        Camera.main.transform.position = transform.position;
    }
}