using System.Data;
using System.Data.SqlClient;

namespace WebApplication2.Repositores;
public interface IWarehouseRepository
{
    public Task<int?> RegisterProductInWarehouseAsync(int idWarehouse, int idProduct, int idOrder, DateTime createdAt);
    public Task RegisterProductInWarehouseByProcedureAsync(int idWarehouse, int idProduct, DateTime createdAt);

    public Task<bool> CheckProductExist(int idProduct);
    public Task<bool> CheckWarehouseExist(int idWarehouse);
    public Task<bool> CheckOrderExist(int idProduct, DateTime createdAt);
    public Task<bool> CheckOrderInProductWarehouse(int idOrder);
    public Task UpdateFulfilledAt(int idOrder);
    public Task<int?> InsertProductWarehouse(int idWarehouse, int idProduct, int idOrder, DateTime createdAt);

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
    public async Task<bool> CheckWarehouseExist(int idWarehouse)
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await connection.OpenAsync();
        
        await using var transaction = await connection.BeginTransactionAsync();
        var query = "SELECT COUNT(*) FROM Warehouse WHERE IdWarehouse = @IdWarehouse";
        await using var cmd = new SqlCommand(query);
        cmd.Transaction = (SqlTransaction)transaction;
        cmd.Parameters.AddWithValue("@IdWarehouse", idWarehouse);
        var counter = (int)await cmd.ExecuteScalarAsync();
        if (counter > 0) return true;
        return false;

    }
    public async Task<bool> CheckOrderExist(int idProduct, DateTime createdAt)
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await connection.OpenAsync();
        
        await using var transaction = await connection.BeginTransactionAsync();
        var query = "SELECT COUNT(*) FROM Order WHERE IdProduct = @IdProduct AND Amount > 0 AND CreatedAt < @CreatedAt";
        await using var cmd = new SqlCommand(query);
        cmd.Transaction = (SqlTransaction)transaction;
        cmd.Parameters.AddWithValue("@IdProduct", idProduct);
        cmd.Parameters.AddWithValue("@CreatedAt", createdAt);
        var counter = (int)await cmd.ExecuteScalarAsync();
        if (counter > 0) return true;
        return false;
    }
    public async Task<bool> CheckOrderInProductWarehouse(int idOrder)
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await connection.OpenAsync();
        
        await using var transaction = await connection.BeginTransactionAsync();
        var query = "SELECT COUNT(*) FROM Product_Warehouse WHERE IdOrder = @IdOrder";
        await using var cmd = new SqlCommand(query);
        cmd.Transaction = (SqlTransaction)transaction;
        cmd.Parameters.AddWithValue("@IdOrder", idOrder);
        var counter = (int)await cmd.ExecuteScalarAsync();
        if (counter > 0) return true;
        return false;
    }
    public async Task UpdateFulfilledAt(int idOrder)
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await connection.OpenAsync();
        
        await using var transaction = await connection.BeginTransactionAsync();
        var query = "UPDATE Order SET FulfilledAt = @FulfilledAt WHERE IdOrder = @IdOrder";
        await using var cmd = new SqlCommand(query);
        cmd.Transaction = (SqlTransaction)transaction;
        cmd.Parameters.AddWithValue("@IdOrder", idOrder);
        cmd.Parameters.AddWithValue("@FulfilledAt", DateTime.UtcNow);
        await cmd.ExecuteNonQueryAsync();
    }
    public async Task<int?> InsertProductWarehouse(int idWarehouse, int idProduct, int idOrder, DateTime createdAt)
    
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await connection.OpenAsync();
        
        await using var transaction = await connection.BeginTransactionAsync();
        var query = @"INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, CreatedAt, Amount, Price)
        OUTPUT Inserted.IdProductWarehouse
        VALUES (@IdWarehouse, @IdProduct, @IdOrder, @CreatedAt, 0, (SELECT Price FROM Product WHERE IdProduct = @IdProduct) * (SELECT Amount FROM [Order] WHERE IdOrder = @IdOrder))";
        await using var cmd = new SqlCommand(query);
        cmd.Transaction = (SqlTransaction)transaction;
        cmd.Parameters.AddWithValue("@IdWarehouse", idWarehouse);
        cmd.Parameters.AddWithValue("@IdProduct", idProduct);
        cmd.Parameters.AddWithValue("@IdOrder", idOrder);
        cmd.Parameters.AddWithValue("@CreatedAt", createdAt);
        var idProductWarehouse = (int)await cmd.ExecuteScalarAsync();
        return idProductWarehouse;
    }
    
    public async Task<int?> RegisterProductInWarehouseAsync(int idWarehouse, int idProduct, int idOrder,
        DateTime createdAt)
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await connection.OpenAsync();
    
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {

            if (!await CheckProductExist(idProduct))
                throw new Exception("produkt o tym id nie istnieje");

            if (!await CheckWarehouseExist(idWarehouse))
                throw new Exception("warehouse o tym id nie istnieje");

            if (!await CheckOrderExist(idProduct, createdAt))
                throw new Exception("zamowienie z tym id nie istnieje");

            if (!await CheckOrderInProductWarehouse(idOrder))
                throw new Exception("zamowienie zostalo zrealizowane");

            await UpdateFulfilledAt(idOrder);

            var id = await InsertProductWarehouse(idWarehouse, idProduct, idOrder, createdAt);

            await transaction.CommitAsync();
            return id;
        }
        catch
        {
            await transaction.RollbackAsync();
            return null;
        }
    }

    public async Task RegisterProductInWarehouseByProcedureAsync(int idWarehouse, int idProduct, DateTime createdAt)
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await connection.OpenAsync();
        
        await using var cmd = new SqlCommand("AddProductToWarehouse", connection);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("IdProduct", idProduct);
        cmd.Parameters.AddWithValue("IdWarehouse",idWarehouse);
        cmd.Parameters.AddWithValue("Amount", 0);
        cmd.Parameters.AddWithValue("CreatedAt", createdAt);
        await cmd.ExecuteNonQueryAsync();
        
    }

}