using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flag_animation : MonoBehaviour
{
    float tim;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float duration = 1f;
        float angle = 5;
        tim += Time.deltaTime * angle / duration;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.PingPong(tim, angle) - angle / 2);
    }
}
