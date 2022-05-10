using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MainLib;

namespace OrderLib
{   
    [Serializable]
    public enum OrderType
    {
        Stop,
        AttackTarget,
        AttackEnemy, // ? Зачем отдельно от АтакаЦели?
        Move
    }

    [Serializable]
    public class Order
    {
		public OrderType Type { get; private set; }
		public OrderMarkScr Mark { get; private set; }
        public bool IsToQueue;
        
        public Order(OrderType _type, OrderMarkScr _mark, bool _isToQueue)
        {
            Type = _type;
			Mark = _mark;
			IsToQueue = _isToQueue;
        }
    }
}
