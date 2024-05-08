using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class FarmerCharacterScript : MonoBehaviour
{
    public float speed = 1f;
    public VariableJoystick variableJoystick;
    public Camera cam;
    private Animator anim;
    private float someThreshold = 0.15f;
    public GameObject TreePrefab;
    List<Vector3> treePositions = new List<Vector3>();
    private List<GameObject> trees = new List<GameObject>();
    public GameObject MyLight;
    private float[] angles = { 5, -5, 10, -10, 15, -15, 20, -20, 25, -25, 30, -30, 35, -35, 40, -40, 45, -45 };
    private Vector3? target;
    public GameObject GocCayPrefab;
    private List<GameObject> TreeWaitToDestroy = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        for (int j = 0; j < 10; j++)
        {
            for (int i = 0; i < 10; i++)
            {
                if (j % 2 == 0)
                {
                    treePositions.Add(new Vector3(i * 0.25f, 0, j * 0.2f));
                }
                else
                {
                    treePositions.Add(new Vector3(i * 0.25f + 0.125f, 0, j * 0.2f));
                }
            }
        }

        foreach (var treePosition in treePositions)
        {
            GameObject newTree = Instantiate(TreePrefab);
            newTree.transform.position = treePosition;
            trees.Add(newTree);
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

        if (target != null)
        {
            Vector3 targetDirection = CalculateDirection(transform.position, target.Value);
            targetDirection.y = 0;
            transform.rotation = Quaternion.LookRotation(targetDirection);
            Debug.Log($"targetDirection {targetDirection}");
            if (Vector3.Distance(transform.position, target.Value) < someThreshold + 0.01f)
            {
                GameObject tree = trees.Find(tree => Vector3.Equals(tree.transform.position, target.Value));
                trees.Remove(tree);
                Debug.Log($"tree {tree.transform.position}");
                Transform childTransform = tree.transform.Find("tree");
                Animator childAnimator = childTransform.GetComponent<Animator>();
                if (childAnimator != null)
                {
                    childAnimator.SetTrigger("CutDown");
                }

                target = null;
                anim.SetTrigger("CutTree");

                AddGocCay(tree.transform.position);
                TreeWaitToDestroy.Add(tree);

            }
            else
            {
                transform.Translate(targetDirection * speed * Time.deltaTime, Space.World);
                anim.SetBool("Running", true);
            }
        }

        DestroyTree();

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

    public void CutTree()
    {
        target = FindNearestTreePosition(transform.position);
        Debug.Log($"target {target}");
    }

    public Vector3 FindNearestTreePosition(Vector3 characterPosition)
    {
        Vector3 nearestPosition = Vector3.zero;
        float minDistance = float.MaxValue;

        foreach (Vector3 treePosition in treePositions)
        {
            float distance = Vector3.Distance(characterPosition, treePosition);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPosition = treePosition;
            }
        }

        return nearestPosition;
    }

    Vector3 CalculateDirection(Vector3 positionA, Vector3 positionB)
    {
        Vector3 direction = positionB - positionA;
        return direction.normalized; // Trả về vector đơn vị chỉ hướng từ A đến B
    }

    private void DestroyTree()
    {
        for (int i = TreeWaitToDestroy.Count - 1; i >= 0; i--)
        {
            Transform childTransform = TreeWaitToDestroy[i].transform.Find("tree");
            Animator childAnimator = childTransform.GetComponent<Animator>();
            Debug.Log($"GetCurrentAnimatorStateInfo {childAnimator.GetCurrentAnimatorStateInfo(0).IsName("CutDown")}");

            if (childAnimator.GetCurrentAnimatorStateInfo(0).IsName("CutDown"))
            {
                RemoveTreeVector(TreeWaitToDestroy[i].transform.position);
                Destroy(TreeWaitToDestroy[i]);
                TreeWaitToDestroy.RemoveAt(i);
            }
        }
    }

    private void AddGocCay(Vector3 position)
    {
        GameObject newGocCay = Instantiate(GocCayPrefab);
        newGocCay.transform.position = position;
    }

    private void RemoveTreeVector(Vector3 position)
    {
        int index = treePositions.FindIndex(treePosition => Vector3.Equals(treePosition, position));
        treePositions.RemoveAt(index);
    }

}
