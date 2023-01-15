using Ironclad.Entities;
using Ironclad.Helper;
using System.IO;
using System.Reflection;
using System.Text;

namespace Ironclad.Features
{
    static class SelectHeir
    {
        static StringBuilder c = new StringBuilder();

        public static Script Get()
        {
            var isAlwaysActive = false;
            var scriptGroup = "SelectHeir";
            if (Properties.Settings.Default.cbSelectHeir || isAlwaysActive)
            {
                c.Clear();
                HEGenerator.Add($"selcan", $"Designate Next Heir", $"Choose the family member who will be the next heir of the faction?||" +
                    $"If you decline, this option will not reappear until the factions leader dies. Choose a family member and press the zoom button to pick your desired heir.", $"@68");
                HEGenerator.Add($"selected", $"Inheritance Granted", $"The selected general will have rights of inheritance to the throne, followed by the current heir.", $"@31");
                c.Append("\nmonitor_event CeasedFactionHeir CharacterIsLocal");
                c.Append("\n\tand I_CompareCounter candidate = 00");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append("\n\t\thistoric_event selcan true");
                c.Append("\n\t\tset_counter first_candidate 1");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append("\nend_monitor");
                c.Append("\nmonitor_event BrotherAdopted CharacterIsLocal");
                c.Append("\n\tand I_TurnNumber >= 0");
                c.Append("\n\tand I_CompareCounter candidate = 0");
                c.Append("\n\tand I_CompareCounter first_candidate = 0");
                c.Append("\n\tand I_CompareCounter active_heir_bucle = 0");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append("\n\t\thistoric_event selcan true");
                c.Append("\n\t\tset_counter first_candidate 1");
                c.Append("\n\t\tset_counter active_heir_bucle 1");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append("\n\t\tterminate_monitor");
                c.Append("\nend_monitor");
                c.Append("\nmonitor_event CharacterComesOfAge FactionIsLocal");
                c.Append("\n\tand IsGeneral");
                c.Append("\n\tand I_CompareCounter candidate = 0");
                c.Append("\n\tand I_CompareCounter first_candidate = 0");
                c.Append("\n\tand I_CompareCounter active_heir_bucle = 0");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append("\n\t\thistoric_event selcan true");
                c.Append("\n\t\tset_counter first_candidate 1");
                c.Append("\n\t\tset_counter active_heir_bucle 1");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append("\n\t\tterminate_monitor");
                c.Append("\nend_monitor");
                c.Append("\nmonitor_event FactionTurnEnd FactionType slave");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append("\n\tset_counter checking 1");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append("\nend_monitor");
                c.Append("\nmonitor_event CharacterTurnEnd CharacterIsLocal");
                c.Append("\n\tand I_TurnNumber > 0");
                c.Append("\n\tand Trait HeirCandidate > 0");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append("\n\t\tset_counter checking 2");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append("\nend_monitor");
                c.Append("\nmonitor_event FactionTurnEnd FactionIsLocal");
                c.Append("\n\tand I_CompareCounter checking = 1");
                c.Append("\n\tand I_CompareCounter active_heir_bucle = 1");
                c.Append("\n\tand I_CompareCounter decline_sh = 0");
                c.Append("\n\tand I_CompareCounter candidate = 0");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append("\n\t\thistoric_event selcan true");
                c.Append("\n\t\tset_counter first_candidate 1");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append("\nend_monitor");
                c.Append("\nmonitor_event EventCounter EventCounterType selcan_accepted");
                c.Append("\n\tand I_EventCounter selcan_accepted = 1");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append(Script.ClickOn("faction_button"));
                c.Append(Script.ClickOn("family_tree_button"));
                c.Append(Script.ClickOn("faction_button"));
                c.Append("\n\t\tui_flash_start family_tree_zoom_to_button");
                c.Append("\n\t\tset_event_counter selcan_accepted 0");
                c.Append("\n\t\tset_counter candidate 1");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append("\nend_monitor");
                c.Append("\nmonitor_event CharacterSelected CharacterIsLocal");
                c.Append("\n\tand I_CompareCounter candidate = 1");
                c.Append("\n\tand I_ScrollOpen family_tree_scroll");
                c.Append("\n\tand IsGeneral");
                c.Append("\n\tand ! IsFactionLeader");
                c.Append("\n\tand ! IsFactionHeir");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append("\n\t\tui_flash_stop");
                c.Append("\n\t\tconsole_command give_trait this HeirCandidate 1");
                c.Append("\n\t\tset_counter candidate 0");
                c.Append("\n\t\thistoric_event selected");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append("\nend_monitor");
                c.Append("\nmonitor_event EventCounter EventCounterType selcan_declined");
                c.Append("\n\tand I_EventCounter selcan_declined = 1");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append("\n\t\tset_counter decline_sh 1");
                c.Append(Script.xl() ? $"\nlog always {MethodBase.GetCurrentMethod().DeclaringType.Name}" : "");
                c.Append("\nend_monitor");
                return new Script(scriptGroup, c.ToString(), isAlwaysActive);
            }
            return new Script(scriptGroup, "", isAlwaysActive);
        }
    }
}
