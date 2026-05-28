using Microsoft.AspNetCore.Mvc;
using crud.Data;
using crud.Models;

namespace crud.Controllers
{
    [Route("api/funcionario")]
    [ApiController]
    public class FuncionarioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FuncionarioController(AppDbContext context)
        {
            _context = context;
        }

        // buscar todos os funcionarios
        [HttpGet]
        public IActionResult GetAll()
        {
            //vai no bnaco de dados e pega todos os funcionarios
            var funcionarios = _context.Funcionarios.ToList();
            //retorna a lista de funcionarios
            return Ok(funcionarios);
        }

        // buscar funcionario por id
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            //busca o funcionario pelo id dele
            var funcionario = _context.Funcionarios.Find(id);
            //senao achar retornar 404
            if (funcionario == null) return NotFound();
            //se achar retorna o funcionario
            return Ok(funcionario);
        }

        //adcionar novos funcionarios
        [HttpPost]
        public IActionResult Create(Funcionario funcionario)
        {
            //adiciona o funcionario na memoria
            _context.Funcionarios.Add(funcionario);
            //salva no banco de dados
            _context.SaveChanges();
            return Ok(funcionario);
        }

        //editar funcionario existente
        [HttpPut("{id}")]
        public IActionResult Update(int id, Funcionario funcionario)
        {
            //procura o funcionario no banco de dados
            var funcionarioExistente = _context.Funcionarios.Find(id);
            if (funcionarioExistente == null) return NotFound();
            funcionarioExistente.nm_funcionario = funcionario.nm_funcionario;
            funcionarioExistente.cd_cpf = funcionario.cd_cpf;
            _context.SaveChanges();
            return Ok(funcionarioExistente);
        }

        //deletar funcionario existente
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            //procura o funcionario e caso ele nao exita ele devolve 404 e caso ele exista ele deleta o funcionario
            var funcionario = _context.Funcionarios.Find(id);
            if (funcionario == null) return NotFound();
            _context.Funcionarios.Remove(funcionario);
            //salva as alteracoes no banco de dados
            _context.SaveChanges();
            return Ok();
        }
    }
}