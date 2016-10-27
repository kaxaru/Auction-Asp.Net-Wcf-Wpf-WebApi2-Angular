using System.Collections.Generic;
using System.Linq;
using Auction.Presentation.Models;

namespace Auction.Presentation.Localization
{
    public class LocalizationService 
    {
        private static IList<LocalizationViewModel> _localization = new List<LocalizationViewModel>()
        {
            new LocalizationViewModel { LocalizationId = "en-Us", Name = "English" },
            new LocalizationViewModel { LocalizationId = "ru-Ru", Name = "Русский" },
            new LocalizationViewModel { LocalizationId = "be-By", Name = "Белорусский" }
        };
                     
        public static IEnumerable<LocalizationViewModel> GetAvalaibleLocalization()
        {
            return _localization;
        }

        public static LocalizationViewModel GetDefaultLang()
        {
            return _localization.First();
        }
    }
}