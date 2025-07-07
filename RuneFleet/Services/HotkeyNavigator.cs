using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace RuneFleet.Services
{
    internal static class HotkeyNavigator
    {
        public static void Handle(ref Message m, int wmHotkey, ListView listViewAccounts,
            int pgDownId, int pgUpId, int delId, Action itemActivate)
        {
            if (m.Msg != wmHotkey)
                return;

            int id = m.WParam.ToInt32();
            if (listViewAccounts.Items.Count == 0)
                return;

            // Only consider items that have a PID assigned (sub item index 1)
            var validIndices = new List<int>();
            for (int i = 0; i < listViewAccounts.Items.Count; i++)
            {
                var item = listViewAccounts.Items[i];
                if (item.SubItems.Count > 1 && !string.IsNullOrWhiteSpace(item.SubItems[1].Text))
                {
                    validIndices.Add(i);
                }
            }

            if (validIndices.Count == 0)
                return;

            int currentIndex;
            if (listViewAccounts.SelectedIndices.Count > 0)
            {
                currentIndex = listViewAccounts.SelectedIndices[0];
                listViewAccounts.Items[currentIndex].Selected = false;
                listViewAccounts.Items[currentIndex].Focused = false;
            }
            else
            {
                currentIndex = -1;
            }

            int currentPos = validIndices.IndexOf(currentIndex);
            int newPos = 0;

            if (id == pgDownId)
            {
                newPos = (currentPos + 1) % validIndices.Count;
            }
            else if (id == pgUpId)
            {
                newPos = (currentPos - 1 + validIndices.Count) % validIndices.Count;
            }
            else if (id == delId)
            {
                newPos = 0;
            }

            int newIndex = validIndices[newPos];
            listViewAccounts.Items[newIndex].Selected = true;
            listViewAccounts.Select();
            listViewAccounts.Focus();

            listViewAccounts.Items[newIndex].Focused = true;
            itemActivate();
        }
    }
}
