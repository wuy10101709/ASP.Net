using Travel.Models;
using Travel.Repositories;
namespace Travel.Modules.Categorys;

public interface ICategoryRepository : IRepository<Category>
{
    // Lấy danh sách danh mục theo Type (ví dụ: "Tour" hoặc "Accommodation")
    Task<IEnumerable<Category>> GetByTypeAsync(string type);
    
    // Kiểm tra nhanh xem một CategoryId có tồn tại và đúng loại không
    Task<bool> IsValidCategoryAsync(int id, string type);
}