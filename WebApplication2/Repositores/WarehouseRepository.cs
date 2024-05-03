using System.Data.SqlClient;

namespace WebApplication2.Repositores;
public interface IWarehouseRepository
{
    public Task<int?> RegisterProductInWarehouseAsync(int idWarehouse, int idProduct, int idOrder, DateTime createdAt);
    public Task RegisterProductInWarehouseByProcedureAsync(int idWarehouse, int idProduct, DateTime createdAt);

    public Task<bool> CheckProductExistenceAsync(int idProduct);
}

public class WarehouseRepository : IWarehouseRepository
{
    private readonly IConfiguration _configuration;
    public WarehouseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task<bool> CheckProductExist(int idProduct)
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await connection.OpenAsync();
        
        await using var transaction = await connection.BeginTransactionAsync();
        var query = "SELECT COUNT(*) FROM Product WHERE IdProduct = @IdProduct";
        await using var checkProductCmd = new SqlCommand(query);
        checkProductCmd.Transaction = (SqlTransaction)transaction;
        checkProductCmd.Parameters.AddWithValue("@IdProduct", idProduct);
        var counter = (int)await checkProductCmd.ExecuteScalarAsync();
        if (counter > 0)
        {
            return true;
        }

        return false;
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