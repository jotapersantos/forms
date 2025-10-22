using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.Linq;
using System.Threading.Tasks;
using CRM.Application.DTOs.Formularios.Modelos;
using CRM.Application.DTOs.Shared;
using CRM.Application.Interfaces.Formularios.Modelos;
using CRM.Contracts.Formulario;
using CRM.Domain.Dtos.Formularios;
using CRM.WebUI.ViewModels.Formularios;
using CRM.WebUI.ViewModels.Formularios.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM.WebUI.Controllers.Formularios.Modelos;

[Route("cadastros/formularios/modelos")]
public class ModeloController : Controller
{
    private readonly IModeloService _modeloService;

    public ModeloController(IModeloService modeloService)
    {
        _modeloService = modeloService;
    }

    #region CRUD Modelo
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("novo")]
    public IActionResult Create()
    {
        return View();
    }


    [HttpPost("novo")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ModeloCadastroVM modeloCadastroVM)
    {
        try
        {
            if (ModelState.IsValid == false)
            {
                var listaMensagens = new List<string> { "Ocorreu um erro ao salvar o modelo de formulário" };
                listaMensagens.AddRange(ModelState.Values.SelectMany(value => value.Errors.Select(error => error.ErrorMessage)).ToList());
                return Json(new
                {
                    IsSuccess = false,
                    Messages = listaMensagens,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            if (await _modeloService.CreateAsync(GerarModeloDTO(modeloCadastroVM)))
            {
                return Json(new
                {
                    IsSuccess = true,
                    Messages = new List<string> { "Modelo de formulário criado com sucesso" },
                    UrlRedirect = Url.Action("Index", "Modelo"),
                    StatusCode = System.Net.HttpStatusCode.OK
                });
            }

            return Json(new
            {
                IsSuccess = false,
                Messages = new List<string> { "Ocorreu um erro ao salvar o modelo de formulário. Tente atualizar a página e caso o erro persista, contate o Administrador" },
                StatusCode = System.Net.HttpStatusCode.BadRequest
            });
        }
        catch (InvalidOperationException ex)
        {
            return Json(new
            {
                IsSuccess = false,
                Messages = new List<string> { $"Não foi possível criar o modelo de formulário. {ex.Message}" },
                StatusCode = System.Net.HttpStatusCode.BadRequest
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                IsSuccess = false,
                Messages = new List<string> { $"Ocorreu um erro ao salvar o modelo de formulário. Erro: {ex.Message}" },
                StatusCode = System.Net.HttpStatusCode.InternalServerError
            });
        }
    }

    [HttpGet("novo-por-modelo")]
    public async Task<IActionResult> CreateFrom(Guid modeloId)
    {
        ModeloDTO modelo = await _modeloService.GetByIdWithTemaESecoesEPerguntasAsync(modeloId);
        
        ModeloCadastroVM modeloCadastroVM = GerarModeloCadastroVM(modelo);

        return View(modeloCadastroVM);
    }

    [HttpGet("editar/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id)
    {
        if(await _modeloService.IsModeloAssociatedWithFormularioOrAvaliacaoAsync(id))
        {
            return RedirectToAction(nameof(Index));
        }

        ModeloCadastroVM modeloCadastroVM = GerarModeloCadastroVM(await _modeloService.GetByIdWithTemaESecoesEPerguntasAsync(id));

        return View(modeloCadastroVM);
    }

    [HttpPost("editar/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ModeloCadastroVM modeloCadastroVM)
    {
        try
        {
            if (ModelState.IsValid == false)
            {
                var listaMensagens = new List<string> { "Ocorreu um erro ao atualizar o modelo de formulário" };
                listaMensagens.AddRange(ModelState.Values.SelectMany(value => value.Errors.Select(error => error.ErrorMessage)).ToList());
                return Json(new
                {
                    IsSuccess = false,
                    Messages = listaMensagens,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            if (await _modeloService.UpdateAsync(GerarModeloDTO(modeloCadastroVM)))
            {
                return Json(new
                {
                    IsSuccess = true,
                    Messages = new List<string> { "Modelo de formulário atualizado com sucesso" },
                    UrlRedirect = Url.Action("Index", "Modelo"),
                    StatusCode = System.Net.HttpStatusCode.OK
                });
            }

            return Json(new
            {
                IsSuccess = false,
                Messages = new List<string> { "Ocorreu um erro ao salvar o modelo de formulário. Tente atualizar a página e caso o erro persista, contate o Administrador" },
                StatusCode = System.Net.HttpStatusCode.BadRequest
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                IsSuccess = false,
                Messages = new List<string> { $"Ocorreu um erro ao salvar o modelo de formulário. Ex: {ex.Message}" },
                StatusCode = System.Net.HttpStatusCode.InternalServerError
            });
        }
    }

    [HttpPost("excluir/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        try
        {
            if (await _modeloService.DeleteAsync(id) == true)
            {
                return Json(new
                {
                    IsSuccess = true,
                    Messages = new List<string> { "Modelo de formulário removido com sucesso." },
                    UrlReload = true,
                    StatusCode = System.Net.HttpStatusCode.OK
                });
            }

            return Json(new
            {
                IsSuccess = false,
                Messages = new List<string> { "Ocorreu um erro ao remover o modelo de formulário. Tente atualizar a página e caso o erro persista, contate o suporte" },
                StatusCode = System.Net.HttpStatusCode.BadRequest
            });
        }
        catch(InvalidOperationException ex)
        {
            return Json(new
            {
                IsSuccess = false,
                Messages = new List<string> { $"{ex.Message}" },
                StatusCode = System.Net.HttpStatusCode.BadRequest
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                IsSuccess = false,
                Messages = new List<string> { $"Ocorreu um erro ao processar as informações. Erro: {ex.Message}" },
                StatusCode = System.Net.HttpStatusCode.InternalServerError
            });
        }
    }


    [HttpGet("ativar/{id:guid}")]
    public async Task<IActionResult> Ativar(Guid id)
    {
        try
        {
            if (await _modeloService.Ativar(id) == true)
            {
                return new JsonResult(new
                {
                    IsSuccess = true,
                    Messages = new List<string> { "Modelo ativado com sucesso." },
                    UrlReload = false
                })
                {
                    StatusCode = 200
                };
            }

            return new JsonResult(new
            {
                IsSuccess = false,
                Messages = new List<string> { "Não foi possível realizar a ação. Atualize a página e caso o erro persista, contate o suporte" }
            })
            {
                StatusCode = 400
            };
        }
        catch (Exception ex)
        {
            return new JsonResult(new
            {
                IsSuccess = false,
                Messages = new List<string> { $"Ocorreu um erro ao processar as informações. Erro: {ex.Message}" },
            })
            {
                StatusCode = 500
            };
        }
    }

    [HttpGet("desativar/{id:guid}")]
    public async Task<IActionResult> Desativar(Guid id)
    {
        try
        {
            if (await _modeloService.Desativar(id) == true)
            {
                return new JsonResult(new
                {
                    IsSuccess = true,
                    Messages = new List<string> { "Modelo desativado com sucesso." },
                    UrlReload = false
                })
                {
                    StatusCode = 200
                };
            }

            return new JsonResult(new
            {
                IsSuccess = false,
                Messages = new List<string> { "Não foi possível realizar a ação. Atualize a página e caso o erro persista, contate o suporte" },
            })
            {
                StatusCode = 400
            };
        }
        catch (Exception ex)
        {
            return new JsonResult(new
            {
                IsSuccess = false,
                Messages = new List<string> { $"Ocorreu um erro ao processar as informações. Erro: {ex.Message}" },
            })
            {
                StatusCode = 500
            };
        }
    }

    [HttpGet("{id:guid}/detalhar")]
    public async Task<IActionResult> OnGetPreview(Guid id)
    {
        ModeloDTO modeloDto = await _modeloService.GetByIdWithTemaESecoesEPerguntasAsync(id);

        return PartialView("_Preview", modeloDto);
    }
    #endregion

    #region Partial Views
    [HttpGet("obter-nova-secao")]
    public IActionResult OnGetSecaoPartial()
    {
        return PartialView("_Secao", Guid.NewGuid());
    }

    [HttpGet("obter-nova-pergunta")]
    public IActionResult OnGetPerguntaPartial(Guid secaoId, string tipo)
    {
        var perguntaComTipoVM = new PerguntaComTipoVM { SecaoIndex = secaoId, Tipo = tipo };
        return tipo switch
        {
            "TextoCurto" => PartialView("_TextoCurto", perguntaComTipoVM),
            "TextoLongo" => PartialView("_TextoLongo", perguntaComTipoVM),
            "MultiplaEscolha" => PartialView("_MultiplaEscolha", perguntaComTipoVM),
            "CaixaSelecao" => PartialView("_CaixaSelecao", perguntaComTipoVM),
            "ListaSuspensa" => PartialView("_ListaSuspensa", perguntaComTipoVM),
            _ => Json(new
            {
                IsSuccess = false,
                Messages = new List<string>() { "Tipo de pergunta inexistente. Tente atualizar a página" },
                StatusCode = System.Net.HttpStatusCode.NotFound
            }),
        };
    }

    [HttpGet("obter-nova-opcao")]
    public IActionResult OnGetAlternativaPartial(Guid perguntaId, Guid secaoId, string tipoDaPergunta)
    {
        var alternativaDaPerguntaComTipoVM = new AlternativaDaPerguntaComTipoVM { PerguntaIndex = perguntaId, SecaoIndex = secaoId, Tipo = tipoDaPergunta };
        return tipoDaPergunta switch
        {
            "MultiplaEscolha" => PartialView("_AlternativaMultiplaEscolha", alternativaDaPerguntaComTipoVM),
            "CaixaSelecao" => PartialView("_AlternativaCaixaSelecao", alternativaDaPerguntaComTipoVM),
            "ListaSuspensa" => PartialView("_AlternativaListaSuspensa", alternativaDaPerguntaComTipoVM),
            _ => Json(new
            {
                IsSuccess = false,
                Messages = new List<string>() { "O tipo de pergunta não possui alternativas. Tente atualizar a página" },
                StatusCode = System.Net.HttpStatusCode.NotFound
            }),
        };
    }
    #endregion

    #region get Json
    [HttpPost]
    [Route("listagem")]
    public async Task<IActionResult> ObterModelos(GetFormulariosRequest data)
    {
        int page = data.start / data.length + 1;
        PagedResultDto<ModeloIndexDTO> paginaDeModelos = await _modeloService.GetModeloIndexPagedAsync(page, data.length, data.search?.value);

        JsonResult result = Json(new
        {
            data.draw,
            recordsTotal = paginaDeModelos.TotalResult,
            recordsFiltered = paginaDeModelos.TotalResult,
            data = paginaDeModelos.List
        });

        return result;
    }

    [HttpGet]
    [Route("obter-modelos-ativos")]
    public async Task<IActionResult> GetModelosAtivosDictionary(string searchString, int quantity = 15, Guid? ignoredId = null)
    {
        Dictionary<Guid, string> response = await _modeloService.GetDictionaryActivesByTitleAsync(searchString, quantity);

        if (ignoredId is not null)
        {
            response.Remove((Guid)ignoredId);
        }

        return Json(response);
    }

    #endregion

    #region Métodos Privados
    private ModeloCadastroVM GerarModeloCadastroVM(ModeloDTO modeloDto)
    {
        return new ModeloCadastroVM
        {
            Id = modeloDto.Id,
            Titulo = modeloDto.Titulo,
            Ativo = modeloDto.Ativo,
            Secoes = modeloDto.Secoes?.Select(secao => new SecaoCadastroVM
            {
                Id = secao.Id,
                Titulo = secao.Titulo,
                Ordem = secao.Ordem,
                Perguntas = secao.Perguntas?.Select(pergunta => new PerguntaCadastroVM
                {
                    Id = pergunta.Id,
                    Tipo = pergunta.Tipo,
                    Enunciado = pergunta.Enunciado,
                    Descricao = pergunta.Descricao,
                    QuantidadeMinimaCaracteres = pergunta.QuantidadeMinimaCaracteres,
                    QuantidadeMaximaCaracteres = pergunta.QuantidadeMaximaCaracteres,
                    TipoTexto = pergunta.TipoTexto,
                    Obrigatorio = pergunta.Obrigatorio,
                    Ordem = pergunta.Ordem,
                    Alternativas = pergunta.Alternativas?.Select(alternativa => new AlternativaCadastroVM
                    {
                        Id = alternativa.Id,
                        Texto = alternativa.Texto,
                        Ordem = alternativa.Ordem
                    }).ToList()
                }).ToList()
            }).ToList(),
        };
    }

    private ModeloDTO GerarModeloDTO(ModeloCadastroVM modeloCadastroVM)
    {
        return new ModeloDTO
        {
            Id = modeloCadastroVM.Id,
            Titulo = modeloCadastroVM.Titulo,
            Ativo = modeloCadastroVM.Ativo,
            Secoes = modeloCadastroVM.Secoes?.Select(secao => new SecaoDTO
            {
                Id = secao.Id,
                Titulo = secao.Titulo,
                Ordem = secao.Ordem,
                Perguntas = secao.Perguntas?.Select(pergunta => new PerguntaDTO
                {
                    Id = pergunta.Id,
                    Tipo = pergunta.Tipo,
                    Enunciado = pergunta.Enunciado,
                    Descricao = pergunta.Descricao,
                    QuantidadeMinimaCaracteres = pergunta.QuantidadeMinimaCaracteres,
                    QuantidadeMaximaCaracteres = pergunta.QuantidadeMaximaCaracteres,
                    TipoTexto = pergunta.TipoTexto,
                    Obrigatorio = pergunta.Obrigatorio,
                    Ordem = pergunta.Ordem,
                    Alternativas = pergunta.Alternativas?.Select(opcao => new AlternativaDTO
                    {
                        Id = opcao.Id,
                        Texto = opcao.Texto,
                        Ordem = opcao.Ordem
                    }).ToList()
                }).ToList()
            }).ToList()
        };
    }
    #endregion
}
