using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AudioM : MonoBehaviour {

    private static Dictionary<string, AudioClip> audios = null;
    public static void Play(string name, float volume = 1f) {
        if (audios == null) {
            audios = new();
            Resources.LoadAll<AudioClip>("").ToList().ForEach(a => audios.Add(a.name, a));
        }
        AudioSource.PlayClipAtPoint(audios[name], Camera.allCameras[0].transform.position, volume);
    }

    private void Awake() {
        audios = null;
    }

}
