using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {

    public float cameraSpeed = 0.1f;
    public Vector3[] cameraPositions;
    /*
    public enum GameState {
        INITIAL,                (0, 0,  -20)
        REQUEST,                (0, 12, -8)
        PRE_CLIPPING,           (0, -1, -4)
        CLIPPING,               (0, -1, -4)
        REACTION,               (0, 4,  -12)
        SCORE,                  (0, 4,  -12)
        TRANSITION              (0, 0,  -20)
    }
    */

    private Vector3 toLocation = new Vector3();


    void Update() {
         if (Vector3.Distance(transform.position, toLocation) > 0.001) {
            transform.position = Vector3.Lerp(transform.position, toLocation, cameraSpeed);
        }
    }

    public void updatePosition(GameController.GameState gameState) {
        toLocation = cameraPositions[(int)gameState];
    }
}
