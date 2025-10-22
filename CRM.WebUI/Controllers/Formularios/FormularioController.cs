using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRM.Application.DTOs.Formularios;
using CRM.Application.DTOs.Formularios.Modelos;
using CRM.Application.DTOs.Formularios.Respostas;
using CRM.Application.DTOs.Shared;
using CRM.Application.Interfaces.Formularios;
using CRM.Application.Interfaces.Formularios.Modelos;
using CRM.Contracts.Formulario;
using CRM.Domain.Common.Extensions;
using CRM.WebUI.ViewModels.Formularios;
using Microsoft.AspNetCore.Mvc;

namespace CRM.WebUI.Controllers.Formularios;

[Route("cadastros/formularios")]
public class FormularioController : Controller
{
    private readonly IFormularioService _formularioService;
    private readonly IFormularioGabaritoService _formularioGabaritoService;
    private readonly IModeloService _modeloService;

    public FormularioController(IFormularioService formularioService,
                                IFormularioGabaritoService formularioGabaritoService,
                                IModeloService modeloService)
    {
        _formularioService = formularioService;
        _formularioGabaritoService = formularioGabaritoService;
        _modeloService = modeloService;
    }

    #region CRUD Formulários
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [Route("listagem")]
    public async Task<IActionResult> ObterFormularios(GetFormulariosRequest data)
    {
        int page = data.start / data.length + 1;

        PagedResultDto<FormularioDTO> formulariosPagina = await _formularioService.GetAllPagedAsync(page, data.length, data.search?.value);

        JsonResult result = Json(new
        {
            data.draw,
            recordsTotal = formulariosPagina.TotalResult,
            recordsFiltered = formulariosPagina.TotalResult,
            data = formulariosPagina.List
        });

        return result;
    }

