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
        protected BodyState body;

		public SpiritState(HumanScr _owner, BodyState _body) : base(_owner)
        {
            body = _body;
        }
    }
    
	// * ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// * Варианты состояния духа ////////////////////////////////////////////////////////////////////////////////////////
	class GoodSpirit : SpiritState
    {
        public override SpiritCondition Condition { get => SpiritCondition.good; }
		public GoodSpirit(HumanScr _owner, BodyState _body) : base(_owner, _body) { }
    }
}
