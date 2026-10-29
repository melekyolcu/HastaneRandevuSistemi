namespace HastaneRandevu.Business.Results;

// Bir işlemin başarılı olup olmadığını ve kullanıcıya gösterilecek mesajı taşır
public class IslemSonucu
{
    public bool Basarili { get; }
    public string Mesaj { get; }

    protected IslemSonucu(bool basarili, string mesaj)
    {
        Basarili = basarili;
        Mesaj = mesaj;
    }

    public static IslemSonucu Basari(string mesaj) => new(true, mesaj);
    public static IslemSonucu Hata(string mesaj) => new(false, mesaj);
}

// Mesajın yanında bir veri de döndürmek gerektiğinde (örneğin oluşturulan randevu)
public class IslemSonucu<T> : IslemSonucu
{
    public T? Veri { get; }

    private IslemSonucu(bool basarili, string mesaj, T? veri) : base(basarili, mesaj)
    {
        Veri = veri;
    }

    public static IslemSonucu<T> Basari(T veri, string mesaj) => new(true, mesaj, veri);
    public static new IslemSonucu<T> Hata(string mesaj) => new(false, mesaj, default);
}