using System.Configuration;

namespace Auction.Presentation.Infrastructure.СustomSettings
{
    [ConfigurationCollection(typeof(AuctionHouseElement))]
    public class AuctionHousesElement : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        public AuctionHouseElement this[int index]
        {
            get
            {
                return (AuctionHouseElement)BaseGet(index);
            }

            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }

                BaseAdd(index, value);
            }
        }

        public new AuctionHouseElement this[string name]
        {
            get
            {
                return (AuctionHouseElement)BaseGet(name);
            }
        }

        public void Add(AuctionHouseElement element)
        {
            BaseAdd(element);
        }

      /*  protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }*/

        public void Remove(AuctionHouseElement element)
        {
            if (BaseIndexOf(element) >= 0)
            {
                BaseRemove(element.Name);
            }
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        public void Clear()
        {
            BaseClear();
        }

        public AuctionHouseElement Search(string name)
        {
            foreach (AuctionHouseElement el in this)
            {
                if (el.Name == name)
                {
                    return el;
                }
            }

            return null;
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AuctionHouseElement)element).Name;
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new AuctionHouseElement();
        }
    }
}