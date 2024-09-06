using GameMasterHelper.Logic.DnD;
using GameMasterHelper.Manage.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GameMasterHelper.Manage
{
    public class Module
    {
        public Module() 
        {
            p_creatures = DefaultCreatures;
            p_creatures_imgs = DefaultCreatureImages;
        }

        static public List<DnDCreature> DefaultCreatures { get => new List<DnDCreature>(); }
        static public Catalog<BitmapSource> DefaultCreatureImages { get => new Catalog<BitmapSource>(1); }

        private List<DnDCreature> p_creatures;
        public List<DnDCreature> Creatures
        {
            get => p_creatures;
            set => p_creatures = value;
        }

        private Catalog<BitmapSource> p_creatures_imgs;
        public Catalog<BitmapSource> CreatureImages
        {
            get => p_creatures_imgs; set => p_creatures_imgs = value;
        }
    }
}
