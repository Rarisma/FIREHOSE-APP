//LIVING WITH DETERMINATION
namespace VESTIGENEWS;

/// <summary>
/// This determines how an article is displayed in article view.
/// </summary>
public class ArticleSelector : DataTemplateSelector
{
    public DataTemplate NoImageTemplate { get; set; }
    public DataTemplate HeadlineTemplate { get; set; }
    public DataTemplate MinimalTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is Article multiTypeItem)
        {
            if (multiTypeItem.LowQuality) { return HeadlineTemplate; }
            if (multiTypeItem.ImageURL == "?" & multiTypeItem.LowQuality == false) { return NoImageTemplate; }
            if ( multiTypeItem.LowQuality == false) { return MinimalTemplate; }

        }
        return base.SelectTemplateCore(item, container);
    }
}
