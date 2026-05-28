using Microsoft.AspNetCore.Mvc;
using crud.Data;
using crud.Models;

namespace crud.Controllers
{
    [Route("api/empresa")]
    [ApiController]
    public class EmpresaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmpresaController(AppDbContext context)
        {
            _context = context;
        }

        // buscar todas as empresas
        [HttpGet]
        public IActionResult GetAll()
        {
            //vai no bnaco de dados e pega todas as empresas 
            var empresas = _context.Empresas.ToList();
            //retorna a lista de empresas
            return Ok(empresas);
        }

        // buscar empresa por id
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            //busca a empresa pelo id dela
            var empresa = _context.Empresas.Find(id);
            //senao achar retornar 404
            if (empresa == null) return NotFound();
            //se achar retorna a empresa
            return Ok(empresa);
        }

        //adcionar novas empresas
        [HttpPost]
        public IActionResult Create(Empresa empresa)
        {
            //adiciona a empresa na memoria
            _context.Empresas.Add(empresa);
            //salva no banco de dados
            _context.SaveChanges();
            return Ok(empresa);
        }

        //editar empresa existente
        [HttpPut("{id}")]
        public IActionResult Update(int id, Empresa empresa)
        {
            //procura a empresa no banco de dados
            var empresaExistente = _context.Empresas.Find(id);
            if (empresaExistente == null) return NotFound();
            empresaExistente.nm_razao_social_empresa = empresa.nm_razao_social_empresa;
            empresaExistente.cd_cnpj = empresa.cd_cnpj;
            empresaExistente.ds_endereco = empresa.ds_endereco;
            _context.SaveChanges();
            return Ok(empresaExistente);
        }

        //deletar empresa existente
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            //procura a empresa e caso ela nao exita ele devolve 404 e caso ela exista ele deleta a empresa
            var empresa = _context.Empresas.Find(id);
            if (empresa == null) return NotFound();
            _context.Empresas.Remove(empresa);
            //salva as alteracoes no banco de dados
            _context.SaveChanges();
            return Ok();
        }
    }
}