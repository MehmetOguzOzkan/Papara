using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Papara.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        /// <summary>
        /// Enqueues a fire-and-forget job.
        /// </summary>
        /// <remarks>
        /// This endpoint schedules a background job to be executed immediately. The job simply writes "Fire-and-forget!" to the console.
        /// </remarks>
        /// <response code="200">Returns the job ID of the enqueued job.</response>
        [HttpGet("FireAndForget")]
        public string FireAndForget()
        {
            var jobId = BackgroundJob.Enqueue(() => Console.WriteLine("Fire-and-forget!"));
            return jobId;
        }

        /// <summary>
        /// Schedules a delayed job.
        /// </summary>
        /// <remarks>
        /// This endpoint schedules a background job to be executed after a delay of 27 seconds. The job writes "Delayed!" to the console.
        /// </remarks>
        /// <response code="200">Returns the job ID of the scheduled job.</response>
        [HttpGet("Delayed")]
        public string Delayed()
        {
            var jobId = BackgroundJob.Schedule(() => Console.WriteLine("Delayed!"), TimeSpan.FromSeconds(27));
            return jobId;
        }

        /// <summary>
        /// Adds or updates a recurring job.
        /// </summary>
        /// <remarks>
        /// This endpoint schedules a recurring background job that runs daily and another that runs at 3:15 PM on the 1st of every month. The job writes "Recurring!" to the console.
        /// </remarks>
        /// <response code="200">Returns the ID of the recurring job.</response>
        [HttpGet("Recurring")]
        public string Recurring()
        {
            RecurringJob.AddOrUpdate("nntdr", () => Console.WriteLine("Recurring!"), Cron.Daily);
            RecurringJob.AddOrUpdate("agust", () => Console.WriteLine("Recurring!"), "15 14 1 * *");

            return "nntdr";
        }

        /// <summary>
        /// Schedules a continuation job.
        /// </summary>
        /// <remarks>
        /// This endpoint schedules a background job to be executed after a delay of 50 seconds. Once the initial job is completed, a continuation job is scheduled to write "Continuation!" to the console.
        /// </remarks>
        /// <response code="200">Returns the job ID of the initial delayed job.</response>
        [HttpGet("Continuations")]
        public string Continuations()
        {
            var jobId = BackgroundJob.Schedule(() => Console.WriteLine("Delayed!"), TimeSpan.FromSeconds(50));
            BackgroundJob.ContinueJobWith(jobId, () => Console.WriteLine("Continuation!"));
            return jobId;
        }
    }
}
