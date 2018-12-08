using System;
using System.Linq;
using System.Threading.Tasks;
using AcrInsight.Extensions;
using Microsoft.Azure.ContainerRegistry;
using Microsoft.Rest;



namespace AcrInsight.Models
{
    /// <summary>
    /// Represents a repository of Azure Container Registory.
    /// </summary>
    public class AcrRepository
    {
        #region Properties
        /// <summary>
        /// Gets repository name.
        /// </summary>
        public string Name { get; }


        /// <summary>
        /// Gets container image manifests.
        /// </summary>
        public AcrManifest[] Manifests { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="manifests"></param>
        private AcrRepository(string name, AcrManifest[] manifests)
        {
            this.Name = name;
            this.Manifests = manifests;
        }
        #endregion


        #region Load
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="loginServer"></param>
        /// <returns></returns>
        public static async ValueTask<AcrRepository[]> LoadAsync(string userName, string password, string loginServer)
        {
            if (userName == null) throw new ArgumentNullException(nameof(userName));
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (loginServer == null) throw new ArgumentNullException(nameof(loginServer));

            //--- create client
            var credentials = new BasicAuthenticationCredentials
            {
                UserName = userName.Trim(),
                Password = password.Trim(),
            };
            var client = new AzureContainerRegistryClient(credentials)
            {
                BaseUri = new Uri($"https://{loginServer.Trim()}"),
            };

            //--- load reposigories
            var names = (await client.GetRepositoriesAsync().ConfigureAwait(false)).Names;
            return (await names
                .Select(async x =>
                {
                    var manifests
                        = (await client.GetAcrManifestsAsync(x).ConfigureAwait(false))
                        .Manifests
                        .OrderByDescending(y => y.CreatedTime)
                        .Select(y => new AcrManifest(y))
                        .ToArray();
                    return new AcrRepository(x, manifests);
                })
                .WhenAll()
                .ConfigureAwait(false))
                .OrderBy(x => x.Name)
                .ToArray();
        }
        #endregion
    }
}
