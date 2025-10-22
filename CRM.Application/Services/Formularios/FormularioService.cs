using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CRM.Application.DTOs.Formularios;
using CRM.Application.DTOs.Formularios.Respostas;
using CRM.Application.DTOs.Shared;
using CRM.Application.Interfaces.Formularios;
using CRM.Application.Services.Formularios.Respostas;
using CRM.Domain.Entities.Formularios;
using CRM.Domain.Entities.Formularios.Modelos;
using CRM.Domain.Entities.Formularios.Modelos.Abstractions;
using CRM.Domain.Entities.Formularios.Respostas;
using CRM.Domain.Interfaces;
using CRM.Domain.Interfaces.Formularios;
using CRM.Domain.Interfaces.Formularios.Modelos;

namespace CRM.Application.Services.Formularios;

public class FormularioService : IFormularioService
{
    private readonly IFormularioRepository _formularioRepository;
    private readonly IModeloRepository _modeloRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public FormularioService(IFormularioRepository formularioRepository, IModeloRepository modeloRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _formularioRepository = formularioRepository;
        _modeloRepository = modeloRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResultDto<FormularioDTO>> GetAllPagedAsync(int pagina, int tamanhoPagina, string termo)
    {
        int totalFormularios = await _formularioRepository.CountAsync(termo);

        var paginaDeFormularios = new PagedResultDto<FormularioDTO>(totalFormularios, pagina, tamanhoPagina);

        List<Formulario> formularios = await _formularioRepository.GetAllPagedAsync(paginaDeFormularios.PageIndex, paginaDeFormularios.PageSize, termo);

        paginaDeFormularios.List = _mapper.Map<List<FormularioDTO>>(formularios);

        return paginaDeFormularios;
    }

    public async Task<FormularioDTO> GetByIdAsync(Guid id)
    {
        Formulario formulario = await _formularioRepository.GetByIdAsync(id);
        return _mapper.Map<FormularioDTO>(formulario);
    }

    public async Task<bool> CreateAsync(FormularioDTO formularioDto)
    {

        Modelo modeloDeFormulario = await _modeloRepository.GetActivedByIdAsync(formularioDto.ModeloId);

        if (modeloDeFormulario is null)
        {
            return false;
        }

        var formularioDb = Formulario.Build(
            formularioDto.Nome,
            formularioDto.MensagemConfirmacao,
            formularioDto.DataInicio,
            formularioDto.DataTermino,
            modeloDeFormulario
        );

        await _formularioRepository.AddAsync(formularioDb);

        await _unitOfWork.SaveAsync();

        return true;
    }

    public async Task<bool> UpdateAsync(FormularioDTO formulario)
    {
        try
        {
            Formulario formularioDb = await _formularioRepository.GetByIdAsync(formulario.Id);

            formularioDb.Update(
                formulario.Nome,
                formulario.MensagemConfirmacao,
                formulario.DataInicio,
                formulario.DataTermino
            );

            await _unitOfWork.SaveAsync();

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            Formulario formulario = await _formularioRepository.GetByIdAsync(id);

            _formularioRepository.Remove(formulario);

            await _unitOfWork.SaveAsync();

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> ResponderAsync(FormularioGabaritoDTO formularioGabarito)
    {
        try
        {
            Formulario formulario = await _formularioRepository.GetByIdAsync(formularioGabarito.FormularioId);

            if (formulario == null)
            {
                throw new InvalidOperationException("O formulário não foi encontrado.");
            }

            IEnumerable<Pergunta> perguntasDoModelo = await _modeloRepository.GetAllPerguntasByModeloId(formulario.ModeloId);

            IEnumerable<Resposta> respostasDoFormulario = RespostaFactory.GerarRespostas(perguntasDoModelo, formularioGabarito.Respostas);

            formulario.Responder(perguntasDoModelo, respostasDoFormulario);

            await _unitOfWork.SaveAsync();

            return true;
        }
        catch (InvalidOperationException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
