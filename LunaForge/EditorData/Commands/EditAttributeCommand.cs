﻿using LunaForge.EditorData.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Commands;

public class EditAttributeCommand : Command
{
    private NodeAttribute toEdit { get; set; }
    private string originalValue { get; set; }
    private string newValue { get; set; }

    public EditAttributeCommand(NodeAttribute toEdit, string originalValue, string newValue)
    {
        this.toEdit = toEdit;
        this.originalValue = originalValue;
        this.newValue = newValue;
    }

    public override void Execute()
    {
        toEdit.AttrValue = newValue;
        toEdit.TempAttrValue = newValue;
    }

    public override void Undo()
    {
        toEdit.AttrValue = originalValue;
        toEdit.TempAttrValue = originalValue;
    }

    public override string ToString() => $"Change Attribute \"{toEdit.AttrName}\" -> \"{newValue}\"";
}
