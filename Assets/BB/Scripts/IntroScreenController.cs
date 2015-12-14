using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class IntroScreenController : MonoBehaviour {

    public TextMesh yesterday;
    public TextMesh best;

    public GameObject sign_closed;
    public GameObject sign_open;

    public GameObject keyboardLeft;
    public GameObject keyboardRight;
    public int keyboardIndicatorInterval = 20; //flip every 20 frames
    public int frameCounter = 0;

    private bool progressing = false;

	// Use this for initialization
	void Start () {
        yesterday.text = PlayerPrefs.GetInt("yesterday").ToString();
        best.text = PlayerPrefs.GetInt("best").ToString();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)) {
            progressing = true;
            sign_closed.SetActive(false);
            sign_open.SetActive(true);
            StartCoroutine(Progress());
            disableKeyboardIndicator();
        }
        if (progressing) {
            Camera.main.orthographicSize -= 0.005f;
        } else {
            frameCounter++;
            if (frameCounter > keyboardIndicatorInterval) {
                frameCounter = 0;
                switchKeyboardIndicator();
            }
        }
    }

    public void switchKeyboardIndicator() {
        if (keyboardLeft.activeInHierarchy) {
            keyboardLeft.SetActive(false);
            keyboardRight.SetActive(true);
        } else {
            keyboardLeft.SetActive(true);
            keyboardRight.SetActive(false);
        }
    }

    public void disableKeyboardIndicator() {
        keyboardLeft.SetActive(false);
        keyboardRight.SetActive(false);
    }

    IEnumerator Progress() {
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene("game");
    }
}
