using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritesDB : MonoBehaviour
{
	[SerializeField] private Sprite groundSprite;
    public Sprite GroundSprite { get => groundSprite; }

    public static SpritesDB DB { get; private set; }

    void Awake()
    {
		DB = this;
    }
}
