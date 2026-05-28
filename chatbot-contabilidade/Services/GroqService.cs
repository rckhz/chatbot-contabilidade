using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace chatbot_contabilidade.Services
{
    public class GroqService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = string.Empty;
        private readonly string _crudBaseUrl = "https://localhost:7109/api";

        public GroqService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiKey = configuration["Groq:ApiKey"] ?? string.Empty;
        }

        // Define as ferramentas disponíveis pro Groq usar
        private object[] GetTools()
        {
            return new object[]
            {
                new {
                    type = "function",
                    function = new {
                        name = "listar_empresas",
                        description = "Lista todas as empresas cadastradas"
                    }
                },
                new {
                    type = "function",
                    function = new {
                        name = "listar_funcionarios",
                        description = "Lista todos os funcionários cadastrados"
                    }
                },
                new {
                    type = "function",
                    function = new {
                        name = "criar_empresa",
                        description = "Cadastra uma nova empresa",
                        parameters = new {
                            type = "object",
                            properties = new {
                                cd_cnpj = new { type = "string", description = "CNPJ da empresa" },
                                nm_razao_social_empresa = new { type = "string", description = "Razão social da empresa" },
                                ds_endereco = new { type = "string", description = "Endereço da empresa" }
                            },
                            required = new[] { "cd_cnpj", "nm_razao_social_empresa", "ds_endereco" }
                        }
                    }
                },
                new {
                    type = "function",
                    function = new {
                        name = "criar_funcionario",
                        description = "Cadastra um novo funcionário",
                        parameters = new {
                            type = "object",
                            properties = new {
                                nm_funcionario = new { type = "string", description = "Nome do funcionário" },
                                cd_cpf = new { type = "string", description = "CPF do funcionário" },
                                cd_empresa = new { type = "integer", description = "ID da empresa do funcionário" }
                            },
                            required = new[] { "nm_funcionario", "cd_cpf", "cd_empresa" }
                        }
                    }
                },
                new {
                    type = "function",
                    function = new {
                        name = "deletar_empresa",
                        description = "Deleta uma empresa pelo ID",
                        parameters = new {
                            type = "object",
                            properties = new {
                                id = new { type = "integer", description = "ID da empresa" }
                            },
                            required = new[] { "id" }
                        }
                    }
                },
                new {
                    type = "function",
                    function = new {
                        name = "deletar_funcionario",
                        description = "Deleta um funcionário pelo ID",
                        parameters = new {
                            type = "object",
                            properties = new {
                                id = new { type = "integer", description = "ID do funcionário" }
                            },
                            required = new[] { "id" }
                        }
                    }
                },


                new {
                    type = "function",
                    function = new {
                        name = "editar_empresa",
                        description = "Edita as informações de uma empresa pelo ID",
                        parameters = new {
                            type = "object",
                            properties = new {
                                id = new { type = "integer", description = "ID da empresa" },
                                cd_cnpj = new { type = "string", description = "CNPJ da empresa" },
                                nm_razao_social_empresa = new { type = "string", description = "Razão social da empresa" },
                                ds_endereco = new { type = "string", description = "Endereço da empresa" }
                            },
                            required = new[] { "id", "cd_cnpj", "nm_razao_social_empresa", "ds_endereco" }
                        }
                    }
                },
                new {
                    type = "function",
                    function = new {
                        name = "editar_funcionario",
                        description = "Edita as informações de um funcionário pelo ID",
                        parameters = new {
                            type = "object",
                            properties = new {
                                id = new { type = "integer", description = "ID do funcionário" },
                                nm_funcionario = new { type = "string", description = "Nome do funcionário" },
                                cd_cpf = new { type = "string", description = "CPF do funcionário" },
                                cd_empresa = new { type = "integer", description = "ID da empresa do funcionário" }
                            },
                            required = new[] { "id", "nm_funcionario", "cd_cpf", "cd_empresa" }
                        }
                    }
                },
                new {
                    type = "function",
                    function = new {
                        name = "buscar_empresa_por_nome",
                        description = "Busca uma empresa pelo nome para obter o ID correto",
                        parameters = new {
                            type = "object",
                            properties = new {
                                nome = new { type = "string", description = "Nome ou parte do nome da empresa" }
                            },
                            required = new[] { "nome" }
                        }
                    }
                },

            };
        }

        // Executa a ferramenta escolhida pelo Groq
        private async Task<string> ExecutarFerramenta(string nomeFerramenta, JsonElement argumentos)
        {
            switch (nomeFerramenta)
            {
                case "listar_empresas":
                    var empresas = await _httpClient.GetStringAsync($"{_crudBaseUrl}/empresa");
                    return empresas;

                case "listar_funcionarios":
                    var funcionarios = await _httpClient.GetStringAsync($"{_crudBaseUrl}/funcionario");
                    return funcionarios;

                case "criar_empresa":
                    var novaEmpresa = new
                    {
                        cd_cnpj = argumentos.GetProperty("cd_cnpj").GetString(),
                        nm_razao_social_empresa = argumentos.GetProperty("nm_razao_social_empresa").GetString(),
                        ds_endereco = argumentos.GetProperty("ds_endereco").GetString()
                    };
                    var jsonEmpresa = JsonSerializer.Serialize(novaEmpresa);
                    var resEmpresa = await _httpClient.PostAsync($"{_crudBaseUrl}/empresa",
                        new StringContent(jsonEmpresa, Encoding.UTF8, "application/json"));
                    return await resEmpresa.Content.ReadAsStringAsync();

                case "criar_funcionario":
                    // Verifica se a empresa existe antes de criar
                    var empresasJson = await _httpClient.GetStringAsync($"{_crudBaseUrl}/empresa");
                    var docVerifica = JsonDocument.Parse(empresasJson);
                    var empresaExiste = docVerifica.RootElement.EnumerateArray()
                        .Any(e => e.GetProperty("cd_empresa").GetInt32() == argumentos.GetProperty("cd_empresa").GetInt32());

                    if (!empresaExiste)
                    {
                        return $"Empresa com ID {argumentos.GetProperty("cd_empresa").GetInt32()} não existe. Empresas disponíveis: {empresasJson}";
                    }

                    var novoFunc = new
                    {
                        nm_funcionario = argumentos.GetProperty("nm_funcionario").GetString(),
                        cd_cpf = argumentos.GetProperty("cd_cpf").GetString(),
                        cd_empresa = argumentos.GetProperty("cd_empresa").GetInt32()
                    };
                    var jsonFunc = JsonSerializer.Serialize(novoFunc);
                    var resFunc = await _httpClient.PostAsync($"{_crudBaseUrl}/funcionario",
                        new StringContent(jsonFunc, Encoding.UTF8, "application/json"));
                    return await resFunc.Content.ReadAsStringAsync();

                case "deletar_empresa":
                    var idEmpresa = argumentos.GetProperty("id").GetInt32();
                    var delEmpresa = await _httpClient.DeleteAsync($"{_crudBaseUrl}/empresa/{idEmpresa}");
                    return delEmpresa.IsSuccessStatusCode ? "Empresa deletada com sucesso" : "Erro ao deletar empresa";

                case "deletar_funcionario":
                    var idFunc = argumentos.GetProperty("id").GetInt32();
                    var delFunc = await _httpClient.DeleteAsync($"{_crudBaseUrl}/funcionario/{idFunc}");
                    return delFunc.IsSuccessStatusCode ? "Funcionário deletado com sucesso" : "Erro ao deletar funcionário";
                case "editar_empresa":
                    var idEditEmpresa = argumentos.GetProperty("id").GetInt32();
                    var empresaEditada = new
                    {
                        cd_empresa = idEditEmpresa,
                        cd_cnpj = argumentos.GetProperty("cd_cnpj").GetString(),
                        nm_razao_social_empresa = argumentos.GetProperty("nm_razao_social_empresa").GetString(),
                        ds_endereco = argumentos.GetProperty("ds_endereco").GetString()
                    };
                    var jsonEditEmpresa = JsonSerializer.Serialize(empresaEditada);
                    var resEditEmpresa = await _httpClient.PutAsync($"{_crudBaseUrl}/empresa/{idEditEmpresa}",
                        new StringContent(jsonEditEmpresa, Encoding.UTF8, "application/json"));
                    return resEditEmpresa.IsSuccessStatusCode ? "Empresa atualizada com sucesso" : "Erro ao atualizar empresa";

                case "editar_funcionario":
                    var idEditFunc = argumentos.GetProperty("id").GetInt32();
                    var funcEditado = new
                    {
                        cd_funcionario = idEditFunc,
                        nm_funcionario = argumentos.GetProperty("nm_funcionario").GetString(),
                        cd_cpf = argumentos.GetProperty("cd_cpf").GetString(),
                        cd_empresa = argumentos.GetProperty("cd_empresa").GetInt32()
                    };
                    var jsonEditFunc = JsonSerializer.Serialize(funcEditado);
                    var resEditFunc = await _httpClient.PutAsync($"{_crudBaseUrl}/funcionario/{idEditFunc}",
                        new StringContent(jsonEditFunc, Encoding.UTF8, "application/json"));
                    return resEditFunc.IsSuccessStatusCode ? "Funcionário atualizado com sucesso" : "Erro ao atualizar funcionário";

                case "buscar_empresa_por_nome":
                    var todasEmpresas = await _httpClient.GetStringAsync($"{_crudBaseUrl}/empresa");
                    var nome = argumentos.GetProperty("nome").GetString()?.ToLower();
                    var docEmpresas = JsonDocument.Parse(todasEmpresas);
                    var empresaEncontrada = docEmpresas.RootElement.EnumerateArray()
                        .Where(e => e.GetProperty("nm_razao_social_empresa").GetString()?.ToLower().Contains(nome!) == true)
                        .Select(e => e.ToString())
                        .FirstOrDefault();
                    return empresaEncontrada ?? "Empresa não encontrada";

                default:
                    return "Ferramenta não encontrada";
            }
        }

        public async Task<string> EnviarMensagemAsync(List<object> historico)
        {
            var requestBody = new
            {
                model = "llama-3.3-70b-versatile",
                messages = historico,
                tools = GetTools(),
                tool_choice = "auto"
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            var response = await _httpClient.PostAsync("https://api.groq.com/openai/v1/chat/completions", content);
            var responseString = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseString);

            if (!doc.RootElement.TryGetProperty("choices", out var choices) || choices.GetArrayLength() == 0)
            {
                return "Serviço temporariamente indisponível. Tente novamente em alguns instantes.";
            }

            var choice = choices[0];
            var message = choice.GetProperty("message");
            var finishReason = choice.GetProperty("finish_reason").GetString();

            // Se o Groq quer usar uma ferramenta
            if (finishReason == "tool_calls")
            {
                var toolCalls = message.GetProperty("tool_calls");
                var resultados = new List<object>();

                // Serializa o tool_calls pra não perder a referência
                var toolCallsSerialized = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(toolCalls));

                foreach (var toolCall in toolCallsSerialized.EnumerateArray())
                {
                    var toolName = toolCall.GetProperty("function").GetProperty("name").GetString()!;
                    var toolArgs = toolCall.GetProperty("function").GetProperty("arguments").GetString();
                    JsonElement argumentos = default;
                    if (!string.IsNullOrEmpty(toolArgs) && toolArgs != "null")
                    {
                        argumentos = JsonSerializer.Deserialize<JsonElement>(toolArgs);
                    }
                    var toolId = toolCall.GetProperty("id").GetString();

                    var resultado = await ExecutarFerramenta(toolName, argumentos);
                    resultados.Add(new { role = "tool", tool_call_id = toolId, content = resultado });
                }

                var mensagensComResultado = new List<object>(historico)
    {
            new {
                role = "assistant",
                tool_calls = toolCallsSerialized
            }
    };
                mensagensComResultado.AddRange(resultados);

                var requestBody2 = new
                {
                    model = "llama-3.3-70b-versatile",
                    messages = mensagensComResultado
                };

                var json2 = JsonSerializer.Serialize(requestBody2);
                var content2 = new StringContent(json2, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                var response2 = await _httpClient.PostAsync("https://api.groq.com/openai/v1/chat/completions", content2);
                var responseString2 = await response2.Content.ReadAsStringAsync();

                var doc2 = JsonDocument.Parse(responseString2);

                if (!doc2.RootElement.TryGetProperty("choices", out var choices2) || choices2.GetArrayLength() == 0)
                {
                    return "Serviço temporariamente indisponível. Tente novamente em alguns instantes.";
                }

                return choices2[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? "";
            }

            // Resposta normal sem ferramenta
            return message.GetProperty("content").GetString() ?? "";
        }
    }
}