using System;
using System.Collections.Generic;
using System.Linq;

namespace Shamar_PrintNotify
{
    public class PrintMonitorService : IDisposable
    {
        private readonly System.Threading.Timer _timer;

        private readonly Dictionary<string, PrintJobInfo> _activeJobs =
            new Dictionary<string, PrintJobInfo>();

        private readonly HashSet<string> _errorJobs =
            new HashSet<string>();

        private readonly PrintHistoryService _historyService;

        public event Action<PrintJobInfo>? PrintJobDetected;

        public event Action<PrintJobInfo>? PrintJobFinished;

        public event Action<PrintJobInfo>? PrintJobError;

        public event Action<PrintJobInfo>? PrintJobCancelled;

        public PrintMonitorService()
        {
            _historyService =
                new PrintHistoryService();

            _timer =
                new System.Threading.Timer(
                    CheckPrintJobs,
                    null,
                    1000,
                    1000);
        }

        // =============================================================
        // VERIFICAR TRABALHOS
        // =============================================================

        private void CheckPrintJobs(
            object? state)
        {
            try
            {
                var service =
                    new PrintJobService();

                var jobs =
                    service.GetPrintJobs();

                var currentKeys =
                    new HashSet<string>();

                // =====================================================
                // ANALISAR TRABALHOS ATUAIS
                // =====================================================

                foreach (var job in jobs)
                {
                    string key =
                        CreateJobKey(job);

                    currentKeys.Add(key);

                    // =================================================
                    // TRABALHO COM ERRO
                    // =================================================

                    if (IsJobError(job))
                    {
                        if (!_errorJobs.Contains(key))
                        {
                            _errorJobs.Add(key);

                            _activeJobs[key] =
                                job;

                            _historyService.Add(
                                new PrintHistoryItem
                                {
                                    DateTime =
                                        DateTime.Now,

                                    JobId =
                                        job.JobId,

                                    Document =
                                        job.Document,

                                    PrinterName =
                                        job.PrinterName,

                                    UserName =
                                        job.UserName,

                                    Status =
                                        "Erro"
                                });

                            PrintJobError?.Invoke(
                                job);
                        }
                        else
                        {
                            _activeJobs[key] =
                                job;
                        }

                        continue;
                    }

                    // =================================================
                    // NOVO TRABALHO
                    // =================================================

                    if (!_activeJobs.ContainsKey(key))
                    {
                        _activeJobs[key] =
                            job;

                        _historyService.Add(
                            new PrintHistoryItem
                            {
                                DateTime =
                                    DateTime.Now,

                                JobId =
                                    job.JobId,

                                Document =
                                    job.Document,

                                PrinterName =
                                    job.PrinterName,

                                UserName =
                                    job.UserName,

                                Status =
                                    "Processando"
                            });

                        PrintJobDetected?.Invoke(
                            job);
                    }
                    else
                    {
                        _activeJobs[key] =
                            job;
                    }
                }

                // =====================================================
                // TRABALHOS QUE SAÍRAM DA FILA
                // =====================================================

                var finishedKeys =
                    _activeJobs.Keys
                        .Where(
                            key =>
                                !currentKeys.Contains(key))
                        .ToList();

                foreach (var key in finishedKeys)
                {
                    var finishedJob =
                        _activeJobs[key];

                    _activeJobs.Remove(
                        key);

                    // =================================================
                    // TRABALHO QUE ESTAVA COM ERRO
                    // =================================================

                    if (_errorJobs.Contains(key))
                    {
                        _errorJobs.Remove(key);

                        _historyService.UpdateStatus(
                            finishedJob.JobId,
                            finishedJob.PrinterName,
                            "Cancelado");

                        PrintJobCancelled?.Invoke(
                            finishedJob);

                        continue;
                    }

                    // =================================================
                    // TRABALHO FINALIZADO NORMALMENTE
                    // =================================================

                    _historyService.UpdateStatus(
                        finishedJob.JobId,
                        finishedJob.PrinterName,
                        "Finalizado");

                    PrintJobFinished?.Invoke(
                        finishedJob);
                }
            }
            catch
            {
                // Nunca interromper o monitoramento
                // por causa de uma falha de consulta.
            }
        }

        // =============================================================
        // VERIFICAR ERRO
        // =============================================================

        private bool IsJobError(
            PrintJobInfo job)
        {
            if (string.IsNullOrWhiteSpace(
                job.Status))
            {
                return false;
            }

            string status =
                job.Status
                    .Trim()
                    .ToLowerInvariant();

            return
                status.Contains("error") ||
                status.Contains("erro") ||
                status.Contains("failed") ||
                status.Contains("failure") ||
                status.Contains("falha");
        }

        // =============================================================
        // CRIAR CHAVE DO TRABALHO
        // =============================================================

        private string CreateJobKey(
            PrintJobInfo job)
        {
            return
                $"{job.PrinterName}|{job.JobId}";
        }

        // =============================================================
        // DISPOSE
        // =============================================================

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}