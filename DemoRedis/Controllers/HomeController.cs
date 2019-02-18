using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DemoRedis.Models;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace DemoRedis.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDistributedCache _distributedCache;

        public HomeController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        private void ArmazenarCache(string chave, string valor)
        {
            DistributedCacheEntryOptions opcoesCache = new DistributedCacheEntryOptions();
            //Define o tempo em que o cache vai ser expirado
            opcoesCache.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
            //Define o tempo em que o cache pode ficar inativa antes de ser removido
            // opcoesCache.SetSlidingExpiration(TimeSpan.FromMinutes(2));

            _distributedCache.SetString(chave, valor, opcoesCache);
        }

        public IActionResult Index()
        {
            var totalAlunos = 
                _distributedCache.GetString("TotalAlunos");            
            if (totalAlunos == null)
            {
                totalAlunos = "15";
                ArmazenarCache("TotalAlunos", totalAlunos);                
            }
            ViewBag.TotalAlunos = totalAlunos;

            var alunoCache = 
                _distributedCache.GetString("Aluno");
            if (alunoCache == null)
            {
                Aluno aluno = new Aluno
                {
                    Id = 1,
                    Nome = "Nicolas Rezende",
                    DataNascimento = new DateTime(1999, 06, 04)
                };
                var strAluno = JsonConvert.SerializeObject(aluno);
                ArmazenarCache("Aluno", strAluno);
                ViewBag.Aluno = aluno;
            }
            else
            {
                Aluno aluno = JsonConvert.DeserializeObject<Aluno>(alunoCache);
                ViewBag.Aluno = aluno;
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
