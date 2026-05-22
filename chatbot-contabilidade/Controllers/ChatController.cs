using chatbot_contabilidade.Models;
using chatbot_contabilidade.Services;
using Microsoft.AspNetCore.Mvc;

namespace chatbot_contabilidade.Controllers
{
    //controlador para lidar com as requisições de chat
    [ApiController]
    //rota para acessar o controlador, seguindo o padrão api/nome-do-controlador
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly GroqService _groqService;

        //injeção de dependência do GroqService para usar o serviço de comunicação com a API da Groq
        public ChatController(GroqService groqService)
        {
            _groqService = groqService;
        }

        //método para receber a mensagem do usuário e o histórico de mensagens, enviar para o GroqService e retornar a resposta
        [HttpPost]
        public async Task<IActionResult> Enviar([FromBody] MensagemRequest request)
        {
            //construir o histórico de mensagens para enviar ao GroqService, incluindo a mensagem do usuário e o histórico anterior
            var historico = new List<object>
            {
                
                new { role = "system", content = @"Você é um assistente especializado em contabilidade brasileira. 
                Responda sempre em português. Ajude com dúvidas sobre impostos, DRE, balanço patrimonial, 
                fluxo de caixa, legislação fiscal e contábil brasileira. 
                Se não tiver certeza, recomende consultar um contador, mas lembre-se se o usuario comentar algo sem haver com contabilidade diga que vc nao foi projetado 
                pra isso pois vc so fale sobre contabilidade e afins. Outra coisa sempre va direto ao ponto nao fique dando respostas redundantes "}
            };

            //adicionar o histórico de mensagens anteriores ao histórico que será enviado para o GroqService
            foreach (var msg in request.Historico)
            {
                historico.Add(new { role = msg.role, content = msg.content });
            }

            //adicionar a mensagem atual do usuário ao histórico
            historico.Add(new { role = "user", content = request.Mensagem });

            //enviar o histórico de mensagens para o GroqService e obter a resposta
            var resposta = await _groqService.EnviarMensagemAsync(historico);

            return Ok(new { resposta });
        }
    }
}