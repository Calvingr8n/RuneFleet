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

                Assert.Equal("atk1", manager.Accounts[0].AccessToken);
                Assert.Equal("ref1", manager.Accounts[0].RefreshToken);
                Assert.Equal("sid1", manager.Accounts[0].SessionId);
                Assert.Equal("display1", manager.Accounts[0].DisplayName);
                Assert.Equal("char1", manager.Accounts[0].CharacterId);
                Assert.Equal(new[] { "g1", "g2" }, manager.Accounts[0].Group);
                Assert.Equal("client1", manager.Accounts[0].Client);
                Assert.Equal("args1", manager.Accounts[0].Arguments);

                Assert.Equal("atk2", manager.Accounts[1].AccessToken);
                Assert.Equal("ref2", manager.Accounts[1].RefreshToken);
                Assert.Equal("sid2", manager.Accounts[1].SessionId);
                Assert.Equal("display2", manager.Accounts[1].DisplayName);
                Assert.Equal("char2", manager.Accounts[1].CharacterId);
                Assert.Equal(new[] { "g2" }, manager.Accounts[1].Group);
                Assert.Equal("client2", manager.Accounts[1].Client);
                Assert.Equal("args2", manager.Accounts[1].Arguments);

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

