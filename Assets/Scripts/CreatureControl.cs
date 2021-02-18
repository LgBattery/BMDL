using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureControl : MonoBehaviour
{
    public int health = 1200;
    public GameObject body;
    public AudioSource ASource;

    private bool QMarked;
    public bool Marked_;

    [SerializeField] private AudioClip[] SFX;    

    Coroutine LastRoutine;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Marked_ && !QMarked)
        {
            body.layer = 14;
            Marked_ = false;
        }
        else if(!QMarked)
        {
            body.layer = 10;
        }
    }
    public void DoDamage(int Damage)
    {
        health = health - Damage;
    }
    public void Marked(float Time, bool playSound_)
    {
        QMarked = true;
        body.layer = 9;
        LastRoutine = StartCoroutine(ClearMark(Time));
        if(body.GetComponent<MeshRenderer>())
        {
            body.GetComponent<MeshRenderer>().material.SetFloat("MarkedVision", 10000); 
        }
        else
        {
            body.GetComponent<SkinnedMeshRenderer>().material.SetFloat("MarkedVision", 10000);
        }
        
        if (playSound_)
        {
            playSound(0);
        }
    }
    public void slow(float duration, float strength) //FIX ME LATER PLEASE
    {

    }
    public void clearMark()
    {
        QMarked = false;
        body.layer = 10;
        if (body.GetComponent<MeshRenderer>())
        {
            body.GetComponent<MeshRenderer>().material.SetFloat("MarkedVision", 0);
        }
        else
        {
            body.GetComponent<SkinnedMeshRenderer>().material.SetFloat("MarkedVision", 0);
        }
    }
    public void dontClearMark()
    {
        StopCoroutine(LastRoutine);
    }
    private void playSound(int Sound)
    {
        ASource.PlayOneShot(SFX[Sound]);
    }
    private IEnumerator ClearMark(float Time)
    {
        yield return new WaitForSeconds(Time);

        clearMark();
    }
}
