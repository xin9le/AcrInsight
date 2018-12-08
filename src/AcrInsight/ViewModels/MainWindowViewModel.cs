using System.Reactive.Linq;
using System.Threading.Tasks;
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
        /// Gets login command.
        /// </summary>
        public AsyncReactiveCommand LoginCommand { get; }
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
            this.LoginCommand
                = this.UserName.CombineLatest
                (
                    this.Password,
                    this.LoginServer,
                    (userName, password, loginServer) => !string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password) && !string.IsNullOrWhiteSpace(loginServer)
                )
                .ToAsyncReactiveCommand();

            //--- command
            this.LoginCommand.Subscribe(async () =>
            {
                await Task.Delay(500);
            });
        }
        #endregion
    }
}
