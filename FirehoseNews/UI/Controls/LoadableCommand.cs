//Copied from UNO UI Loadable page
namespace FirehoseNews.UI.Controls;
public class LoadableCommand(Func<Task> executeAsync) : ICommand, ILoadable
{
    public event EventHandler? CanExecuteChanged;
    public event EventHandler? IsExecutingChanged;

    private bool _isExecuting;

    public bool CanExecute(object? parameter) => !IsExecuting;

    public bool IsExecuting
    {
        get => _isExecuting;
        set
        {
            if (_isExecuting != value)
            {
                _isExecuting = value;
                IsExecutingChanged?.Invoke(this, EventArgs.Empty);
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public async void Execute(object? parameter)
    {
        try
        {
            IsExecuting = true;
            await executeAsync();
        }
        finally
        {
            IsExecuting = false;
        }
    }
}
