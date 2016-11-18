using UnityEngine;
using System.Collections;
using SocketIO;

public class GameManager : MonoBehaviour
{
    /* Keep track of current game state
       0 = Lobby (Open and close connections)
       1 = Game has started 
    */
    public GameObject leftHand;
    public GameObject rightHand;
    public Material handReady;
    public Material handNotReady;
    int CurrentGameState = 0;
    int PlayersConnected = 0;
    bool leftReady = false;
    bool rightReady = false;
    bool hasSentPrep = false;

    private SocketIOComponent socket;

    // Use this for initialization
    void Start()
    {
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();
        socket.On("error", SocketError);
        socket.On("close", SocketClose);
        socket.On("lobby_update", LobbyUpdate);
    }

    // Update is called once per frame
    void Update()
    {
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
            if(rightReady && leftReady && PlayersConnected > 0)
            {
                //Start game here
                CurrentGameState = 1;

            }
        }
        
    }


    public void SocketError(SocketIOEvent e)
    {
        Debug.Log("[SocketIO] Error received: " + e.name + " " + e.data);
    }

    public void SocketClose(SocketIOEvent e)
    {
        Debug.Log("[SocketIO] Close received: " + e.name + " " + e.data);
    }
    public void LobbyUpdate(SocketIOEvent e)
    {
        Debug.Log("[SocketIO] Lobby Update: " + e.name + " " + e.data);
    }
}
