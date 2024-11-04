using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PharmaStockManager.Services;
using PharmaStockManager.ViewModels;

public class StockController : Controller
{
    public async Task<ActionResult> Dashboard()
    {
        var stocks = await StockService.FetchStockDataAsync();
        var chartData = await StockService.FetchChartDataAsync();
        var alerts = AlertService.GenerateAlerts(stocks);

        var viewModel = new DashboardViewModel
        {
            Stocks = stocks,
            ChartData = chartData,
            Alerts = alerts
        };

        return View(viewModel);
    }
}