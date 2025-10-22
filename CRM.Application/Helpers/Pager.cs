using System;

namespace CRM.Application.Helpers;

public class Pager
{
    public long TotalItems { get; private set; }
    public int PaginaAtual { get; private set; }
    public int TamanhoPagina { get; private set; }
    public int TotalPaginas { get; private set; }
    public int PaginaInicial { get; private set; }
    public int PaginaFinal { get; private set; }

    public Pager()
    {
    }

    public Pager(long totalItems, int pagina, int tamanhoPagina = 10)
    {
        int totalPaginas = (int)Math.Ceiling((decimal)totalItems / (decimal)tamanhoPagina);

        int paginaAtual = pagina;

        int paginaInicial = paginaAtual - 5;
        int paginaFinal = paginaAtual + 4;

        if (paginaInicial <= 0)
        {
            paginaFinal -= paginaInicial - 1;
            paginaInicial = 1;
        }

        if (paginaFinal > totalPaginas)
        {
            paginaFinal = totalPaginas;
            if (paginaFinal > 10)
            {
                paginaInicial = paginaFinal - 9;
            }
        }

        TotalItems = totalItems;
        PaginaAtual = pagina;
        TamanhoPagina = tamanhoPagina;
        TotalPaginas = totalPaginas;
        PaginaInicial = paginaInicial;
        PaginaFinal = paginaFinal;
    }
}
