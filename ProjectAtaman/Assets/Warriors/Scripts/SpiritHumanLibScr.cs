using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanLib
{
	[Serializable]
    public enum SpiritCondition
    {
        good
    }

    public abstract class SpiritState : State // Родительский класс для состояний духа
    {
        public abstract SpiritCondition Condition { get; }
		//Spirit Check // TODO Добавить способы воздействовать на дух
        protected BodyState body;

		public SpiritState(WarriorScr _owner, BodyState _body) : base(_owner)
        {
            body = _body;
        }
    }

	// * Варианты состояния духа //////////////////////////////////////////////////////////////////////////////
	class GoodSpirit : SpiritState
    {
        public override SpiritCondition Condition { get => SpiritCondition.good; }
		public GoodSpirit(WarriorScr _owner, BodyState _body) : base(_owner, _body) { }
    }
}
