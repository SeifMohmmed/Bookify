using Microsoft.Extensions.Options;
using Quartz;

namespace Bookify.Infrastructure.Outbox;
/// <summary>
/// Configures Quartz job scheduling for the Outbox processor.
/// </summary>
internal class ProcessOutboxMessagesJobSetup(
    IOptions<OutboxOptions> outboxOptions) : IConfigureOptions<QuartzOptions>
{
    private readonly OutboxOptions _outboxOptions = outboxOptions.Value;

    public void Configure(QuartzOptions options)
    {
        var jobKey = new JobKey(nameof(ProcessOutboxMessagesJob));

        options
            // Register the job
            .AddJob<ProcessOutboxMessagesJob>(job => job.WithIdentity(jobKey))

            // Configure the trigger that runs the job
            .AddTrigger(trigger =>
                trigger
                    .ForJob(jobKey)
                    .WithSimpleSchedule(schedule =>
                        schedule
                            .WithIntervalInSeconds(_outboxOptions.IntervalInSeconds)
                            .RepeatForever()));
    }
}
