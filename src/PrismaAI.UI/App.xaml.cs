using Microsoft.Extensions.Logging;

namespace PrismaAI.UI;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }
}
