namespace Projem.Models;

public class KullaniciSifre
{
    public String? Ad { get; set; } = String.Empty;
    public String? Soyad { get; set; } = String.Empty;
    public String? Sifre { get; set; } = String.Empty;
    public DateTime  KayitTarihi { get; set; }

    public KullaniciSifre()
    {
        KayitTarihi = DateTime.Now;
    }
}