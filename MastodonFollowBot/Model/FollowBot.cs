using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MastodonFollowBot.Settings;
using System.Globalization;
using mastodon;
using mastodon.Enums;
using mastodon.Models;

namespace MastodonFollowBot.Model
{
    public class FollowBot
    {
        //private readonly AppSettings _config;
        private readonly MastodonClient _mastodonClientSource;
        private readonly MastodonClient _mastodonClientTarget;
        private readonly string _accessTokenSource;
        private readonly string _accessTokenTarget;
        private readonly string _instanceNameSource;


        #region Ctor
        public FollowBot(AppSettings config)
        {
            //_config = config;
            _mastodonClientSource = new MastodonClient(config.Source.InstanceName);
            _mastodonClientTarget = new MastodonClient(config.Target.InstanceName);

            _accessTokenSource = GetToken(config.Source);
            _accessTokenTarget = GetToken(config.Target);

            _instanceNameSource = config.Source.InstanceName.Replace("https://", "")
                .Replace("http:/", "")
                .Replace("/", "");
        }
        #endregion

        public void Run()
        {
            foreach (var account in GetAllAccounts(_mastodonClientSource, _accessTokenSource))
            {
                FollowAccount(_mastodonClientTarget, _accessTokenTarget, account);
            }
        }

        private void FollowAccount(MastodonClient client, string token, Account mastodonAccount)
        {
            var userId = mastodonAccount.acct;
            if (!userId.Contains("@")) userId = $"@{mastodonAccount.acct}@{_instanceNameSource}";

            try
            {
                client.FollowRemote(userId, token);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private IEnumerable<Account> GetAllAccounts(MastodonClient client, string token)
        {
            var iter = 1;
            var usersNotFound = 0;
            
            for (;;)
            {
                if (iter % 100 == 0)
                {
                    Console.WriteLine($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)} - Iteration: {iter}");
                    if (usersNotFound >= 100)
                    {
                        Console.WriteLine("No users founds");
                        yield break;
                    }
                    usersNotFound = 0;
                }

                Account account = null;
                try
                {
                    account = client.GetAccount(iter, token);
                }
                catch (Exception)
                {
                    usersNotFound++;
                }

                //if (account == null) yield break;
                if(account != null) yield return account;

                iter++;
            }
        }

        private string GetToken(AppAccount config)
        {
            var authClient = new AuthHandler(config.InstanceName);
            var token = authClient.GetTokenInfo(config.AppClientId, config.AppClientSecret,  config.Email, config.Password, AppScopeEnum.Follow | AppScopeEnum.Read | AppScopeEnum.Write);
            return token.access_token;
        }
    }
}