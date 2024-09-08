using LunaForge.EditorData.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Project;

public abstract class LunaProjectFile
{
    public LunaForgeProject ParentProject { get; set; }

    public int Hash { get; set; } = -1;
    public string FullFilePath { get; set; }
    public string FileName { get; set; }

    public bool IsOpened = true;

    public Stack<Command> CommandStack { get; set; } = [];
    public Stack<Command> UndoCommandStack { get; set; } = [];
    public Command? SavedCommand { get; set; } = null;

    public bool IsUnsaved
    {
        get
        {
            try
            {
                return CommandStack.Peek() != SavedCommand;
            }
            catch (InvalidOperationException)
            {
                return SavedCommand != null;
            }
        }
    }

    public void AllocHash(ref int maxHash)
    {
        if (Hash != -1)
            return; // Hash has already been allocated. Return immediately. Not actually needed, it's more of a satefy measure.
        Hash = maxHash;
        maxHash++;
    }

    #region Commands

    public void Undo()
    {
        CommandStack.Peek().Undo();
        UndoCommandStack.Push(CommandStack.Pop());
    }

    public void Redo()
    {
        UndoCommandStack.Peek().Execute();
        CommandStack.Push(UndoCommandStack.Pop());
    }

    public void PushSavedCommand()
    {
        try { SavedCommand = CommandStack.Peek(); }
        catch (InvalidOperationException) { SavedCommand = null; }
    }

    public bool AddAndExecuteCommand(Command command)
    {
        if (command == null)
            return false;
        CommandStack.Push(command);
        CommandStack.Peek().Execute();
        UndoCommandStack = [];
        return true;
    }

    public void RevertUntilSaved()
    {
        if (SavedCommand == null || CommandStack.Contains(SavedCommand))
            while (CommandStack.Count != 0 && CommandStack.Peek() != SavedCommand)
                Undo();
        else
            while (UndoCommandStack.Count != 0 && UndoCommandStack.Peek() != SavedCommand)
                Redo();
    }

    #endregion
    #region IO

    public abstract bool Save(bool saveAs = false);

    #endregion

    public abstract void Render();

    public abstract void Close();
}
