using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
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
        /// Gets container repository names.
        /// </summary>
        public ObservableCollection<AcrManifest> Manifests { get; } = new ObservableCollection<AcrManifest>();
        #endregion


        #region Commands
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
            //--- initialize
            this.LoadCommand
                = this.UserName.CombineLatest
                (
                    this.Password,
                    this.LoginServer,
                    (userName, password, loginServer) => !string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password) && !string.IsNullOrWhiteSpace(loginServer)
                )
                .ToAsyncReactiveCommand();

            //--- operations
            IReadOnlyDictionary<string, AcrManifest[]> repos = null;
            this.LoadCommand.Subscribe(async () =>
            {
                var loaded = await AcrRepository.LoadAsync(this.UserName.Value, this.Password.Value, this.LoginServer.Value);
                repos = loaded.ToDictionary(x => x.Name, x => x.Manifests);  // cache

                //--- reset repository names
                this.RepositoryNames.Clear();
                foreach (var x in loaded)
                    this.RepositoryNames.Add(x.Name);
            });

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
