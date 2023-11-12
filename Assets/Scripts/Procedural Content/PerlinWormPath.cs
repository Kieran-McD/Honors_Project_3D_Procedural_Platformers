using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinWormPath : PerlinWorm
{
    public bool isMoving = true;

    public new void Update()
    {
        if (isMoving)
        {
            base.Update();
        }    
    }

}
