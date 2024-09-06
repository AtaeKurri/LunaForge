using LunaForge.EditorData.Nodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TreeNode = LunaForge.EditorData.Nodes.TreeNode;
using ImGuiNET;

namespace LunaForge.EditorData.Project;

public class LunaDefinition : LunaProjectFile
{
    public WorkTree TreeNodes { get; set; } = [];
    public int TreeNodeMaxHash { get; set; } = 0;

    public LunaDefinition(LunaForgeProject parentProj, string path)
    {
        ParentProject = parentProj;
        FullFilePath = path;
        FileName = Path.GetFileName(path);
    }

    public override string ToString() => FileName;

    #region Rendering

    public override void Render()
    {
        if (TreeNodes[0] == null)
            RenderRootTypeSelector();
        else
            RenderTreeView();
    }

    private void RenderRootTypeSelector()
    {
        ImGui.Text("This file is currently empty. Please choose the Definition Type to start editing:");

        if (ImGui.BeginListBox(string.Empty))
        {
            if (ImGui.Selectable("Test"))
            {
                Console.WriteLine("Definition Type selected");
            }
            ImGui.EndListBox();
        }
    }

    private void RenderTreeView()
    {

    }

    #endregion
    #region Serialization

    public static async Task<WorkTree> DeserializeTree(StreamReader sr, LunaDefinition def)
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
                    tempNode.ParentDef = def;
                    parent.AddChild(tempNode);
                    parent = tempNode;
                    previousLevel += levelGraduation;
                }
                else
                {
                    root = TreeSerializer.DeserializeTreeNode(nodeToDeserialize);
                    root.ParentDef = def;
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

    #endregion
    #region IO

    public bool Save(bool saveAs = false)
    {
        bool result = false;
        Thread dialogThread = new(() =>
        {
            string path = "";
            if (string.IsNullOrEmpty(FullFilePath) || saveAs)
            {
                Microsoft.Win32.SaveFileDialog dialog = new()
                {
                    Filter = "LunaForge Definition (*.lfd)|*.lfd",
                    InitialDirectory = Path.GetDirectoryName(path),
                    FileName = saveAs ? string.Empty : FileName
                };
                do
                {
                    if (dialog.ShowDialog() == false) return;
                } while (string.IsNullOrEmpty(dialog.FileName));
                path = dialog.FileName;
                FullFilePath = path;
                FileName = path[(path.LastIndexOf('\\') + 1)..];
            }
            else path = FullFilePath;
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
                Console.WriteLine(ex.ToString());
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

    public void SerializeToFile(StreamWriter sw)
    {
        TreeNodes[0].SerializeToFile(sw, 0);
    }

    public static async Task<LunaDefinition> CreateFromFile(LunaForgeProject parentProject, string filePath)
    {
        LunaDefinition definition = new(parentProject, filePath);
        try
        {
            using (StreamReader sr = new(filePath, Encoding.UTF8))
            {
                definition.TreeNodes = await DeserializeTree(sr, definition);
            }
            return definition;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return default;
        }
    }

    public override void Close()
    {
        ParentProject.ProjectFiles.Remove(this);
    }

    #endregion


}
