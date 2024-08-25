using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GameMasterHelper.Logic
{
    public class Dice: ICloneable
    {
        public Dice() : this(20) {}
        public Dice(uint diceSide) { DiceSide = diceSide; p_rand = new Random(); }
        
        public object Clone()
        {
            return new Dice(DiceSide);
        }

        private uint p_diceSide;
        private Random p_rand;


        public uint DiceSide { get { return p_diceSide; } set { p_diceSide = value; }}
        public double DiceAverageResult { get { return (1.0 + p_diceSide) / 2.0; } }
        public uint Roll()
        {
            return (uint)p_rand.Next(1, (int)DiceSide);
        }
    }

    public class DiceExpr : ICloneable
    {
        
        public DiceExpr() : this(0, null) { }
        
        public DiceExpr(uint diceCount, Dice dice, int mod = 0) 
        {
            DiceMod = mod;
            DiceCount = diceCount;
            p_dice = dice;
        }

        public override string ToString()
        {
            if (p_dice == null)
                return null;
            StringBuilder sb = new StringBuilder()
                .Append(DiceCount.ToString())
                .Append("к")
                .Append(p_dice.DiceSide.ToString());
            if(DiceMod != 0)
            {
                sb.Append((DiceMod > 0 ? "+" : "-"))
                    .Append(DiceMod.ToString());
            }
            return sb.ToString();
        }

        public object Clone()
        {
            return new DiceExpr(DiceCount, (Dice)p_dice.Clone(), DiceMod);
        }

        public uint Roll()
        {
            int sum = 0;
            for (int i = 0; i < DiceCount; i++) { sum += (int)p_dice.Roll(); }
            sum += DiceMod;
            return (sum < 0 ? 0u : (uint)sum);
        }

        private Dice p_dice;
        public Dice Dice
        {
            get { return p_dice; }
            set { p_dice = value; }
        }
        private int p_diceMod;

        public int DiceMod
        {
            get { return p_diceMod; }
            set { p_diceMod = value; }
        }
        public uint DiceExprAverageResult { get { return (uint)(Math.Ceiling(p_dice.DiceAverageResult * DiceCount)+ DiceMod); } }

        private uint p_diceCount;

        public uint DiceCount
        {
            get { return p_diceCount; }
            set { p_diceCount = value; }
        }


    }

}
