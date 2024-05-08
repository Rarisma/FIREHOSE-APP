namespace VESTIGENEWS.Controls;
public sealed partial class BindableSwipeItem : SwipeItem
{
    /// <summary>
    /// Set this to the Article Object you want to access.
    /// </summary>
    public static readonly DependencyProperty ItemSourceProperty =
        DependencyProperty.Register(
            "ItemSource", // Name of the property
            typeof(Article), // Type of the property
            typeof(BindableSwipeItem), // Owner of the property (this control)
            new PropertyMetadata(default(string)));

    /// <summary>
    /// Publication information for given article.
    /// </summary>
    public Article ItemSource
    {
        get { return (Article)GetValue(ItemSourceProperty); }
        set { SetValue(ItemSourceProperty, value); }
    }
    public BindableSwipeItem()
    {
        this.InitializeComponent();
    }
}
