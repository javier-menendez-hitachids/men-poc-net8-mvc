using MenulioPocMvc.Models.Apis;

namespace MenulioPocMvc.Mappers
{
    public class TreatsMapper
    {
        // Adapt to Contentful
    //    public static IEnumerable<IWidgetModel> Map(IEnumerable<ICustomerOffer> source, bool isLazy = false, WidgetColumn columnSize = WidgetColumn.OneCol)
    //    {
            
    //        var dashboard = ContentHelper.GetYourDashboardPage();

    //        int count = 0;
    //        foreach (var offer in source)
    //        {
    //            count++;
    //            if (count < 13)
    //            {
    //                isLazy = false;
    //            }
    //            else
    //            {
    //                isLazy = true;
    //            }

    //            var widget = new TreatWidgetModel()
    //            {
    //                Title = offer.PassbookTitle,
    //                BarcodeNumber = offer.BarcodeNumber,
    //                BarcodeUrl = offer.BarcodeUrl,
    //                MoreButtonText = dashboard.GetPropertyValue("moreButtonText", false, "More"),
    //                ColumnSize = columnSize,
    //                Conditions = offer.Conditions,
    //                Description = offer.Title + " " + offer.Title2,
    //                Instructions = offer.Description,
    //                LessButtonText = dashboard.GetPropertyValue("lessButtonText", false, "Less"),
    //                IsLazy = isLazy,
    //                ImageUrl = !string.IsNullOrEmpty(offer.ImageUrl) ? offer.ImageUrl : string.Empty,
    //                IsHiddenIfOddLast = false,
    //                IgnoreLastOdd = true,
    //                ConditionsUrl = offer.ConditionsUrl
    //            };

    //            yield return widget;
    //        }
    //    }

    //    // TODO: Add lazy load support
    //    public static TreatWidgetModel Map(ICustomerOffer offer, YourTreatsCategory category, WidgetColumn columnSize = WidgetColumn.OneCol)
    //    {
    //        var dashboard = ContentHelper.GetYourDashboardPage();

    //        var widget = new TreatWidgetModel()
    //        {
    //            Title = offer.PassbookTitle,
    //            BarcodeNumber = offer.BarcodeNumber,
    //            BarcodeUrl = offer.BarcodeUrl,
    //            MoreButtonText = dashboard.GetPropertyValue("moreButtonText", false, "More"),
    //            ColumnSize = columnSize,
    //            Conditions = offer.Conditions,
    //            Description = offer.Title + " " + offer.Title2,
    //            Instructions = offer.Description,
    //            LessButtonText = dashboard.GetPropertyValue("lessButtonText", false, "Less"),
    //            IsLazy = false,
    //            ImageUrl = !string.IsNullOrEmpty(offer.ImageUrl) ? offer.ImageUrl : string.Empty,
    //            IsHiddenIfOddLast = false,
    //            IgnoreLastOdd = true,
    //            ConditionsUrl = offer.ConditionsUrl,
    //            CategoryId = category.Id,
    //            CategoryName = category.IsToHide ? string.Empty : category.Heading,
    //            IsTeaser = offer.IsTeaser
    //        };

    //        return widget;
    //    }

    //    public static ICustomerOffer Map(VillageTeaser villageTease)
    //    {
    //        var villageTeaserOffer = new VillageTeaserOffer()
    //        {
    //            PassbookTitle = villageTease.Title,
    //            Conditions = villageTease.ConditionDescription,
    //            Title = villageTease.Description,
    //            ImageUrl = !string.IsNullOrEmpty(villageTease.ImageUrl) ? villageTease.ImageUrl : string.Empty,
    //            CategoryId = villageTease.CategoryId,
    //        };

    //        return villageTeaserOffer;
    //    }
    }
}
