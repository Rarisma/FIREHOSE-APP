using HYDRANT.Definitions;
//LIVING WITH DETERMINATION
namespace FirehoseApp.UI.Controls;
/// <summary>
/// This determines how an article is displayed in article view.
/// </summary>
public class ArticleSelector : DataTemplateSelector
{
    public DataTemplate? HeadlineTemplate { get; set; }
    public DataTemplate? MinimalTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is Article Article)
        {
            if (Article.Impact >= 70 && Article.ImageURL != "?")
            {
                return HeadlineTemplate!;
            }
            return MinimalTemplate!;
        }
        return base.SelectTemplateCore(item, container);
    }
}
