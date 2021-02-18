using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class Ward : MonoBehaviour
{
    [SerializeField] private Collider[] PiecesInRange;
    [SerializeField] private float radius;
    [SerializeField] private Transform ExampleTransform;

    private Rigidbody rb;
    private bool wait;
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Destroy(gameObject, 30);
        rb = gameObject.GetComponent<Rigidbody>();
        StartCoroutine(waitFor());
    }

    // Update is called once per frame
    void Update()
    {
        PiecesInRange = Physics.OverlapSphere(transform.position, radius);
        for(int i = 0; i < PiecesInRange.Length; i++)
        {
            if(PiecesInRange[i].gameObject.CompareTag("Creature"))
            {
                PiecesInRange[i].transform.parent.GetComponent<CreatureControl>().Marked_ = true;
            }
        }
        if(rb.velocity.magnitude < 1 && wait)
        {
            rb.isKinematic = true;
            transform.rotation = Quaternion.Lerp(transform.rotation, ExampleTransform.rotation, Time.deltaTime * 5);
        }
    }
    private IEnumerator waitFor()
    {
        yield return new WaitForSeconds(1);
        wait = true;
    }
}
