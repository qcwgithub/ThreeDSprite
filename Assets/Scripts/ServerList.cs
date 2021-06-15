using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public class ServerList
{
    public string aaaIp;
    public int aaaPort;
    public string purpose;
    public bool needLogin;
    public bool enterGame;


    public class BoardConfig
    {
        public string title;
        public string content;
        public string buttonText;
        public string buttonAction;
    }
    
    public bool showBoard;
    public BoardConfig board;
}
