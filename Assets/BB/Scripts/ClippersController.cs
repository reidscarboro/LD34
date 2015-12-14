using UnityEngine;
using System.Collections;

public class ClippersController : MonoBehaviour {

    public Vector3 initialPosition;

    public float speed = 0.025f;
    public float turnAngle = 5; //degrees
    public float angle = 90;

    private Vector2 position = new Vector2();

    void Start() {
        transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
    }

	public void move() { 

        if (Input.GetKey("left") && Input.GetKey("right")) {
            //do nothing for now
        } else if (Input.GetKey("left")) {
            angle += turnAngle;
        } else if (Input.GetKey("right")) {
            angle -= turnAngle;
        }

        Vector2 moveVector = MathUtil.polarToCartesian(new Vector2(speed, angle));
        transform.position += new Vector3(moveVector.x, moveVector.y, 0);
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
   
    }

    public Vector2 getPosition() {
        position.x = transform.position.x;
        position.y = transform.position.y;
        return position;
    }

    public void enable() {
        angle = 90;
        transform.position = initialPosition;
        transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
        gameObject.SetActive(true);
    }

    public void disable() {
        gameObject.SetActive(false);
    }
}
