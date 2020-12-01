using System;
using System.Threading.Tasks;
using Nethereum.JsonRpc.Client;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.StandardTokenEIP20.Events.DTO;
using Wallet.Core;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Hex.HexConvertors.Extensions;
using Wallet.Models;
using System.Linq;
using Xamarin.Forms.Internals;
using Nethereum.Contracts;
using Nethereum.StandardTokenEIP20;
using Nethereum.StandardTokenEIP20.ContractDefinition;

namespace Wallet.Services
{
    public interface IAccountsManager
    {
        string DefaultAccountAddress { get; }

        Task<string[]> GetAccountsAsync();

        Task<decimal> GetTokensAsync(string accountAddress);
        Task<decimal> GetBalanceInETHAsync(string accountAddress);

        Task<TransactionModel[]> GetTransactionsAsync(bool sent = false);

        Task<string> TransferAsync(string from, string to, decimal amount);
    }

    public class AccountsManager : IAccountsManager
    {
        const string CONTRACT_ADDRESS = "0xCFa31A040D2D389E34490B637ef116714e759329";

        public string DefaultAccountAddress => DefaultAccount?.Address;

        Account DefaultAccount => walletManager.Wallet?.GetAccount(0);

        readonly IWalletManager walletManager;
        StandardTokenService standardTokenService;
        Web3 web3;

        public AccountsManager(IWalletManager walletManager)
        {
            this.walletManager = walletManager;

            Initialize();
        }

        public async Task<string[]> GetAccountsAsync()
        {
            return new string[] { DefaultAccountAddress };
        }

        public async Task<decimal> GetTokensAsync(string accountAddress)
        {
            var wei = await standardTokenService.BalanceOfQueryAsync(accountAddress);

            return (decimal)wei;
        }

        public async Task<decimal> GetBalanceInETHAsync(string accountAddress)
        {
            var balanceInWei = await web3.Eth.GetBalance.SendRequestAsync(accountAddress);

            return Web3.Convert.FromWei(balanceInWei);
        }

        public async Task<string> TransferAsync(string from, string to, decimal amount)
        {
            var receipt = await standardTokenService.TransferFromRequestAndWaitForReceiptAsync(from, to, new System.Numerics.BigInteger((int)amount));

            return receipt.TransactionHash;
        }

        public Task<TransactionModel[]> GetTransactionsAsync(bool sent = false)
        {
            return Task.Run(async delegate
            {
                var transferEvent = standardTokenService.GetTransferEvent();

                var paddedAccountAddress = DefaultAccountAddress.RemoveHexPrefix()
                                                 .PadLeft(64, '0')
                                                 .EnsureHexPrefix();

                var filter = transferEvent.CreateFilterInput(
                    new object[] { sent ? paddedAccountAddress : null },
                    new object[] { sent ? null : paddedAccountAddress },
                    BlockParameter.CreateEarliest(),
                    BlockParameter.CreateLatest());

                var changes = await transferEvent.GetAllChanges(filter);

                var timestampTasks = changes.Select(x => Task.Factory.StartNew(async (state) =>
                {
                    var log = (EventLog<TransferEventDTO>)state;

                    var block = await web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(log.Log.BlockNumber);

                    return new TransactionModel
                    {
                        Sender = log.Event.From,
                        Receiver = log.Event.To,
                        Amount = (decimal)log.Event.Value,
                        Inward = false == sent,
                        Timestamp = (long)block.Timestamp.Value
                    };
                }, x));

                return await Task.WhenAll(timestampTasks).ContinueWith(tt => {
                    return tt.Result.Select(x => x.Result).ToArray();
                });
            });
        }

        void Initialize()
        {
            
            var client = new RpcClient(new Uri("https://rinkeby.infura.io/v3/78e8427cc8b54e50b5100fc28e30e059"));

            web3 = new Web3(DefaultAccount, client);
            standardTokenService = new StandardTokenService(web3, CONTRACT_ADDRESS);
        }
    }

}
