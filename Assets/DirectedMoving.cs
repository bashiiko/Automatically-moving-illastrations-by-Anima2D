using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DirectedMoving : MonoBehaviour
{
    public float timeOut = 5f;
    private float timeElapsed;
    private float dir_x = 0.1f;
    private float dir_y = 0.1f;
    private Vector3 now_position;
    private Vector3 destination;  // 目的地
    private bool arrived;　　　 　// 到着フラグ
    private Vector3 direction;     // 移動方向
    private Vector3 velocity;
    private float walkSpeed = 0.01f;

    // Start is called before the first frame update
    private void Start()
    {
        timeElapsed = 0.0f;
        float pos_y = UnityEngine.Random.Range(-5.0f, 5.0f);
        float pos_z = Decide_Zpos(pos_y);

        destination = new Vector3(UnityEngine.Random.Range(-9.5f, 9.5f), pos_y, pos_z);
        arrived = false;
    }

    private void Update()
    {
        Vector3 pos = transform.position;
        pos.z = Decide_Zpos(transform.position.y);

        transform.position = pos;

        if (!arrived)
        {
            Debug.Log(Vector3.Distance(pos, destination));
            velocity = Vector3.zero;
            direction = (destination - pos).normalized;
            velocity = direction * walkSpeed;
            // 位置の決定
            //now_position = this.GetComponent<Transform>().position;
            transform.position = pos+velocity;
            //this.GetComponent<Transform>().position = new Vector3(now_position.x + dir_x, now_position.y + dir_y, 1);
            timeElapsed = 0.0f;
            if(Vector3.Distance(pos, destination) < 0.5f)  arrived = true;
        }else
        {
            Debug.Log("arrive");
            float pos_y = UnityEngine.Random.Range(-6.0f, 6.0f);
            float pos_z = Decide_Zpos(pos_y);

            destination = new Vector3(UnityEngine.Random.Range(-12.0f, 12.0f), pos_y, pos_z);
            arrived = false;
        }


    }

    private float Decide_Zpos(float pos_y)
    {
        float pos_z;
        if (pos_y > -2.8) pos_z = 5.0f;
        else if (pos_y > -6.0) pos_z = 3.0f;
        else pos_z = 0.0f;

        return pos_z;
    }

}
