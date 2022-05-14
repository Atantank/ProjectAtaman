using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MainLib;

public class TileScr : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    private int x;
    private int y;
    private SpritesDB db;

    public void Init(int _x, int _y) 
    {
		db = SpritesDB.DB;
		spriteRenderer.sprite = db.GroundSprite;
        x = _x;
        y = _y;
        this.name = "Tile_" + x + "_" + y;
    }
}
