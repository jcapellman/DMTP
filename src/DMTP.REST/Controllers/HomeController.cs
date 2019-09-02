using System;
using System.Collections.Generic;
using System.Linq;

using DMTP.lib.Common;
using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.dal.Manager;
using DMTP.lib.Managers;

using DMTP.REST.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DMTP.REST.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public HomeController(DatabaseManager database, Settings settings) : base(database, settings) { }

        public IActionResult Index()
        {
            var model = new HomeDashboardModel
            {
                Jobs = new JobManager(Database).GetJobs().Where(a => !a.Completed).ToList()
            };

            var assemblies = new AssemblyManager(Database).GetUploadedAssembliesList();

            model.ModelTypes = new List<SelectListItem>();

            foreach (var assembly in assemblies)
            {
                foreach (var predictor in assembly.Predictors)
                {
                    model.ModelTypes.Add(new SelectListItem(predictor, assembly.ID.ToString()));
                }
            }

            model.Workers = new WorkerManager(Database).GetWorkers().OrderBy(a => a.Name).ToList();

            model.AssignWorkers = new List<SelectListItem>
            {
                new SelectListItem(Constants.WORKER_AUTO_ASSIGN, Constants.WORKER_AUTO_ASSIGN)
            };

            model.AssignWorkers.AddRange(model.Workers.Select(a => new SelectListItem(a.Name, a.Name)));

            return View(model);
        }

        [HttpGet]
        public IActionResult AddJob([FromQuery]string name, [FromQuery]string trainingDataPath, [FromQuery]string modelType, [FromQuery]string selectedWorker)
        {
            var job = new Jobs
            {
                Name = name,
                TrainingDataPath = trainingDataPath,
                ModelType = modelType
            };

            if (selectedWorker != Constants.WORKER_AUTO_ASSIGN)
            {
                job.AssignedHost = selectedWorker;
            }

            SaveJob(job);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("/Download")]
        public FileResult Download([FromQuery]Guid id)
        {
            var job = new JobManager(Database).GetJob(id);

            return job == null ? 
                File(new byte[0], System.Net.Mime.MediaTypeNames.Text.Plain, "Model not found") : 
                File(job.Model, System.Net.Mime.MediaTypeNames.Application.Octet, $"{job.Name}.{Constants.MODEL_EXTENSION}");
        }
    }
}