using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CRM.Application.DTOs.Formularios.Respostas;
using CRM.Application.Interfaces.Formularios;
using CRM.Domain.Entities.Formularios.Respostas;
using CRM.Domain.Interfaces.Formularios.Respostas;

namespace CRM.Application.Services.Formularios;

public class FormularioGabaritoService : IFormularioGabaritoService
{
    private readonly IFormularioGabaritoRepository _gabaritoRepository;
    private readonly IMapper _mapper;

    public FormularioGabaritoService(IFormularioGabaritoRepository gabaritoRepository, IMapper mapper)
    {
        _gabaritoRepository = gabaritoRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Guid>> GetAllFormularioGabaritosIdsByFormularioId(Guid formularioId)
        => await _gabaritoRepository.GetAllFormularioGabaritosIdsByFormularioId(formularioId);

    public async Task<FormularioGabaritoDTO> GetByIdWithRespostasAsync(Guid id)
    {
        FormularioGabarito gabarito = await _gabaritoRepository.GetByIdWithRespostasAsync(id);
        return _mapper.Map<FormularioGabaritoDTO>(gabarito);
    }
}
