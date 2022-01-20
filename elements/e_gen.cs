using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;



/// <summary>
/// Generate element
/// </summary>
/// <remarks>
/// Bedrock, Stone,  Brick, Sand
/// </remarks>
public class e_gen {
    private static Unity.Mathematics.Random _random = new Unity.Mathematics.Random(Constants.seed);
    public static element_s Sand(Vector2Int pos, Vector2? speed = null) {
        Vector2 tspeed; 
        if (speed == null) {
            tspeed = Vector2.zero;
        } else {
            tspeed = (Vector2)speed;
        }
        element_s sand = new element_s(pos, tspeed); 

        sand.matter = Matter.Powder;
        sand.element = e_name.Sand;
        sand.IsFreeFalling = 0;
        byte off = (byte) Mathf.Round(_random.NextInt(50, 150));
        sand.color = new Color32(255, 244, off, 255);
        sand.inertialResistance = 0.2f;
        sand.airResistance = 1.9f; //inverse
        sand.friction = 0.1f; 
        sand.blastResistance=0.1f; 
        return sand; 
    }

    public static element_s Dirt(Vector2Int pos, Vector2? speed = null) {
        Vector2 tspeed; 
        if (speed == null) {
            tspeed = Vector2.zero;
        } else {
            tspeed = (Vector2)speed;
        }
        element_s sand = new element_s(pos, tspeed); 

        sand.matter = Matter.Powder;
        sand.element = e_name.Dirt;
        sand.IsFreeFalling = 0;
        byte off = (byte) Mathf.Round(_random.NextInt(100, 120));
        sand.color = new Color32(off, 60, 50, 255);
        sand.inertialResistance = 0.2f;
        sand.airResistance = 0.8f; //inverse
        sand.friction = 0.6f; 
        sand.blastResistance=0.2f; 
        return sand; 
    }

    public static element_s Bedrock(Vector2Int pos, Vector2? speed = null) {
        Vector2 tspeed; 
        if (speed == null) {
            tspeed = Vector2.zero;
        } else {
            tspeed = (Vector2)speed;
        }
        element_s thisp = new element_s(pos, tspeed); 

        thisp.matter = Matter.Bedrock;
        // thisp.element = "bedrock";
        thisp.element = e_name.Bedrock;
        thisp.IsFreeFalling = 0;
        byte off = (byte) Mathf.Round(_random.NextInt(50, 150));
        thisp.color = new Color32(0,0,0,0);
        thisp.inertialResistance = 1f;
        return thisp; 
    }

    public static element_s Stone(Vector2Int pos, Vector2? speed = null) {
        Vector2 tspeed; 
        if (speed == null) {
            tspeed = Vector2.zero;
        } else {
            tspeed = (Vector2)speed;
        }
        element_s thisp = new element_s(pos, tspeed); 
        thisp.IsFreeFalling = 0;


        thisp.matter = Matter.Solid;
        // thisp.element = "bedrock";
        thisp.element = e_name.Stone;
        byte off = (byte) Mathf.Round(_random.NextInt(110, 130));
        thisp.color = new Color32(off,off,off,255);

        // thisp.inertialResistance = 1f;
        thisp.blastResistance = 0.8f;
        return thisp; 
    }

    public static element_s Brick(Vector2Int pos, Vector2? speed = null) {
        Vector2 tspeed; 
        if (speed == null) {
            tspeed = Vector2.zero;
        } else {
            tspeed = (Vector2)speed;
        }
        element_s thisp = new element_s(pos, tspeed); 
        thisp.IsFreeFalling = 0;


        thisp.matter = Matter.Solid;
        // thisp.element = "bedrock";
        thisp.element = e_name.Brick;
        if (Chunks.mod(pos.x, 5) != 0 && Chunks.mod(pos.y, 5)!= 0) {
            byte off = (byte) Mathf.Round(_random.NextInt(150, 270));
            thisp.color = new Color32(off,60,60,255);
        } else {
            byte off = (byte) Mathf.Round(_random.NextInt(190, 210));
            thisp.color = new Color32(off,off,off,255);
        }
        // thisp.inertialResistance = 1f;
        thisp.blastResistance = 0.8f;
        return thisp; 
    }

}
