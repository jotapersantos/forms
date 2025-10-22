using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CRM.Application.DTOs.Formularios.Modelos;
using CRM.Application.DTOs.Shared;
using CRM.Application.Interfaces.Formularios.Modelos;
using CRM.Application.Services.Formularios.Patterns;
using CRM.Application.Services.FormularioSecaoItens;
using CRM.Domain.Dtos.Formularios;
using CRM.Domain.Entities.Formularios.Modelos;
using CRM.Domain.Entities.Formularios.Modelos.Abstractions;
using CRM.Domain.Interfaces;
using CRM.Domain.Interfaces.Formularios.Modelos;

namespace CRM.Application.Services.Formularios.Modelos;

public class ModeloService : IModeloService
{
    private readonly IModeloRepository _modeloRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ModeloService(IModeloRepository modeloRepository,
                         IUnitOfWork unitOfWork,
                         IMapper mapper)
    {
        _modeloRepository = modeloRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResultDto<ModeloDTO>> GetPagedAsync(int page, int pageSize, string termo)
    {
        int totalModelos = await _modeloRepository.CountAsync(page, pageSize, termo);

        var paginaDeModelos = new PagedResultDto<ModeloDTO>(totalModelos, page, pageSize)
        {
            List = _mapper.Map<IEnumerable<ModeloDTO>>(await _modeloRepository.GetPagedAsync(page, pageSize, termo))
        };

        return paginaDeModelos;
    }

    public async Task<PagedResultDto<ModeloIndexDTO>> GetModeloIndexPagedAsync(int page, int pageSize, string termo)
    {
        int totalModelos = await _modeloRepository.CountAsync(page, pageSize, termo);

        var paginaDeModelos = new PagedResultDto<ModeloIndexDTO>(totalModelos, page, pageSize)
        {
            List = await _modeloRepository.GetModeloIndexPagedAsync(page, pageSize, termo)
        };

        return paginaDeModelos;
    }

    public async Task<Dictionary<Guid, string>> GetDictionaryActivesByTitleAsync(string title, int quantity)
        => await _modeloRepository.GetDictionaryActivesByTitleAsync(title, quantity);

    public async Task<ModeloDTO> GetByIdAsync(Guid id)
    {
        Modelo modelo = await _modeloRepository.GetByIdAsync(id);

        return _mapper.Map<ModeloDTO>(modelo);   
    }

    public async Task<ModeloDTO> GetByIdWithTemaESecoesEPerguntasAsync(Guid id)
    {
        Modelo modelo = await _modeloRepository.GetByIdWithTemaESecoesEPerguntasAsync(id);

        return _mapper.Map<ModeloDTO>(modelo);
    }

    public async Task<bool> IsModeloAssociatedWithFormularioOrAvaliacaoAsync(Guid id)
        => await _modeloRepository.IsModeloAssociatedWithFormularioOrAvaliacaoAsync(id);

    public async Task<bool> CreateAsync(ModeloDTO modeloDto)
    {
        try
        {
            var modelo = Modelo.Build(modeloDto.Titulo, modeloDto.Ativo);

            IEnumerable<Secao> secoes = GerarSecoes(modeloDto.Secoes);

            modelo.AddSecoes(secoes);

            await _modeloRepository.AddAsync(modelo);

            await _unitOfWork.SaveAsync();

            return true;
        }
        catch(InvalidOperationException ex)
        {
            _ = ex.Message;
            throw;
        }
        catch (Exception ex)
        {
            _ = ex.Message;
            throw;
        }
    }

    public async Task<bool> UpdateAsync(ModeloDTO modeloDto)
    {
        try
        {
            Modelo modeloDb = await _modeloRepository.GetByIdWithTemaESecoesEPerguntasAsync(modeloDto.Id);

            modeloDb.Update(modeloDto.Titulo, modeloDto.Ativo);

            IEnumerable<Secao> secoes = GerarSecoes(modeloDto.Secoes);

            modeloDb.UpdateSecoes(secoes);

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
            Modelo modeloDb = await _modeloRepository.GetByIdWithThemeAsync(id);

            bool modeloTemAssociacoes = await _modeloRepository.IsModeloAssociatedWithFormularioOrAvaliacaoAsync(modeloDb.Id);

            if (modeloTemAssociacoes)
            {
                throw new InvalidOperationException("Não é possível excluir o modelo. O modelo está sendo usado em outros formulários e avaliações");
            }

            _modeloRepository.Remove(modeloDb);

            await _unitOfWork.SaveAsync();

            return true;
        }
        catch(InvalidOperationException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<IEnumerable<PerguntaDTO>> GetAllRequiredPerguntasByModeloId(Guid modeloId)
    {
        IEnumerable<Pergunta> perguntasDoModelo = await _modeloRepository.GetAllRequiredPerguntasByModeloId(modeloId);

        return _mapper.Map<IEnumerable<PerguntaDTO>>(perguntasDoModelo);
    }

    public async Task<bool> Ativar(Guid id)
    {
        Modelo modelo = await _modeloRepository.GetByIdAsync(id);
        if (modelo is null)
        {
            return false;
        }
        modelo.Ativar();
        _modeloRepository.Update(modelo);
        await _unitOfWork.SaveAsync();
        return true;
    }

    public async Task<bool> Desativar(Guid id)
    {
        Modelo modelo = await _modeloRepository.GetByIdAsync(id);
        if (modelo is null)
        {
            return false;
        }

        modelo.Desativar();
        _modeloRepository.Update(modelo);
        await _unitOfWork.SaveAsync();
        return true;
    }

    private List<Secao> GerarSecoes(IEnumerable<SecaoDTO> secoesDto)
    {
        var secoes = new List<Secao>();

        foreach (SecaoDTO secaoDto in secoesDto)
        {
            var secao =  Secao.Build(secaoDto.Id, secaoDto.Titulo, secaoDto.Ordem);

            IEnumerable<Pergunta> perguntas = GerarPerguntas(secaoDto.Perguntas);

            secao.AddPerguntas(perguntas);

            secoes.Add(secao);
        }

        return secoes;
    }

    private List<Pergunta> GerarPerguntas(IEnumerable<PerguntaDTO> perguntasDto)
    {
        var perguntas = new List<Pergunta>();

        TextoCurtoChainOR textoCurtoChainOR = new();
        TextoLongoChainOR textoLongoChainOR = new();
        
        MultiplaEscolhaChainOR multiplaEscolhaChainOR = new();
        CaixaSelecaoChainOR caixaSelecaoChainOR = new();
        ListaSuspensaChainOR listaSuspensaChainOR = new();
        
        textoCurtoChainOR.SetProximo(textoLongoChainOR);
        textoLongoChainOR.SetProximo(multiplaEscolhaChainOR);
        multiplaEscolhaChainOR.SetProximo(caixaSelecaoChainOR);
        caixaSelecaoChainOR.SetProximo(listaSuspensaChainOR);

        foreach (PerguntaDTO perguntaDto in perguntasDto)
        {
            Pergunta pergunta = textoCurtoChainOR.GerarPergunta(perguntaDto);

            perguntas.Add(pergunta);
        }

        return perguntas;
    }
}
