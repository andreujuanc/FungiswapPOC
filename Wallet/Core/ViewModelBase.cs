using Prism.Navigation;
namespace Wallet.Core
{
    public abstract class ViewModelBase : ModelBase, INavigationAware
    {
        bool _ShouldShowNavigationBar = true;
        public bool ShouldShowNavigationBar
        {
            get => _ShouldShowNavigationBar;
            set => SetProperty(ref _ShouldShowNavigationBar, value);
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
        }
    }
}
