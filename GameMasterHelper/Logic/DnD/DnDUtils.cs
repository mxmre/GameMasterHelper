using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static GameMasterHelper.Logic.DnD.DnDCreature;

namespace GameMasterHelper.Logic.DnD
{
    public enum DnDDamageType
    {
        Acid = 0,
        Bludgeoning,
        Cold,
        Fire,
        Force,
        Lightning,
        Necrotic,
        Piercing,
        Poison,
        Psychic,
        Radiant,
        Slashing,
        Thunder,
    }
    public enum DnDArmorType
    {
        NoArmor= 0,
        NatureArmor,
        LightArmor,
        MediumArmor,
        HeavyArmor,
    }

    public abstract class DiceOP
    {
        public static Dice D4() { return new Dice(4); }
        public static Dice D6() { return new Dice(6); }
        public static Dice D8() { return new Dice(8); }
        public static Dice D10() { return new Dice(10); }
        public static Dice D12() { return new Dice(12); }
        public static Dice D20() { return new Dice(20); }
        public static Dice D100() { return new Dice(100); }

        static private string REGEX_DND_DICEEXPR = @"^\d{0,5}[кd]([468]|1([02]|00)|20)([\+\-]\d{1,5})?$";
        //public static string RegExDnDDiceExpr { get => REGEX_DND_DICEEXPR; }
        public static bool IsDnDDiceExpr(string input)
        {
            return Regex.IsMatch(input, REGEX_DND_DICEEXPR);
        }
        public static DiceExpr GetDnDDiceExprFromStr(string diceStr)
        {
            DiceExpr diceExpr = new DiceExpr();
            if (!Regex.IsMatch(diceStr, REGEX_DND_DICEEXPR))
                throw new ArgumentException("Wrong string to create dice expr!");
            var strs = Regex.Split(diceStr, @"[кd]");
            string diceSide = string.Empty;
            diceExpr.DiceMod = 0;
            if (strs.Length == 1)
            {
                diceSide = strs[0];
                diceExpr.DiceCount = 1;
            }
            else if (strs.Length == 2)
            {
                if (strs[0] != string.Empty)
                    diceExpr.DiceCount = uint.Parse(strs[0]);
                else diceExpr.DiceCount = 1;
                var matches = Regex.Matches(strs[1], @"[-+]");
                diceSide = strs[1];

                if (matches.Count == 1)
                {
                    strs = Regex.Split(strs[1], @"[-+]");
                    diceSide = strs[0];
                    diceExpr.DiceMod = int.Parse(strs[1]);
                    if (matches[0].Value == "-")
                        diceExpr.DiceMod = -diceExpr.DiceMod;
                }
            }
            else
                throw new ArgumentException("Unknown Exception!");
            diceExpr.Dice = new Dice(uint.Parse(diceSide));
            return diceExpr;
        }
    }

    public abstract class DnDUtils
    {
        static private uint[] LEVEL_CREATURE_EXP_VALUES = new uint[]
                    {
                        
                        25u,
                        50u,
                        100u,
                        1u,
                        200u,
                        450u,
                        700u,
                        1100u,
                        1800u,
                        2300u,
                        2900u,
                        3900u,
                        5000u,
                        5900u,
                        7200u,
                        8400u,
                        10000u,
                        11500u,
                        13000u,
                        15000u,
                        18000u,
                        20000u,
                        22000u,
                        25000u,
                        33000u,
                        41000u,
                        50000u,
                        62000u,
                        75000u,
                        90000u,
                        105000u,
                        120000u,
                        135000u,
                        155000u,
        };
        static public uint GetExpForKill(DnDCreature creature)
        {
            return LEVEL_CREATURE_EXP_VALUES[creature.RawCreatureLevel + 3];
        }
        static public string ToStringInfo(int value)
        {
            return new StringBuilder()
                .Append(((value).CompareTo(0) >= 0 ? "+" : ""))
                .Append(value.ToString()).ToString();
        }
        static public double FeetToMeters(double feet) => feet / 5.0 * 1.5;
        static public double MetersToFeet(double meters) => meters / 1.5 * 5.0;
        public static List<string> GetListfEnumFields<EnumType>()
        {
            List<string> resultList = new List<string>();
            foreach (var item in Enum.GetValues(typeof(EnumType)))
            {
                resultList.Add(((EnumType)item).ToString());
            }
            return resultList;
        }

        private static Dictionary<DndCreatureSkill, DndCreatureAbility> DICT_SKILL_TO_ABILITY =
            new Dictionary<DndCreatureSkill, DndCreatureAbility>()
        {
                { DndCreatureSkill.Str_Athletics,       DndCreatureAbility.Strength},
                { DndCreatureSkill.Dex_Acrobatics,      DndCreatureAbility.Dexterity},
                { DndCreatureSkill.Dex_SleightOfHand,   DndCreatureAbility.Dexterity},
                { DndCreatureSkill.Dex_Stealth,         DndCreatureAbility.Dexterity},
                { DndCreatureSkill.Int_Arcana,          DndCreatureAbility.Intelligence},
                { DndCreatureSkill.Int_History,         DndCreatureAbility.Intelligence},
                { DndCreatureSkill.Int_Investigation,   DndCreatureAbility.Intelligence},
                { DndCreatureSkill.Int_Nature,          DndCreatureAbility.Intelligence},
                { DndCreatureSkill.Int_Religion,        DndCreatureAbility.Intelligence},
                { DndCreatureSkill.Wis_AnimalHandling,  DndCreatureAbility.Wisdom},
                { DndCreatureSkill.Wis_Insight,         DndCreatureAbility.Wisdom},
                { DndCreatureSkill.Wis_Medicine,        DndCreatureAbility.Wisdom},
                { DndCreatureSkill.Wis_Perception,      DndCreatureAbility.Wisdom},
                { DndCreatureSkill.Wis_Survival,        DndCreatureAbility.Wisdom},
                { DndCreatureSkill.Cha_Deception,       DndCreatureAbility.Charisma},
                { DndCreatureSkill.Cha_Intimidation,    DndCreatureAbility.Charisma},
                { DndCreatureSkill.Cha_Performance,     DndCreatureAbility.Charisma},
                { DndCreatureSkill.Cha_Persuasion,      DndCreatureAbility.Charisma},
        };

        public static DndCreatureAbility SkillToAbility(DndCreatureSkill skill)
        { return DICT_SKILL_TO_ABILITY[skill]; }

        public static string RawLevelToStringLevel(int level)
        {
            if (level < 0)
            {
                switch (level)
                {
                    case -1:
                        return "1/2";
                    case -2:
                        return "1/4";
                    case -3:
                        return "1/8";
                }
            }
            return level.ToString();
        }
        public static int StringLevelToRawLevel(string level)
        {
            level.Trim();
            switch (level)
            {
                case "1/2":
                    return -1;
                case "1/4":
                    return -2;
                case "1/8":
                    return -3;
                default:
                    {
                        int tempLvl = 0;
                        if (int.TryParse(level, out tempLvl))
                            return tempLvl;
                        return 0;
                    }
            }
        }
    }
}
