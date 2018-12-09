using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
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
        public ReactiveProperty<string> UserName { get; } = new ReactiveProperty<string>();


        /// <summary>
        /// Gets ACR password.
        /// </summary>
        public ReactiveProperty<string> Password { get; } = new ReactiveProperty<string>();


        /// <summary>
        /// Gets ACR login server.
        /// </summary>
        public ReactiveProperty<string> LoginServer { get; } = new ReactiveProperty<string>();


        /// <summary>
        /// Gets container repository names.
        /// </summary>
        public ObservableCollection<string> RepositoryNames { get; } = new ObservableCollection<string>();


        /// <summary>
        /// Gets selected repository name.
        /// </summary>
        public ReactiveProperty<string> SelectedRepositoryName { get; } = new ReactiveProperty<string>(mode: ReactivePropertyMode.DistinctUntilChanged);


        /// <summary>
        /// Gets selected manifest.
        /// </summary>
        public ReactiveProperty<AcrManifest> SelectedManifest { get; } = new ReactiveProperty<AcrManifest>(mode: ReactivePropertyMode.DistinctUntilChanged);


        /// <summary>
        /// Gets container repository names.
        /// </summary>
        public ObservableCollection<AcrManifest> Manifests { get; } = new ObservableCollection<AcrManifest>();
        #endregion


        #region Commands
        /// <summary>
        /// Gets load command.
        /// </summary>
        public AsyncReactiveCommand LoadCommand { get; }


        /// <summary>
        /// Gets copy digest to clipboard command.
        /// </summary>
        public ReactiveCommand CopyDigestCommand { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Create instance.
        /// </summary>
        public MainWindowViewModel()
        {
            //--- initialize
            this.LoadCommand
                = this.UserName.CombineLatest
                (
                    this.Password,
                    this.LoginServer,
                    (userName, password, loginServer) => !string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password) && !string.IsNullOrWhiteSpace(loginServer)
                )
                .ToAsyncReactiveCommand();
            this.CopyDigestCommand = this.SelectedManifest.Select(x => x != null).ToReactiveCommand(false);

            //--- operations
            IReadOnlyDictionary<string, AcrManifest[]> repos = null;
            this.LoadCommand.Subscribe(async () =>
            {
                AcrRepository[] loaded = null;
                try
                {
                    loaded = await AcrRepository.LoadAsync(this.UserName.Value, this.Password.Value, this.LoginServer.Value);
                }
                catch
                {
                    var message = "Couldn't load repositories.\nPlease check your account out again.";
                    var caption = "Repository load failed";
                    MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                repos = loaded.ToDictionary(x => x.Name, x => x.Manifests);  // cache

                //--- reset repository names
                this.RepositoryNames.Clear();
                foreach (var x in loaded)
                    this.RepositoryNames.Add(x.Name);
            });

            this.CopyDigestCommand
                .Select(_ => this.SelectedManifest.Value.Digest)
                .Subscribe(Clipboard.SetText);

            this.SelectedRepositoryName
                .Do(_ => this.Manifests.Clear())
                .Where(_ => repos != null)
                .Subscribe(x =>
                {
                    foreach (var y in repos[x])
                        this.Manifests.Add(y);
                });
        }
        #endregion
    }
}
