using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLogin : MonoBehaviour
{
    public string ip = "localhost";
    public int port = 8021;

    public void Start()
    {
        RealServer server = new RealServer();
        server.start(this.ip, this.port);
    }
}
