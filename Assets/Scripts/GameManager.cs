using UnityEngine;
using System.Collections;
using SocketIO;

public class GameManager : MonoBehaviour
{
    /* Keep track of current game state
       0 = Lobby (Open and close connections)
       1 = Game has started 
    */
    int CurrentGameState = 0;
    int PlayersConnected = 0;

    private SocketIOComponent socket;

    // Use this for initialization
    void Start()
    {
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();
        socket.On("error", SocketError);
        socket.On("close", SocketClose);
        socket.On("player_connect", PlayerConnect);
        socket.On("player_dc", PlayerDisconnect);
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void SocketError(SocketIOEvent e)
    {
        Debug.Log("[SocketIO] Error received: " + e.name + " " + e.data);
    }

    public void SocketClose(SocketIOEvent e)
    {
        Debug.Log("[SocketIO] Close received: " + e.name + " " + e.data);
    }
    public void PlayerConnect(SocketIOEvent e)
    {
        
        Debug.Log("[SocketIO] Player connect received: " + e.name + " " + e.data);
    }
    public void PlayerDisconnect(SocketIOEvent e)
    {
        Debug.Log("[SocketIO] Player disconnect received: " + e.name + " " + e.data);
    }
}
