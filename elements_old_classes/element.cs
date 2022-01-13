using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class element 
{
    /*
    This will be the base class. 
    */
    public Vector2 speed {get; set;}
    public Vector2Int speedInt {
        get {
            return new Vector2Int((int) this.speed.x,(int) this.speed.y);
        }
    }
    public Vector2Int position {get; set;}
    
    public Color32 color{get; set;}
    public Matter matter{get; set;}
    public int IsFreeFalling{get; set;} //0 sleeping, 1 and 2 are awake
    public float inertialResistance; 

    public float friction;

    public element(Vector2Int pos, Vector2? speed = null)
    {
        this.IsFreeFalling = 2;
        this.position = pos; 
        if (speed == null) {
            this.speed = Vector2.zero;
        } else {
            this.speed = (Vector2)speed;
        }
        this.matter = Matter.None;
        this.color = new Color32(100,100,100,100);
        this.friction = 0.5f;
        this.inertialResistance = 0f; 
    }

    public virtual Vector3Int Step()
    {
        return Vector3Int.one;
        //Do nothing here. 
    }
    public virtual Vector3Int StepSimple()
    {
        return Vector3Int.one;
        //Do nothing here. 
    }

    public virtual void TryWakeCell() {
        if (this.IsFreeFalling >0) { return;}

        this.IsFreeFalling = Random.Range(0f, 1f) >= inertialResistance ? 2: 0;
        // this.IsFreeFalling = true;
    }



}
