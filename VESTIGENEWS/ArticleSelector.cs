//LIVING WITH DETERMINATION
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
            if (multiTypeItem.Headline) { return HeadlineTemplate; }
            if (multiTypeItem.ImageURL == "?" & !multiTypeItem.Headline) { return NoImageTemplate; }
            if ( multiTypeItem.Headline == false) { return MinimalTemplate; }

        }
        return base.SelectTemplateCore(item, container);
    }
}
