using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace crud.Models
{
    public class Funcionario
    {
        //cria as classes pro funcionario, com os mesmos nomes do banco de dados    
        //a chave primária da tabela funcionario
        [Key]
        public int cd_funcionario { get; set; }
        public string nm_funcionario { get; set; }
        public string cd_cpf { get; set; }

        //chave extrangeira pra empresa, relacionando as tabelas
        [ForeignKey("Empresa")]
        public int cd_empresa { get; set; }
        public Empresa? Empresa { get; set; }
    }
}
