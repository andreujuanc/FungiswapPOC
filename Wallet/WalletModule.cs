using Wallet.Views;
using Wallet.ViewModels;
using Prism.Modularity;
using Prism.Ioc;
using Wallet.Services;
using System;

namespace Wallet
{
    public class WalletModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAccountsManager, AccountsManager>();
            containerRegistry.RegisterSingleton<IWalletManager, WalletManager>();

            containerRegistry.RegisterSingleton<NewWalletController>();
            containerRegistry.RegisterSingleton<RecoverWalletController>();

            containerRegistry.RegisterForNavigation<ScanQRCodeView, ScanQRCodeViewModel>();
            containerRegistry.RegisterForNavigation<UnlockView, UnlockViewModel>();
            containerRegistry.RegisterForNavigation<PasscodeView, PasscodeViewModel>();
            containerRegistry.RegisterForNavigation<PasscodeConfirmationView, PasscodeConfirmationViewModel>();
            containerRegistry.RegisterForNavigation<PassphraseView, PassphraseViewModel>();
            containerRegistry.RegisterForNavigation<PassphraseConfirmationView, PassphraseConfirmationViewModel>();
            containerRegistry.RegisterForNavigation<WalletView, WalletViewModel>();
            containerRegistry.RegisterForNavigation<RecoverView, RecoverViewModel>();
            containerRegistry.RegisterForNavigation<TransactionHistoryView, TransactionHistoryViewModel>();
        }
    }

    public static class NavigationKeys
    {
        public const string ScanQRCode = "__wallet__SCAN_QRCODE";
        public const string UnlockWallet = "__wallet__UNLOCK__UNLOCK_WALLET";
        public const string CreateWallet = "__wallet__UNLOCK__CREATE_WALLET";
        public const string RecoverWallet = "__wallet__UNLOCK__RECOVER_WALLET";
        public const string RecoverWalletOk = "__wallet__UNLOCK__RECOVER_WALLET_OK";
        public const string ConfirmPasscode = "__wallet__PASSCODE_CONFIRMATION";
        public const string ConnfirmPasscodeOk = "__wallet__PASSCODE_CONFIRMATION_OK";
        public const string ConfirmPassphrase = "__wallet__PASSPRASE_CONFIRMATION";
        public const string ConfirmPassphraseOk = "__wallet__PASSPRASE_CONFIRMATION_OK";
        public const string WalletViewHistory = "__wallet__WALLET_VIEW_HISTORY";
    }
}

