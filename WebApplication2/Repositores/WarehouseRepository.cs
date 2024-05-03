namespace WebApplication2.Repositores;
public interface IWarehouseRepository
{
    public Task<int?> RegisterProductInWarehouseAsync(int idWarehouse, int idProduct, int idOrder, DateTime createdAt);
    public Task RegisterProductInWarehouseByProcedureAsync(int idWarehouse, int idProduct, DateTime createdAt);
}

public class WarehouseRepository : IWarehouseRepository
{
    private readonly IConfiguration _configuration;
    public WarehouseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<int?> RegisterProductInWarehouseAsync(int idWarehouse, int idProduct, int idOrder,
        DateTime createdAt)
    {
        return null;
    }

    public async Task RegisterProductInWarehouseByProcedureAsync(int idWarehouse, int idProduct, DateTime createdAt)
    {
        return;
    }

}