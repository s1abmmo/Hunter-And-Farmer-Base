using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MyCharacterScript : MonoBehaviour
{
    public float speed = 1f;
    public VariableJoystick variableJoystick;
    public Camera cam;
    private Animator anim;
    private float someThreshold = 0.2f;
    public GameObject TreePrefab;
    List<Vector3> treePositions = new List<Vector3>();
    public GameObject MyLight;
    private float[] angles = { 5, -5, 10, -10, 15, -15, 20, -20, 25, -25, 30, -30, 35, -35, 40, -40, 45, -45 };

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        for (int i = 0; i < 10; i++)
        {
            treePositions.Add(new Vector3(i * 0.25f, 0.15f, 0));
        }

        for (int i = 0; i < 10; i++)
        {
            treePositions.Add(new Vector3(i * 0.25f, 0.15f, 0.4f));
        }

        foreach (var treePosition in treePositions)
        {
            GameObject newTree = Instantiate(TreePrefab);
            newTree.transform.position = treePosition;
        }

    }

    // Update is called once per frame
    void Update()
    {
        cam.transform.position = new Vector3(this.transform.position.x, 3, this.transform.position.z - 2.5f);
        MyLight.transform.position = new Vector3(transform.position.x, 2, transform.position.z);
    }

    public void FixedUpdate()
    {
        // Lấy hướng di chuyển từ joystick
        Vector3 direction = new Vector3(variableJoystick.Horizontal, 0.0f, variableJoystick.Vertical);

        // Kiểm tra nếu có di chuyển (joystick không ở vị trí giữa)
        if (direction != Vector3.zero)
        {

            // Quay mặt nhân vật theo hướng di chuyển
            transform.rotation = Quaternion.LookRotation(direction);

            Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;

            // Kiểm tra để ngăn di chuyển vào vị trí cây
            foreach (Vector3 treePosition in treePositions)
            {
                if (Vector3.Distance(newPosition, treePosition) < someThreshold)
                {
                    anim.SetBool("Running", false);

                    foreach (float angle in angles)
                    {
                        Debug.Log($"quay goc {angle}");
                        Quaternion rotation = Quaternion.Euler(0, angle, 0);
                        Vector3 rotatedDirectionCCW = rotation * direction;
                        if (XoayGocKhac(rotatedDirectionCCW))
                        {
                            Debug.Log($"trung goc {angle}");
                            break;
                        }
                    }

                    return; // Ngừng di chuyển nếu quá gần cây
                }
            }

            // Di chuyển nhân vật dựa trên hướng và tốc độ, đảm bảo mượt mà theo thời gian
            transform.Translate(direction * speed * Time.deltaTime, Space.World);

            anim.SetBool("Running", true);

        }
        else
        {
            anim.SetBool("Running", false);
        }
    }

    private bool XoayGocKhac(Vector3 direction)
    {
        // Quay mặt nhân vật theo hướng di chuyển
        transform.rotation = Quaternion.LookRotation(direction);

        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;

        // Kiểm tra để ngăn di chuyển vào vị trí cây
        foreach (Vector3 treePosition in treePositions)
        {
            if (Vector3.Distance(newPosition, treePosition) < someThreshold)
            {
                anim.SetBool("Running", false);
                return false; // Ngừng di chuyển nếu quá gần cây
            }
        }

        // Di chuyển nhân vật dựa trên hướng và tốc độ, đảm bảo mượt mà theo thời gian
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        anim.SetBool("Running", true);
        return true;
    }

}
