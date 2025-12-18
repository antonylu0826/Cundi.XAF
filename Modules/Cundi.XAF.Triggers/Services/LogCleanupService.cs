using Cundi.XAF.Triggers.BusinessObjects;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;

namespace Cundi.XAF.Triggers.Services;

/// <summary>
/// Service for cleaning up old trigger logs based on retention policy.
/// </summary>
public static class LogCleanupService
{
    /// <summary>
    /// Default retention period in days.
    /// </summary>
    public const int DefaultRetentionDays = 90;

    /// <summary>
    /// Cleans up trigger logs older than the specified number of days.
    /// </summary>
    /// <param name="objectSpaceFactory">The object space factory to use.</param>
    /// <param name="retentionDays">Number of days to retain logs. Logs older than this will be deleted.</param>
    /// <returns>The number of logs deleted.</returns>
    public static int CleanupOldLogs(IObjectSpaceFactory objectSpaceFactory, int retentionDays = DefaultRetentionDays)
    {
        if (retentionDays <= 0)
        {
            return 0; // Retention disabled
        }

        try
        {
            using var objectSpace = objectSpaceFactory.CreateObjectSpace<TriggerLog>();

            var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);
            var criteria = CriteriaOperator.Parse("ExecutionTime < ?", cutoffDate);

            var oldLogs = objectSpace.GetObjects<TriggerLog>(criteria);
            var count = oldLogs.Count;

            if (count > 0)
            {
                objectSpace.Delete(oldLogs);
                objectSpace.CommitChanges();
            }

            return count;
        }
        catch
        {
            // Swallow exceptions to avoid affecting application startup
            return 0;
        }
    }
}
