using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QCam : MonoBehaviour
{   
    [SerializeField] private Transform HeadBone;
    void Update()
    {
        transform.position = HeadBone.position;
    }
}
