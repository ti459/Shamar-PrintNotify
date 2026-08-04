using System.Diagnostics;

namespace Shamar_PrintNotify
{
    internal static class Program
    {
        private static TrayIconService? _trayIcon;


    [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // =========================================================
            // TESTE DA KYOCERA
            // =========================================================

            var testeKyocera =
                new TesteKyoceraService();

            testeKyocera.Executar();

            // =========================================================
            // INICIA O PRINTNOTIFY NORMALMENTE
            // =========================================================

            _trayIcon =
                new TrayIconService();

            Application.Run();

            _trayIcon.Dispose();
        }
    }


}
