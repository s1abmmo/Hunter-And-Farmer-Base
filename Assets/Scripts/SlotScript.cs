using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotScript : MonoBehaviour
{
    public string TokenPlayer = null;
    public GameObject ReadyText;
    public Image Image;
    public Sprite PersonImage;
    public Sprite EmptyImage;

    // Start is called before the first frame update
    void Start()
    {
        HideReadyText();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetTokenPlayer(string tokenPlayer)
    {
        TokenPlayer = tokenPlayer;
        Image.sprite = PersonImage;
    }

    public void RemovePlayerToken()
    {
        TokenPlayer = null;
        Image.sprite = EmptyImage;
    }

    public void ShowReadyText()
    {
        ReadyText.SetActive(true);
    }

    public void HideReadyText()
    {
        ReadyText.SetActive(false);
    }

    public void SetIsHost()
    {
        ReadyText.SetActive(true);
        var textMesh= ReadyText.GetComponent<TextMeshProUGUI>();
        if (textMesh != null)
        {
            textMesh.text = "Host";
        }
    }

}
