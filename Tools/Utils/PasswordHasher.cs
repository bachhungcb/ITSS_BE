using Application.Interfaces;
using BCrypt.Net; // Đảm bảo bạn đã cài gói BCrypt.Net-Next

namespace Tools.Utils;

public class PasswordHasher : IPasswordHasher
{
    // Không cần Constructor tạo Salt nữa vì thư viện tự lo
    public PasswordHasher()
    {
    }

    public string HashPassword(string password)
    {
        // Hàm này tự động tạo Salt ngẫu nhiên và hash
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
    {
        // Hàm Verify sẽ tự tách Salt từ hashedPassword ra để đối chiếu
        // Lưu ý: Tham số đầu tiên là mật khẩu thô (nhập vào), tham số thứ 2 là hash trong DB
        return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
    }
}