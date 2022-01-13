using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bedrock : element 
{
    public Bedrock(Vector2Int pos, Vector2? speed = null) : base (pos, speed)
    {
        this.matter = Matter.Bedrock;
        // this.color = new Color32(0,0,0,0);
        byte off = (byte) Mathf.Round(Random.Range(70, 130));

        this.color = new Color32(off, 100, 30, 255);

    }

}
