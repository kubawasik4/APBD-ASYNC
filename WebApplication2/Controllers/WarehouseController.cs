﻿using Microsoft.AspNetCore.Mvc;
using WebApplication2.Dto;
using WebApplication2.Services;

namespace WebApplication2.Controllers;


[ApiController]
[Route("/api/[controller]")]
public class WarehouseController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;
    public WarehouseController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterProductInWarehouseAsync([FromBody] RegisterProductInWarehouseRequestDTO dto)
    {
        try
        {
            var idProductWarehouse = await _warehouseService.RegisterProductInWarehouseAsync(dto);
            return Ok(idProductWarehouse);
        }
        catch (Exception e)
        {
            return NotFound();
        }
        
    }
    [HttpPost]
    public async Task<IActionResult> RegisterProductInWarehouseStoredProcAsync([FromBody] RegisterProductInWarehouseRequestDTO dto)
    {
        try
        {
            var idProductWarehouse = await _warehouseService.RegisterProductInWarehouseByProcedureAsync(dto);
          
            return Ok(idProductWarehouse);
            
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}