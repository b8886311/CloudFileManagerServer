using System.Collections;
using System.Globalization;

namespace CloudFileManagerServer
{
    public interface ICommand
    {
        public string Description { get; }
        public void Execute();
        public void Undo();
    }

    public class CommandManager
    {
        private readonly Stack<ICommand> _undoStack = new Stack<ICommand>();
        private readonly Stack<ICommand> _redoStack = new Stack<ICommand>();

        public bool CanUndo { get { return _undoStack.Count > 0; } }
        public bool CanRedo { get { return _redoStack.Count > 0; } }

        public string Execute(ICommand command)
        {
            command.Execute();
            _undoStack.Push(command);
            _redoStack.Clear();
            return command.Description;
        }

        public string Undo()
        {
            if (!CanUndo)
            {
                return "沒有可復原的操作";
            }
            ICommand command = _undoStack.Pop();
            command.Undo();
            _redoStack.Push(command);
            return "已復原：" + command.Description;
        }

        public string Redo()
        {
            if (!CanRedo)
            {
                return "沒有可重做的操作";
            }
            ICommand command = _redoStack.Pop();
            command.Execute();
            _undoStack.Push(command);
            return "已重做：" + command.Description;
        }

        public List<string> GetHistory()
        {
            List<string> history = new List<string>();
            foreach (ICommand command in _undoStack)
            {
                history.Add(command.Description);
            }
            return history;
        }

        public ICommand GetLastCutOrCopyItem()
        {
            if (_undoStack.Count == 0)
            {
                throw new InvalidOperationException("No cut or copy operation found");
            }


            foreach (var item in _undoStack)
            {
                if (item is CutCommand || item is CopyCommand)
                {
                    return item;
                }
            }

            return null;
        }
    }
}
