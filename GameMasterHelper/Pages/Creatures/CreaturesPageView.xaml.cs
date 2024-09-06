using GameMasterHelper.Logic;
using GameMasterHelper.Logic.DnD;
using GameMasterHelper.Manage;
using GameMasterHelper.View.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameMasterHelper.Pages.Creatures
{
    /// <summary>
    /// Логика взаимодействия для CreaturesPageView.xaml
    /// </summary>
    public partial class CreaturesPageView : Page
    {
        private void UpdateAbility(AttributeInfoTextBox aitb,
            DnDCreature.DndCreatureAbility dndCreatureAbility)
        {
            aitb.AttrValue = p_creatureRef.Creature
                .GetAbilityValue(dndCreatureAbility).ToString();
            aitb.AttrMod = string.Format("({0})", DnDUtils.ToStringInfo((int)p_creatureRef.Creature
                .GetAbilityModifierValue(dndCreatureAbility)));
        }
        private string GetStrForSpeed(double speed, string info = "")
        {
            string result = new StringBuilder()
                .Append(info)
                .Append(" ")
                .Append(string.Format("{0} ф [{1} м] ", speed, DnDUtils.FeetToMeters(speed)))
                .ToString();
            return result;
        }
        private string GetSpeedInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetStrForSpeed(p_creatureRef.Creature.BaseSpeed));
            if(p_creatureRef.Creature.HaveAtLeastOneUniqueSpeed)
            {
                sb.Append("(")
                .Append((!p_creatureRef.Creature.HaveClimbSpeed ?
                    "" : GetStrForSpeed(p_creatureRef.Creature.ClimbSpeed, "лазая")))
                .Append((!p_creatureRef.Creature.HaveFlySpeed ?
                    "" : GetStrForSpeed(p_creatureRef.Creature.FlySpeed, "летая")))
                .Append((!p_creatureRef.Creature.HaveSwimSpeed ?
                    "" : GetStrForSpeed(p_creatureRef.Creature.SwimSpeed, "плавая")))
                .Append((!p_creatureRef.Creature.HaveBurrowSpeed ?
                    "" : GetStrForSpeed(p_creatureRef.Creature.BurrowSpeed, "копая")))
                .Append(")");
            }
            return sb.ToString();
        }
        private string GetSaveThrows(List<DnDCreature.DndCreatureAbility> abilities)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in abilities)
            {
                sb.Append(string.Format("{0}({1}); ", item.ToString(),
                    DnDUtils.ToStringInfo(p_creatureRef.Creature.GetSaveThrowProfCheckBonus(item))));
            }
            return sb.ToString();
        }
        private string GetSkills(List<DnDCreature.DndCreatureSkill> skills)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in skills)
            {
                sb.Append(string.Format("{0}({1}); ", item.ToString(),
                    DnDUtils.ToStringInfo( p_creatureRef.Creature.GetSkillCheckBonus(item))));
            }
            return sb.ToString();
        }
        private string GetListInfo<TList>(List<TList> someTypes,
            DnDCreature.DnDCreatureResistance targetDmgResistance)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in someTypes)
            {
                sb.Append(string.Format("{0} {1}; ", targetDmgResistance.ToString(),
                    item.ToString()));
            }
            return sb.ToString();
        }
        private string GetConditionsInfo(List<DnDCreature.DnDCreatureCondition> conditionTypes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in conditionTypes)
            {
                sb.Append(string.Format("{0}; ", item.ToString()));
            }
            return sb.ToString();
        }
        private string GetResistanceInfo(List<DnDDamageType> damageTypes,
            DnDCreature.DnDCreatureResistance targetDmgResistance)
        {
            return GetListInfo(damageTypes, targetDmgResistance);
        }
        void UpdateSkills()
        {
            var skills = Enum.GetNames(typeof(DnDCreature.DndCreatureSkill)).ToList();
            List<Tuple<string, string>> tmpList = new List<Tuple<string, string>>();
            int i = 0;
            foreach (var item in skills)
            {
                DnDCreature.DndCreatureSkill dndCreatureSkill = (DnDCreature.DndCreatureSkill)i;
                tmpList.Add(new Tuple<string, string>(string.Format("({1})\t{0}",
                        item,
                        DnDUtils.SkillToAbility(dndCreatureSkill).ToString()), 
                    string.Format("{0} [{1}/{2}/{3}] {4}",
                        DnDUtils.ToStringInfo(CreatureItem.Creature.GetSkillCheckBonus(dndCreatureSkill)),
                        CreatureItem.Creature.GetMinPassiveCheck(dndCreatureSkill),
                        CreatureItem.Creature.GetPassiveCheck(dndCreatureSkill),
                        CreatureItem.Creature.GetMaxPassiveCheck(dndCreatureSkill),
                        CreatureItem.Creature.GetSkillProficiency(dndCreatureSkill).ToString())));
                ++i;
            }
            spSkills.Skills = tmpList;
        }
        void UpdateSaveThrows()
        {
            var abilities = Enum.GetNames(typeof(DnDCreature.DndCreatureAbility)).ToList();
            List<Tuple<string, string>> tmpList = new List<Tuple<string, string>>();
            int i = 0;
            foreach (var item in abilities)
            {
                DnDCreature.DndCreatureAbility dndCreatureAbility = (DnDCreature.DndCreatureAbility)i;
                tmpList.Add(new Tuple<string, string>(item,
                    string.Format("{0} {1}",
                    DnDUtils.ToStringInfo(CreatureItem.Creature.GetSaveThrowProfCheckBonus(dndCreatureAbility)),
                    CreatureItem.Creature.GetSaveThrowProf(dndCreatureAbility).ToString())));
                ++i;
            }
            spSaveThrows.Skills = tmpList;
        }
        private void UpdateEntities(Border border, BasicEntity entity)
        {
            border.Visibility = Visibility.Visible;
            if(entity.IsEmpty())
                border.Visibility = Visibility.Collapsed;
        }
        public void UpdatePage()
        {
            
            if(CreatureItem.Creature.ImageID != CreatureItem.Creature.DefaultImageID)
            {
                imgCreature.Source = DataManager.CurrentModule.CreatureImages.GetItem
                    (CreatureItem.Creature.ImageID);
            }
            spMagicCaster.Visibility = Visibility.Collapsed;
            if(CreatureItem.Creature is DnDCreatureMagicCaster)
            {
                spMagicCaster.Visibility = Visibility.Visible;
                tbMagicAttr.Text = (CreatureItem.Creature as DnDCreatureMagicCaster)
                    .SpellCastAbility.ToString();
                tbDCMagic.Text = (CreatureItem.Creature as DnDCreatureMagicCaster)
                    .SpellDC.ToString();
            }
            tbCreatureAligment.Text = p_creatureRef.Creature.CreatureAlignment.ToString();
            tbCreatureLevel.Text = string.Format("[{0}]", p_creatureRef.Creature.CreatureLevel);
            
            tbCreatureName.Text = p_creatureRef.Creature.Name;
            tbCreatureSize.Text = p_creatureRef.Creature.CreatureSize.ToString();
            tbCreatureType.Text = p_creatureRef.Creature.CreatureType.ToString();
            tbProfBonus.Text = DnDUtils.ToStringInfo((int)p_creatureRef.Creature.ProficiencyBonus);
            tbDescr.Text = p_creatureRef.Creature.Description;
            tbArmorClass.Text = string.Format("{0} ({1})",
                p_creatureRef.Creature.ArmorClass.ToString(),
                p_creatureRef.Creature.EquipedArmorType.ToString());
            tbInitiative.Text = string.Format("{0}",
                DnDUtils.ToStringInfo(p_creatureRef.Creature.Initiative));
            tbHits.Text = string.Format("{0} ({1})",
                p_creatureRef.Creature.HitDices.DiceExprAverageResult.ToString(),
                p_creatureRef.Creature.HitDices.ToString());

            tbSpeed.Text = GetSpeedInfo();

            UpdateAbility(aitbStr, DnDCreature.DndCreatureAbility.Strength);
            UpdateAbility(aitbDex, DnDCreature.DndCreatureAbility.Dexterity);
            UpdateAbility(aitbCon, DnDCreature.DndCreatureAbility.Constitution);
            UpdateAbility(aitbInt, DnDCreature.DndCreatureAbility.Intelligence);
            UpdateAbility(aitbWis, DnDCreature.DndCreatureAbility.Wisdom);
            UpdateAbility(aitbCha, DnDCreature.DndCreatureAbility.Charisma);


            //tbSkills.Text = new StringBuilder()
            //    .Append(GetSkills(p_creatureRef.Creature.GetSkillsList
            //            (DnDCreature.DndCreatureSkillProf.Mastery)))
            //    .Append(GetSkills(p_creatureRef.Creature.GetSkillsList
            //            (DnDCreature.DndCreatureSkillProf.Proficiency)))
            //    .ToString();
            //tbSaveThrows.Text = new StringBuilder()
            //    .Append(GetSaveThrows(p_creatureRef.Creature.GetSaveThrowsList()))
            //    .ToString();
            spDmg.Visibility = Visibility.Visible;
            spCond.Visibility = Visibility.Visible;
            tbResist.Text = new StringBuilder()
                .Append(GetResistanceInfo(
                    p_creatureRef.Creature.GetDmgTypeResistanceList(
                        DnDCreature.DnDCreatureResistance.Vulnerability),
                    DnDCreature.DnDCreatureResistance.Vulnerability))
                .Append(GetResistanceInfo(
                    p_creatureRef.Creature.GetDmgTypeResistanceList(
                        DnDCreature.DnDCreatureResistance.Resistance),
                    DnDCreature.DnDCreatureResistance.Resistance))
                .Append(GetResistanceInfo(
                    p_creatureRef.Creature.GetDmgTypeResistanceList(
                        DnDCreature.DnDCreatureResistance.Immunity),
                    DnDCreature.DnDCreatureResistance.Immunity))
                .ToString();
            tbCond.Text = new StringBuilder()
                .Append(GetConditionsInfo(
                    CreatureItem.Creature.GetConditionsList()))
                .ToString();

            if(tbResist.Text == string.Empty)
                spDmg.Visibility = Visibility.Collapsed;
            if (tbCond.Text == string.Empty)
                spCond.Visibility = Visibility.Collapsed;

            UpdateSkills();
            UpdateSaveThrows();

            tbExp.Text = p_creatureRef.Creature.Exp.ToString();
            tbWeightCarry.Text = p_creatureRef.Creature.WeightCarry.ToString();

            spLangs.Visibility = Visibility.Visible;
            spWeapons.Visibility = Visibility.Visible;
            spSenses.Visibility = Visibility.Visible;
            spArmors.Visibility = Visibility.Visible;

            tbLangs.Text = p_creatureRef.Creature.Languages;
            tbWeapons.Text = p_creatureRef.Creature.WeaponsProf;
            tbSenses.Text = p_creatureRef.Creature.Senses;
            tbArmors.Text = p_creatureRef.Creature.ArmorsProf;

            if (p_creatureRef.Creature.Languages == string.Empty)
                spLangs.Visibility = Visibility.Collapsed;
            if (p_creatureRef.Creature.WeaponsProf == string.Empty)
                spWeapons.Visibility = Visibility.Collapsed;
            if (p_creatureRef.Creature.Senses == string.Empty)
                spSenses.Visibility = Visibility.Collapsed;
            if (p_creatureRef.Creature.ArmorsProf == string.Empty)
                spArmors.Visibility = Visibility.Collapsed;

            UpdateEntities(bActions, p_creatureRef.Creature.Actions);
            tbActions.Text = p_creatureRef.Creature.Actions.Description;
            UpdateEntities(bReactions, p_creatureRef.Creature.Reactions);
            tbReactions.Text = p_creatureRef.Creature.Reactions.Description;
            UpdateEntities(bFeatures, p_creatureRef.Creature.Features);
            tbFeatures.Text = p_creatureRef.Creature.Features.Description;
            UpdateEntities(bLegActions, p_creatureRef.Creature.LegendaryActions);
            tbLegActions.Text = p_creatureRef.Creature.LegendaryActions.Description;
            UpdateEntities(bLairActions, p_creatureRef.Creature.LairActions);
            tbLairActions.Text = p_creatureRef.Creature.LairActions.Description;
            UpdateEntities(bRegEffects, p_creatureRef.Creature.RegionalEffects);
            tbRegEffects.Text = p_creatureRef.Creature.RegionalEffects.Description;
        }
        static public void UpdateThisPage(CreaturesPageView page)
        {
            page.UpdatePage();
        }
        public CreaturesPageView(CreaturesPage.ListItemCreature creature)
        {
            InitializeComponent();
            CreatureItem = creature;
        }

        private CreaturesPage.ListItemCreature p_creatureRef;

        public CreaturesPage.ListItemCreature CreatureItem
        {
            get { return p_creatureRef; }
            set { p_creatureRef = value; UpdatePage(); }
        }
    }
}
