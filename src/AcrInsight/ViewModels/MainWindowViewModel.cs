using System.Reactive.Linq;
using System.Threading.Tasks;
using AcrInsight.Models;
using Reactive.Bindings;



namespace AcrInsight.ViewModels
{
    /// <summary>
    /// Provides ViewModel for <see cref="Views.MainWindow"/>
    /// </summary>
    public class MainWindowViewModel
    {
        #region Properties
        /// <summary>
        /// Gets ACR user name.
        /// </summary>
        public ReactiveProperty<string> UserName { get; }


        /// <summary>
        /// Gets ACR password.
        /// </summary>
        public ReactiveProperty<string> Password { get; }


        /// <summary>
        /// Gets ACR login server.
        /// </summary>
        public ReactiveProperty<string> LoginServer { get; }


        /// <summary>
        /// Gets load command.
        /// </summary>
        public AsyncReactiveCommand LoadCommand { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Create instance.
        /// </summary>
        public MainWindowViewModel()
        {
            //--- initialize properties
            this.UserName = new ReactiveProperty<string>();
            this.Password = new ReactiveProperty<string>();
            this.LoginServer = new ReactiveProperty<string>();
            this.LoadCommand
                = this.UserName.CombineLatest
                (
                    this.Password,
                    this.LoginServer,
                    (userName, password, loginServer) => !string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password) && !string.IsNullOrWhiteSpace(loginServer)
                )
                .ToAsyncReactiveCommand();

            //--- command
            this.LoadCommand.Subscribe(async () =>
            {
                var repos = await AcrRepository.LoadAsync(this.UserName.Value, this.Password.Value, this.LoginServer.Value);
            });
        }
        #endregion
    }
}
