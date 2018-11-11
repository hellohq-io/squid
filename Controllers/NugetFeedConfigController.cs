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
    public class NugetFeedConfigController : Controller
    {
        private readonly NugetFeedConfigRepository _repo;
        private readonly IMapper _mapper;

        public NugetFeedConfigController(
            NugetFeedConfigRepository repo,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns all NuGet feed configurations.
        /// </summary>
        /// <remarks>
        /// Returns all NuGet feed configurations.
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(typeof(List<NuGetFeedModel>), (int)HttpStatusCode.OK)]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(NuGetFeedExamples))]
        public async Task<IActionResult> Get()
        {
            var feed = await _repo.GetAll();
            var returnModels = _mapper.Map<IEnumerable<NuGetFeedModel>>(feed);
            return Ok(returnModels);
        }

        /// <summary>
        /// Returns the NuGet feed configuration with the specified id.
        /// </summary>
        /// <remarks>
        /// Returns the NuGet feed configuration with the specified id.
        /// </remarks>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(NuGetFeedModel), (int)HttpStatusCode.OK)]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(NuGetFeedExample))]
        public async Task<IActionResult> Get(string id)
        {
            var feed = await _repo.Get(id);
            if (feed == null)
                return NotFound();

            var returnModel = _mapper.Map<NuGetFeedModel>(feed);
            return Ok(returnModel);
        }

        /// <summary>
        /// Adds a new NuGet feed configuration.
        /// </summary>
        /// <remarks>
        /// Adds a new NuGet feed configuration.
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(NuGetFeedModel), (int)HttpStatusCode.OK)]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(NuGetFeedExample))]
        [SwaggerRequestExample(typeof(NuGetFeedFormModel), typeof(NuGetFeedFormExample))]
        public async Task<IActionResult> Post([FromBody] NuGetFeedFormModel model)
        {
            if (model == null)
                return BadRequest("Model invalid");

            var feed = _mapper.Map<NugetFeedConfig>(model);
            feed.ApiKey = Crypto.EncryptString(model.ApiKey, SquidConfig.CryptoKey);
            feed.Valid = false;
            feed.ErrorMessage = "Validation pending...";

            await _repo.Add(feed);

            var returnModel = _mapper.Map<NuGetFeedModel>(feed);
            return Ok(returnModel);
        }

        /// <summary>
        /// Updates the NuGet feed configuration with the specified id.
        /// </summary>
        /// <remarks>
        /// Updates the NuGet feed configuration with the specified id.
        /// </remarks>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(NuGetFeedModel), (int)HttpStatusCode.OK)]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(NuGetFeedExample))]
        [SwaggerRequestExample(typeof(NuGetFeedFormModel), typeof(NuGetFeedFormExample))]
        public async Task<IActionResult> Put(string id, [FromBody] NuGetFeedFormModel model)
        {
            if (model == null)
                return BadRequest("Model invalid");

            var feed = await _repo.Get(id);
            if (feed == null)
                return NotFound();

            feed.Name = model.Name;
            feed.FeedURL = model.FeedURL;
            feed.ApiKeyHeaderName = model.ApiKeyHeaderName;
            if (!string.IsNullOrWhiteSpace(model.ApiKey))
                feed.ApiKey = Crypto.EncryptString(model.ApiKey, SquidConfig.CryptoKey);
            feed.Valid = false;
            feed.ErrorMessage = "Validation pending...";

            await _repo.Update(feed);

            var returnModel = _mapper.Map<NuGetFeedModel>(feed);
            return Ok(returnModel);
        }

        /// <summary>
        /// Deletes the NuGet feed configuration with the specified id.
        /// </summary>
        /// <remarks>
        /// Deletes the NuGet feed configuration with the specified id.
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
