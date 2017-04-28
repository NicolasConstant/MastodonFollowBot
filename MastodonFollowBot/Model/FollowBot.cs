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


        #region Ctor
        public FollowBot(AppSettings config)
        {
            //_config = config;
            _mastodonClientSource = new MastodonClient(config.Source.InstanceName);
            _mastodonClientTarget = new MastodonClient(config.Target.InstanceName);

            _accessTokenSource = GetToken(config.Source);
            _accessTokenTarget = GetToken(config.Target);
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
            client.FollowRemote(mastodonAccount.url, token);
        }

        private IEnumerable<Account> GetAllAccounts(MastodonClient client, string token)
        {
            var iter = 0;

            for (;;)
            {
                if (iter % 100 == 0) Console.WriteLine($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)} - Iteration: {iter}");

                var account = client.GetAccount(iter, token);

                if (account == null) yield break;
                yield return account;

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