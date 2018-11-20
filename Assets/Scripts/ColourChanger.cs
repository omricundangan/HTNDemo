using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourChanger : MonoBehaviour {

    float duration = 15f;
    private float t = 0;
   // bool isReset = false;

    void Update()
    {
        ColorChangerr();
    }


    void ColorChangerr()
    {

            GetComponent<Renderer>().material.color = Color.Lerp(Color.green, Color.red, t);

            if (t < 1)
            {
                t += Time.deltaTime / duration;
            }

        }

}
