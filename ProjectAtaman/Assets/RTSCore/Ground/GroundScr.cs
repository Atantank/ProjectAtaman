using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundScr : MonoBehaviour
{
    public static GroundScr Ground;

    [SerializeField] private int groundWidth;
    [SerializeField] private int groundHeight;
    [SerializeField] private TileScr TilePref;

    private TileScr [,] groundTiles;
    private float startPointX;
    private float startPointY;

    Quaternion zeroQ = new Quaternion(0, 0, 0, 0);

    void Awake()
    {
		Ground = this;
        groundTiles = new TileScr [groundWidth, groundHeight];
		startPointX = -groundWidth /2 + 0.5f;
		startPointY = -groundHeight /2 + 0.5f;
    }

    void Start()
    {
		SpawnGround();
    }

    void SpawnGround()
    {
        for(int i = 0; i < groundWidth; i++)
        {
			for (int j = 0; j < groundHeight; j++)
            {
                Vector3 spawnPosition = new Vector3(startPointX + i, startPointY + j, this.transform.position.y);
				groundTiles[i, j] = Instantiate(TilePref, spawnPosition, zeroQ, this.transform);
				groundTiles[i, j].Init(i, j);
            }
        }
    }
}
