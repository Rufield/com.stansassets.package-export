namespace StansAssets.PackageExport.Editor
{
    /// <summary>
    /// Package Export Context.
    /// </summary>
    public class PackageExportContext
    {
        private string m_Destination;

        // <summary>
        /// The <c>*.unitypackage</c> install destination
        /// </summary>
        public string Destination { get { return m_Destination; } }
        
        private string m_name;

        // <summary>
        /// Get <c>.unitypackage</c> name
        /// </summary>
        public string Name { get { return m_name; } }

        /// <summary>
        /// If set to <c>true</c> package version postfix is added. Example: <c>MyAwesomeAsset_v3.2.unitypackage</c>
        /// </summary>
        public bool AddPackageVersionPostfix { get; set; }

        internal string[] ExcludedPaths { get; private set; }

        /// <summary>
        /// Creates Package Export Context.
        /// </summary>
        /// <param name="name">Exported <c>.unitypackage</c> name. For example: <c>MyAwesomeAsset</c>.</param>
        /// <param name="destination">The <c>*.unitypackage</c> install destination. For example: <c>Assets/Plugins/StansAssets</c>.</param>
        public PackageExportContext(string name, string destination)
        {
            m_Destination = destination;
            m_name = name;
        }

        /// <summary>
        /// Use to set export excluded paths.
        /// </summary>
        /// <param name="excludedPaths">Package relative excluded paths list</param>
        public void SetExcludedPaths(params string[] excludedPaths)
        {
            ExcludedPaths = excludedPaths;
        }
    }
}
