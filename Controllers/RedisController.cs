using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using RedisNet6.Models;
using System.Text;
using System.Text.Json;

namespace RedisNet6.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RedisController : ControllerBase
    {
        private readonly ILogger<RedisController> _logger;
        private readonly IDistributedCache _redis;

        public RedisController(ILogger<RedisController> logger, IDistributedCache redis)
        {
            _logger = logger;
            _redis = redis;
        }

        [HttpGet("get")]
        public IActionResult GetPessoa([FromQuery] int id)
        {
            _logger.LogInformation("Obter Pessoa: {0}", id);
            var pessoaByte = _redis.Get(id.ToString());
            var pessoaString = Encoding.UTF8.GetString(pessoaByte);
            var pessoa = JsonSerializer.Deserialize<Pessoa>(pessoaString);
            return Ok(pessoa);
        }

        [HttpPost("create")]
        public IActionResult CreatePessoa([FromBody] Pessoa pessoa)
        {
            _logger.LogInformation("Criar Pessoa: {0}", pessoa);
            var pessoaString = JsonSerializer.Serialize(pessoa);
            var pessoaByte = Encoding.UTF8.GetBytes(pessoaString);
            var uri = $"{Request.Scheme}://{Request.Host}/redis/get?id={pessoa.Id}";
            _redis.Set(pessoa.Id.ToString(), pessoaByte);
            return Created(new Uri(uri), pessoa);
        }

        [HttpDelete("delete")]
        public IActionResult DeletePessoa([FromQuery] int id)
        {
            _logger.LogInformation("Deletar Pessoa: {0}", id);
            _redis.Remove(id.ToString());
            return Ok(id);
        }
    }
}