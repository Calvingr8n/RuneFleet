using System;
using System.Windows.Forms;

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

            int newIndex = 0;

            if (id == pgDownId)
            {
                newIndex = (currentIndex + 1) % listViewAccounts.Items.Count;
            }
            else if (id == pgUpId)
            {
                newIndex = (currentIndex - 1 + listViewAccounts.Items.Count) % listViewAccounts.Items.Count;
            }
            else if (id == delId)
            {
                newIndex = 0;
            }

            listViewAccounts.Items[newIndex].Selected = true;
            listViewAccounts.Select();
            listViewAccounts.Focus();

            listViewAccounts.Items[newIndex].Focused = true;
            itemActivate();
        }
    }
}
