using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DirectedExplanation : MonoBehaviour
{
    private float timeOut = 7;
    private float timeElapsed;
    private int count;
    // Start is called before the first frame update
    void Start()
    {
        count = 1;
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= timeOut)
        {
            Texture2D exp = Resources.Load("exp"+ count.ToString()) as Texture2D;
            Sprite exp_sprite = Sprite.Create(exp, new Rect(0, 0, exp.width, exp.height), new Vector2(0.5f, 0.5f));
            Texture2D img = Resources.Load("img_processing" + count.ToString()) as Texture2D;
            Sprite img_sprite = Sprite.Create(img, new Rect(0, 0, img.width, img.height), new Vector2(0.5f, 0.5f));
            gameObject.GetComponent<SpriteRenderer>().sprite = exp_sprite;
            //transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = img_sprite;
            timeElapsed = 0.0f;
            count++;
            if (count == 5) count = 1;
        }
    }
}