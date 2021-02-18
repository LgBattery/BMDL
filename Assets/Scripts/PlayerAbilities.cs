using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Security;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    public KeyCode Qab;
    public KeyCode Wab;
    public KeyCode Eab;
    public KeyCode Rab;
    public KeyCode Dab;
    public KeyCode Fab;
    public KeyCode Fourab;
    public KeyCode Atk;

    public float Qlength = 10;
    public int Qdam = 300;
    public int Edam = 100;
    public int Rdam = 400;
    public float QRange = 20;
    public Transform qHit;
    public bool qAb; //if hit
    public bool qFirstCast = true;

    [SerializeField] private GameObject Cam;
    [SerializeField] private GameObject Qprojectile;
    [SerializeField] private GameObject Speed;
    [SerializeField] private GameObject Ward;
    [SerializeField] private GameObject Eparticles;
    [SerializeField] private AnimationCurve qCurve;
    [SerializeField] private AudioClip[] SFX;
    [SerializeField] private Animator anim;

    private Rigidbody rb;
    private Player pScript;
    private bool Traveling = false;
    private bool WTraveling = false;
    private bool EFirstCast = true;
    private bool wasTraveling = false;
    private AudioSource ASource;
    private Transform wHit;
    private Collider[] EHits;

    Coroutine lastRoutine = null;
    #region coolDowns
    private float qCoolDown;
    [SerializeField] private float qCoolDownLength = 7;
    private float fourCoolDown;
    [SerializeField] private float fourCoolDownLength = 1;
    private float eCoolDown;
    [SerializeField] private float eCoolDownLength = 5;
    private float wCoolDown;
    [SerializeField] private float wCoolDownLength = 1;
    private float rCoolDown;
    [SerializeField] private float rCoolDownLength = 7;
    private float atkCoolDown;
    [SerializeField] private float atkCoolDownLength = 0.5f;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
        pScript = transform.GetComponent<Player>();
        ASource = transform.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        #region input
        if (Input.GetKeyDown(Qab))
        {
            Qability();
        }
        if(Input.GetKeyDown(Wab))
        {
            Wability();
        }
        if(Input.GetKeyDown(Eab))
        {
            Eability();
        }
        if(Input.GetKeyDown(Rab))
        {
            Rability();
        }
        if(Input.GetKeyDown(Dab))
        {
            Dability();
        }
        if(Input.GetKeyDown(Fab))
        {
            Fability();
        }
        if(Input.GetKeyDown(Fourab))
        {
            Fourability();
        }
        if (Input.GetKeyDown(Atk))
        {
            MainAttack();
        }
        #endregion 
        if (Traveling)
        {
            wasTraveling = true;
            rb.isKinematic = true;
            Speed.SetActive(true);
            transform.position = Vector3.Lerp(transform.position, qHit.position, Time.deltaTime * 10);
            anim.SetBool("Flying", true);
            if(Vector3.Distance(transform.position, qHit.position) < 1)
            {
                Traveling = false;
                Qdamage();
            }
        }
        else
        {
            rb.isKinematic = false;
            Speed.SetActive(false);
            anim.SetBool("Flying", false);
        }
        if(WTraveling)
        {
            anim.SetBool("Flying", true);
            rb.isKinematic = true;
            Speed.SetActive(true);
            transform.position = Vector3.Lerp(transform.position, wHit.position + transform.up, Time.deltaTime * 10);
            if(qHit && wasTraveling)
            {
                wasTraveling = false;
                qHit.GetComponent<CreatureControl>().clearMark();
                qCoolDown = Time.time + 2;
            }
            if (Vector3.Distance(transform.position - transform.up, wHit.position) < 1)
            {
                WTraveling = false;
                anim.SetBool("Flying", false);
            }
        }
        
    }
    //Refer to notes
    void MainAttack()
    {
        if (Time.time > atkCoolDown)
        {
            anim.Play("ATK1");
            atkCoolDown = Time.time + atkCoolDownLength;
            StartCoroutine(ATK1());
        }
    }
    void Qability()
    {
        if (qFirstCast && Time.time > qCoolDown) //first cast
        {
            anim.Play("Q");
            GameObject proj;
            proj = Instantiate(Qprojectile, Cam.transform);
            proj.GetComponent<Qprojectile>().parent = this;
            qCoolDown = Time.time + qCoolDownLength;
            playSound(3, 1);
        }
        else if(qAb && !qFirstCast) //second cast
        {
            print(Vector3.Distance(qHit.position, transform.position));
            if (Vector3.Distance(qHit.position, transform.position) < QRange)
            {
                Traveling = true;
                WTraveling = false;
                qFirstCast = true;
                qAb = false;
                playSound(1, 1);
                playSound(0, 1);
                StopCoroutine(lastRoutine);
                qHit.GetComponent<CreatureControl>().dontClearMark();
                qHit.GetComponent<CreatureControl>().ASource.Stop();
            }
            else //if too far from target
            {
                print("outOfRange");
            }
        }
    }

    void Wability()
    {
        RaycastHit hit;
        if (Physics.Raycast(Cam.transform.position, Cam.transform.forward, out hit, 30, 1 << LayerMask.NameToLayer("WardTo")) && Time.time > wCoolDown)
        {
            wCoolDown = Time.time + wCoolDownLength;
            playSound(7, 1);
            wHit = hit.transform;
            Traveling = false;
            WTraveling = true;
        }
    }
    void Eability()
    {
        if (Time.time > eCoolDown && EFirstCast) //firstcast
        {
            StartCoroutine(ESound());
            eCoolDown = Time.time + eCoolDownLength;
            Collider[] PiecesInRange = Physics.OverlapSphere(transform.position, 5);
            EHits = PiecesInRange;
            anim.Play("E");
            for (int i = 0; i < PiecesInRange.Length; i++)
            {
                if (PiecesInRange[i].gameObject.CompareTag("Creature"))
                {
                    print("eMark");
                    if (!qHit)
                    {
                        PiecesInRange[i].transform.parent.GetComponent<CreatureControl>().Marked(3f, false);
                    }
                    PiecesInRange[i].transform.parent.GetComponent<CreatureControl>().DoDamage(Edam);
                    EFirstCast = false;
                    StartCoroutine(EReset());
                }
            }
        }
        else if (!EFirstCast) //Second cast
        {
            playSound(11, 1);
            EFirstCast = true;
            for (int i = 0; i < EHits.Length; i++)
            {
                if (EHits[i].gameObject.CompareTag("Creature"))
                {
                    RaycastHit hit;
                    Physics.Raycast(EHits[i].transform.position, -EHits[i].transform.up, out hit);
                    GameObject particles = Instantiate(Eparticles, hit.point + Vector3.up * 0.01f, Eparticles.transform.rotation, EHits[i].transform.parent);
                    Destroy(particles, 0.7f);
                    EHits[i].transform.parent.gameObject.GetComponent<CreatureControl>().slow(5f, 5f);
                }
            }
        }
    }
    void Rability()
    {
        RaycastHit hit;
        if (Physics.Raycast(Cam.transform.position, Cam.transform.forward, out hit, 3) && hit.transform.GetComponent<Rigidbody>() && Time.time > rCoolDown)
        {
            anim.Play("Kick");
            StartCoroutine(Kick(hit));
            rCoolDown = rCoolDownLength + Time.time;
        }
    }
    void Dability()
    {

    }
    void Fability()
    {

    }
    void Fourability()
    {
        if (Time.time > fourCoolDown)
        {
            fourCoolDown = fourCoolDownLength + Time.time;
            GameObject WInstance = Instantiate(Ward, transform.position, transform.rotation, null);
            WInstance.GetComponent<Rigidbody>().AddForce(Cam.transform.forward * 1000);
            print("Warded");
        }
    }
    private IEnumerator EReset()
    {
        yield return new WaitForSeconds(3);
        EFirstCast = true;
    }
    private IEnumerator ESound()
    {
        yield return new WaitForSeconds(0.25f);
        playSound(8, 0.7f);
        playSound(10, 1f); 
    }
    private IEnumerator Kick(RaycastHit hit)
    {
        yield return new WaitForSeconds(0.7f);
        playSound(9, 1);
        Rigidbody RBody = hit.transform.GetComponent<Rigidbody>();
        RBody.AddForce((transform.forward * 1000 * RBody.mass) + transform.up * 200);
        if (hit.transform.GetComponent<CreatureControl>())
        {
            hit.transform.GetComponent<CreatureControl>().DoDamage(Rdam);
        }
    }
    private void Qdamage()
    {
        qHit.GetComponent<CreatureControl>().DoDamage(Qdam);
        playSound(4, 1);
        print("damaged");
        qHit.GetComponent<CreatureControl>().clearMark();
        qCoolDown = Time.time + 2;
        wasTraveling = false;
    }
    public void hitQ()
    {
        print("shouldClear");
        lastRoutine = StartCoroutine(ClearQTGT());
    }
    public void Mark()
    {
        qHit.GetComponent<CreatureControl>().Marked(5, true);
    }
    int punchSound()
    {
        return Random.Range(1, 3) + 3;
    }
    public IEnumerator ClearQTGT()
    {
        yield return new WaitForSeconds(5); 
            qFirstCast = true;
            qAb = false;
            print("cleared");
    }
    private IEnumerator ATK1()
    {
        yield return new WaitForSeconds(0.1f);
        RaycastHit hit;
        if(Physics.Raycast(Cam.transform.position, Cam.transform.forward, out hit, 2) && hit.transform.gameObject.CompareTag("Creature"))
        {
            playSound(5, 1);
            hit.transform.GetComponent<CreatureControl>().DoDamage(120);
        }
    }
    void playSound(int sound, float volume)
    {
        ASource.volume = volume;
        if(sound == 4 || sound == 5 || sound == 6)
        {
            ASource.volume = 1;
            ASource.PlayOneShot(SFX[punchSound()]);
        }
        else
        {
            ASource.PlayOneShot(SFX[sound]);
        }
    }
}
