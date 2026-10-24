using HastaneRandevu.Entities.Abstract;
using HastaneRandevu.Entities.Enums;

namespace HastaneRandevu.Entities.Concrete;

public class Hasta : Kullanici
{
    private string _tcKimlikNo = string.Empty;

    public string TcKimlikNo
    {
        get => _tcKimlikNo;
        set
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length != 11 || !value.All(char.IsDigit))
                throw new ArgumentException("TC Kimlik No 11 haneli ve sadece rakamlardan oluşmalıdır.");
            _tcKimlikNo = value;
        }
    }

    public DateTime DogumTarihi { get; set; }

    public int Yas
    {
        get
        {
            var bugun = DateTime.Today;
            int yas = bugun.Year - DogumTarihi.Year;
            if (DogumTarihi.Date > bugun.AddYears(-yas)) yas--;
            return yas;
        }
    }

    public override KullaniciRolu Rol => KullaniciRolu.Hasta;

    public override string BilgiGetir() => $"{base.BilgiGetir()} - Yaş: {Yas}";
}