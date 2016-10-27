using System;
using System.ComponentModel;
using System.Resources;

namespace Auction.Presentation.Infrastructure.Filters
{
    public class LocalizedDescriptionFilter : DescriptionAttribute
    {
        private readonly string _resourceKey;

        private readonly ResourceManager _resource;

        public LocalizedDescriptionFilter(string resourceKey, Type resourceType)
        {
            _resource = new ResourceManager(resourceType);
            _resourceKey = resourceKey;
        }

        public override string Description
        {
            get
            {
                string displayName = _resource.GetString(_resourceKey);

                return string.IsNullOrEmpty(displayName)
                    ? string.Format("[[{0}]]", _resourceKey)
                    : displayName;
            }
        }
    }
}