    [HttpGet]
    [Route("novo")]
    public IActionResult Create()
    {
        var formularioCadastroVM = new FormularioCadastroVM
        {
            DataInicio = DateOnly.FromDateTime(DateTime.UtcNow.UtcToBrazilTime()),
            HoraInicio = TimeOnly.FromDateTime(DateTime.UtcNow.UtcToBrazilTime()),
        };

        return View(formularioCadastroVM);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("novo")]
    public async Task<IActionResult> Create(FormularioCadastroVM formularioCadastroVM)
    {
        try
        {
            if (ModelState.IsValid == false)
            {
                var listaMensagens = new List<string> { "Ocorreu um erro ao salvar o formulário" };
                listaMensagens.AddRange(ModelState.Values.SelectMany(value => value.Errors.Select(error => error.ErrorMessage)).ToList());
                return Json(new
                {
                    IsSuccess = false,
                    Messages = listaMensagens,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            if (await _formularioService.CreateAsync(BuildFormularioDTO(formularioCadastroVM)))
            {
                return Json(new
                {
                    IsSuccess = true,
                    Messages = new List<string> { "Formulário criado com sucesso" },
                    UrlRedirect = Url.Action("Index", "Formulario"),
                    StatusCode = System.Net.HttpStatusCode.OK
                });
            }

            return Json(new
            {
                IsSuccess = false,
                Messages = new List<string> { "Não foi possível executar a ação. Verifique se o modelo está ativo, atualize a página, caso o erro persista, contate o Administrador." },
                StatusCode = System.Net.HttpStatusCode.BadRequest
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                IsSuccess = false,
                Messages = new List<string> { $"Não foi possível executar a ação. erro: {ex.Message}" },
                StatusCode = System.Net.HttpStatusCode.BadRequest
            });
        }
    }

    [HttpGet]
    [Route("{id:guid}/editar")]
    public async Task<IActionResult> Edit(Guid id)
    {
        FormularioDTO formularioDto = await _formularioService.GetByIdAsync(id);

        ModeloDTO modeloDto = await _modeloService.GetByIdAsync(formularioDto.ModeloId);

        FormularioCadastroVM formularioCadastroVM = BuildFormularioCadastroVM(formularioDto, modeloDto);

        return View(formularioCadastroVM);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("{id:guid}/editar")]
    public async Task<IActionResult> Edit(FormularioCadastroVM formularioCadastroVM)
    {
        try
        {
            if (ModelState.IsValid == false)
            {
                var listaMensagens = new List<string> { "Ocorreu um erro ao atualizar o formulário" };
                listaMensagens.AddRange(ModelState.Values.SelectMany(value => value.Errors.Select(error => error.ErrorMessage)).ToList());
                return Json(new
                {
                    IsSuccess = false,
                    Messages = listaMensagens,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            if (await _formularioService.UpdateAsync(BuildFormularioDTO(formularioCadastroVM)))
            {
                return Json(new
                {
                    IsSuccess = true,
                    Messages = new List<string> { "Formulário atualizado com sucesso" },
                    UrlRedirect = Url.Action("Index", "Formulario"),
                    StatusCode = System.Net.HttpStatusCode.OK
                });
            }

            return Json(new
            {
                IsSuccess = false,
                Messages = new List<string> { "Ocorreu um erro ao salvar o formulário. Tente atualizar a página e caso o erro persista, contate o Administrador" },
                StatusCode = System.Net.HttpStatusCode.BadRequest
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                IsSuccess = false,
                Messages = new List<string> { $"Ocorreu um erro ao salvar o formulário. Ex: {ex.Message}" },
                StatusCode = System.Net.HttpStatusCode.InternalServerError
            });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("{id:guid}/excluir")]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        try
        {
            if (await _formularioService.DeleteAsync(id) == true)
            {
                return Json(new
                {
                    IsSuccess = true,
                    Messages = new List<string> { "Formulário removido com sucesso." },
                    UrlReload = true,
                    StatusCode = System.Net.HttpStatusCode.OK
                });
            }

            return Json(new
            {
                IsSuccess = false,
                Messages = new List<string> { "Ocorreu um erro ao remover o formulário. Tente atualizar a página e caso o erro persista, contate o suporte" },
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
    #endregion

    #region Responder Formulário
    [HttpGet]
    [Route("/formularios/{id:guid}/responder")]
    public async Task<IActionResult> Responder(Guid id)
    { 
        FormularioDTO formularioDto = await _formularioService.GetByIdAsync(id);

        ModeloDTO formularioModeloDto = await _modeloService.GetByIdWithTemaESecoesEPerguntasAsync(formularioDto.ModeloId);

        var responderFormularioVM = new ResponderFormularioViewModel
        {
            FormularioId = formularioDto.Id,
            FormularioNome = formularioDto.Nome,
            FormularioModelo = formularioModeloDto
        };

        return View(responderFormularioVM);
    }

    [HttpPost]
    [Route("/formularios/{id:guid}/responder")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Responder(FormularioGabaritoDTO gabaritoDto)
    {
        try
        {
            if (await _formularioService.ResponderAsync(gabaritoDto) == true)
            {
                return Json(new
                {
                    IsSuccess = true,
                    Messages = new List<string> { "Formulário respondido com sucesso" },
                    UrlRedirect = Url.Action("Index", "Home"),
                    StatusCode = System.Net.HttpStatusCode.OK
                });
            }

            return Json(new
            {
                IsSuccess = false,
                Messages = new List<string> { "Ocorreu um erro ao salvar a resposta do formulário. Tente atualizar a página e caso o erro persista, contate o suporte" },
                StatusCode = System.Net.HttpStatusCode.BadRequest
            });
        }
        catch (InvalidOperationException ex)
        {
            return Json(new
            {
                IsSuccess = false,
                Messages = new List<string> { $"Ocorreu um erro ao salvar a resposta do formulário. {ex.Message}" },
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
    #endregion

    #region Respostas do Formulário
    [HttpGet]
    [Route("/formularios/{id:guid}/respostas")]
    public async Task<IActionResult> RespostasDetalhadas(Guid id)
    {
        FormularioDTO formularioDto = await _formularioService.GetByIdAsync(id);

        IEnumerable<Guid> gabaritosIds = await _formularioGabaritoService.GetAllFormularioGabaritosIdsByFormularioId(formularioDto.Id);

        var respostasDetalhadasFormularioVM = new RespostasDetalhadasFormularioVM(formularioDto.Id, formularioDto.Nome, formularioDto.ModeloId, gabaritosIds);

        return View(respostasDetalhadasFormularioVM);
    }

    [HttpGet]
    [Route("/formularios/{id:guid}/respostas/{gabaritoId:guid}/obter-gabarito-com-modelo")]
    public async Task<IActionResult> OnGetGabaritoComModeloPartial(Guid gabaritoId, Guid modeloId)
    {
        FormularioGabaritoDTO gabaritoDto = await _formularioGabaritoService.GetByIdWithRespostasAsync(gabaritoId);

        ModeloDTO modeloDto = await _modeloService.GetByIdWithTemaESecoesEPerguntasAsync(modeloId);


        var formularioGabaritoComModeloVM = new FormularioGabaritoComModeloVM(gabaritoDto.RespondidoEm,
                                                                              modeloDto,
                                                                              gabaritoDto);

        return PartialView("_GabaritoComModelo", formularioGabaritoComModeloVM);
    }
    #endregion

    #region Mapeamento FormularioDTO e FormularioCadastroVM
    private FormularioDTO BuildFormularioDTO(FormularioCadastroVM formularioCadastroVM)
    {
        var formularioDto = new FormularioDTO
        {
            Id = formularioCadastroVM.Id,
            Nome = formularioCadastroVM.Nome,
            MensagemConfirmacao = formularioCadastroVM.MensagemConfirmacao,
            DataInicio = formularioCadastroVM.DataInicio.ToDateTime(formularioCadastroVM.HoraInicio),
            ModeloId = formularioCadastroVM.ModeloId
        };

        if (formularioCadastroVM.DataTermino.HasValue == true && formularioCadastroVM.HoraTermino.HasValue == true)
        {
            formularioDto.DataTermino = formularioCadastroVM.DataTermino.Value.ToDateTime(formularioCadastroVM.HoraTermino.Value);
        }

        return formularioDto;
    }

    private FormularioCadastroVM BuildFormularioCadastroVM(FormularioDTO formularioDto, ModeloDTO modeloDto)
    {
        var formularioCadastroVM = new FormularioCadastroVM
        {
            Id = formularioDto.Id,
            Nome = formularioDto.Nome,
            MensagemConfirmacao = formularioDto.MensagemConfirmacao,
            DataInicio = DateOnly.FromDateTime(formularioDto.DataInicio),
            HoraInicio = TimeOnly.FromDateTime(formularioDto.DataInicio),
            ModeloId = modeloDto.Id,
            ModeloTitulo = modeloDto.Titulo,
        };

        if (formularioDto.DataTermino.HasValue)
        {
            formularioCadastroVM.DataTermino = DateOnly.FromDateTime(formularioDto.DataTermino.Value);
            formularioCadastroVM.HoraTermino = TimeOnly.FromDateTime(formularioDto.DataTermino.Value);
        }

        return formularioCadastroVM;
    }
    #endregion
}
