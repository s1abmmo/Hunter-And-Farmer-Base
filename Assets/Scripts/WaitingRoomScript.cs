using Assets.Models;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Assets.Models.Dtos;
using System.Text;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class WaitingRoomScript : MonoBehaviour
{
    public Image ImageReadyButton;
    public TextMeshProUGUI TextReadyButton;

    public Sprite OrangeImage;
    public Sprite BlueImage;

    public SlotScript Slot1;
    public SlotScript Slot2;
    public SlotScript Slot3;
    public SlotScript Slot4;
    public SlotScript Slot5;
    private bool ready = false;

    private string myPlayerToken;
    private string myRoomId;
    private bool isHost = false;

    private string api { get; set; } = "http://192.168.55.156";

    private static readonly HttpClient client = new HttpClient();

    // Start is called before the first frame update
    void Start()
    {
        myPlayerToken = PlayerPrefs.GetString("MyPlayerToken");
        myRoomId = PlayerPrefs.GetString("MyRoomId");
        Debug.Log($"{myPlayerToken} {myRoomId}");

        ImageReadyButton.sprite = BlueImage;
        TextReadyButton.text = "Ready";

        StartCoroutine(RepeatRequest());

    }

    // Update is called once per frame
    void Update()
    {

    }

    public async void ReadyButtonClick()
    {

        if (isHost)
        {
            RequestStart();
            return;
        }

        bool success = await RequestReady();


        if (success)
        {
            ready = !ready;

            if (ready)
            {
                ImageReadyButton.sprite = OrangeImage;
                TextReadyButton.text = "CANCEL";
            }
            else
            {
                ImageReadyButton.sprite = BlueImage;
                TextReadyButton.text = "READY";
            }
        }
    }

    IEnumerator RepeatRequest()
    {
        while (true)
        {
            yield return new WaitForSeconds(1); // Chờ 1 giây trước khi lặp lại
            Task getRequestTask = GetRequest();
            yield return new WaitUntil(() => getRequestTask.IsCompleted);
        }
    }

    private async Task GetRequest()
    {
        try
        {
            string url = $"{api}/api/Room/{myRoomId}";
            Debug.Log(url);
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string jsonResponse = await response.Content.ReadAsStringAsync();
            Debug.LogError(jsonResponse);
            WaitingRoomDto room = JsonConvert.DeserializeObject<WaitingRoomDto>(jsonResponse);

            UpdateUI(room);
        }
        catch (HttpRequestException e)
        {
            Debug.LogError("Request error: " + e.Message);
        }
    }

    private void UpdateUI(WaitingRoomDto room)
    {
        if (room.TokenPlayerAsHost == myPlayerToken)
        {
            SetIsHost();
        }

        // Cập nhật trạng thái sẵn sàng của người chơi
        for (int i = 0; i < room.Players.Count; i++)
        {
            if (i == 0)
            {
                Slot1.SetTokenPlayer(room.Players[0].TokenPlayer);
                if (room.TokenPlayerAsHost == room.Players[0].TokenPlayer)
                {
                    Slot1.SetIsHost();
                    isHost = true;
                }
                else
                {
                    if (room.Players[0].IsReady)
                    {
                        Slot1.ShowReadyText();
                    }
                    else
                    {
                        Slot1.HideReadyText();
                    }
                }
            }

            if (i == 1)
            {
                Slot2.SetTokenPlayer(room.Players[1].TokenPlayer);
                if (room.TokenPlayerAsHost == room.Players[1].TokenPlayer)
                {
                    Slot2.SetIsHost();
                }
                else
                {
                    if (room.Players[1].IsReady)
                    {
                        Slot2.ShowReadyText();
                    }
                    else
                    {
                        Slot2.HideReadyText();
                    }
                }
            }

            if (i == 2)
            {
                Slot3.SetTokenPlayer(room.Players[2].TokenPlayer);
                if (room.TokenPlayerAsHost == room.Players[2].TokenPlayer)
                {
                    Slot3.SetIsHost();
                }
                else
                {
                    if (room.Players[2].IsReady)
                    {
                        Slot3.ShowReadyText();
                    }
                    else
                    {
                        Slot3.HideReadyText();
                    }
                }
            }

            if (i == 3)
            {
                Slot4.SetTokenPlayer(room.Players[3].TokenPlayer);
                if (room.TokenPlayerAsHost == room.Players[3].TokenPlayer)
                {
                    Slot4.SetIsHost();
                }
                else
                {
                    if (room.Players[3].IsReady)
                    {
                        Slot4.ShowReadyText();
                    }
                    else
                    {
                        Slot4.HideReadyText();
                    }

                }
            }

            if (i == 4)
            {
                Slot5.SetTokenPlayer(room.Players[4].TokenPlayer);
                if (room.TokenPlayerAsHost == room.Players[4].TokenPlayer)
                {
                    Slot5.SetIsHost();
                }
                else
                {
                    if (room.Players[4].IsReady)
                    {
                        Slot5.ShowReadyText();
                    }
                    else
                    {
                        Slot5.HideReadyText();
                    }
                }
            }
        }

        for (int i = room.Players.Count; i < 5; i++)
        {
            if (i == 1)
            {
                Slot2.RemovePlayerToken();
                Slot2.HideReadyText();
            }

            if (i == 2)
            {
                Slot3.RemovePlayerToken();
                Slot3.HideReadyText();
            }

            if (i == 3)
            {
                Slot4.RemovePlayerToken();
                Slot4.HideReadyText();
            }

            if (i == 4)
            {
                Slot5.RemovePlayerToken();
                Slot5.HideReadyText();
            }
        }

        if (room.StartTime != null)
        {
            CountDownToPlay(room.StartTime.Value);
        }

    }

    public void SetIsHost()
    {
        ImageReadyButton.sprite = BlueImage;
        TextReadyButton.text = "START";
    }

    private async Task<bool> RequestReady()
    {
        try
        {
            string json = JsonConvert.SerializeObject(
                new SendReadyRequest()
                {
                    PlayerToken = myPlayerToken,
                    IsReady = !ready
                }
                );

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            string url = $"{api}/api/Room/{myRoomId}/SetPlayerReady";
            Debug.Log(url);
            HttpResponseMessage response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (HttpRequestException e)
        {
            Debug.LogError("Request error: " + e.Message);
            return false;
        }
    }

    private async void RequestStart()
    {
        try
        {
            string json = JsonConvert.SerializeObject(
                new StartRequest()
                {
                    HostToken = myPlayerToken,
                }
                );

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            string url = $"{api}/api/Room/{myRoomId}/StartGame";
            Debug.Log(url);
            HttpResponseMessage response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            Debug.LogError(await response.Content.ReadAsStringAsync());
        }
        catch (HttpRequestException e)
        {
            Debug.LogError("Request error: " + e.Message);
        }
    }

    private void CountDownToPlay(DateTime timeStart)
    {
        ImageReadyButton.sprite = OrangeImage;

        if (DateTime.UtcNow > timeStart)
        {
            TextReadyButton.text = "0";
            SceneManager.LoadScene("PlayingScene");
            return;
        }

        int seconds = (int)Math.Round((timeStart - DateTime.UtcNow).TotalSeconds);
        TextReadyButton.text = seconds.ToString();
    }

}
