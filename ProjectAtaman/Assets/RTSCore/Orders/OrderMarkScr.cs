using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OrderLib;
using MainLib;

public class OrderMarkScr : MonoBehaviour
{
	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] private Sprite movePoint;
	[SerializeField] private Sprite attackPoint;
	public IDamageable DamageableTarget { get; private set; }

	public void Init(OrderType _orderType)
	{
		switch (_orderType)
		{
			case OrderType.AttackTarget:
				spriteRenderer.sprite = attackPoint;
				break;
			case OrderType.Move:
				spriteRenderer.sprite = movePoint;
				break;
		}
	}
}
