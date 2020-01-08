
using System.Collections.Generic;

namespace Deanta.Cosmos.ServiceLayer.DatabaseServices.Concrete
{
    public class TodoInfoJson
    {
        public string Title { get; set; }

        public List<string> Owners { get; set; }

        public string Publisher { get; set; }

        public string CreatedDate { get; set; }

        public string Description { get; set; }

        public int PageCount { get; set; }

        public List<string> Categories { get; set; }

        public double? AverageRating { get; set; }

        public int? RatingsCount { get; set; }

        public string ImageLinksThumbnail { get; set; }

        public string SaleInfoCountry { get; set; }

        public bool? SaleInfoForSale { get; set; }

        public string SaleInfoBuyLink { get; set; }

        public double? SaleInfoListPriceAmount { get; set; }

        public string SaleInfoListPriceCurrencyCode { get; set; }
    }
}