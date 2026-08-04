using System;
using System.Windows.Forms;

namespace Shamar_PrintNotify
{
    public static class TesteSnmp
    {
        public static void Executar()
        {
            try
            {
                string ip =
                    "192.168.200.250";

                var snmp =
                    new SnmpService();

                bool respondeu =
                    snmp.TestConnection(ip);

                string resultado;

                if (respondeu)
                {
                    resultado =
                        "SNMP respondeu corretamente.";
                }
                else
                {
                    resultado =
                        "A impressora não respondeu ao SNMP.";
                }

                MessageBox.Show(
                    "TESTE SNMP\r\n\r\n" +

                    "Impressora:\r\n" +
                    "Kyocera ECOSYS P3145dn\r\n\r\n" +

                    "IP:\r\n" +
                    ip +
                    "\r\n\r\n" +

                    "Resultado:\r\n" +
                    resultado,

                    "PrintNotify - Teste SNMP",

                    MessageBoxButtons.OK,

                    respondeu
                        ? MessageBoxIcon.Information
                        : MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "ERRO NO TESTE SNMP\r\n\r\n" +
                    ex.Message,

                    "PrintNotify",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Error);
            }
        }
    }
}