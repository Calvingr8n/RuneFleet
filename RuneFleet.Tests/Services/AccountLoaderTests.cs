using System;
using System.IO;
using Xunit;
using RuneFleet.Models;
using RuneFleet.Services;

namespace RuneFleet.Tests.Services
{
    public class AccountLoaderTests
    {
        [Fact]
        public void LoadFromCsv_ReturnsAccountsFromFile()
        {
            // Arrange
            string tempFile = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempFile,
                    "JX_ACCESS_TOKEN,JX_REFRESH_TOKEN,JX_SESSION_ID,JX_DISPLAY_NAME,JX_CHARACTER_ID,Group,Client,Arguments\n" +
                    "atk1,ref1,sid1,display1,char1,g1;g2,client1,args1\n" +
                    "atk2,ref2,sid2,display2,char2,g2,client2,args2\n");

                // Act
                var accounts = AccountLoader.LoadFromCsv(tempFile);

                // Assert
                Assert.Equal(2, accounts.Count);

                Assert.Equal("atk1", accounts[0].AccessToken);
                Assert.Equal("ref1", accounts[0].RefreshToken);
                Assert.Equal("sid1", accounts[0].SessionId);
                Assert.Equal("display1", accounts[0].DisplayName);
                Assert.Equal("char1", accounts[0].CharacterId);
                Assert.Equal(new[] { "g1", "g2" }, accounts[0].Group);
                Assert.Equal("client1", accounts[0].Client);
                Assert.Equal("args1", accounts[0].Arguments);

                Assert.Equal("atk2", accounts[1].AccessToken);
                Assert.Equal("ref2", accounts[1].RefreshToken);
                Assert.Equal("sid2", accounts[1].SessionId);
                Assert.Equal("display2", accounts[1].DisplayName);
                Assert.Equal("char2", accounts[1].CharacterId);
                Assert.Equal(new[] { "g2" }, accounts[1].Group);
                Assert.Equal("client2", accounts[1].Client);
                Assert.Equal("args2", accounts[1].Arguments);
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }
    }
}
