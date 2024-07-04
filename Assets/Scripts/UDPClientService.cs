using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using System.Text;

public class UDPClientService : MonoBehaviour
{
    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;

    // Set the server IP and port
    public string serverIP = "127.0.0.1";
    public int serverPort = 11000;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        udpClient = new UdpClient();
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);

        // Send a test message
        SendMessage1("Hello, Server!");

        // Join the room
        string joinMessage = "JOIN|room1|";
        byte[] joinData = Encoding.UTF8.GetBytes(joinMessage);
        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, 11000);
        udpClient.SendAsync(joinData, joinData.Length, serverEndPoint);

    }

    // Update is called once per frame
    void Update()
    {
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
        }
    }

    void SendMessage1(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        udpClient.Send(data, data.Length, remoteEndPoint);
        Debug.Log("Sent: " + message);
    }

    void OnApplicationQuit()
    {
        udpClient.Close();
    }

}
