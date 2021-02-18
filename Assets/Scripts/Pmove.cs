using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pmove : MonoBehaviour
{
    public float Wait = 0.5f;
    public float MoveSpeed = 0.5f;
    public AnimationCurve Ease;
    public string ColTag = "Player";

    public bool debug = false; //Toggles
    public bool move = true;
    public bool rotate = false;
    public bool return_ = false;
    public bool Playsound = false;

    [SerializeField] private AudioClip[] soundEffects;

    [SerializeField] private Vector3[] MoveToPoints;
    [SerializeField] private Vector3[] RotatePoints;

    private AudioSource ASource;

    void Start()
    {
        if (Playsound)
            ASource = gameObject.GetComponent<AudioSource>();
    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag(ColTag))
        {
            if (debug)
            {
                print("collision with " + collision.name);
            }
            if (move)
            {
                LeanTween.moveLocal(gameObject, MoveToPoints[1], MoveSpeed).setEase(Ease);
                if (return_)
                {
                    StartCoroutine(returnpos(0));
                }
            }
            else if (rotate)
            {
                
                LeanTween.rotateLocal(gameObject, RotatePoints[1], MoveSpeed).setEase(Ease);
                if(return_)
                {
                    StartCoroutine(returnpos(1));
                }
            }
            if (Playsound)
            {
                ASource.clip = soundEffects[0];
                ASource.Play();
            }
        }
    }
    private IEnumerator returnpos(int type)
    {
        yield return new WaitForSeconds(Wait);
        if(type == 0)
        {
            LeanTween.moveLocal(gameObject, MoveToPoints[0], MoveSpeed).setEase(Ease);
        }
        else
        {
            LeanTween.rotateLocal(gameObject, RotatePoints[0], MoveSpeed).setEase(Ease);
        }
        if (Playsound)
        {
            ASource.clip = soundEffects[1];
            ASource.Play();
        }
    }
}
