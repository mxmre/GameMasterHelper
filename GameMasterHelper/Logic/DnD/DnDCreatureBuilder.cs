using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMasterHelper.Logic.DnD
{
    public abstract class DnDCreatureBuilder
    {
        public enum DnDCreatureType
        {
            Default = 0,
            MagicCaster,
        }
        public static DnDCreature GetCreature(DnDCreatureType creatureType)
        {
            switch (creatureType)
            {
                case DnDCreatureType.MagicCaster:
                    return new DnDCreatureMagicCaster();
                default:
                    return new DnDCreature();
            }

        }
    }
}
