namespace chatbot_contabilidade.Models
{

    //enviar mensagem para o backend
    public class MensagemRequest
    {
        //representa a mensagem enviada pelo usuário
        public string Mensagem { get; set; }
        //representa o histórico de mensagens, onde cada mensagem tem um role (user ou assistant) e um content (o conteúdo da mensagem)
        public List<MensagemHistorico> Historico { get; set; } = new();
    }

    //representa o histórico de mensagens
    public class MensagemHistorico
    {
        //role pode ser "user" ou "assistant"
        public string role { get; set; }
        //content é o conteúdo da mensagem
        public string content { get; set; }
    }
}