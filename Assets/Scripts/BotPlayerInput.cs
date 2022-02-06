using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotPlayerInput : PlayerInput
{
    private float counter;
    // Update is called once per frame
    void Update()
    {
        counter += Time.deltaTime * 3;
        Horizontal = Mathf.Sin(counter);
        if (counter > 1e7) { counter = 0; }
        Vertical = 0;
        Jump = 0;
        if (Random.Range(0, 1) > 0.95)
        {
            Jump = 1;
        }
    }
}
