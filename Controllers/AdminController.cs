using Diplom.Data;

namespace Diplom.Controllers;

public class AdminController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ExpController> _logger;

    public AdminController(ApplicationDbContext context, ILogger<ExpController> logger)
    {
        _logger = logger;

        try
        {
            _context = context;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing ExpController.");
            throw;
        }
    }
    
    
}