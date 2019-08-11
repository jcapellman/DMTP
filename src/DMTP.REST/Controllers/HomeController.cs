using System;
using System.Collections.Generic;
using System.Linq;

using DMTP.lib.Common;
using DMTP.lib.Databases.Base;
using DMTP.lib.Databases.Tables;
using DMTP.lib.ML.Base;
using DMTP.REST.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DMTP.REST.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly List<BasePrediction> _assemblies;

        public HomeController(IDatabase database, List<BasePrediction> assemblies) : base(database) { _assemblies = assemblies; }

        public IActionResult Index()
        {
            var model = new HomeDashboardModel
            {
                Jobs = Database.GetJobs().Where(a => !a.Completed).ToList(),
                ModelTypes = _assemblies.OrderBy(a => a.MODEL_NAME)
                    .Select(a => new SelectListItem(a.MODEL_NAME, a.MODEL_NAME)).ToList()
            };

            model.Workers = Database.GetWorkers().Where(a => model.Jobs.Any(b => b.AssignedHost == a.Name)).ToList();

            return View(model);
        }

        [HttpGet]
        public IActionResult AddJob([FromQuery]string name, [FromQuery]string trainingDataPath, [FromQuery]string modelType)
        {
            var job = new Jobs
            {
                Name = name,
                TrainingDataPath = trainingDataPath,
                ModelType = modelType
            };

            SaveJob(job);

            return Index();
        }

        [HttpGet]
        [Route("/Download")]
        public FileResult Download([FromQuery]Guid id)
        {
            var job = Database.GetJob(id);

            return job == null ? 
                File(new byte[0], System.Net.Mime.MediaTypeNames.Text.Plain, "Model not found") : 
                File(job.Model, System.Net.Mime.MediaTypeNames.Application.Octet, $"{job.Name}.{Constants.MODEL_EXTENSION}");
        }
    }
}