using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DirectedRotating : MonoBehaviour
{
    private float dir =0.5f;
    private float arc_base;
    private float arc_random;

    // Start is called before the first frame update
    private void Start()
    {
        // 回転角の決定
        Vector3 worldAngle = this.GetComponent<Transform>().eulerAngles;
        arc_base = worldAngle.z;
        arc_random = UnityEngine.Random.Range(10.0f, 30.0f);
    }

    private void Update()
    {
        Vector3 worldAngle = this.GetComponent<Transform>().eulerAngles;
        float arc_old = worldAngle.z;

        float arc_now = arc_old + dir;
        this.GetComponent<Transform>().eulerAngles = new Vector3(worldAngle.x, worldAngle.y, arc_now);
        if (arc_now > arc_base + arc_random || arc_now< arc_base- arc_random)
        {
          dir = dir * -1;
        }
    }

}
