using LunaForge.GUI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using LunaForge.EditorData.Project;
using TreeNode = LunaForge.EditorData.Nodes.TreeNode;
using LunaForge.EditorData.Nodes;
using System.Numerics;

namespace LunaForge.GUI.Windows;

public class NodeAttributeWindow : ImGuiWindow
{
    public TreeNode? CurrentNode => (ParentWindow.Workspaces.Current?.CurrentProjectFile as LunaDefinition)?.SelectedNode;

    public NodeAttributeWindow(MainWindow parent)
        : base(parent, true)
    {

    }

    public override void Render()
    {
        if (BeginNoClose("Node Attributes"))
        {
            if (CurrentNode == null)
            {
                End();
                return;
            }

            ImGuiTableFlags flags = ImGuiTableFlags.Resizable | ImGuiTableFlags.RowBg | ImGuiTableFlags.Borders;
            if (ImGui.BeginTable("NodeAttributeTable", 3, flags))
            {
                ImGui.TableSetupColumn("Properties");
                ImGui.TableSetupColumn("Parameters");
                ImGui.TableSetupColumn(string.Empty);
                ImGui.TableHeadersRow();

                foreach (NodeAttribute attr in CurrentNode.Attributes)
                {
                    ImGui.TableNextRow();

                    // Attr Name
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextWrapped(attr.AttrName);

                    // Attr Value
                    ImGui.TableSetColumnIndex(1);
                    ImGui.SetNextItemWidth(-1);
                    // TODO: Replace this with a ComboBox when the EditWindow types are defined.
                    ImGuiInputTextFlags Tflags = ImGuiInputTextFlags.EnterReturnsTrue;
                    if (attr.AttrValue != string.Empty && attr.TempAttrValue == string.Empty)
                        attr.TempAttrValue = attr.AttrValue;
                    if (ImGui.InputText($"##{attr.AttrName}_input", ref attr.TempAttrValue, 2048, Tflags))
                        CommitEdit(attr);
                    if (ImGui.IsItemDeactivated())
                        CommitEdit(attr);

                    // More...
                    ImGui.TableSetColumnIndex(2);
                    if (ImGui.Button($"...##{attr.AttrName}", new Vector2(ImGui.GetContentRegionAvail().X, 0)))
                    {
                        Console.WriteLine($"More clicked for: {attr.AttrName}");
                    }
                }

                ImGui.EndTable();
            }
            End();
        }
    }

    public void CommitEdit(NodeAttribute attr)
    {
        attr.RaiseEdit(attr.TempAttrValue);
    }
}
