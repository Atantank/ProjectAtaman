using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MainLib;

public class TileScr : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    private int x;
    private int y;

    public void Init(Sprite _sprite, int _x, int _y) 
    {
		spriteRenderer.sprite = _sprite;
        x = _x;
        y = _y;
        this.name = "Tile_" + x + "_" + y;
    }
}
