using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FightLib
{
    [Serializable]
    public enum CommandType
    {
        Stop,
        AttackThisUnit,
        AttackEnemyUnit,
        MoveToPosition,
        MoveToObject
    }

    [Serializable]
    public struct Command
    {
		public CommandType Type { get; private set; }
		public GameObject Target { get; private set; }

        public Command(CommandType _type, GameObject _target)
        {
            Type = _type;
            Target = _target;
        }
    }
}
