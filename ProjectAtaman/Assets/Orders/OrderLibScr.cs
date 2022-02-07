using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OrderLib
{
    [Serializable]
    public enum OrderType
    {
        Stop,
        AttackTarget,
        AttackEnemy,
        Move
    }

    [Serializable]
    public class Order
    {
		public OrderType Type { get; private set; }
		public GameObject Target { get; private set; }

        public Order(OrderType _type, GameObject _target)
        {
            Type = _type;
            Target = _target;
        }
    }

    public interface IOrder
    {
        public void SetOrder(Order _order);
    }
}