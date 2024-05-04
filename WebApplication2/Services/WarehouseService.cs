using System.Data.SqlClient;
using System.Xml;
using WebApplication2.Dto;
using WebApplication2.Repositores;

namespace WebApplication2.Services;
public interface IWarehouseService
{
    public Task<int> RegisterProductInWarehouseAsync(RegisterProductInWarehouseRequestDTO dto);
}
public class WarehouseService : IWarehouseService
{
    private readonly IWarehouseRepository _warehouseRepository;
    public WarehouseService(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }
    
    public async Task<int> RegisterProductInWarehouseAsync(RegisterProductInWarehouseRequestDTO dto)
    {
        return;
    }
}