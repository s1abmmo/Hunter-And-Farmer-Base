using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoomScript : MonoBehaviour
{
    public float perspectiveZoomSpeed = 0.1f;        // Tốc độ zoom cho camera Perspective
    public float orthoZoomSpeed = 0.1f;

    public float MinZoomPerspectiveCamera = 15f;
    public float MaxZoomPerspectiveCamera = 80f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Chỉ sử dụng khi có đúng hai ngón tay chạm vào màn hình.
        if (Input.touchCount == 2)
        {
            // Lưu trữ hai ngón tay chạm vào.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Tìm vị trí của từng ngón tay trong frame trước.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Tính toán khoảng cách giữa các vị trí của từng ngón tay trong frame hiện tại và frame trước.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Tính toán thay đổi khoảng cách giữa các frames.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // Nếu camera sử dụng perspective
            if (GetComponent<Camera>().orthographic)
            {
                // Thay đổi kích thước của camera theo orthographic size.
                GetComponent<Camera>().orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;

                // Đảm bảo orthographic size không âm.
                GetComponent<Camera>().orthographicSize = Mathf.Max(GetComponent<Camera>().orthographicSize, 0.1f);
            }
            else
            {
                // Thay đổi Field of View của camera dựa trên thay đổi khoảng cách.
                GetComponent<Camera>().fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;

                // Đảm bảo field of view trong giới hạn hợp lý.
                GetComponent<Camera>().fieldOfView = Mathf.Clamp(GetComponent<Camera>().fieldOfView, MinZoomPerspectiveCamera, MaxZoomPerspectiveCamera);
            }
        }
    }
}
