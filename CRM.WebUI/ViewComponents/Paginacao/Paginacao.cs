using System.Threading.Tasks;
using CRM.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRM.WebUI.ViewComponents.Paginacao;

public class Paginacao : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(IPagedList listaPaginada)
    {
        return View(listaPaginada);
    }
}
