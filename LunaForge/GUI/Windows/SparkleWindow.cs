using LunaForge.GUI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using NetSparkleUpdater.SignatureVerifiers;
using NetSparkleUpdater;
using System.ComponentModel;
using NetSparkleUpdater.Events;

namespace LunaForge.GUI.Windows;

public class SparkleWindow : ImGuiWindow
{
    public SparkleUpdater Sparkle { get; set; }

    public SparkleWindow()
        : base(false)
    {
        Sparkle = new(
            "", // TODO: Replace with AppCast.xml url.
            new Ed25519Checker(NetSparkleUpdater.Enums.SecurityMode.Unsafe)
        )
        {
            RelaunchAfterUpdate = true,
            CustomInstallerArguments = "",
            CheckServerFileName = false
        };
        Sparkle.UpdateDetected += OnUpdateDetected;
        Sparkle.PreparingToExit += SparkleCloseFiles;
        Sparkle.CloseApplication += MainWindow.ForceClose;
        Sparkle.StartLoop(true, true);
    }

    private void OnUpdateDetected(object sender, UpdateDetectedEventArgs e)
    {

    }

    private void SparkleCloseFiles(object sender, CancelEventArgs e)
    {
        MainWindow.RenderCloseOpenedProjects();
    }

    public override void Render()
    {
        if (Begin("LunaForge Updater"))
        {
            End();
        }
    }
}
