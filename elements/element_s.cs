using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;


public struct element_s
{
    /*
    powder will be the base class. 
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
    public e_name element; 


    public int IsFreeFalling{get; set;} //0 sleeping, 1 and 2 are awake
    public float inertialResistance; 
    public float friction;
    public float airResistance;
    public float blastResistance;

    public element_s(Vector2Int pos, Vector2? speed = null)
    {
        this.IsFreeFalling = 2;
        this.position = pos; 
        if (speed == null) {
            this.speed = Vector2.zero;
        } else {
            this.speed = (Vector2)speed;
        }
        this.element = e_name.Nothing; 
        this.matter = Matter.None;
        this.color = new Color32(100,100,100,100);
        this.friction = 0.5f;
        this.inertialResistance = 0f; 
        this.airResistance = 0.5f; 
        this.blastResistance = 1f; 
    }

}

// public struct element_s
// {
//     /*
//     powder will be the base class. 
//     */
//     public float2 speed {get; set;}
//     public int2 speedInt {
//         get {
//             return new int2((int) this.speed.x,(int) this.speed.y);
//         }
//     }
//     public int2 position {get; set;}
    
//     // public Color32 color{get; set;}
//     public Matter matter{get; set;}
//     public e_name element; 
//     public Color32 color{get; set;}

//     public int IsFreeFalling{get; set;} //0 sleeping, 1 and 2 are awake
//     public float inertialResistance; 
//     public float friction;
//     public float airResistance;

//     public element_s(int2 pos, float2? speed = null)
//     {
//         this.IsFreeFalling = 2;
//         this.position = pos; 
//         this.color = new Color32(100,100,100,100);


//         if (speed == null) {
//             this.speed = float2.zero;
//         } else {
//             this.speed = (float2)speed;
//         }

//         this.element = e_name.Sand; 
//         this.matter = Matter.None;
//         // this.color = new Color32(100,100,100,100);
//         this.friction = 0.5f;
//         this.inertialResistance = 0f; 
//         this.airResistance = 0.5f; 
//     }

// }

public enum e_name {
    //Solids
    Bedrock, Stone, Brick,

    //Powders,
    Sand, Dirt,
    //Liquids
    Water, Oil,

    //Gas,
    Smoke, 

    //Other
    Nothing
    
}