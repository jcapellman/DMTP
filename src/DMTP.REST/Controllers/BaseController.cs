using System;
using System.Linq;
using System.Net;
using System.Net.Mail;

using DMTP.lib.Common;
using DMTP.lib.Databases.Base;
using DMTP.lib.Databases.Tables;

using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IDatabase Database;

        protected Settings CurrentSettings;

        private readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        protected BaseController(IDatabase database, Settings settings)
        {
            Database = database;
            CurrentSettings = settings;
        }

        protected void SendEmail(string receiverEmail, string subject, string body)
        {
            try
            {
                var client = new SmtpClient(CurrentSettings.SMTPHostName)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(CurrentSettings.SMTPUsername, CurrentSettings.SMTPPassword),
                    Port = CurrentSettings.SMTPPortNumber
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(lib.Common.Constants.SENDER_EMAIL)
                };

                mailMessage.To.Add(receiverEmail);
                mailMessage.Body = body;
                mailMessage.Subject = subject;

                client.Send(mailMessage);
            }
            catch (Exception ex)
            {
                Log.Error($"Failure sending email to {receiverEmail}, subject: {subject}, body: {body} due to {ex}");
            }
        }

        protected Guid? SaveJob(Jobs job)
        {
            job.ID = Guid.NewGuid();
            job.SubmissionTime = DateTime.Now;

            var hosts = Database.GetWorkers();

            if (hosts == null)
            {
                return null;
            }

            if (hosts.Any())
            {
                var jobs = Database.GetJobs().Where(a => !a.Completed).ToList();

                foreach (var host in hosts)
                {
                    if (jobs.Any(a => a.AssignedHost == host.Name))
                    {
                        continue;
                    }

                    job.AssignedHost = host.Name;

                    break;
                }

                if (string.IsNullOrEmpty(job.AssignedHost))
                {
                    job.AssignedHost = Constants.UNASSIGNED_JOB;
                }
            }
            else
            {
                job.AssignedHost = Constants.UNASSIGNED_JOB;
            }

            if (Database.AddJob(job))
            {
                return job.ID;
            }

            return null;
        }
    }
}