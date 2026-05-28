using System.ComponentModel.DataAnnotations;
namespace crud.Models
{
    public class Empresa
    {
        //cria as classes pra empresa, com os mesmos nomes do banco de dados

        //a chave primária da tabela empresa
        [Key]
        public int cd_empresa { get; set; }
        public string cd_cnpj { get; set; }
        public string ds_endereco { get; set; }
        public string nm_razao_social_empresa { get; set; }
    }
}
