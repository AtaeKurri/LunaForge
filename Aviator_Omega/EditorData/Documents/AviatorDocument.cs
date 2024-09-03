using Aviator_Omega.EditorData.Commands;
using Aviator_Omega.EditorData.Nodes;
using Aviator_Omega.GUI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using TreeNode = Aviator_Omega.EditorData.Nodes.TreeNode;

namespace Aviator_Omega.EditorData.Documents;

public class AviatorDocument
{
    public ProjectConfiguration Configuration { get; set; } = new();
    public WorkTree TreeNodes { get; set; } = [];
    public string RawDocName { get; set; }
    public int Hash { get; set; }
    public int TreeNodeMaxHash { get; set; } = 0;

    public DocumentCollection Parent;
    public MainWindow MainWin;

    public string DocPath { get; set; } = string.Empty;

    public string DocName
    {
        get => RawDocName + (IsUnsaved ? " *" : "");
        set
        {
            RawDocName = value;
        }
    }

    public bool IsSelected;

    public bool IsUnsaved { get; set; } = false;

    public Stack<Command> CommandStack = [];
    public Stack<Command> UndoCommandStack = [];
    public Command? SavedCommand { get; set; } = null;

    public AviatorDocument(string name, string path, bool _isSelected)
    {
        DocName = name;
        DocPath = path;
        IsSelected = _isSelected;
    }

    public AviatorDocument(string name, string path, ProjectConfiguration conf, bool _isSelected)
        : this(name, path, _isSelected)
    {
        Configuration = conf;
    }

    public override string ToString()
    {
        return RawDocName;
    }

    #region IO

    public bool Save(bool saveAs = false)
    {
        bool result = false;
        Thread dialogThread = new(() =>
        {
            string path = "";
            if (string.IsNullOrEmpty(DocPath) || saveAs)
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new()
                {
                    Filter = "Aviator Project (*.avtr)|*.avtr",
                    InitialDirectory = Properties.Settings.Default.LastUsedPath,
                    FileName = saveAs ? "" : RawDocName
                };
                do
                {
                    if (saveFileDialog.ShowDialog() == false) return;
                } while (string.IsNullOrEmpty(saveFileDialog.FileName));
                path = saveFileDialog.FileName;
                Properties.Settings.Default.LastUsedPath = Path.GetDirectoryName(path);
                DocPath = path;
                DocName = path[(path.LastIndexOf('\\') + 1)..];
            }
            else path = DocPath;
            PushSavedCommand();
            try
            {
                using (StreamWriter sw = new(path))
                {
                    SerializeToFile(sw);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }
            result = true;
            return;
        });
        dialogThread.SetApartmentState(ApartmentState.STA); // Set to STA for UI thread
        dialogThread.Start();
        dialogThread.Join();
        return result;
    }

    public void SerializeToFile(StreamWriter stream)
    {
        string SerializedConf = JsonConvert.SerializeObject(Configuration, settings: new() { DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate });
        stream.WriteLine(SerializedConf);
        TreeNodes[0].SerializeToFile(stream, 0);
    }

    public static async Task<WorkTree> CreateNodesFromFile(StreamReader sr, AviatorDocument doc)
    {
        WorkTree tree = [];
        TreeNode root = null;
        TreeNode parent = null;
        TreeNode tempNode = null;
        int previousLevel = -1;
        int i;
        int levelGraduation;
        string nodeToDeserialize;
        char[] temp;
        try
        {
            while (!sr.EndOfStream)
            {
                temp = (await sr.ReadLineAsync()).ToCharArray();
                i = 0;
                while (temp[i] != ',') i++;
                nodeToDeserialize = new string(temp, i + 1, temp.Length - i - 1);
                if (previousLevel != -1)
                {
                    levelGraduation = Convert.ToInt32(new string(temp, 0, i)) - previousLevel;
                    if (levelGraduation <= 0)
                    {
                        for (int j = 0; j >= levelGraduation; j--)
                        {
                            parent = parent.Parent;
                        }
                    }
                    tempNode = TreeSerializer.DeserializeTreeNode(nodeToDeserialize);
                    tempNode.ParentDocument = doc;
                    parent.AddChild(tempNode);
                    parent = tempNode;
                    previousLevel += levelGraduation;
                }
                else
                {
                    root = TreeSerializer.DeserializeTreeNode(nodeToDeserialize);
                    root.ParentDocument = doc;
                    parent = root;
                    previousLevel = 0;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        tree.Add(root);
        return tree;
    }

    public static async Task<AviatorDocument> CreateDocumentFromFileAsync(string name, string filePath, ProjectConfiguration conf = null)
    {
        AviatorDocument doc = new(name, filePath, true);
        try
        {
            using (StreamReader sr = new(filePath, Encoding.UTF8))
            {
                if (conf != null)
                    doc.Configuration = conf;
                else
                    doc.Configuration = JsonConvert.DeserializeObject<ProjectConfiguration>(
                        sr.ReadLine(),
                        settings: new() { DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate }
                    );
                doc.TreeNodes = await CreateNodesFromFile(sr, doc);
            }
            return doc;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return default;
        }
    }

    #endregion
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
}
