using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Shamar_PrintNotify
{
    public class TesteKyoceraService
    {
        private const string KyoceraIp =
        "192.168.200.250";


    public void Executar()
        {
            try
            {
                Debug.WriteLine(
                    "==========================================");

                Debug.WriteLine(
                    "INÍCIO DO TESTE KYOCERA");

                Debug.WriteLine(
                    "Impressora: Kyocera ECOSYS P3145dn");

                Debug.WriteLine(
                    "IP: " + KyoceraIp);

                Debug.WriteLine(
                    "Printer-MIB: 1.3.6.1.2.1.43");

                Debug.WriteLine(
                    "Tabela de alertas: 1.3.6.1.2.1.43.18");

                Debug.WriteLine(
                    "==========================================");

                var service =
                    new PrinterStatusService();

                PrinterStatusResult result =
                    service.GetStatus(KyoceraIp);

                Debug.WriteLine(
                    "STATUS: " +
                    result.Status);

                Debug.WriteLine(
                    "MENSAGEM: " +
                    result.Message);

                Debug.WriteLine(
                    "TOTAL DE ALERTAS: " +
                    result.Alerts.Count);

                Debug.WriteLine("");

                if (result.Alerts.Count == 0)
                {
                    Debug.WriteLine(
                        "NENHUM ALERTA ENCONTRADO.");
                }
                else
                {
                    Debug.WriteLine(
                        "ALERTAS ENCONTRADOS:");

                    Debug.WriteLine("");

                    foreach (var alert in result.Alerts)
                    {
                        Debug.WriteLine(
                            "OID: " +
                            alert.Oid);

                        Debug.WriteLine(
                            "VALOR: " +
                            alert.Value);

                        Debug.WriteLine(
                            "------------------------------------------");
                    }
                }

                Debug.WriteLine("");

                Debug.WriteLine(
                    "FIM DO TESTE KYOCERA");

                Debug.WriteLine(
                    "==========================================");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    "ERRO NO TESTE KYOCERA:");

                Debug.WriteLine(
                    ex.ToString());
            }
        }
    }


}
