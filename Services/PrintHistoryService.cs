using System.Text.Json;

namespace Shamar_PrintNotify
{
    public class PrintHistoryService
    {
        private readonly string _folderPath;
        private readonly string _filePath;

        public PrintHistoryService()
        {
            _folderPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Historico");

            _filePath = Path.Combine(
                _folderPath,
                "historico.json");

            Directory.CreateDirectory(_folderPath);
        }

        public void Add(PrintHistoryItem item)
        {
            try
            {
                var history = GetAll();

                history.Add(item);

                Save(history);
            }
            catch
            {
                // O histórico não deve interromper
                // o funcionamento do PrintNotify.
            }
        }

        public void UpdateStatus(
            uint jobId,
            string printerName,
            string status)
        {
            try
            {
                var history = GetAll();

                var item = history
                    .LastOrDefault(x =>
                        x.JobId == jobId &&
                        x.PrinterName == printerName);

                if (item == null)
                {
                    return;
                }

                item.Status = status;

                Save(history);
            }
            catch
            {
                // Não interromper o monitoramento.
            }
        }

        public List<PrintHistoryItem> GetAll()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    return new List<PrintHistoryItem>();
                }

                string json = File.ReadAllText(
                    _filePath);

                return JsonSerializer.Deserialize<
                    List<PrintHistoryItem>>(json)
                    ?? new List<PrintHistoryItem>();
            }
            catch
            {
                return new List<PrintHistoryItem>();
            }
        }

        private void Save(
            List<PrintHistoryItem> history)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json =
                JsonSerializer.Serialize(
                    history,
                    options);

            File.WriteAllText(
                _filePath,
                json);
        }
    }
}