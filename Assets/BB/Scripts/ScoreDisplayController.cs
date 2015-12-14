using UnityEngine;
using System.Collections;

public class ScoreDisplayController : MonoBehaviour {

    public GameObject starsAndXs;

    public GameObject x1;
    public GameObject x2;
    public GameObject x3;

    public GameObject star1;
    public GameObject star2;
    public GameObject star3;

    public GameObject zeroStars;

    public void showScore(int score) {
        if (score == 0) {
            zeroStars.SetActive(true);
        } else if (score == 1) {
            star1.SetActive(true);
        } else if (score == 2) {
            star1.SetActive(true);
            star2.SetActive(true);
        } else {
            star1.SetActive(true);
            star2.SetActive(true);
            star3.SetActive(true);
        }
        StartCoroutine(FlickerScoreOn());
    }

    public void showFailures(int failures) {
        if (failures == 1) {
            x1.SetActive(true);
        } else if (failures == 2) {
            x1.SetActive(true);
            x2.SetActive(true);
        } else {
            x1.SetActive(true);
            x2.SetActive(true);
            x3.SetActive(true);
        }
        StartCoroutine(FlickerScoreOn());
    }

    IEnumerator FlickerScoreOn() {
        starsAndXs.SetActive(true);
        yield return new WaitForSeconds(0.15f);
        starsAndXs.SetActive(false);
        yield return new WaitForSeconds(0.15f);
        starsAndXs.SetActive(true);
    }

    public void clear() {
        x1.SetActive(false);
        x2.SetActive(false);
        x3.SetActive(false);
        star1.SetActive(false);
        star2.SetActive(false);
        star3.SetActive(false);
        zeroStars.SetActive(false);
        starsAndXs.SetActive(false);
    }

}
