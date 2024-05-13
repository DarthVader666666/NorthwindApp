using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Northwind.Application.Models.PageModels;

namespace Northwind.Application.TagHelpers
{
    public class PageLinkTagHelper: TagHelper
    {
        private readonly IUrlHelperFactory _urlHelperFactory;

        public PageLinkTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = null;
        public PageModelBase? PageModel { get; set; }
        public string PageAction { get; set; } = "";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (PageModel == null)
            { 
                throw new Exception("PageModel is not set");
            }

            var urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "div";

            var tag = new TagBuilder("ul");
            tag.AddCssClass("pagination");

            TagBuilder currentItem = CreateTag(urlHelper, PageModel.PageNumber);

            if (PageModel.HasPreviousPage)
            {
                TagBuilder prevItem = CreateTag(urlHelper, PageModel.PageNumber - 1);
                tag.InnerHtml.AppendHtml(prevItem);
            }

            tag.InnerHtml.AppendHtml(currentItem);

            if (PageModel.HasNextPage)
            {
                TagBuilder nextItem = CreateTag(urlHelper, PageModel.PageNumber + 1);
                tag.InnerHtml.AppendHtml(nextItem);
            }

            output.Content.AppendHtml(tag);
        }

        TagBuilder CreateTag(IUrlHelper urlHelper, int pageNumber = 1)
        {
            var item = new TagBuilder("li");
            var link = new TagBuilder("a");

            if (pageNumber == PageModel?.PageNumber)
            {
                item.AddCssClass("active");
            }
            else
            {
                link.Attributes["href"] = PageModel switch
                {
                    PageModelBase x when x is ProductPageModel => urlHelper.Action(PageAction, new 
                    { 
                        categoryId = ((ProductPageModel)PageModel).CategoryId, 
                        page = pageNumber 
                    }),

                    PageModelBase x when x is OrderPageModel => urlHelper.Action(PageAction, new 
                    { 
                        customerId = ((OrderPageModel)PageModel).CustomerId, 
                        page = pageNumber 
                    }),

                    PageModelBase x when x is OrderDetailsPageModel => urlHelper.Action(PageAction, new 
                    { 
                        orderId = ((OrderDetailsPageModel)PageModel).OrderId, 
                        productId = ((OrderDetailsPageModel)PageModel).ProductId, 
                        page = pageNumber
                    }),

                    _ => urlHelper.Action(PageAction, new
                    {
                        page = pageNumber
                    }),
                };
            }

            item.AddCssClass("page-item");
            link.AddCssClass("page-link");
            link.InnerHtml.Append(pageNumber.ToString());
            item.InnerHtml.AppendHtml(link);

            return item;
        }
    }
}
