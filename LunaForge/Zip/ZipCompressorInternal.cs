using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;

namespace LunaForge.Zip;

public class ZipCompressorInternal : ZipCompressor
{
    private readonly string TargetArchivePath;
    private FileStream TargetArchiveFS;
    private ZipFile TargetArchive;

    public ZipCompressorInternal(string targetArchivePath)
    {
        ZipStrings.UseUnicode = true;
        TargetArchivePath = targetArchivePath;
    }

    public override void PackByDict(Dictionary<string, string> path, bool removeIfExists)
    {
        try
        {
            foreach (string s in PackByDictReporting(path, removeIfExists)) { }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Packaging failed.\n{e}");
        }
    }

    public override IEnumerable<string> PackByDictReporting(Dictionary<string, string> path, bool removeIfExists)
    {
        HashSet<string> zipNames = [];
        try
        {
            if (File.Exists(TargetArchivePath))
            {
                if (removeIfExists)
                {
                    File.Delete(TargetArchivePath);
                    TargetArchive = ZipFile.Create(TargetArchivePath);
                }
                else
                {
                    TargetArchive = new ZipFile(TargetArchivePath);
                }
            }
            else
            {
                TargetArchive = ZipFile.Create(TargetArchivePath);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Packaging failed.\n{e}");
            yield break;
        }
        foreach (ZipEntry ze in TargetArchive)
        {
            zipNames.Add(ze.Name);
        }
        foreach (KeyValuePair<string, string> kvp in path)
        {
            TargetArchive.BeginUpdate();
            yield return $"Adding file \"{kvp.Value}\" in to zip.";
            if (TargetArchive.FindEntry(kvp.Key, true) > 0)
            {
                TargetArchive.Delete(kvp.Key);
            }
            TargetArchive.Add(kvp.Value, kvp.Key);
            yield return $"Added file \"{kvp.Value}\" in to zip.";
            TargetArchive.CommitUpdate();
        }
        ((IDisposable)TargetArchive).Dispose();
    }
}
