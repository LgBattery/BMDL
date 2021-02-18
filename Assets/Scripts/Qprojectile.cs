using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Timeline;

public class Qprojectile : MonoBehaviour
{
    public PlayerAbilities parent;
    public GameObject HitParticles;

    private Transform Target = null;
    private bool qAb = false;
    private bool sent = false;

    public float wait;
    public float speed;
    
    private void Start()
    {
        gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * speed);
        StartCoroutine(waitForNull());
        transform.parent = null;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("Creature"))
        {
            qAb = true;
            Target = collision.transform;
            Target.GetComponent<CreatureControl>().DoDamage(50);
            sendInfoHome(false);
            parent.hitQ();
            parent.Mark();
            GameObject dest = Instantiate(HitParticles, collision.GetContact(0).point, new Quaternion(0,0,0,1));
            Destroy(dest, 1f);

        }
        else if (!sent)
        {
            sendInfoHome(true);
        }
        Destroy(gameObject);
    }
    void sendInfoHome(bool firstCast)
    {
        parent.qHit = Target;
        parent.qAb = qAb;
        parent.qFirstCast = firstCast;
        sent = true; 
    }
    private IEnumerator waitForNull()
    {
        yield return new WaitForSeconds(wait + 1);
        if (!sent)
        {
            sendInfoHome(true);
        }
        Destroy(gameObject);
    }
}
