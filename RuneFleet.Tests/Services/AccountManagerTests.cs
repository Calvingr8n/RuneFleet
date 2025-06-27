using System;
using System.IO;
using System.Linq;
using Xunit;
using RuneFleet.Services;

namespace RuneFleet.Tests.Services
{
    public class AccountManagerTests
    {
        [Fact]
        public void LoadAndGroupRetrieval_Works()
        {
            string tempFile = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempFile,
                    "JX_ACCESS_TOKEN,JX_REFRESH_TOKEN,JX_SESSION_ID,JX_DISPLAY_NAME,JX_CHARACTER_ID,Group,Client,Arguments\n" +
                    "atk1,ref1,sid1,display1,char1,g1;g2,client1,args1\n" +
                    "atk2,ref2,sid2,display2,char2,g2,client2,args2\n");

                var manager = new AccountManager();
                manager.Load(tempFile);

                Assert.Equal(2, manager.Accounts.Count);
                var groups = manager.GetGroups().ToList();
                Assert.Contains("g1", groups);
                Assert.Contains("g2", groups);
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }
    }
}

