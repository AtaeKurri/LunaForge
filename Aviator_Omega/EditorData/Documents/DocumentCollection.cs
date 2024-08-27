using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator_Omega.EditorData.Documents;

public class DocumentCollection : List<AviatorDocument>
{
    public int MaxHash { get; private set; } = 0;
    
    public void AddAndAllocHash(AviatorDocument document)
    {
        base.Add(document);
        document.Parent = this;
        document.Hash = MaxHash;
        MaxHash++;
    }
}
