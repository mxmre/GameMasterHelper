using GameMasterHelper.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMasterHelper.Logic.DnD
{
    [Serializable]
    public class DnDCreatureMagicCaster : DnDCreature
    {
        public new object Clone()
        {
            return MemoryObject<DnDCreatureMagicCaster>.DeepClone(this);
        }
        public DnDCreatureMagicCaster() : base()
        {
            p_spellCastAbility = DndCreatureAbility.Intelligence;
        }

        private DndCreatureAbility p_spellCastAbility;

        public DndCreatureAbility SpellCastAbility
        {
            get { return p_spellCastAbility; }
            set { p_spellCastAbility = value; }
        }

        public uint SpellDC
        {
            get { return 8u + this.ProficiencyBonus + (uint)this.GetAbilityModifierValue(this.SpellCastAbility); }
        }
    }
}
