using System.Windows.Input;

namespace EGGPLANT._12_SUB_UI.ViewModels
{
    public class ActionButtonVM 
    {
        public string Title { get; set; } = "";
        public ICommand? Command { get; }
        public object? CommandParameter { get; }
        public ActionButtonVM(string title, ICommand? command, object? commandParameter)
        {
            Title = title;
            Command = command;
            CommandParameter = commandParameter;
        }

        public ActionButtonVM()
        {
        }
    }
}
