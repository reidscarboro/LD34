using UnityEngine;
using System.Collections;

public class CustomerController : MonoBehaviour {

    public Sprite face_neutral;
    public Sprite face_fail;
    public Sprite face_zeroStar;
    public Sprite face_oneStar;
    public Sprite face_twoStar;
    public Sprite face_threeStar;

    public GameObject[] hairs;
    public GameObject[] clothes;
    public Vector3[] colors;
    public Vector3[] colors_clothes;
    public GameObject pants;

    public Vector3[] body_colors;
    public GameObject body;

    void Start() {
        GameObject hair = (GameObject)Instantiate(hairs[Random.Range(0, hairs.Length)], transform.position + new Vector3(0, -3, 0), Quaternion.identity);
        hair.transform.parent = transform;
        Vector3 colorVector = colors[Random.Range(0, colors.Length)];
        Color color = new Color(colorVector.x / 255, colorVector.y / 255, colorVector.z / 255);
        hair.GetComponent<SpriteRenderer>().color = color;

        int clothChoice = Random.Range(0, clothes.Length);
        GameObject cloth = (GameObject)Instantiate(clothes[clothChoice], transform.position + new Vector3(0, -3, 0.01f), Quaternion.identity);
        cloth.transform.parent = transform;
        Vector3 colorVector2 = colors_clothes[Random.Range(0, colors_clothes.Length)];
        Color color2 = new Color(colorVector2.x / 255, colorVector2.y / 255, colorVector2.z / 255);
        cloth.GetComponent<SpriteRenderer>().color = color2;
        if (clothChoice > 0) {
            GameObject pant = (GameObject)Instantiate(pants, transform.position + new Vector3(0, -3, 0.01f), Quaternion.identity);
            pant.transform.parent = transform;
        }

        Vector3 colorVector3 = body_colors[Random.Range(0, body_colors.Length)];
        Color color3 = new Color(colorVector3.x / 255, colorVector3.y / 255, colorVector3.z / 255);
        body.GetComponent<SpriteRenderer>().color = color3;
    }

    public void updateFace(int score) {
        if (score == 0) {
            transform.Find("CustomerFace").GetComponent<SpriteRenderer>().sprite = face_zeroStar;
        } else if (score == 1) {
            transform.Find("CustomerFace").GetComponent<SpriteRenderer>().sprite = face_oneStar;
        } else if (score == 2) {
            transform.Find("CustomerFace").GetComponent<SpriteRenderer>().sprite = face_twoStar;
        } else if (score == 3) {
            transform.Find("CustomerFace").GetComponent<SpriteRenderer>().sprite = face_threeStar;
        } else if (score == -1) {
            transform.Find("CustomerFace").GetComponent<SpriteRenderer>().sprite = face_fail;
        }
    }
}
