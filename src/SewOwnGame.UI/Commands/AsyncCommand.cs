using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SewOwnGame.UI.Commands;

public class AsyncCommand : ICommand
{
    private readonly Func<Task> _execute;
    private bool _isExecuting;
    
    public event EventHandler? CanExecuteChanged;
    
    public AsyncCommand(Func<Task> execute)
    {
        _execute = execute;
    }
    
    public bool CanExecute(object? parameter) => !_isExecuting;
    
    public async void Execute(object? parameter)
    {
        if (_isExecuting) return;
        
        _isExecuting = true;
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        
        try
        {
            await _execute();
        }
        finally
        {
            _isExecuting = false;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}