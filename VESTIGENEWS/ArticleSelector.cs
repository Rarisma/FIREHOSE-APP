//LIVING WITH DETERMINATION

using HYDRANT.Definitions;

namespace VESTIGENEWS;

/// <summary>
/// This determines how an article is displayed in article view.
/// </summary>
public class ArticleSelector : DataTemplateSelector
{
    public DataTemplate? NoImageTemplate { get; set; }
    public DataTemplate? HeadlineTemplate { get; set; }
    public DataTemplate? MinimalTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is Article multiTypeItem)
        {
            if (multiTypeItem.IsHeadline) { return HeadlineTemplate!; }
            if (multiTypeItem.ImageURL == "?" & !multiTypeItem.IsHeadline) { return NoImageTemplate!; }
            if ( multiTypeItem.IsHeadline == false) { return MinimalTemplate!; }

        }
        return base.SelectTemplateCore(item, container);
    }
}
