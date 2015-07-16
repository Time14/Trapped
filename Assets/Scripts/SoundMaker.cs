using UnityEngine;
using System.Collections;


public class SoundMaker : MonoBehaviour
{

    public float volume = 1;
    public bool sound;
    public bool soundOnHit;
    public bool soundAtLocation;
    public Vector3 soundLocation;

    void OnCollisionEnter(Collision col)
    {
        if (soundOnHit)
            Sound.MakeSound((soundAtLocation) ? soundLocation : transform.position, volume);
    }

    void Update()
    {
        if (sound) 
            Sound.MakeSound((soundAtLocation) ? soundLocation : transform.position, volume);
    }
}
