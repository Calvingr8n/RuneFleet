using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using RuneFleet.Models;

namespace RuneFleet.Services
{
    /// <summary>
    /// Provides a higher level interface for managing accounts and client processes.
    /// </summary>
    internal class ClientController
    {
        private readonly AccountManager accountManager = new();
        private readonly ClientProcessService clientService;

        public List<Account> Accounts => accountManager.Accounts;

        public ClientController(Form form, FlowLayoutPanel panel, Action refreshList)
        {
            clientService = new ClientProcessService(form, panel, accountManager.Accounts, refreshList);
        }

        public void LoadAccounts(string path)
        {
            accountManager.Load(path);
        }

        public IEnumerable<string> GetGroups()
        {
            return accountManager.GetGroups();
        }

        public IEnumerable<Account> GetAccounts(string? group)
        {
            return accountManager.GetAccounts(group);
        }

        public Account? GetAccountByDisplayName(string displayName)
        {
            return accountManager.GetAccountByDisplayName(displayName);
        }

        public void LaunchClient(Account acc)
        {
            clientService.LaunchClient(acc);
        }

        public void RefreshClients()
        {
            clientService.RefreshProcessDisplay();
        }

        public Task WatchForClientsAsync(CancellationToken token)
        {
            return clientService.WatchForClientsAsync(token);
        }

        public void FocusClient(int pid)
        {
            clientService.FocusClient(pid);
        }
    }
}

