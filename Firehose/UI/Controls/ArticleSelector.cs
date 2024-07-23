using HYDRANT.Definitions;

//LIVING WITH DETERMINATION
namespace FirehoseApp.UI.Controls;

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
        if (item is Article Article)
        {
            if (Article.ImageURL == "?") { return NoImageTemplate!; }
            if (Article.Impact >= 70) { return HeadlineTemplate!; }
            else{ return MinimalTemplate!; }

        }
        return base.SelectTemplateCore(item, container);
    }
}
