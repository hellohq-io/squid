using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using Q.Squid.Config;
using Q.Squid.DAL;
using Q.Squid.Models;
using Q.Squid.Models.Examples;
using Q.Squid.Utils;

using Swashbuckle.AspNetCore.Examples;

namespace Q.Squid.Controllers
{
    [Route("api/[controller]")]
    public class RepositoryConfigController : Controller
    {
        private readonly RepositoryConfigRepository _repo;
        private readonly IMapper _mapper;

        public RepositoryConfigController(
            RepositoryConfigRepository repo,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns all repository configurations.
        /// </summary>
        /// <remarks>
        /// Returns all repository configurations.
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(typeof(List<RepositoryConfigModel>), (int)HttpStatusCode.OK)]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(RepositoryConfigExamples))]
        public async Task<IActionResult> Get()
        {
            var repos = await _repo.GetAll();
            var returnModels = _mapper.Map<IEnumerable<RepositoryConfigModel>>(repos);
            return Ok(returnModels);
        }

        /// <summary>
        /// Returns the repository configuration with the specified id.
        /// </summary>
        /// <remarks>
        /// Returns the repository configuration with the specified id.
        /// </remarks>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RepositoryConfigModel), (int)HttpStatusCode.OK)]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(RepositoryConfigExample))]
        public async Task<IActionResult> Get(string id)
        {
            var repo = await _repo.Get(id);
            if (repo == null)
                return NotFound();

            var returnModel = _mapper.Map<RepositoryConfigModel>(repo);
            return Ok(returnModel);
        }

        /// <summary>
        /// Adds a new repository configuration.
        /// </summary>
        /// <remarks>
        /// Adds a new repository configuration.
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(RepositoryConfigModel), (int)HttpStatusCode.OK)]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(RepositoryConfigExample))]
        [SwaggerRequestExample(typeof(RepositoryConfigFormModel), typeof(RepositoryConfigFormExample))]
        public async Task<IActionResult> Post([FromBody] RepositoryConfigFormModel model)
        {
            if (model == null)
                return BadRequest("Model invalid");

            var repo = _mapper.Map<RepositoryConfig>(model);
            repo.Password = Crypto.EncryptString(model.Password, SquidConfig.CryptoKey);
            repo.Valid = false;
            repo.ErrorMessage = "Validation pending...";

            await _repo.Add(repo);

            var returnModel = _mapper.Map<RepositoryConfigModel>(repo);
            return Ok(returnModel);
        }

        /// <summary>
        /// Updates the repository configuration with the specified id.
        /// </summary>
        /// <remarks>
        /// Updates the repository configuration with the specified id.
        /// </remarks>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RepositoryConfigModel), (int)HttpStatusCode.OK)]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(RepositoryConfigExample))]
        [SwaggerRequestExample(typeof(RepositoryConfigFormModel), typeof(RepositoryConfigFormExample))]
        public async Task<IActionResult> Put(string id, [FromBody] RepositoryConfigFormModel model)
        {
            if (model == null)
                return BadRequest("Model invalid");

            var repo = await _repo.Get(id);
            if (repo == null)
                return NotFound();

            repo.Name = model.Name;
            repo.Branch = model.Branch;
            repo.RepoSlug = model.RepoSlug;
            repo.ProjectFile = model.ProjectFile;
            repo.Password = Crypto.EncryptString(model.Password, SquidConfig.CryptoKey);
            repo.Valid = false;
            repo.ErrorMessage = "Validation pending...";

            await _repo.Update(repo);

            var returnModel = _mapper.Map<RepositoryConfigModel>(repo);
            return Ok(returnModel);
        }

        /// <summary>
        /// Deletes the repository configuration with the specified id.
        /// </summary>
        /// <remarks>
        /// Deletes the repository configuration with the specified id.
        /// </remarks>
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Delete(string id)
        {
            var repo = await _repo.Get(id);
            if (repo == null)
                return NotFound();

            await _repo.Delete(id);

            return NoContent();
        }
    }
}
