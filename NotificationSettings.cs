using System;
using System.IO;
using System.Text.Json;

namespace Shamar_PrintNotify
{
    public class NotificationSettings
    {
        public bool NotifyOnError { get; set; } = true;

        public bool NotifyOnFinished { get; set; } = false;

        public bool NotifyOnStarted { get; set; } = false;

        public bool DisableWindowsNotifications { get; set; } = false;

        public int NotificationDurationSeconds { get; set; } = 7;
    }

    public static class NotificationSettingsService
    {
        private static readonly string FilePath =
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "notification-settings.json");

        public static NotificationSettings Load()
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    return new NotificationSettings();
                }

                string json =
                    File.ReadAllText(FilePath);

                var settings =
                    JsonSerializer.Deserialize<NotificationSettings>(
                        json);

                return settings ??
                       new NotificationSettings();
            }
            catch
            {
                return new NotificationSettings();
            }
        }

        public static void Save(
            NotificationSettings settings)
        {
            try
            {
                string json =
                    JsonSerializer.Serialize(
                        settings,
                        new JsonSerializerOptions
                        {
                            WriteIndented = true
                        });

                File.WriteAllText(
                    FilePath,
                    json);
            }
            catch
            {
                // Não interromper o PrintNotify
                // caso não seja possível salvar.
            }
        }
    }
}