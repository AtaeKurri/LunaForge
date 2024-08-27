using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator_Omega.EditorData.Documents;

public class AviatorDocument
{
    public string RawDocName { get; set; }
    public int Hash { get; set; }

    public DocumentCollection Parent;

    public string DocPath { get; set; } = string.Empty;

    public string DocName
    {
        get => RawDocName + (IsUnsaved ? " *" : "");
        set
        {
            RawDocName = value;
        }
    }

    public bool IsUnsaved { get; set; } = false;

    public AviatorDocument(int hash, bool suppressMessage = false)
    {
        Hash = hash;
    }
}
