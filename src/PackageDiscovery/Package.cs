namespace PackageDiscovery
{
    public class Package
    {
        public Package(string kind, string id, string version, bool isDevelopmentPackage = false)
        {
            this.Kind = kind;
            this.Id = id;
            this.Version = version;
            this.IsDevelopmentPackage = isDevelopmentPackage;
        }

        public string Id { get; set; }

        public string Version { get; set; }

        public string Kind { get; set; }

        public bool IsDevelopmentPackage { get; set; }
    }
}
