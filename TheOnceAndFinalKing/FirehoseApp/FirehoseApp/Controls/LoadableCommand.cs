//Copied from UNO UI Loadable page
namespace FirehoseApp.Controls;
public class LoadableCommand : ICommand, ILoadable
{
    public event EventHandler CanExecuteChanged;
    public event EventHandler IsExecutingChanged;

    private Func<Task> _executeAsync;
    private bool _isExecuting;

    public LoadableCommand(Func<Task> executeAsync)
    {
        _executeAsync = executeAsync;
    }

    public bool CanExecute(object parameter) => !IsExecuting;

    public bool IsExecuting
    {
        get => _isExecuting;
        set
        {
            if (_isExecuting != value)
            {
                _isExecuting = value;
                IsExecutingChanged?.Invoke(this, new());
                CanExecuteChanged?.Invoke(this, new());
            }
        }
    }

    public async void Execute(object parameter)
    {
        try
        {
            IsExecuting = true;
            await _executeAsync();
        }
        finally
        {
            IsExecuting = false;
        }
    }
}
