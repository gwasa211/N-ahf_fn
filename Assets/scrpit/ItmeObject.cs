using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItmeObject : MonoBehaviour
{
    [SerializeField] ItemOS data;
    // Start is called before the first frame update
    public int Getpoint()
    {
        return data.point;
    }
}
