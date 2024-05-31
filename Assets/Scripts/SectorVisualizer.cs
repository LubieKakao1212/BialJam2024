using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

// class MatRecord{
//     public Material mat;
//     public float def;
//     public float wet;

//     public MatRecord(Material mat, float def, float wet)
//     {
//         this.mat = mat;
//         string[] matProperties = mat.GetPropertyNames(MaterialPropertyType.Float);

//     }
// }

public class SectorVisualizer : MonoBehaviour
{
    //Vector3 startingPos
    //    = new Vector3(-200, 0, -200);
    public int sectorSize = 20;
    public int mapSize = 400;
    public float height = 100;
    public float gap = 0.05f;
    public float trasparency = 0.9f;
    [SerializeField] GameObject player;
    [SerializeField] GameObject rainSector;
    public ArrayList sectorsList;

    public float transitionTime = 15;
    public float dryTime = 10;
    public float wetTime = 30;
    private bool isWet = false;
    private float timer;

    [SerializeField] TerrainLayer layer1;
    [SerializeField] private float targetL1;
    public float defLayerSmooth1;
    // [SerializeField] TerrainLayer layer2;
    // [SerializeField] private float targetL2;
    // private float defLayerSmooth2;

    // [SerializeField] Material concreteMat;
    // [SerializeField] private float targetMat1;
    // private float defSmoothConcrete;
    // [SerializeField] Material rockMat1;
    // [SerializeField] private float targetMat2;
    // private float defSmoothRock1;
    // [SerializeField] Material rockMat2;
    // [SerializeField] private float targetMat3;
    // private float defSmoothrock2;



    public void OnEnable()
    {
        if(sectorsList == null)
            sectorsList = new ArrayList();

        if(player == null)
            player = GameObject.FindGameObjectWithTag("player");

        if(sectorsList.Count == 0)
        {
             for(int x = -mapSize/2 + sectorSize / 2; x <= mapSize/2 - sectorSize / 2; x+=sectorSize)
            {
                for(int z = -mapSize/2 + sectorSize / 2; z <= mapSize/2 - sectorSize / 2; z+=sectorSize)
                {
                    var sector = Instantiate(rainSector, new Vector3(x, 0, z), Quaternion.identity);
                    sectorsList.Add(sector);
                    //var source = sector.GetComponentInChildren<AudioSource>();
                    //source.PlayDelayed(Random.Range(0, source.clip.length));
                }
            }
        }

        timer = dryTime;

        defLayerSmooth1 = layer1.smoothness;
        //defLayerSmooth2 = layer2.smoothness;
    }

    public void OnDisable()
    {
        layer1.smoothness = defLayerSmooth1;
    }

    public void Update()
    {
        timer -= Time.deltaTime;
        
        if(timer < 0)
        {
            if(!isWet)
            {
                if(layer1.smoothness < targetL1)
                    layer1.smoothness += (targetL1 / transitionTime) * Time.deltaTime;
                else
                {
                    timer = wetTime;
                    isWet = true;
                }
            }
            else
            {
                if(layer1.smoothness < defLayerSmooth1)
                    layer1.smoothness -= (defLayerSmooth1 / transitionTime) * Time.deltaTime;
                else 
                {
                    timer = dryTime;
                    isWet = false;
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        for(int x = -mapSize/2 + sectorSize / 2; x <= mapSize/2 - sectorSize / 2; x+=sectorSize)
        {
            for(int z = -mapSize/2 + sectorSize / 2; z <= mapSize/2 - sectorSize / 2; z+=sectorSize)
            {
                //Gizmos.color = randomColor();
                Gizmos.DrawWireCube(new Vector3( x, 0, z), 
                new Vector3(
                    sectorSize - 2 * gap * sectorSize,
                    height,
                    sectorSize - 2 * gap * sectorSize
                ));
            }
        }
    }

    private float distanceToPlayer(GameObject obj)
    {
        return Vector3.Distance(player.transform.position, obj.transform.position);
    }

    private Color randomColor()
    {
        return new Color(
            UnityEngine.Random.Range(0, 255),
            UnityEngine.Random.Range(0, 255),
            UnityEngine.Random.Range(0, 255),
            trasparency
        );
    }
}
