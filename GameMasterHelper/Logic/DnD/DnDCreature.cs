using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GameMasterHelper.Logic.DnD
{
    [Serializable]
    public class DnDCreature : BasicEntity
    {   
        private static ValueWithBonus<TValue, int> NewStat<TValue>(TValue abilityValue, int abilityBonus = 0)
        {
            return new ValueWithBonus<TValue, int>(abilityValue, abilityBonus);
        }

        public Dictionary<DndCreatureAbility, ValueWithBonus<bool, int>> SaveThrows 
        { get => p_saveThrows; set => p_saveThrows = value; }
        public Dictionary<DndCreatureSkill, ValueWithBonus<DndCreatureSkillProf, int>> Skills
        { get => p_skills; set => p_skills = value; }
        public Dictionary<DndCreatureAbility, ValueWithBonus<uint, int>> Abilities
        { get => p_abilities; set => p_abilities = value; }

        public DnDCreature() : base()
        {
            p_conditions = new Dictionary<DnDCreatureCondition, bool>();
            int len = Enum.GetNames(typeof(DnDCreatureCondition)).Length;
            for (int i = 0; i < len; ++i)
            {
                p_conditions.Add((DnDCreatureCondition)i, false);
            }
            p_saveThrows = new Dictionary<DndCreatureAbility, ValueWithBonus<bool, int>>();
            for (int i = 0; i < 6; ++i) 
            {
                p_saveThrows.Add((DndCreatureAbility)i, NewStat(false));
            }
            p_skills = new Dictionary<DndCreatureSkill, ValueWithBonus<DndCreatureSkillProf, int>>();
            for (int i = 0; i < 18; ++i)
            {
                p_skills.Add((DndCreatureSkill)i, NewStat(DndCreatureSkillProf.None));
            }
            p_abilities = new Dictionary<DndCreatureAbility, ValueWithBonus<uint, int>>();
            for (int i = 0; i < 6; ++i)
            {
                p_abilities.Add((DndCreatureAbility)i, NewStat(10u));
            }
            p_dmgResist = new Dictionary<DnDDamageType, DnDCreatureResistance>();
            for (int i = 0; i < 13; ++i)
            {
                p_dmgResist.Add((DnDDamageType)i, DnDCreatureResistance.None);
            }

            p_features              = new BasicEntity();
            p_actions               = new BasicEntity();
            p_legendaryActions      = new BasicEntity();
            p_reactions             = new BasicEntity();
            p_lairActions           = new BasicEntity();
            p_regionalEffects       = new BasicEntity();

            EquipedArmorType        = DnDArmorType.NoArmor;
            p_armorClassBonus       = 0;
            p_armorValue            = 10u;
            UpdateArmorClass();
            
            UpdateInitiative();
            p_initiativeBonus       = 0;
            p_creatureLevel         = 1;
            UpdateProficiencyBonus();

            p_baseSpeedFeet         = new ValueWithBonus<double, bool>(30, true);
            p_burrowSpeedFeet       = new ValueWithBonus<double, bool>(-1, false);
            p_climbSpeedFeet        = new ValueWithBonus<double, bool>(-1, false);
            p_flySpeedFeet          = new ValueWithBonus<double, bool>(-1, false);
            p_swimSpeedFeet         = new ValueWithBonus<double, bool>(-1, false);
            UpdateSpeed();


            p_creatureAlignment     = DnDCreatureAlignment.None;
            p_creatureSize          = DnDCreatureSize.Medium;
            p_creatureType          = DnDCreatureType.Humanoids;

            UpdateHitDicesFromStats();
            SetAverageHP();
            p_tempHp                = 0u;

            p_senses                = "---";
            p_languages             = "---";
            p_conditionImmunity     = "---";
            p_weaponsText           = "---";
            p_armorsText            = "---";

            p_image = null;
        }

        [NonSerialized] private BitmapSource p_image;


        [JsonIgnore]
         public BitmapSource Image
        {
            
            get 
            {
                return p_image; 
            }
            set { p_image = value; }
        }

        public byte[] ImagePixels
        {
            get 
            {
                byte[] bytes;
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                using (MemoryStream stream = new MemoryStream())
                {
                    encoder.Frames.Add(BitmapFrame.Create(p_image));
                    encoder.Save(stream);
                    bytes = stream.ToArray();
                    stream.Close();
                }
                return bytes;
            }
            set 
            {
                using (var memoryStream = new System.IO.MemoryStream(value))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = memoryStream;
                    image.EndInit();

                    p_image = image;
                }
            }
        }


        public void LoadImage(Uri uri)
        {
            p_image = new BitmapImage(uri);
        }

        public enum DnDCreatureSize
        {
            Tiny = 0,
            Small,
            Medium,
            Large,
            Huge,
            Gargantuan
        }

        public enum DnDCreatureCondition
        {
            Blinded = 0,
            Charmed,
            Deafened,
            Frightened,
            Grappled,
            Incapacitated,
            Invisible,
            Paralyzed,
            Petrified,
            Poisoned,
            Prone,
            Restrained,
            Stunned,
            Unconscious,
            Exhaustion,
        }

        private Dictionary<DnDCreatureCondition, bool> p_conditions;

        public Dictionary<DnDCreatureCondition, bool> Conditions
        {
            get { return p_conditions; }
            set { p_conditions = value; }
        }


        private DnDArmorType p_dndArmorType;

        public DnDArmorType EquipedArmorType
        {
            get { return p_dndArmorType; }
            set { p_dndArmorType = value; UpdateArmorClass(); }
        }
        public DnDCreature UpdateDexterityStats()
        {
            UpdateArmorClass();
            UpdateInitiative();
            return this;
        }
        public DnDCreature SetRandomHP()
        {
            uint rolls = p_hitDice.Roll();
            p_hp = new Slider<uint>(0u, rolls,
                rolls);
            return this;
        }
        public DnDCreature SetAverageHP()
        {
            p_hp = new Slider<uint>(0u, p_hitDice.DiceExprAverageResult,
                p_hitDice.DiceExprAverageResult);
            return this;
        }
        public static DiceExpr GetDefaultHitDices(DnDCreature creature)
        {
            Dice hitDice = DiceOP.D8();
            uint diceCount = 1u;

            switch (creature.p_creatureSize)
            {
                case DnDCreatureSize.Tiny:
                    hitDice = DiceOP.D4();
                    break;
                case DnDCreatureSize.Small:
                    hitDice = DiceOP.D6();
                    break;
                case DnDCreatureSize.Large:
                    hitDice = DiceOP.D10();
                    break;
                case DnDCreatureSize.Huge:
                    hitDice = DiceOP.D12();
                    break;
                case DnDCreatureSize.Gargantuan:
                    hitDice = DiceOP.D20();
                    break;
            }
            if (creature.p_creatureLevel > 1)
                diceCount = (uint)creature.p_creatureLevel;

            var hitDices = new DiceExpr(diceCount, hitDice,
                (int)diceCount * creature.GetAbilityModifierValue(DndCreatureAbility.Constitution));
            return hitDices;
        }
        public DnDCreature UpdateHitDicesFromStats()
        {
            p_hitDice = GetDefaultHitDices(this);
            return this;
        }
        public DnDCreature UpdateBurrowSpeed()
        {
            if (!p_burrowSpeedFeet.Bonus)
                p_burrowSpeedFeet.Value = 0;
            return this;
        }
        public DnDCreature UpdateSwimSpeed()
        {
            if (!p_swimSpeedFeet.Bonus)
                p_swimSpeedFeet.Value = ((int)BaseSpeed / 2);
            return this;
        }
        public DnDCreature UpdateFlySpeed()
        {
            if (!p_flySpeedFeet.Bonus)
                p_flySpeedFeet.Value = 0;
            return this;
        }
        public DnDCreature UpdateClimbSpeed()
        {
            if (!p_climbSpeedFeet.Bonus)
                p_climbSpeedFeet.Value = ((int)BaseSpeed / 2);
            return this;
        }
        public DnDCreature UpdateSpeed()
        {
            UpdateBurrowSpeed();
            UpdateClimbSpeed();
            UpdateSwimSpeed();
            UpdateFlySpeed();
            return this;
        }
        public DnDCreature UpdateProficiencyBonus()
        {
            p_proficiencyBonus = 2u;
            if (p_creatureLevel > 0)
                p_proficiencyBonus += (uint)((p_creatureLevel - 1) / 4);
            return this;
        }
        public DnDCreature UpdateInitiative()
        {
            p_initiative = GetAbilityModifierValue(DndCreatureAbility.Dexterity);
            return this;
        }
        public DnDCreature UpdateArmorClass()
        {
            p_armorClass = GetArmorClass();
            if (p_armorClass > 30)
            {
                p_armorClass = 30;
            }
            return this;
        }
        public uint GetArmorClass()
        {
            return GetArmorClass(p_armorValue, (uint)GetAbilityModifierValue(DndCreatureAbility.Dexterity));
        }
        public uint GetArmorClass(uint armor, uint modDex)
        {
            switch (p_dndArmorType)
            {
                case DnDArmorType.LightArmor:
                    return armor +
                        modDex;
                case DnDArmorType.MediumArmor:
                    return armor +
                        ((modDex) > 2
                        ? 2u : modDex);
                case DnDArmorType.HeavyArmor:
                    return armor;
                default:
                    return armor +
                        modDex;
            }
        }

        private List<TList> GetItemsFromDict<TList, TValue>
            (Dictionary<TList, ValueWithBonus<TValue, int>> dict, TValue searchedValue)
        {
            List<TList> result = new List<TList>();
            foreach (var item in dict)
            {
                if (item.Value.Value.Equals(searchedValue))
                    result.Add(item.Key);
            }
            return result;
        }
        private List<TList> GetItemsFromDict<TList, TValue>
            (Dictionary<TList, TValue> dict, TValue searchedValue)
        {
            List<TList> result = new List<TList>();
            foreach (var item in dict)
            {
                if (item.Value.Equals(searchedValue))
                    result.Add(item.Key);
            }
            return result;
        }
        public void SetConditionImmunity(DnDCreatureCondition condition, bool immunity) 
        {
            p_conditions[condition] = immunity;
        }
        public bool GetConditionImmunity(DnDCreatureCondition condition)
        {
            return p_conditions[condition];
        }
        public List<DnDCreatureCondition> GetConditionsList
            (bool searchedValue = true)
        {
            return GetItemsFromDict(p_conditions, searchedValue);
        }
        public List<DnDDamageType> GetDmgTypeResistanceList
            (DnDCreatureResistance resistance)
        {
            return GetItemsFromDict(p_dmgResist, resistance);
        }
        public List<DndCreatureAbility> GetSaveThrowsList(bool prof = true)
        {
            return GetItemsFromDict(p_saveThrows, prof);
        }
        public List<DndCreatureSkill> GetSkillsList(DndCreatureSkillProf prof)
        {
            return GetItemsFromDict(p_skills, prof);
        }
        private DnDCreatureSize p_creatureSize;

        public DnDCreatureSize CreatureSize
        {
            get { return p_creatureSize; }
            set { p_creatureSize = value; }
        }

        public enum DnDCreatureType
        {
            None = 0,
            Aberrations,
            Beasts,
            Celestials,
            Constructs,
            Dragons,
            Elementals,
            Fey,
            Fiends,
            Giants,
            Humanoids,
            Monstrosities,
            Oozes,
            Plants,
            Undead,
        }

        private DnDCreatureType p_creatureType;

        public DnDCreatureType CreatureType
        {
            get { return p_creatureType; }
            set { p_creatureType = value; }
        }

        public enum DnDCreatureAlignment
        {
            None = 0,
            LawfullGood,
            LawfullNeutral,
            LawfullEvil,
            NeutralGood,
            TrueNeutral,
            NeutralEvil,
            ChaoticGood,
            ChaoticNeutral,
            ChaoticEvil,
        }

        private DnDCreatureAlignment p_creatureAlignment;

        public DnDCreatureAlignment CreatureAlignment
        {
            get { return p_creatureAlignment; }
            set { p_creatureAlignment = value; }
        }

        private int p_initiative;

        public int Initiative
        {
            get { return p_initiative; }
            set { p_initiative = value; }
        }

        private int p_initiativeBonus;

        public int InitiativeBonus
        {
            get { return p_initiativeBonus; }
            set { p_initiativeBonus = value; }
        }

        

        public uint Exp
        {
            get
            {
                return DnDUtils.GetExpForKill(this);
            }
        }

        public int InitiativeFull
        {
            get { return p_initiativeBonus + p_initiative; }
        }

        private BasicEntity p_features;
        private BasicEntity p_actions;
        private BasicEntity p_legendaryActions;
        private BasicEntity p_reactions;
        private BasicEntity p_lairActions;
        private BasicEntity p_regionalEffects;

        public BasicEntity Features { get => p_features; set=> p_features = value; }
        public BasicEntity Actions { get => p_actions; set => p_actions = value; }
        public BasicEntity LegendaryActions { get => p_legendaryActions; set => p_legendaryActions = value; }
        public BasicEntity Reactions { get => p_reactions; set => p_reactions = value; }
        public BasicEntity LairActions { get => p_lairActions; set => p_lairActions = value; }
        public BasicEntity RegionalEffects { get => p_regionalEffects; set => p_regionalEffects = value; }

        private string p_conditionImmunity;

        public string ConditionImmunity
        {
            get { return p_conditionImmunity; }
            set { p_conditionImmunity = value; }
        }


        private string p_armorsText;

        public string ArmorsProf
        {
            get { return p_armorsText; }
            set { p_armorsText = value; }
        }

        private string p_senses;

        public string Senses
        {
            get { return p_senses; }
            set { p_senses = value; }
        }

        private string p_weaponsText;

        public string WeaponsProf
        {
            get { return p_weaponsText; }
            set { p_weaponsText = value; }
        }

        private string p_languages;

        public string Languages
        {
            get { return p_languages; }
            set { p_languages = value; }
        }

        private uint p_armorValue;

        public uint ArmorValue
        {
            get { return p_armorValue; }
            set { p_armorValue = value; UpdateArmorClass(); }
        }

        private uint p_armorClass;

        public uint ArmorClass
        {
            get { return p_armorClass; }
            private set { p_armorClass = value; UpdateArmorClass(); }
        }
        private int p_armorClassBonus;

        public int ArmorClassBonus
        {
            get { return p_armorClassBonus; }
            set { p_armorClassBonus = value; }
        }
        public int ArmorClassFull
        {
            get { return p_armorClassBonus + (int)p_armorClass; }
        }

        private Slider<uint> p_hp;

        public Slider<uint> HP
        {
            get { return p_hp; }
            set { p_hp = value; }
        }

        private uint p_tempHp;

        public uint TemporaryHP
        {
            get { return p_tempHp; }
            set { p_tempHp = value; }
        }

        private DiceExpr p_hitDice;

        public DiceExpr HitDices
        {
            get { return (DiceExpr)p_hitDice.Clone(); }
            set { p_hitDice = value; }
        }


        private int p_creatureLevel;

        public int RawCreatureLevel
        {
            get { return p_creatureLevel; }
            set
            {
                if (!(value > -4 && value < 31))
                {
                    p_creatureLevel = 0;
                }
                p_creatureLevel = value; UpdateProficiencyBonus();
            }
        }
        public string CreatureLevel
        {
            get 
            {
                return DnDUtils.RawLevelToStringLevel(p_creatureLevel);
            }
            set 
            {
                RawCreatureLevel = DnDUtils.StringLevelToRawLevel(value);
            }
        }

        //
        //  SPEED
        //

        private ValueWithBonus<double, bool> p_baseSpeedFeet;
        public bool CanWalk
        {
            get => p_baseSpeedFeet.Bonus;
            set => p_baseSpeedFeet.Bonus = value;
        }
        public double BaseSpeed
        {
            get { return p_baseSpeedFeet.Value; }
            set { p_baseSpeedFeet.Value = value; UpdateSpeed(); }
        }
        
        private ValueWithBonus<double, bool> p_burrowSpeedFeet;
        public bool HaveBurrowSpeed
        {
            get => p_burrowSpeedFeet.Bonus;
            set => p_burrowSpeedFeet.Bonus = value;
        }
        public double BurrowSpeed
        {
            get { return p_burrowSpeedFeet.Value; }
            set { p_burrowSpeedFeet.Value = value ; UpdateBurrowSpeed(); }
        }
        

        private ValueWithBonus<double, bool> p_swimSpeedFeet;
        public bool HaveSwimSpeed
        {
            get => p_swimSpeedFeet.Bonus;
            set => p_swimSpeedFeet.Bonus = value;
        }
        public double SwimSpeed
        {
            get { return p_swimSpeedFeet.Value; }
            set { p_swimSpeedFeet.Value = value; UpdateSwimSpeed(); }
        }
        

        private ValueWithBonus<double, bool> p_climbSpeedFeet;
        public bool HaveClimbSpeed
        {
            get => p_climbSpeedFeet.Bonus;
            set => p_climbSpeedFeet.Bonus = value;
        }
        public double ClimbSpeed
        {
            get { return p_climbSpeedFeet.Value; }
            set { p_climbSpeedFeet.Value = value; UpdateClimbSpeed(); }
        }
        

        private ValueWithBonus<double, bool> p_flySpeedFeet;
        public bool HaveFlySpeed
        {
            get => p_flySpeedFeet.Bonus;
            set => p_flySpeedFeet.Bonus = value;
        }
        public double FlySpeed
        {
            get { return p_flySpeedFeet.Value; }
            set { p_flySpeedFeet.Value = value; UpdateFlySpeed(); }
        }
        

        public bool HaveAtLeastOneUniqueSpeed { get=> HaveFlySpeed || HaveClimbSpeed 
                || HaveBurrowSpeed || HaveSwimSpeed; }
        //
        //  ABILITIES
        //

        public enum DndCreatureAbility
        {
            Strength = 0,
            Dexterity,
            Constitution,
            Intelligence,
            Wisdom,
            Charisma,
        }

        public uint WeightCarry { get => 15u * GetAbilityValue(DndCreatureAbility.Strength); }


        private Dictionary<DndCreatureAbility, ValueWithBonus<uint, int>> p_abilities;

        public uint GetAbilityValue(DndCreatureAbility ability)
        {
            return p_abilities[ability].Value;
        }

        public void SetAbilityValue(DndCreatureAbility ability, uint abilityValue)
        {
            p_abilities[ability] = NewStat(abilityValue, p_abilities[ability].Bonus);
            switch (ability)
            {
                case DndCreatureAbility.Strength:
                    break;
                case DndCreatureAbility.Dexterity:
                    UpdateDexterityStats();
                    break;
                case DndCreatureAbility.Constitution:
                    break;
                case DndCreatureAbility.Intelligence:
                    break;
                case DndCreatureAbility.Wisdom:
                    break;
                case DndCreatureAbility.Charisma:
                    break;
            }
            
        }
        
        public int GetAbilityModifierValue(DndCreatureAbility ability)
        {
            return (int)p_abilities[ability].Value / 2 - 5 + p_abilities[ability].Bonus;
        }

        //
        //  SAVE THROWS
        //

        private Dictionary<DndCreatureAbility,ValueWithBonus<bool, int>> p_saveThrows;

        public bool GetSaveThrowProf(DndCreatureAbility ability)
        {
            return p_saveThrows[ability].Value;
        }

        public void SetSaveThrowProf(DndCreatureAbility ability, bool saveThrowProf)
        {
            p_saveThrows[ability] = NewStat(saveThrowProf, p_saveThrows[ability].Bonus);
        }
        public int GetSaveThrowBonus(DndCreatureAbility ability)
        {
            return p_saveThrows[ability].Bonus;
        }
        public int GetSaveThrowProfCheckBonus(DndCreatureAbility ability)
        {
            return p_saveThrows[ability].Bonus +
                (p_saveThrows[ability].Value ? (int)p_proficiencyBonus : 0) +
                GetAbilityModifierValue(ability);
        }

        //
        //  SKILLS
        //

        private uint p_proficiencyBonus;

        public uint ProficiencyBonus
        {
            get { return p_proficiencyBonus; }
            set { p_proficiencyBonus = value; }
        }

        public enum DndCreatureSkill
        {
            Str_Athletics = 0,
            Dex_Acrobatics,
            Dex_SleightOfHand,
            Dex_Stealth,
            Int_Arcana,
            Int_History,
            Int_Investigation,
            Int_Nature,
            Int_Religion,
            Wis_AnimalHandling,
            Wis_Insight,
            Wis_Medicine,
            Wis_Perception,
            Wis_Survival,
            Cha_Deception,
            Cha_Intimidation,
            Cha_Performance,
            Cha_Persuasion,
        }

        

        public enum DndCreatureSkillProf
        {
            None = 0,
            Proficiency,
            Mastery,
        }

        private Dictionary<DndCreatureSkill, ValueWithBonus<DndCreatureSkillProf, int>> p_skills;

        public int GetMinPassiveCheck(DndCreatureSkill skill)
        {
            return GetSkillCheckBonus(skill) + 5;
        }
        public int GetPassiveCheck(DndCreatureSkill skill)
        {
            return GetSkillCheckBonus(skill) + 10;
        }
        public int GetMaxPassiveCheck(DndCreatureSkill skill)
        {
            return GetSkillCheckBonus(skill) + 15;
        }

        public void SetSkillProficiency(DndCreatureSkill skill, DndCreatureSkillProf proficiency)
        {
            p_skills[skill] = NewStat(proficiency, p_skills[skill].Bonus);
        }
        public DndCreatureSkillProf GetSkillProficiency(DndCreatureSkill skill)
        {
            return p_skills[skill].Value;
        }
        public void SetSkillProficiencyBonus(DndCreatureSkill skill, int proficiencyBonus)
        {
            p_skills[skill] = NewStat(p_skills[skill].Value, proficiencyBonus); 
        }
        public int SetSkillBonus(DndCreatureSkill skill)
        {
            return p_skills[skill].Bonus;
        }
        public int GetSkillCheckBonus(DndCreatureSkill skill)
        {
            return p_skills[skill].Bonus + GetAbilityModifierValue( DnDUtils.SkillToAbility(skill) )+ (int)p_proficiencyBonus * 
                (int)GetSkillProficiency(skill);
        }

        //
        //  Vulnerabilities, Resistances, and Immunities
        //


        

        public enum DnDCreatureResistance
        {
            None = 0,
            Vulnerability,
            Resistance,
            Immunity
        }

        private Dictionary<DnDDamageType, DnDCreatureResistance> p_dmgResist;

        public Dictionary<DnDDamageType, DnDCreatureResistance> DMGResist 
        { get => p_dmgResist; set => p_dmgResist = value; }

        public void SetResistOfDamageType(DnDDamageType dmgType, DnDCreatureResistance dmgResist )
        {
            p_dmgResist[dmgType] = dmgResist;
        }
        public DnDCreatureResistance GetResistOfDamageType(DnDDamageType dmgType)
        {
            return p_dmgResist[dmgType];
        }
    }

    [Serializable]
    public class DnDCreatureMagicCaster : DnDCreature
    {
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

    public abstract class DnDCreatureBuilder
    {
        public enum DnDCreatureType
        {
            Default =0,
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
