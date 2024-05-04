using System.Data.SqlClient;
using System.Xml;
using WebApplication2.Dto;
using WebApplication2.Repositores;

namespace WebApplication2.Services;
public interface IWarehouseService
{
    public Task<int> RegisterProductInWarehouseAsync(RegisterProductInWarehouseRequestDTO dto);
    public Task<int> RegisterProductInWarehouseByProcedureAsync(RegisterProductInWarehouseRequestDTO dto);
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
        const int idOrder = 1;
        var idProductWarehouse = await _warehouseRepository.RegisterProductInWarehouseAsync(idWarehouse: dto.IdWarehouse!.Value, idProduct: dto.IdProduct!.Value,
            idOrder: idOrder, createdAt: DateTime.UtcNow);

        if (!idProductWarehouse.HasValue)
            throw new Exception("nie udalo sie dodac produktu");

        return idProductWarehouse.Value;
    }

    public async Task<int> RegisterProductInWarehouseByProcedureAsync(RegisterProductInWarehouseRequestDTO dto)
    {
        const int idOrder = 1;
        var idProductWarehouse = await _warehouseRepository.RegisterProductInWarehouseAsync(idWarehouse: dto.IdWarehouse!.Value, idProduct: dto.IdProduct!.Value,
            idOrder: idOrder, createdAt: DateTime.UtcNow);

        if (!idProductWarehouse.HasValue)
            throw new Exception("nie udalo sie dodac produktu");

        return idProductWarehouse.Value;
    }
}