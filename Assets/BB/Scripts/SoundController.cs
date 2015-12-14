using UnityEngine;
using System.Collections;

public class SoundController : MonoBehaviour {

    public static SoundController instance;

    public AudioClip star;
    public AudioClip zeroStar;
    public AudioClip fail;

    public AudioSource audioSource;

    public float starSoundSpacer = 0.25f;

    void Start() {
        instance = this;
    }

	public static void playScore(int score) {
        if (score < 0) {
            instance.audioSource.clip = instance.fail;
            instance.audioSource.Play();
        } else if (score == 0) {
            instance.audioSource.clip = instance.zeroStar;
            instance.audioSource.Play();
        } else {
            instance.audioSource.clip = instance.star;
            instance.StartCoroutine(instance.PlayMultiStar(score));
        }
    }

    IEnumerator PlayMultiStar(int score) {
        for (int i = 0; i < score; i++) {
            instance.audioSource.Play();
            yield return new WaitForSeconds(starSoundSpacer);
        }
    }
}
