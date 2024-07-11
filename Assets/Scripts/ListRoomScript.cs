using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Assets.Models;
using TMPro;
using System.Linq;
using System;
using System.Text;
using System.Net;
using Assets.Models.Dtos;

public class ListRoomScript : MonoBehaviour
{
    public GameObject Content;
    public GameObject RoomBox;

    Dictionary<string, int> roomIds = new Dictionary<string, int>();
    Dictionary<string, GameObject> RoomBoxies = new Dictionary<string, GameObject>();

    private static readonly HttpClient client = new HttpClient();
    private readonly string url = "http://192.168.55.156/api/Room/GetAllRooms";
    private readonly string urlPlayerRegister = "http://192.168.55.156/api/Player/Register";
    private readonly string urlCreateRoom = "http://192.168.55.156/api/Room/CreateRoom";

    private string idRoomWantJoin;
    private string myTokenPlayer;

    public GameObject WaitingRoom;

    // Start is called before the first frame update
    void Start()
    {
        WaitingRoom.SetActive(false);
        Time.fixedDeltaTime = 1.0f;
        PlayerRegisterRequest(urlPlayerRegister);
        StartCoroutine(RepeatRequest());
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        foreach (var roomId in roomIds)
        {
            if (!RoomBoxies.Keys.Contains(roomId.Key))
            {
                CreateRoomBox(roomId.Key, -RoomBoxies.Count * 275, roomId.Value);
            }
        }
        var rec = Content.GetComponent<RectTransform>();
        rec.sizeDelta = new Vector2(0, 1800);

    }

    public void OnPointerClick(PointerEventData eventData, string roomId)
    {
        GameObject clickedObject = eventData.pointerPress;
        if (clickedObject != null)
        {
            Debug.Log("Clicked on: " + clickedObject.name + " with ID: " + roomId);
            var roomBoxScript = RoomBoxies[roomId].GetComponent<RoomBoxScript>();
            if (roomBoxScript != null)
            {
                roomBoxScript.SetBackgroundBlue();
                ResetAnotherBackground(roomId);
            }
            else
            {
                Debug.LogError("RoomBoxScript not found on the new RoomBox.");
            }

        }
    }

    public void ResetAnotherBackground(string withoutIdRoom)
    {
        foreach (var roomBox in RoomBoxies)
        {
            if (roomBox.Key == withoutIdRoom)
            {
                continue;
            }

            var roomBoxScript = roomBox.Value.GetComponent<RoomBoxScript>();
            if (roomBoxScript != null)
            {
                roomBoxScript.SetBackgroundOrange();
            }
            else
            {
                Debug.LogError("RoomBoxScript not found on the new RoomBox.");
            }

        }
    }

    private void CreateRoomBox(string idRoom, float y, int waitingPlayersCount)
    {
        // Instantiate the RoomBox
        var newRoomBox = Instantiate(RoomBox);
        newRoomBox.transform.SetParent(Content.transform);

        // Reset the local position and scale
        newRoomBox.transform.localPosition = new Vector3(25, y, 0);
        newRoomBox.transform.localScale = Vector3.one;

        // Gán giá trị cho RoomBoxScript
        var roomBoxScript = newRoomBox.GetComponent<RoomBoxScript>();
        if (roomBoxScript != null)
        {
            roomBoxScript.UpdateCountPlayer(waitingPlayersCount);
        }
        else
        {
            Debug.LogError("RoomBoxScript not found on the new RoomBox.");
        }

        // Thêm Event Trigger để lắng nghe sự kiện click
        EventTrigger trigger = newRoomBox.AddComponent<EventTrigger>();

        //Tạo Entry cho sự kiện Pointer Click
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) =>
        {
            Debug.Log($"{idRoom} clicked");
            OnPointerClick((PointerEventData)data, idRoom);
        });
        trigger.triggers.Add(entry);
        RoomBoxies.Add(idRoom, newRoomBox);

    }

    IEnumerator RepeatRequest()
    {
        while (true)
        {
            yield return new WaitForSeconds(1); // Chờ 1 giây trước khi lặp lại
            Task getRequestTask = GetRequest(url);
            yield return new WaitUntil(() => getRequestTask.IsCompleted);
        }
    }

    private async Task GetRequest(string uri)
    {
        try
        {
            HttpResponseMessage response = await client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            string jsonResponse = await response.Content.ReadAsStringAsync();
            List<RoomDto> rooms = JsonConvert.DeserializeObject<List<RoomDto>>(jsonResponse);

            // Xử lý dữ liệu (ví dụ: log ra console)
            foreach (var room in rooms)
            {
                Debug.Log("Room ID: " + room.Id + ", Name: " + room.Name + $" waiting: {room.WaitingPlayersCount}");
                Debug.Log($"RoomBoxies count before checking: {roomIds.Count}");
                if (!roomIds.Keys.Contains(room.Id))
                {
                    roomIds.Add(room.Id, room.WaitingPlayersCount);
                    Debug.Log($"RoomBoxies count after adding: {roomIds.Count}");
                }
            }

        }
        catch (HttpRequestException e)
        {
            Debug.LogError("Request error: " + e.Message);
        }
    }

    private async void PlayerRegisterRequest(string uri)
    {
        string json = JsonConvert.SerializeObject(new { });
        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PostAsync(uri, content);
        response.EnsureSuccessStatusCode();
        if (response.StatusCode == HttpStatusCode.OK)
        {
            myTokenPlayer = await response.Content.ReadAsStringAsync();
            Debug.Log(myTokenPlayer);
            PlayerPrefs.SetString("MyPlayerToken", myTokenPlayer);
        }
    }

    public async void CreateRoom()
    {
        string json = JsonConvert.SerializeObject(
            new CreateRoomRequestDto()
            {
                Name = "con cac",
                PlayerToken = myTokenPlayer,
            }
            );
        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PostAsync(urlCreateRoom, content);
        response.EnsureSuccessStatusCode();
        idRoomWantJoin = await response.Content.ReadAsStringAsync();

        //chuyển qua màn hình room
        ShowWaitingRoom(idRoomWantJoin);
    }

    public async void JoinRoom()
    {

        if (idRoomWantJoin == null)
        {
            throw new Exception("Need ID Room");
        }

        string json = JsonConvert.SerializeObject(
            new AddPlayerToRoomRequestDto()
            {
                PlayerToken = myTokenPlayer,
            }
            );

        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PostAsync($"http://localhost/api/Room/{idRoomWantJoin}/AddPlayerToRoom", content);
        response.EnsureSuccessStatusCode();
        idRoomWantJoin = await response.Content.ReadAsStringAsync();

        //chuyển qua màn hình room
        ShowWaitingRoom(idRoomWantJoin);
    }

    public void ShowWaitingRoom(string myRoomId)
    {
        PlayerPrefs.SetString("MyRoomId", myRoomId);

        var _waitingRoom = Instantiate(WaitingRoom);
        _waitingRoom.SetActive(true);
        this.gameObject.SetActive(false);
    }

}