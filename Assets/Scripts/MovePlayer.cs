using UnityEngine;
using System.Collections;

public class MovePlayer : MonoBehaviour {

    /*

    public GameObject player;


    private Vector3 movementVector;
    private CharacterController characterController;
    private float movementSpeed = 8;

    // Use this for initialization
    void Start () {
        float currentposx = player.transform.position.x;
        float currentposz = player.transform.position.z;
        characterController = GetComponent<CharacterController>();
    }
	
	// Update is called once per frame
	void Update () {
        movementVector.x = Input.GetAxis("LeftJoystickX") * movementSpeed;
        movementVector.z = Input.GetAxis("LeftJoystickY") * movementSpeed;
        movementVector.y = 0;
        characterController.Move(movementVector * Time.deltaTime);


    }*/

    public float speed;
    public float tilt;
    public Boundary boundary;

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rigidbody.velocity = movement * speed;

        rigidbody.position = new Vector3
        (
            Mathf.Clamp(rigidbody.position.x, boundary.xMin, boundary.xMax),
            0.0f,
            Mathf.Clamp(rigidbody.position.z, boundary.zMin, boundary.zMax)
        );

        rigidbody.rotation = Quaternion.Euler(0.0f, 0.0f, rigidbody.velocity.x * -tilt);
    }
}
