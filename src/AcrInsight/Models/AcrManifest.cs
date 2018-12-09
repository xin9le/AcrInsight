using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Azure.ContainerRegistry.Models;



namespace AcrInsight.Models
{
    /// <summary>
    /// Represents a container image manifest.
    /// </summary>
    public class AcrManifest
    {
        #region Properties
        /// <summary>
        /// Gets digest (= image's hash ID).
        /// </summary>
        public string Digest { get; }


        /// <summary>
        /// Gets tags associated with.
        /// </summary>
        public IReadOnlyList<string> Tags { get; }


        /// <summary>
        /// Gets operating system.
        /// </summary>
        public string OperatingSystem { get; }


        /// <summary>
        /// Gets CPU architecture.
        /// </summary>
        public string Architecture { get; }


        /// <summary>
        /// Gets created time.
        /// </summary>
        public DateTimeOffset CreatedTime { get; }


        /// <summary>
        /// Gets last update time.
        /// </summary>
        public DateTimeOffset LastUpdateTime { get; }
        #endregion


        #region Constructor
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="source"></param>
        public AcrManifest(AcrManifestAttributesBase source)
        {
            this.Digest = source.Digest;
            this.Tags = new ReadOnlyCollection<string>(source.Tags);
            this.OperatingSystem = source.Os;
            this.Architecture = source.Architecture;
            var createdTime = DateTimeOffset.Parse(source.CreatedTime);
            var lastUpdateTime = DateTimeOffset.Parse(source.LastUpdateTime);
            this.CreatedTime = TimeZoneInfo.ConvertTime(createdTime, TimeZoneInfo.Local);
            this.LastUpdateTime = TimeZoneInfo.ConvertTime(lastUpdateTime, TimeZoneInfo.Local);
        }
        #endregion
    }
}
