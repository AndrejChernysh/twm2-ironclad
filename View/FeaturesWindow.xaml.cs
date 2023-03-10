using Ironclad.Helper;
using System;
using System.IO;
using System.Windows;

namespace Ironclad
{
    public partial class FeaturesWindow : Window
    {
        public FeaturesWindow()
        {
            InitializeComponent();
            cbPopeMechanics.IsChecked = Properties.Settings.Default.cbPopeMechanics;
            cbAIRecruitmentBoost.IsChecked = Properties.Settings.Default.cbAIRecruitmentBoost;
            cbEventSpawns.IsChecked = Properties.Settings.Default.cbEventSpawns;
            cbSlaveSpawnUnits.IsChecked = Properties.Settings.Default.cbSlaveSpawnUnits;
            cbUnification.IsChecked = Properties.Settings.Default.cbUnification;
            cbMigration.IsChecked = Properties.Settings.Default.cbMigration;
            cbAlliedAssist.IsChecked = Properties.Settings.Default.cbAlliedAssist;
            cbRazing.IsChecked = Properties.Settings.Default.cbRazing;
            cbFirstContact.IsChecked = Properties.Settings.Default.cbFirstContact;
            cbRNGE.IsChecked = Properties.Settings.Default.cbRNGE;
            cbFirstTimeBuild.IsChecked = Properties.Settings.Default.cbFirstTimeBuild;
            cbAICapitalBoost.IsChecked = Properties.Settings.Default.cbAICapitalBoost;
            cbEmpire.IsChecked = Properties.Settings.Default.cbEmpire;
            cbLoans.IsChecked = Properties.Settings.Default.cbLoans;
            cbEmergencyTaxes.IsChecked = Properties.Settings.Default.cbEmergencyTaxes;
            cbLoot.IsChecked = Properties.Settings.Default.cbLoot;
            cbSeaLoot.IsChecked = Properties.Settings.Default.cbSeaLoot;
            cbRaids.IsChecked = Properties.Settings.Default.cbRaids;
            cbDeals.IsChecked = Properties.Settings.Default.cbDeals;
            cbCivilWars.IsChecked = Properties.Settings.Default.cbCivilWars;
            cbRegulation.IsChecked = Properties.Settings.Default.cbRegulation;
            cbReEmergence.IsChecked = Properties.Settings.Default.cbReEmergence;
            cbPeaceseeking.IsChecked = Properties.Settings.Default.cbPeaceseeking;
            cbInvasions.IsChecked = Properties.Settings.Default.cbInvasions;
            cbDebtCrises.IsChecked = Properties.Settings.Default.cbDebtCrises;
            cbCapitalLost.IsChecked = Properties.Settings.Default.cbCapitalLost;
            cbCapitalChange.IsChecked = Properties.Settings.Default.cbCapitalChange;
            cbSiegeRestriction.IsChecked = Properties.Settings.Default.cbSiegeRestriction;
            cbDiplomacyCosts.IsChecked = Properties.Settings.Default.cbDiplomacyCosts;
            cbSelectHeir.IsChecked = Properties.Settings.Default.cbSelectHeir;
            cbExpansionPenalty.IsChecked = Properties.Settings.Default.cbExpansionPenalty;
            cbSiegeCosts.IsChecked = Properties.Settings.Default.cbSiegeCosts;
            cbConsumables.IsChecked = Properties.Settings.Default.cbConsumables;
            cbMobilization.IsChecked = Properties.Settings.Default.cbMobilization;
            cbAutonomies.IsChecked = Properties.Settings.Default.cbAutonomies;
            cbReligiousUnrest.IsChecked = Properties.Settings.Default.cbReligiousUnrest;
            cbGarrisons.IsChecked = Properties.Settings.Default.cbGarrisons;
            cbInformants.IsChecked = Properties.Settings.Default.cbInformants;
            cbBlockades.IsChecked = Properties.Settings.Default.cbBlockades;
            cbForcedMarch.IsChecked = Properties.Settings.Default.cbForcedMarch;
            cbAIExpansion.IsChecked = Properties.Settings.Default.cbAIExpansion;
            cbCouncil.IsChecked = Properties.Settings.Default.cbCouncil;
            cbColonies.IsChecked = Properties.Settings.Default.cbColonies;
            cbBattleAI.IsChecked = Properties.Settings.Default.cbBattleAI;
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.FeatureCount = 0;
            MainWindow.FeatureCount = cbPopeMechanics.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbAIRecruitmentBoost.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbEventSpawns.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbSlaveSpawnUnits.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbUnification.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbMigration.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbAlliedAssist.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbRazing.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbFirstContact.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbRNGE.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbFirstTimeBuild.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbAICapitalBoost.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbEmpire.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbLoans.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbEmergencyTaxes.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbLoot.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbSeaLoot.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbRaids.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbDeals.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbCivilWars.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbRegulation.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbReEmergence.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbPeaceseeking.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbInvasions.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbDebtCrises.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbCapitalLost.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbCapitalChange.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbSiegeRestriction.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbDiplomacyCosts.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbSelectHeir.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbExpansionPenalty.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbSiegeCosts.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbConsumables.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbMobilization.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbAutonomies.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbReligiousUnrest.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbGarrisons.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbInformants.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbBlockades.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbForcedMarch.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbAIExpansion.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbCouncil.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbColonies.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            MainWindow.FeatureCount = cbBattleAI.IsChecked == true ? MainWindow.FeatureCount + 1 : MainWindow.FeatureCount;
            Properties.Settings.Default.FeatureCount = MainWindow.FeatureCount;
            Properties.Settings.Default.cbPopeMechanics = cbPopeMechanics.IsChecked == true;
            Properties.Settings.Default.cbAIRecruitmentBoost = cbAIRecruitmentBoost.IsChecked == true;
            Properties.Settings.Default.cbEventSpawns = cbEventSpawns.IsChecked == true;
            Properties.Settings.Default.cbSlaveSpawnUnits = cbSlaveSpawnUnits.IsChecked == true;
            Properties.Settings.Default.cbUnification = cbUnification.IsChecked == true;
            Properties.Settings.Default.cbMigration = cbMigration.IsChecked == true;
            Properties.Settings.Default.cbAlliedAssist = cbAlliedAssist.IsChecked == true;
            Properties.Settings.Default.cbRazing = cbRazing.IsChecked == true;
            Properties.Settings.Default.cbFirstContact = cbFirstContact.IsChecked == true;
            Properties.Settings.Default.cbRNGE = cbRNGE.IsChecked == true;
            Properties.Settings.Default.cbFirstTimeBuild = cbFirstTimeBuild.IsChecked == true;
            Properties.Settings.Default.cbAICapitalBoost = cbAICapitalBoost.IsChecked == true;
            Properties.Settings.Default.cbEmpire = cbEmpire.IsChecked == true;
            Properties.Settings.Default.cbLoans = cbLoans.IsChecked == true;
            Properties.Settings.Default.cbEmergencyTaxes = cbEmergencyTaxes.IsChecked == true;
            Properties.Settings.Default.cbLoot = cbLoot.IsChecked == true;
            Properties.Settings.Default.cbSeaLoot = cbSeaLoot.IsChecked == true;
            Properties.Settings.Default.cbRaids = cbRaids.IsChecked == true;
            Properties.Settings.Default.cbDeals = cbDeals.IsChecked == true;
            Properties.Settings.Default.cbCivilWars = cbCivilWars.IsChecked == true;
            Properties.Settings.Default.cbRegulation = cbRegulation.IsChecked == true;
            Properties.Settings.Default.cbReEmergence = cbReEmergence.IsChecked == true;
            Properties.Settings.Default.cbPeaceseeking = cbPeaceseeking.IsChecked == true;
            Properties.Settings.Default.cbInvasions = cbInvasions.IsChecked == true;
            Properties.Settings.Default.cbDebtCrises = cbDebtCrises.IsChecked == true;
            Properties.Settings.Default.cbCapitalLost = cbCapitalLost.IsChecked == true;
            Properties.Settings.Default.cbCapitalChange = cbCapitalChange.IsChecked == true;
            Properties.Settings.Default.cbSiegeRestriction = cbSiegeRestriction.IsChecked == true;
            Properties.Settings.Default.cbDiplomacyCosts = cbDiplomacyCosts.IsChecked == true;
            Properties.Settings.Default.cbSelectHeir = cbSelectHeir.IsChecked == true;
            Properties.Settings.Default.cbExpansionPenalty = cbExpansionPenalty.IsChecked == true;
            Properties.Settings.Default.cbSiegeCosts = cbSiegeCosts.IsChecked == true;
            Properties.Settings.Default.cbConsumables = cbConsumables.IsChecked == true;
            Properties.Settings.Default.cbMobilization = cbMobilization.IsChecked == true;
            Properties.Settings.Default.cbAutonomies = cbAutonomies.IsChecked == true;
            Properties.Settings.Default.cbReligiousUnrest = cbReligiousUnrest.IsChecked == true;
            Properties.Settings.Default.cbGarrisons = cbGarrisons.IsChecked == true;
            Properties.Settings.Default.cbInformants = cbInformants.IsChecked == true;
            Properties.Settings.Default.cbBlockades = cbBlockades.IsChecked == true;
            Properties.Settings.Default.cbForcedMarch = cbForcedMarch.IsChecked == true;
            Properties.Settings.Default.cbAIExpansion = cbAIExpansion.IsChecked == true;
            Properties.Settings.Default.cbCouncil = cbCouncil.IsChecked == true;
            Properties.Settings.Default.cbColonies = cbColonies.IsChecked == true;
            Properties.Settings.Default.cbBattleAI = cbBattleAI.IsChecked == true;
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void btnCheckAll_Click(object sender, RoutedEventArgs e)
        {
            cbPopeMechanics.IsChecked = true;
            cbAIRecruitmentBoost.IsChecked = true;
            cbEventSpawns.IsChecked = true;
            cbSlaveSpawnUnits.IsChecked = true;
            cbUnification.IsChecked = true;
            cbMigration.IsChecked = true;
            cbAlliedAssist.IsChecked = true;
            cbRazing.IsChecked = true;
            cbFirstContact.IsChecked = true;
            cbRNGE.IsChecked = true;
            cbFirstTimeBuild.IsChecked = true;
            cbAICapitalBoost.IsChecked = true;
            cbEmpire.IsChecked = true;
            cbLoans.IsChecked = true;
            cbEmergencyTaxes.IsChecked = true;
            cbLoot.IsChecked = true;
            cbSeaLoot.IsChecked = true;
            cbRaids.IsChecked = true;
            cbDeals.IsChecked = true;
            cbCivilWars.IsChecked = true;
            cbRegulation.IsChecked = true;
            cbReEmergence.IsChecked = true;
            cbPeaceseeking.IsChecked = true;
            cbInvasions.IsChecked = true;
            cbDebtCrises.IsChecked = true;
            cbCapitalLost.IsChecked = true;
            cbCapitalChange.IsChecked = true;
            cbSiegeRestriction.IsChecked = true;
            cbDiplomacyCosts.IsChecked = true;
            cbSelectHeir.IsChecked = true;
            cbExpansionPenalty.IsChecked = true;
            cbSiegeCosts.IsChecked = true;
            cbConsumables.IsChecked = true;
            cbMobilization.IsChecked = true;
            cbAutonomies.IsChecked = true;
            cbReligiousUnrest.IsChecked = true;
            cbGarrisons.IsChecked = true;
            cbInformants.IsChecked = true;
            cbBlockades.IsChecked = true;
            cbForcedMarch.IsChecked = true;
            cbAIExpansion.IsChecked = true;
            cbCouncil.IsChecked = true;
            cbColonies.IsChecked = true;
            cbBattleAI.IsChecked = true;
        }

        private void btnUncheckAll_Click(object sender, RoutedEventArgs e)
        {
            cbPopeMechanics.IsChecked = false;
            cbAIRecruitmentBoost.IsChecked = false;
            cbEventSpawns.IsChecked = false;
            cbSlaveSpawnUnits.IsChecked = false;
            cbUnification.IsChecked = false;
            cbAlliedAssist.IsChecked = false;
            cbMigration.IsChecked = false;
            cbRazing.IsChecked = false;
            cbFirstContact.IsChecked = false;
            cbRNGE.IsChecked = false;
            cbFirstTimeBuild.IsChecked = false;
            cbAICapitalBoost.IsChecked = false;
            cbEmpire.IsChecked = false;
            cbLoans.IsChecked = false;
            cbEmergencyTaxes.IsChecked = false;
            cbLoot.IsChecked = false;
            cbSeaLoot.IsChecked = false;
            cbRaids.IsChecked = false;
            cbDeals.IsChecked = false;
            cbCivilWars.IsChecked = false;
            cbRegulation.IsChecked = false;
            cbReEmergence.IsChecked = false;
            cbPeaceseeking.IsChecked = false;
            cbInvasions.IsChecked = false;
            cbDebtCrises.IsChecked = false;
            cbCapitalLost.IsChecked = false;
            cbCapitalChange.IsChecked = false;
            cbSiegeRestriction.IsChecked = false;
            cbDiplomacyCosts.IsChecked = false;
            cbSelectHeir.IsChecked = false;
            cbExpansionPenalty.IsChecked = false;
            cbSiegeCosts.IsChecked = false;
            cbConsumables.IsChecked = false;
            cbMobilization.IsChecked = false;
            cbAutonomies.IsChecked = false;
            cbReligiousUnrest.IsChecked = false;
            cbGarrisons.IsChecked = false;
            cbInformants.IsChecked = false;
            cbBlockades.IsChecked = false;
            cbForcedMarch.IsChecked = false;
            cbAIExpansion.IsChecked = false;
            cbCouncil.IsChecked = false;
            cbColonies.IsChecked = false;
            cbBattleAI.IsChecked = false;
        }
    }
}
