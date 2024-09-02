using GameMasterHelper.Logic.DnD;
using GameMasterHelper.View.UserControls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для CreaturesPageEdit.xaml
    /// </summary>
    public partial class CreaturesPageEdit : Page
    {
        public void UpdatePage()
        {
            gMagicCaster.Visibility = Visibility.Collapsed;
            if (CreatureItem.Creature is DnDCreatureMagicCaster)
            {
                var caster = CreatureItem.Creature as DnDCreatureMagicCaster;
                gMagicCaster.Visibility = Visibility.Visible;

                cbMagicCasterAttr.SelectedIndex =(int)caster.SpellCastAbility;
            }
            
            tbpName.Text = CreatureItem.Creature.Name;
            rtbDescr.Text = CreatureItem.Creature.Description;

            tbpArmors.Text = CreatureItem.Creature.ArmorsProf;
            tbpLanguages.Text= CreatureItem.Creature.Languages;
            tbpSenses.Text = CreatureItem.Creature.Senses;
            tbpWeapons.Text = CreatureItem.Creature.WeaponsProf;

            dctbBaseSpeed.Text = ((uint)CreatureItem.Creature.BaseSpeed).ToString();

            rtbActions.IsEnabled = ((chkbActions.IsChecked = !CreatureItem.Creature.Actions
                .IsEmpty()) ?? false);
            rtbReactions.IsEnabled = ((chkbReactions.IsChecked = !CreatureItem.Creature.Reactions
                .IsEmpty()) ?? false);
            rtbFeatures.IsEnabled = ((chkbFeatures.IsChecked = !CreatureItem.Creature.Features
                .IsEmpty()) ?? false);
            rtbLegActions.IsEnabled = ((chkbLegActions.IsChecked = !CreatureItem.Creature.LegendaryActions
                .IsEmpty()) ?? false);
            rtbLairActions.IsEnabled = ((chkbLairActions.IsChecked = !CreatureItem.Creature.LairActions
                .IsEmpty()) ?? false);
            rtbRegEffects.IsEnabled = ((chkbRegEffects.IsChecked = !CreatureItem.Creature.RegionalEffects
                .IsEmpty()) ?? false);

            rtbActions.Text = CreatureItem.Creature.Actions.Description;
            rtbReactions.Text = CreatureItem.Creature.Reactions.Description;
            rtbFeatures.Text = CreatureItem.Creature.Features.Description;
            rtbLegActions.Text = CreatureItem.Creature.LegendaryActions.Description;
            rtbLairActions.Text = CreatureItem.Creature.LairActions.Description;
            rtbRegEffects.Text = CreatureItem.Creature.RegionalEffects.Description;

            dctbBurrowSpeed.IsEnabled = (chkbHaveBurrow.IsChecked = CreatureItem.Creature.HaveBurrowSpeed) ?? false;
            dctbBurrowSpeed.Text = ((uint)CreatureItem.Creature.BurrowSpeed).ToString();

            dctbSwimSpeed.IsEnabled = (chkbHaveSwim.IsChecked = CreatureItem.Creature.HaveSwimSpeed) ?? false;
            dctbSwimSpeed.Text = ((uint)CreatureItem.Creature.SwimSpeed).ToString();

            dctbFlySpeed.IsEnabled = (chkbHaveFly.IsChecked = CreatureItem.Creature.HaveFlySpeed) ?? false;
            dctbFlySpeed.Text = ((uint)CreatureItem.Creature.FlySpeed).ToString();

            dctbClimbSpeed.IsEnabled = (chkbHaveClimb.IsChecked = CreatureItem.Creature.HaveClimbSpeed) ?? false;
            dctbClimbSpeed.Text = ((uint)CreatureItem.Creature.ClimbSpeed).ToString();

            dctbCha.Text = CreatureItem.Creature
                .GetAbilityValue(DnDCreature.DndCreatureAbility.Charisma).ToString();
            dctbWis.Text = CreatureItem.Creature
                .GetAbilityValue(DnDCreature.DndCreatureAbility.Wisdom).ToString();
            dctbInt.Text = CreatureItem.Creature
                .GetAbilityValue(DnDCreature.DndCreatureAbility.Intelligence).ToString();
            dctbCon.Text = CreatureItem.Creature
                .GetAbilityValue(DnDCreature.DndCreatureAbility.Constitution).ToString();
            dctbDex.Text = CreatureItem.Creature
                .GetAbilityValue(DnDCreature.DndCreatureAbility.Dexterity).ToString();
            dctbStr.Text = CreatureItem.Creature
                .GetAbilityValue(DnDCreature.DndCreatureAbility.Strength).ToString();

            cbAligment.SelectedIndex = (int)CreatureItem.Creature.CreatureAlignment;
            cbSizes.SelectedIndex = (int)CreatureItem.Creature.CreatureSize;
            cbTypes.SelectedIndex = (int)CreatureItem.Creature.CreatureType;
            cbLvl.SelectedIndex = CreatureItem.Creature.RawCreatureLevel + 3;

            cbSaveThrowStr.SelectedIndex = CreatureItem.Creature.GetSaveThrowProf(DnDCreature.DndCreatureAbility.Strength) ? 1 : 0;
            cbSaveThrowDex.SelectedIndex = CreatureItem.Creature.GetSaveThrowProf(DnDCreature.DndCreatureAbility.Dexterity) ? 1 : 0;
            cbSaveThrowCon.SelectedIndex = CreatureItem.Creature.GetSaveThrowProf(DnDCreature.DndCreatureAbility.Constitution) ? 1 : 0;
            cbSaveThrowInt.SelectedIndex = CreatureItem.Creature.GetSaveThrowProf(DnDCreature.DndCreatureAbility.Intelligence) ? 1 : 0;
            cbSaveThrowWis.SelectedIndex = CreatureItem.Creature.GetSaveThrowProf(DnDCreature.DndCreatureAbility.Wisdom) ? 1 : 0;
            cbSaveThrowCha.SelectedIndex = CreatureItem.Creature.GetSaveThrowProf(DnDCreature.DndCreatureAbility.Charisma) ? 1 : 0;
            UpdateSkillCB();
            UpdateConditionImmunityCB();
            UpdateDMGResist();
            cbArmorType.SelectedIndex = (int)CreatureItem.Creature.EquipedArmorType;
            dctbArmorClass.Text = CreatureItem.Creature.ArmorValue.ToString();
            dctbHitDice.Text = p_creatureRef.Creature.HitDices.ToString();
        }
        
        public void UpdateCreature()
        {
            dctbArmorClass.CheckData();
            dctbDex.CheckData();

            if (File.Exists(tbPathToImage.Text))
            {
                CreatureItem.Creature.LoadImage(new Uri(tbPathToImage.Text, UriKind.Absolute));
                var trans = new ScaleTransform(
                    300.0 / (double)CreatureItem.Creature.Image.PixelWidth,
                    400.0 / (double)CreatureItem.Creature.Image.PixelHeight);
                
                var bmp = new TransformedBitmap(CreatureItem.Creature.Image, trans);
                CreatureItem.Creature.Image = bmp;
            }

            if (CreatureItem.Creature is DnDCreatureMagicCaster)
            {
                var caster = CreatureItem.Creature as DnDCreatureMagicCaster;
                caster.SpellCastAbility = 
                    (DnDCreature.DndCreatureAbility)cbMagicCasterAttr.SelectedIndex;
            }

            CreatureItem.Creature.ArmorsProf = tbpArmors.Text;
            CreatureItem.Creature.Languages= tbpLanguages.Text;
            CreatureItem.Creature.Senses = tbpSenses.Text;
            CreatureItem.Creature.WeaponsProf = tbpWeapons.Text;

            CreatureItem.Creature.HaveBurrowSpeed = chkbHaveBurrow.IsChecked ?? false;
            CreatureItem.Creature.HaveSwimSpeed = chkbHaveSwim.IsChecked ?? false;
            CreatureItem.Creature.HaveFlySpeed = chkbHaveFly.IsChecked ?? false;
            CreatureItem.Creature.HaveClimbSpeed = chkbHaveClimb.IsChecked ?? false;

            CreatureItem.Creature.BurrowSpeed = double.Parse(dctbBurrowSpeed.Text);
            CreatureItem.Creature.SwimSpeed = double.Parse(dctbSwimSpeed.Text);
            CreatureItem.Creature.FlySpeed = double.Parse(dctbFlySpeed.Text);
            CreatureItem.Creature.ClimbSpeed = double.Parse(dctbClimbSpeed.Text);
            CreatureItem.Creature.BaseSpeed = double.Parse(dctbBaseSpeed.Text);

            CreatureItem.Creature.Actions.Name = chkbActions.IsChecked == true ? "1" : "null";
            CreatureItem.Creature.Reactions.Name = chkbReactions.IsChecked == true ? "1" : "null";
            CreatureItem.Creature.Features.Name = chkbFeatures.IsChecked == true ? "1" : "null";
            CreatureItem.Creature.LegendaryActions.Name = chkbLegActions.IsChecked == true ? "1" : "null";
            CreatureItem.Creature.LairActions.Name = chkbLairActions.IsChecked == true ? "1" : "null";
            CreatureItem.Creature.RegionalEffects.Name = chkbRegEffects.IsChecked == true ? "1" : "null";

            CreatureItem.Creature.Actions.Description = rtbActions.Text;
            CreatureItem.Creature.Reactions.Description = rtbReactions.Text;
            CreatureItem.Creature.Features.Description = rtbFeatures.Text;
            CreatureItem.Creature.LegendaryActions.Description = rtbLegActions.Text;
            CreatureItem.Creature.LairActions.Description = rtbLairActions.Text;
            CreatureItem.Creature.RegionalEffects.Description = rtbRegEffects.Text;

            dctbBurrowSpeed.IsEnabled = (chkbHaveBurrow.IsChecked = CreatureItem.Creature.HaveBurrowSpeed) ?? false;
            dctbBurrowSpeed.Text = ((uint)CreatureItem.Creature.BurrowSpeed).ToString();

            dctbSwimSpeed.IsEnabled = (chkbHaveSwim.IsChecked = CreatureItem.Creature.HaveSwimSpeed) ?? false;
            dctbSwimSpeed.Text = ((uint)CreatureItem.Creature.SwimSpeed).ToString();

            dctbFlySpeed.IsEnabled = (chkbHaveFly.IsChecked = CreatureItem.Creature.HaveFlySpeed) ?? false;
            dctbFlySpeed.Text = ((uint)CreatureItem.Creature.FlySpeed).ToString();

            dctbClimbSpeed.IsEnabled = (chkbHaveClimb.IsChecked = CreatureItem.Creature.HaveClimbSpeed) ?? false;
            dctbClimbSpeed.Text = ((uint)CreatureItem.Creature.ClimbSpeed).ToString();

            CreatureItem.Creature.Name = tbpName.Text;
            CreatureItem.Creature.Description = rtbDescr.Text;
            CreatureItem.Creature.CreatureAlignment = 
                (DnDCreature.DnDCreatureAlignment)cbAligment.SelectedIndex;
            CreatureItem.Creature.CreatureSize =
                (DnDCreature.DnDCreatureSize)cbSizes.SelectedIndex;
            CreatureItem.Creature.CreatureType =
                (DnDCreature.DnDCreatureType)cbTypes.SelectedIndex;
            CreatureItem.Creature.RawCreatureLevel = cbLvl.SelectedIndex - 3;

            UpdateCreatureAbility(DnDCreature.DndCreatureAbility.Strength, dctbStr);
            UpdateCreatureAbility(DnDCreature.DndCreatureAbility.Dexterity, dctbDex);
            UpdateCreatureAbility(DnDCreature.DndCreatureAbility.Constitution, dctbCon);
            UpdateCreatureAbility(DnDCreature.DndCreatureAbility.Intelligence, dctbInt);
            UpdateCreatureAbility(DnDCreature.DndCreatureAbility.Wisdom, dctbWis);
            UpdateCreatureAbility(DnDCreature.DndCreatureAbility.Charisma, dctbCha);

            CreatureItem.Creature.SetSaveThrowProf(DnDCreature.DndCreatureAbility.Strength,
                cbSaveThrowStr.SelectedIndex == 1 ? true : false);
            CreatureItem.Creature.SetSaveThrowProf(DnDCreature.DndCreatureAbility.Dexterity,
                cbSaveThrowDex.SelectedIndex == 1 ? true : false);
            CreatureItem.Creature.SetSaveThrowProf(DnDCreature.DndCreatureAbility.Constitution,
                cbSaveThrowCon.SelectedIndex == 1 ? true : false);
            CreatureItem.Creature.SetSaveThrowProf(DnDCreature.DndCreatureAbility.Intelligence,
                cbSaveThrowInt.SelectedIndex == 1 ? true : false);
            CreatureItem.Creature.SetSaveThrowProf(DnDCreature.DndCreatureAbility.Wisdom,
                cbSaveThrowWis.SelectedIndex == 1 ? true : false);
            CreatureItem.Creature.SetSaveThrowProf(DnDCreature.DndCreatureAbility.Charisma,
                cbSaveThrowCha.SelectedIndex == 1 ? true : false);
            UpdateCreatureSkills();
            UpdateConditionImmunityCBToCreature();
            UpdateCreatureDMGResist();
            CreatureItem.Creature.EquipedArmorType = (DnDArmorType)cbArmorType.SelectedIndex;
            if (dctbArmorClass.Success)
            {
                CreatureItem.Creature.ArmorValue = uint.Parse(dctbArmorClass.Text);
            }
                
            if (dctbHitDice.Success)
                    CreatureItem.Creature.HitDices = DiceOP.GetDnDDiceExprFromStr(dctbHitDice.Text);

            CreatureItem.UpdateFromDnDCreature();
            p_pageView.UpdatePage();
        }
        private void UpdateCreatureAbility(DnDCreature.DndCreatureAbility ability,
            DataCheckTextBox dctb)
        {
            if (dctb.Success)
                CreatureItem.Creature.SetAbilityValue(ability,
                    uint.Parse(dctb.Text));
        }
        private void UpdateConditionImmunityCBToCreature()
        {
            int i = 0;
            var condList = Enum.GetNames(typeof(DnDCreature.DnDCreatureCondition));
            foreach (var child in ResistGrid.Children)
            {

                if (child is CheckBox)
                {
                    CreatureItem.Creature
                        .SetConditionImmunity(
                            (DnDCreature.DnDCreatureCondition)i,
                            ((child as CheckBox).IsChecked) ?? false);
                    ++i;
                }
            }
        }
        private void UpdateConditionImmunityCB()
        {
            int i = 0;
            var condList = Enum.GetNames(typeof(DnDCreature.DnDCreatureCondition));
            foreach (var child in ResistGrid.Children)
            {

                if (child is CheckBox)
                {
                    (child as CheckBox).IsChecked = CreatureItem.Creature
                        .GetConditionImmunity((DnDCreature.DnDCreatureCondition)i);
                    ++i;
                }
            }
        }
        private void InitConditionImmunityCB()
        {
            int i = 0;
            var condList = Enum.GetNames(typeof(DnDCreature.DnDCreatureCondition));
            foreach (var child in ResistGrid.Children)
            {

                if (child is CheckBox)
                {
                    var specChild = child as CheckBox;
                    specChild.Content = condList[i];
                    ++i;
                }
            }
        }
        private void InitDMGResistTB()
        {
            int i = 0;
            foreach (var child in ResistGrid.Children)
            {

                if (child is Grid)
                {
                    var specChild = child as Grid;
                    foreach (var tb in specChild.Children)
                    {
                        if (tb is TextBlock)
                        {
                            (tb as TextBlock).Text = ((DnDDamageType)i).ToString();
                            ++i;
                        }
                    }
                }
            }
        }
        private void InitDMGResistCB()
        {
            var listResists = Enum.GetNames(typeof(DnDCreature.DnDCreatureResistance)).ToList();

            foreach (var child in ResistGrid.Children)
            {
                if (child is Grid)
                {
                    var specChild = child as Grid;
                    foreach (var cb in specChild.Children)
                    {
                        if (cb is ComboBox)
                        {
                            (cb as ComboBox).ItemsSource = listResists;
                        }
                    }
                }
            }
        }
        private void UpdateDMGResist()
        {
            int i = 0;
            foreach (var child in ResistGrid.Children)
            {
                if (child is Grid)
                {
                    var specChild = child as Grid;
                    foreach (var cb in specChild.Children)
                    {
                        if (cb is ComboBox)
                        {
                            (cb as ComboBox).SelectedIndex =
                                (int)CreatureItem.Creature
                                .GetResistOfDamageType((DnDDamageType)i);
                            ++i;
                        }
                    }
                }
            }
        }
        private void UpdateCreatureDMGResist()
        {
            int i = 0;
            foreach (var child in ResistGrid.Children)
            {
                if (child is Grid)
                {
                    var specChild = child as Grid;
                    foreach (var cb in specChild.Children)
                    {
                        if (cb is ComboBox)
                        {
                            CreatureItem.Creature.SetResistOfDamageType((DnDDamageType)i,
                                (DnDCreature.DnDCreatureResistance)((cb as ComboBox).SelectedIndex));
                            ++i;
                        }
                    }
                }
            }
        }
        private void InitSkillsTB()
        {
            int i = 0;
            tbStrSkill0.Text = ((DnDCreature.DndCreatureSkill)i).ToString(); ++i;
            tbDexSkill0.Text = ((DnDCreature.DndCreatureSkill)i).ToString(); ++i;
            tbDexSkill1.Text = ((DnDCreature.DndCreatureSkill)i).ToString(); ++i;
            tbDexSkill2.Text = ((DnDCreature.DndCreatureSkill)i).ToString(); ++i;
            tbIntSkill0.Text = ((DnDCreature.DndCreatureSkill)i).ToString(); ++i;
            tbIntSkill1.Text = ((DnDCreature.DndCreatureSkill)i).ToString(); ++i;
            tbIntSkill2.Text = ((DnDCreature.DndCreatureSkill)i).ToString(); ++i;
            tbIntSkill3.Text = ((DnDCreature.DndCreatureSkill)i).ToString(); ++i;
            tbIntSkill4.Text = ((DnDCreature.DndCreatureSkill)i).ToString(); ++i;
            tbWisSkill0.Text = ((DnDCreature.DndCreatureSkill)i).ToString(); ++i;
            tbWisSkill1.Text = ((DnDCreature.DndCreatureSkill)i).ToString(); ++i;
            tbWisSkill2.Text = ((DnDCreature.DndCreatureSkill)i).ToString(); ++i;
            tbWisSkill3.Text = ((DnDCreature.DndCreatureSkill)i).ToString(); ++i;
            tbWisSkill4.Text = ((DnDCreature.DndCreatureSkill)i).ToString(); ++i;
            tbChaSkill0.Text = ((DnDCreature.DndCreatureSkill)i).ToString(); ++i;
            tbChaSkill1.Text = ((DnDCreature.DndCreatureSkill)i).ToString(); ++i;
            tbChaSkill2.Text = ((DnDCreature.DndCreatureSkill)i).ToString(); ++i;
            tbChaSkill3.Text = ((DnDCreature.DndCreatureSkill)i).ToString();
        }
        private void InitSkillsCB()
        {
            var listSkillsProf = Enum.GetNames(typeof(DnDCreature.DndCreatureSkillProf)).ToList();
            
            cbStrSkill0.ItemsSource = listSkillsProf;
            cbDexSkill0.ItemsSource = listSkillsProf;
            cbDexSkill1.ItemsSource = listSkillsProf;
            cbDexSkill2.ItemsSource = listSkillsProf;
            cbIntSkill0.ItemsSource = listSkillsProf;
            cbIntSkill1.ItemsSource = listSkillsProf;
            cbIntSkill2.ItemsSource = listSkillsProf;
            cbIntSkill3.ItemsSource = listSkillsProf;
            cbIntSkill4.ItemsSource = listSkillsProf;
            cbWisSkill0.ItemsSource = listSkillsProf;
            cbWisSkill1.ItemsSource = listSkillsProf;
            cbWisSkill2.ItemsSource = listSkillsProf;
            cbWisSkill3.ItemsSource = listSkillsProf;
            cbWisSkill4.ItemsSource = listSkillsProf;
            cbChaSkill0.ItemsSource = listSkillsProf;
            cbChaSkill1.ItemsSource = listSkillsProf;
            cbChaSkill2.ItemsSource = listSkillsProf;
            cbChaSkill3.ItemsSource = listSkillsProf;
        }
        private void UpdateCreatureSkills()
        {
            int i = 0;
            p_creatureRef.Creature.SetSkillProficiency((DnDCreature.DndCreatureSkill)i,
                (DnDCreature.DndCreatureSkillProf)cbStrSkill0.SelectedIndex); ++i;
            p_creatureRef.Creature.SetSkillProficiency((DnDCreature.DndCreatureSkill)i,
                (DnDCreature.DndCreatureSkillProf)cbDexSkill0.SelectedIndex); ++i;
            p_creatureRef.Creature.SetSkillProficiency((DnDCreature.DndCreatureSkill)i,
                (DnDCreature.DndCreatureSkillProf)cbDexSkill1.SelectedIndex); ++i;
            p_creatureRef.Creature.SetSkillProficiency((DnDCreature.DndCreatureSkill)i,
                (DnDCreature.DndCreatureSkillProf)cbDexSkill2.SelectedIndex); ++i;
            p_creatureRef.Creature.SetSkillProficiency((DnDCreature.DndCreatureSkill)i,
                (DnDCreature.DndCreatureSkillProf)cbIntSkill0.SelectedIndex); ++i;
            p_creatureRef.Creature.SetSkillProficiency((DnDCreature.DndCreatureSkill)i,
                (DnDCreature.DndCreatureSkillProf)cbIntSkill1.SelectedIndex); ++i;
            p_creatureRef.Creature.SetSkillProficiency((DnDCreature.DndCreatureSkill)i,
                (DnDCreature.DndCreatureSkillProf)cbIntSkill2.SelectedIndex); ++i;
            p_creatureRef.Creature.SetSkillProficiency((DnDCreature.DndCreatureSkill)i,
                (DnDCreature.DndCreatureSkillProf)cbIntSkill3.SelectedIndex); ++i;
            p_creatureRef.Creature.SetSkillProficiency((DnDCreature.DndCreatureSkill)i,
                (DnDCreature.DndCreatureSkillProf)cbIntSkill4.SelectedIndex); ++i;
            p_creatureRef.Creature.SetSkillProficiency((DnDCreature.DndCreatureSkill)i,
                (DnDCreature.DndCreatureSkillProf)cbWisSkill0.SelectedIndex); ++i;
            p_creatureRef.Creature.SetSkillProficiency((DnDCreature.DndCreatureSkill)i,
                (DnDCreature.DndCreatureSkillProf)cbWisSkill1.SelectedIndex); ++i;
            p_creatureRef.Creature.SetSkillProficiency((DnDCreature.DndCreatureSkill)i,
                (DnDCreature.DndCreatureSkillProf)cbWisSkill2.SelectedIndex); ++i;
            p_creatureRef.Creature.SetSkillProficiency((DnDCreature.DndCreatureSkill)i,
                (DnDCreature.DndCreatureSkillProf)cbWisSkill3.SelectedIndex); ++i;
            p_creatureRef.Creature.SetSkillProficiency((DnDCreature.DndCreatureSkill)i,
                (DnDCreature.DndCreatureSkillProf)cbWisSkill4.SelectedIndex); ++i;
            p_creatureRef.Creature.SetSkillProficiency((DnDCreature.DndCreatureSkill)i,
                (DnDCreature.DndCreatureSkillProf)cbChaSkill0.SelectedIndex); ++i;
            p_creatureRef.Creature.SetSkillProficiency((DnDCreature.DndCreatureSkill)i,
                (DnDCreature.DndCreatureSkillProf)cbChaSkill1.SelectedIndex); ++i;
            p_creatureRef.Creature.SetSkillProficiency((DnDCreature.DndCreatureSkill)i,
                (DnDCreature.DndCreatureSkillProf)cbChaSkill2.SelectedIndex); ++i;
            p_creatureRef.Creature.SetSkillProficiency((DnDCreature.DndCreatureSkill)i,
                (DnDCreature.DndCreatureSkillProf)cbChaSkill3.SelectedIndex);
        }
        private void UpdateSkillCB()
        {
            int i = 0;

            cbStrSkill0.SelectedIndex = (int)p_creatureRef.Creature.GetSkillProficiency((DnDCreature.DndCreatureSkill)i); ++i;
            cbDexSkill0.SelectedIndex = (int)p_creatureRef.Creature.GetSkillProficiency((DnDCreature.DndCreatureSkill)i); ++i;
            cbDexSkill1.SelectedIndex = (int)p_creatureRef.Creature.GetSkillProficiency((DnDCreature.DndCreatureSkill)i); ++i;
            cbDexSkill2.SelectedIndex = (int)p_creatureRef.Creature.GetSkillProficiency((DnDCreature.DndCreatureSkill)i); ++i;
            cbIntSkill0.SelectedIndex = (int)p_creatureRef.Creature.GetSkillProficiency((DnDCreature.DndCreatureSkill)i); ++i;
            cbIntSkill1.SelectedIndex = (int)p_creatureRef.Creature.GetSkillProficiency((DnDCreature.DndCreatureSkill)i); ++i;
            cbIntSkill2.SelectedIndex = (int)p_creatureRef.Creature.GetSkillProficiency((DnDCreature.DndCreatureSkill)i); ++i;
            cbIntSkill3.SelectedIndex = (int)p_creatureRef.Creature.GetSkillProficiency((DnDCreature.DndCreatureSkill)i); ++i;
            cbIntSkill4.SelectedIndex = (int)p_creatureRef.Creature.GetSkillProficiency((DnDCreature.DndCreatureSkill)i); ++i;
            cbWisSkill0.SelectedIndex = (int)p_creatureRef.Creature.GetSkillProficiency((DnDCreature.DndCreatureSkill)i); ++i;
            cbWisSkill1.SelectedIndex = (int)p_creatureRef.Creature.GetSkillProficiency((DnDCreature.DndCreatureSkill)i); ++i;
            cbWisSkill2.SelectedIndex = (int)p_creatureRef.Creature.GetSkillProficiency((DnDCreature.DndCreatureSkill)i); ++i;
            cbWisSkill3.SelectedIndex = (int)p_creatureRef.Creature.GetSkillProficiency((DnDCreature.DndCreatureSkill)i); ++i;
            cbWisSkill4.SelectedIndex = (int)p_creatureRef.Creature.GetSkillProficiency((DnDCreature.DndCreatureSkill)i); ++i;
            cbChaSkill0.SelectedIndex = (int)p_creatureRef.Creature.GetSkillProficiency((DnDCreature.DndCreatureSkill)i); ++i;
            cbChaSkill1.SelectedIndex = (int)p_creatureRef.Creature.GetSkillProficiency((DnDCreature.DndCreatureSkill)i); ++i;
            cbChaSkill2.SelectedIndex = (int)p_creatureRef.Creature.GetSkillProficiency((DnDCreature.DndCreatureSkill)i); ++i;
            cbChaSkill3.SelectedIndex = (int)p_creatureRef.Creature.GetSkillProficiency((DnDCreature.DndCreatureSkill)i);
        }
        public CreaturesPageEdit(CreaturesPageView pageView)
        {
            
            InitializeComponent();
            InitDMGResistCB();
            tbPathToImage.IsEnabled = false;
            cbMagicCasterAttr.ItemsSource = Enum.GetNames(typeof(DnDCreature.DndCreatureAbility))
                .ToList();
            cbAligment.ItemsSource = Enum.GetNames(typeof(DnDCreature.DnDCreatureAlignment))
                .ToList();
            cbSizes.ItemsSource = Enum.GetNames(typeof(DnDCreature.DnDCreatureSize))
                .ToList();
            cbTypes.ItemsSource = Enum.GetNames(typeof(DnDCreature.DnDCreatureType))
                .ToList();
            cbLvl.ItemsSource = GetLvlsStr();
            var saveThrowsProf = new string[]{ "false", "true" }.ToList();
            cbSaveThrowStr.ItemsSource = saveThrowsProf;
            cbSaveThrowDex.ItemsSource = saveThrowsProf;
            cbSaveThrowCon.ItemsSource = saveThrowsProf;
            cbSaveThrowInt.ItemsSource = saveThrowsProf;
            cbSaveThrowWis.ItemsSource = saveThrowsProf;
            cbSaveThrowCha.ItemsSource = saveThrowsProf;
            InitSkillsTB();
            InitSkillsCB();
            InitDMGResistTB();
            InitConditionImmunityCB();
            cbArmorType.ItemsSource = Enum.GetNames(typeof(DnDArmorType)).ToList();

            CreatureItem = pageView.CreatureItem;
            UpdatePage();
            p_pageView = pageView;
        }
        private CreaturesPageView p_pageView;
        private CreaturesPage.ListItemCreature p_creatureRef;
        public CreaturesPage.ListItemCreature CreatureItem
        {
            get { return p_creatureRef; }
            set { p_creatureRef = value; }
        }

        private void bnSave_Click(object sender, RoutedEventArgs e)
        {
            UpdateCreature();
        }

        private void bnDefault_Click(object sender, RoutedEventArgs e)
        {
            UpdatePage();
        }

        private bool dctbStr_DataCheckEvent(string Data)
        {
            return Regex.IsMatch(Data,@"^((\d)|([1-3]\d))$");
        }
        private List<string> GetLvlsStr()
        {
            var result = new List<string>();
            for (int i = -3; i < 31; i++)
            {
                result.Add(DnDUtils.RawLevelToStringLevel(i));
            }
            return result;
        }

        private void btnDefaultHp_Click(object sender, RoutedEventArgs e)
        {
            dctbHitDice.Text = DnDCreature.GetDefaultHitDices(p_creatureRef.Creature).ToString();
        }

        private bool dctbHitDice_DataCheckEvent(string Data)
        {
            return DiceOP.IsDnDDiceExpr(Data);
        }

        private bool dctbArmorClass_DataCheckEvent(string Data)
        {
            return dctbStr_DataCheckEvent(Data);
        }

        private bool dctbDex_DataCheckEvent(string Data)
        {
            return dctbStr_DataCheckEvent(Data);
        }

        private bool dctbBaseSpeed_DataCheckEvent(string Data)
        {
            return Regex.IsMatch(Data, @"^\d{1,4}$");
        }

        private void chkbHaveBurrow_Checked(object sender, RoutedEventArgs e)
        {
            dctbBurrowSpeed.IsEnabled = true;
        }

        private void chkbHaveBurrow_Unchecked(object sender, RoutedEventArgs e)
        {
            dctbBurrowSpeed.IsEnabled = false ;
        }

        private void chkbHaveSwim_Checked(object sender, RoutedEventArgs e)
        {
            dctbSwimSpeed.IsEnabled = true;
        }

        private void chkbHaveSwim_Unchecked(object sender, RoutedEventArgs e)
        {
            dctbSwimSpeed.IsEnabled = false;
        }

        private void chkbClimbSwim_Checked(object sender, RoutedEventArgs e)
        {
            dctbClimbSpeed.IsEnabled = true;
        }

        private void chkbClimbSwim_Unchecked(object sender, RoutedEventArgs e)
        {
            dctbClimbSpeed.IsEnabled = false;
        }

        private void chkbFlySwim_Checked(object sender, RoutedEventArgs e)
        {
            dctbFlySpeed.IsEnabled = true;
        }

        private void chkbFlySwim_Unchecked(object sender, RoutedEventArgs e)
        {
            dctbFlySpeed.IsEnabled = false;
        }

        private void chkbActions_Checked(object sender, RoutedEventArgs e)
        {
            rtbActions.IsEnabled = true;
        }

        private void chkbActions_Unchecked(object sender, RoutedEventArgs e)
        {
            rtbActions.IsEnabled = false;
        }

        private void chkbReactions_Unchecked(object sender, RoutedEventArgs e)
        {
            rtbReactions.IsEnabled = false;          
        }

        private void chkbReactions_Checked(object sender, RoutedEventArgs e)
        {
            rtbReactions.IsEnabled = true;
        }

        private void chkbFeatures_Checked(object sender, RoutedEventArgs e)
        {
            rtbFeatures.IsEnabled = true;
        }

        private void chkbFeatures_Unchecked(object sender, RoutedEventArgs e)
        {
            rtbFeatures.IsEnabled = false;
        }

        private void chkbLegActions_Checked(object sender, RoutedEventArgs e)
        {
            rtbLegActions.IsEnabled = true;
        }

        private void chkbLegActions_Unchecked(object sender, RoutedEventArgs e)
        {
            rtbLegActions.IsEnabled = false;
        }

        private void chkbLairActions_Checked(object sender, RoutedEventArgs e)
        {
            rtbLairActions.IsEnabled = true;
        }

        private void chkbLairActions_Unchecked(object sender, RoutedEventArgs e)
        {
            rtbLairActions.IsEnabled = false;
        }

        private void chkbRegActions_Unchecked(object sender, RoutedEventArgs e)
        {
            rtbRegEffects.IsEnabled = false;
        }

        private void chkbRegActions_Checked(object sender, RoutedEventArgs e)
        {
            rtbRegEffects.IsEnabled = true;
        }

        private void bnView_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            string fileFromat = "Images(*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp";

            ofd.Filter = fileFromat;
            ofd.Title = "Load";

            if(ofd.ShowDialog() ?? false)
            {
                tbPathToImage.Text = ofd.FileName;
            }
        }
    }
}
