using UnityEngine;
using System.Collections;

public class GameCountdown : MonoBehaviour {

    public float timeLeft = 180;
    public bool countingDown;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (countingDown)
        {
            timeLeft -= Time.deltaTime;
        }
	}
}
