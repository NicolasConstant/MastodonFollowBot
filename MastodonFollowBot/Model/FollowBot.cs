using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MastodonFollowBot.Settings;
using Mastonet;
using Mastonet.Entities;
using System.Globalization;

namespace MastodonFollowBot.Model
{
    public class FollowBot
    {
        private readonly AppSettings _config;

        #region Ctor
        public FollowBot(AppSettings config)
        {
            _config = config;
        }
        #endregion

        public async Task Run()
        {
            foreach (var account in GetAllAccounts(_config.Source))
            {
                await FollowAccount(_config.Target, account);
            }
        }

        private async Task FollowAccount(AppAccount configTarget, Account mastodonAccount)
        {
            var client = await GetClient(configTarget);

            await client.Follow(mastodonAccount.ProfileUrl);
        }

        public IEnumerable<Account> GetAllAccounts(AppAccount configSource)
        {
            var iter = 0;
            var clientTask = GetClient(configSource);
            clientTask.Wait();

            for (;;)
            {
                if (iter % 100 == 0) Console.WriteLine($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)} - Iteration: {iter}");

                var accountTask = clientTask.Result.GetAccount(iter);
                accountTask.Wait();

                var account = accountTask.Result;
                if (account == null) yield break;
                yield return account;

                iter++;
            }
        }

        private static async Task<MastodonClient> GetClient(AppAccount config)
        {
            var authClient = new AuthenticationClient(config.InstanceName);
            var auth = await authClient.ConnectWithPassword(config.Email, config.Password);
            var client = new MastodonClient(config.AppRegistration, auth);
            return client;
        }
    }
}