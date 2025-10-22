using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CRM.Application.DTOs.Formularios;
using CRM.Application.DTOs.Formularios.Modelos;
using CRM.Application.DTOs.Formularios.Respostas;
using CRM.Domain.Common.Extensions;
using CRM.Domain.Entities.Formularios;
using CRM.Domain.Entities.Formularios.Modelos;
using CRM.Domain.Entities.Formularios.Modelos.Abstractions;
using CRM.Domain.Entities.Formularios.Respostas;

using FormularioModelo = CRM.Domain.Entities.Formularios.Modelos;

namespace CRM.Application.Mappings;

public class DomainToDtoMappingProfile : Profile
{
    public DomainToDtoMappingProfile()
    {      
        #region Formulário
        CreateMap<FormularioModelo.Modelo, ModeloDTO>()
            .ForMember(dest => dest.CriadoEm, opt => opt.MapFrom(src => src.CriadoEmUtc.UtcToBrazilTime()));

        CreateMap<Formulario, FormularioDTO>()
            .ForMember(dest => dest.DataInicio, opt => opt.MapFrom(src => src.Periodo.DataInicio.UtcToBrazilTime()))
            .ForMember(dest => dest.DataTermino, opt => opt.MapFrom(src => src.Periodo.DataTermino.UtcToBrazilTime()));

        CreateMap<Secao, SecaoDTO>();
        CreateMap<Pergunta, PerguntaDTO>()
            .ForMember(dest => dest.Alternativas, opt => opt.MapFrom(src => MapAlternativasDaPergunta(src)))
            .AfterMap((src, dest) =>
            {
                if (src is PerguntaDiscursiva discursiva)
                {
                    dest.QuantidadeMinimaCaracteres = discursiva.QuantidadeMinimaCaracteres;
                    dest.QuantidadeMaximaCaracteres = discursiva.QuantidadeMaximaCaracteres;
                }
            });
        CreateMap<FormularioGabarito, FormularioGabaritoDTO>()
            .ForMember(dest => dest.RespondidoEm, opt => opt.MapFrom(src => src.RespondidoEm.UtcToBrazilTime()));

        CreateMap<Resposta, RespostaDTO>()
            .ForMember(dest => dest.Texto, opt => opt.MapFrom(src => MapTextoDaRespostaDiscursiva(src)))
            .ForMember(dest => dest.AlternativasSelecionadas, opt => opt.MapFrom(src => MapAlternativasSelecionadasDaRespostaObjetiva(src)));
        #endregion
    }

    private List<AlternativaDTO> MapAlternativasDaPergunta(Pergunta pergunta)
    {
        if (pergunta is PerguntaObjetiva perguntaObjetiva)
        {
            return perguntaObjetiva.Alternativas.Select(opcao => new AlternativaDTO
            {
                Id = opcao.Id,
                Texto = opcao.Texto,
                Ordem = opcao.Ordem
            }).ToList();
        }
        else
        {
            return new List<AlternativaDTO>();
        }
    }

    private List<AlternativaDTO> MapAlternativasSelecionadasDaRespostaObjetiva(Resposta resposta)
    {
        if (resposta is RespostaObjetiva respostaObjetiva)
        {
            return respostaObjetiva.AlternativasSelecionadas.Select(opcao => new AlternativaDTO
            {
                Id = opcao.Id,
                Texto = opcao.Texto,
                Ordem = opcao.Ordem
            }).ToList();
        }
        else
        {
            return new List<AlternativaDTO>();
        }
    }

    private string MapTextoDaRespostaDiscursiva(Resposta resposta)
    {
        if (resposta is RespostaDiscursiva respostaDiscursiva)
        {
            return respostaDiscursiva.Texto;
        }

        return null;
    }
}
