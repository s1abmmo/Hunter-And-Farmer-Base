using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomBoxScript : MonoBehaviour
{
    public TextMeshProUGUI countPlayerText;
    public Image Background;
    public Sprite BackgroundOrange;
    public Sprite BackgroundBlue;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateCountPlayer(int waitingPlayersCount)
    {
        if (countPlayerText != null)
        {
            countPlayerText.text = $"{waitingPlayersCount}/5";
        }
        else
        {
            Debug.LogError("countPlayerText is not assigned in RoomBoxScript.");
        }
    }

    public void SetBackgroundOrange()
    {
        if (Background != null)
        {
            Background.sprite = BackgroundOrange;
        }
        else
        {
            Debug.LogError("Background is not assigned in RoomBoxScript.");
        }
    }

    public void SetBackgroundBlue()
    {
        if (Background != null)
        {
            Background.sprite = BackgroundBlue;
        }
        else
        {
            Debug.LogError("Background is not assigned in RoomBoxScript.");
        }
    }


}
