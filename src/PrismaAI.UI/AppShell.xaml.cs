namespace PrismaAI.UI;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("settings", typeof(SettingsPage));
        Routing.RegisterRoute("subtitle", typeof(SubtitlePage));
    }
}
