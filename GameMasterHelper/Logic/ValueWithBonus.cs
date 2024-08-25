using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMasterHelper.Logic
{
    [Serializable]
    public class ValueWithBonus<TValue, TBonus>
    {
        public ValueWithBonus() :this(default(TValue), default(TBonus)) { }

        public ValueWithBonus(TValue value, TBonus bonus)
		{
			p_bonus = bonus;
			p_value = value;
		}
        private TValue p_value;

		public TValue Value
		{
			get { return p_value; }
			set { p_value = value; }
		}

		private TBonus p_bonus;

		public TBonus Bonus
		{
			get { return p_bonus; }
			set { p_bonus = value; }
		}

	}
}
