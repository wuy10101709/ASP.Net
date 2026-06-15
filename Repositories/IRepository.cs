namespace Travel.Repositories;

public interface IRepository<T> where T : class {
    //IEnumerable nó là cha của list và array được trả về 1 danh sách lớp đối tượng T
    Task<IEnumerable<T>> GetAllAsync();// IEnumerable là một hành vi như disable không cho tương tác vào , chỉ được phép nhìn
    Task<T?> GetByIdAsync(int id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task SaveChangesAsync(); // các thay đổi sẽ đóng gói thành 1 transaction gửi xuống database. 
}       