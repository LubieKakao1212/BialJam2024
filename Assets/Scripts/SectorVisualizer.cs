using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                    sectorsList.Add(Instantiate(rainSector, new Vector3(x, 0, z), Quaternion.identity));
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

    private float distanceToPlayer()
    {
        return 0;
    }

    private Color randomColor()
    {
        return new Color(
            Random.Range(0, 255),
            Random.Range(0, 255),
            Random.Range(0, 255),
            trasparency
        );
    }
}
