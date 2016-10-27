using DevExpress.Mvvm;
using WPF.Models;

namespace WPF.Classes
{
    public class ProductForBinding : BindableBase
    {
        public Product Product
        {
            get { return GetProperty(() => Product); }
            set { SetProperty(() => Product, value); }
        }

        public Bid Bid
        {
            get { return GetProperty(() => Bid); }
            set { SetProperty(() => Bid, value); }
        }

        public long LastPrice
        {
            get { return GetProperty(() => LastPrice); }
            set { SetProperty(() => LastPrice, value); }
        }

        public long NextPrice
        {
            get { return GetProperty(() => NextPrice); }
            set { SetProperty(() => NextPrice, value); }
        }

        public long CounterSeconds
        {
            get { return GetProperty(() => CounterSeconds); }
            set { SetProperty(() => CounterSeconds, value); }
        }
    }
}
