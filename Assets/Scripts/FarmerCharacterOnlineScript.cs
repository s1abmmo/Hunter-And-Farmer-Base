using Assets.Enums;
using Assets.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class FarmerCharacterOnlineScript : MonoBehaviour
{
    public VariableJoystick variableJoystick;
    public Camera cam;
    private Animator anim;
    public GameObject TreePrefab;
    public GameObject MyLight;
    private DateTime lastSend;

    private string myPlayerId = "1";
    private string myRoomId = "room1";

    public List<Player> anotherPlayers = new List<Player>();
    private Player myPlayer;

    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;

    // Set the server IP and port
    public string serverIP = "127.0.0.1";
    public int serverPort = 11000;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        cam.transform.position = new Vector3(this.transform.position.x, 3, this.transform.position.z - 2.5f);
        MyLight.transform.position = new Vector3(transform.position.x, 2, transform.position.z);
        lastSend = DateTime.Now;
        udpClient = new UdpClient();
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);

        string joinMessage = "JOIN|room1|";
        byte[] joinData = Encoding.UTF8.GetBytes(joinMessage);
        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, 11000);
        udpClient.SendAsync(joinData, joinData.Length, serverEndPoint);

    }

    // Update is called once per frame
    void Update()
    {
        cam.transform.position = new Vector3(this.transform.position.x, 3, this.transform.position.z - 2.5f);
        MyLight.transform.position = new Vector3(transform.position.x, 2, transform.position.z);

        // Check for incoming messages
        if (udpClient.Available > 0)
        {
            byte[] data = udpClient.Receive(ref remoteEndPoint);
            string message = Encoding.UTF8.GetString(data);
            Debug.Log("Received: " + message);

            string[] _message = message.Split("|");

            if (_message[0] == "RUNNING")
            {
                anim.SetBool("Running", true);
            }
            else
            {
                anim.SetBool("Running", false);
            }

            string[] _message2 = _message[1].Split(",");
            Vector3 newPosition = new Vector3(float.Parse(_message2[1]), 0, float.Parse(_message2[0]));
            transform.position = newPosition;

            Debug.Log($"{float.Parse(_message2[1])} {float.Parse(_message2[0])} {float.Parse(_message[2])}");

            myPlayer.Angle = float.Parse(_message[2]);
            transform.transform.rotation = Quaternion.Euler(0, myPlayer.Angle, 0);

        }

    }

    public void FixedUpdate()
    {
        if (myPlayer.PlayerState == PlayerState.RUNNING)
        {
            Vector3 _direction = new Vector3(variableJoystick.Horizontal, 0.0f, variableJoystick.Vertical);

            //float radians = Angle.Value * Mathf.Deg2Rad;

            //// Tính toán vector hướng dựa trên góc
            //Vector3 _direction = new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians));

            transform.Translate(_direction * myPlayer.Speed * Time.deltaTime, Space.World);
        }

        if ((DateTime.Now - lastSend).Milliseconds < 30)
        {
            return;
        }

        // Lấy hướng di chuyển từ joystick
        Vector3 direction = new Vector3(variableJoystick.Horizontal, 0.0f, variableJoystick.Vertical);

        // Kiểm tra nếu có di chuyển (joystick không ở vị trí giữa)
        if (direction != Vector3.zero)
        {

            Quaternion rotation = Quaternion.LookRotation(direction);
            SendMoveCommand(rotation);
        }
        else
        {
            SendStopMoveCommand();
        }

    }

    private void SendMoveCommand(Quaternion rotation)
    {
        string message = $"MOVE|{myRoomId}|{Mathf.RoundToInt(rotation.eulerAngles.y)}";
        Debug.Log(message);
        byte[] data = Encoding.UTF8.GetBytes(message);
        udpClient.Send(data, data.Length, remoteEndPoint);
    }

    private void SendStopMoveCommand()
    {
        string message = $"STOPMOVE|{myRoomId}|";
        byte[] data = Encoding.UTF8.GetBytes(message);
        udpClient.Send(data, data.Length, remoteEndPoint);
    }

}
