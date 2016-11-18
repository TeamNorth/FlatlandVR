using UnityEngine;
using System.Collections;
using SocketIO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class GameManager : MonoBehaviour
{
    /* Keep track of current game state
       0 = Lobby (Open and close connections)
       1 = Game has started 
    */
    public GameObject leftHand;
    public GameObject rightHand;
    public GameObject leftHandText;
    public GameObject rightHandText;
    public GameObject player;
    public Material handReady;
    public Material handNotReady;
    public float tableHeight;
    public float radius;
    int CurrentGameState = 0;
    bool leftReady = false;
    bool rightReady = false;
    bool sentPrep = false;
    bool isOpen = false;
    List<string> ConnectedPlayers = new List<string>(); 

    private SocketIOComponent socket;

    // Use this for initialization
    void Start()
    {
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();
        socket.On("error", SocketError);
        socket.On("close", SocketClose);
        socket.On("open", SocketOpen);
        //socket.On("lobby_update", LobbyUpdate);
        //socket.On("vr_attempt", VRAttempt);
        //socket.On("player_update", GameUpdate);
        //socket.On("prep_resp", resp);
    }

    // Update is called once per frame
    void Update()
    {

        if (!sentPrep && isOpen)
        {
            socket.Emit("prep");
            sentPrep = true;

        }
        if (CurrentGameState == 0)
        {
            //Check Left hand
            if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch) >= 0.3 && OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch) >= 0.3)
            {
                if (!leftReady)
                {
                    //Change texture
                    leftHand.GetComponent<Renderer>().material = handReady;
                    leftReady = true;
                }
            }
            else
            {
                if (leftReady)
                {
                    leftHand.GetComponent<Renderer>().material = handNotReady;
                    leftReady = false;
                }
            }
            //Check Right hand
            if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch) >= 0.3 && OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) >= 0.3)
            {
                if (!rightReady)
                {
                    //Change texture
                    rightHand.GetComponent<Renderer>().material = handReady;
                    rightReady = true;
                }
            }
            else
            {
                if (rightReady)
                {
                    rightHand.GetComponent<Renderer>().material = handNotReady;
                    rightReady = false;
                }
            }
            //Check if game is ready to start!
            if(rightReady && leftReady /*&& ConnectedPlayers.Count > 0*/)
            {
                //Start game here
                // CurrentGameState = 1;
                //Tell server to start the game!
                //socket.Emit("");
                Debug.Log("Start game");
                CurrentGameState = 1;
            }
        }
        if(CurrentGameState == 1)
        {
            
            RootBlockers b = new RootBlockers();
            Blocker rightB = new Blocker();
            rightB.coord = new List<float>();
            rightB.coord.Add(leftHand.transform.position.x);
            rightB.coord.Add(leftHand.transform.position.z);
            rightB.coord.Add(leftHand.transform.position.y - tableHeight);
            rightB.radius = radius;
            b.blockers = new List<Blocker>();
            b.blockers.Add(rightB);
            Blocker leftB = new Blocker();
            leftB.coord = new List<float>();
            leftB.coord.Add(rightHand.transform.position.x);
            leftB.coord.Add(rightHand.transform.position.z);
            leftB.coord.Add(rightHand.transform.position.y - tableHeight);
            leftB.radius = radius;
            b.blockers.Add(leftB);
            string jsonToSend = JsonConvert.SerializeObject(b);
            JSONObject send = new JSONObject(jsonToSend);
            socket.Emit("vr_blocker_update", send);
            Debug.Log(send.ToString());
            
        }
    }


    public void SocketError(SocketIOEvent e)
    {
        Debug.Log("[SocketIO] Error received: " + e.name + " " + e.data.ToString());
    }

    public void SocketClose(SocketIOEvent e)
    {
        Debug.Log("[SocketIO] Close received: " + e.name + " " + e.data.ToString());
    }

    public void resp(SocketIOEvent e)
    {
        Debug.Log("[SocketIO] Game Start: ");
        socket.Emit("game_start");
    }
    public void LobbyUpdate(SocketIOEvent e)
    {
        Debug.Log("[SocketIO] Lobby Update: " + e.name + " " + e.data);
        LobbyObject r = JsonConvert.DeserializeObject<LobbyObject>(e.data.ToString());
        ConnectedPlayers = r.lobby;
        if(ConnectedPlayers.Count > 0)
        {
            leftHandText.GetComponent<Renderer>().enabled = true;
            leftHandText.GetComponent<TextMesh>().text = "Grab both cubes to start";
            rightHandText.GetComponent<TextMesh>().text = "" + ConnectedPlayers.Count + " Player(s) connected";
        }
        else
        {
            leftHandText.GetComponent<Renderer>().enabled = false;
            rightHandText.GetComponent<TextMesh>().text = "Waiting for Players";
        }
    }

    public void VRAttempt(SocketIOEvent e)
    {
        socket.Emit("vr_connect");
    }

    public void GameUpdate(SocketIOEvent e)
    {
        //Get shit
        Debug.Log("[SocketIO] Game Update received: " + e.name + " " + e.data.ToString());
    }

    public void SocketOpen(SocketIOEvent e)
    {
        socket.On("lobby_update", LobbyUpdate);
        socket.On("vr_attempt", VRAttempt);
        socket.On("player_update", GameUpdate);
        socket.On("prep_resp", resp);
        Debug.Log("[SocketIO] Open: " + e.name + " " + e.data);
        //socket.Emit("test");
        isOpen = true;
    }

    public class LobbyObject
    {
        public List<string> lobby { get; set; }
    }

    public class Blocker
    {
        public float radius { get; set; }
        public List<float> coord { get; set; }
    }

    public class RootBlockers
    {
        public List<Blocker> blockers { get; set; }
    }

}
