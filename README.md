# Hastane / Klinik Randevu Sistemi

Nesneye Dayalı Programlama dersi projesi. Tek bir klinikten çok şubeli sağlık kuruluşlarına kadar kullanılabilen, rol tabanlı bir randevu sistemi.

## Özellikler

### Kurum yönetimi
- Hastane, klinik ve tıp merkezi tanımlama
- Çok şubeli yapı: her kurumun bir veya birden fazla şubesi olabilir
- Şube bazında haftalık çalışma saatleri
- Bölümler ve hizmetler (süre ve ücret bilgisiyle)

### Çalışan yönetimi
- Doktor ve sağlık personeli kaydı
- Doktorun unvanı, uzmanlık alanı, çalıştığı bölüm ve verdiği hizmetler
- Doktorların haftalık uygunluk saatleri (şubenin çalışma saatleri içinde ve birbiriyle çakışmayacak şekilde)

### Randevu sistemi
- Hasta adım adım randevu alır: hizmet → doktor → gün → boş saat
- Sadece doktorun uygun olduğu ve boş olan saatler listelenir
- Çakışma kontrolü: doktorun da hastanın da aynı saatte başka randevusu olamaz
- Randevu alındığı andaki süre ve ücret randevuya kaydedilir; hizmet fiyatı sonradan değişse de etkilenmez
- Onay mekanizması: **Onay Bekliyor → Onaylandı / Reddedildi → Tamamlandı**, ayrıca hasta tarafından iptal

### Roller

| Rol | Yapabilecekleri |
|---|---|
| **Hasta** | Kayıt olma, randevu alma, randevularını görme ve iptal etme |
| **Doktor** | Randevu onaylama / reddetme / tamamlama, kendi uygunluk saatlerini düzenleme |
| **Sağlık Personeli** | Günlük randevu listesini görüntüleme |
| **Yönetici** | Kurum, şube, bölüm, hizmet, doktor ve personel yönetimi |

## Teknolojiler

- C# / .NET 10
- ASP.NET Core MVC
- PostgreSQL
- Entity Framework Core (Code-First, migration)
- Bootstrap 5
- Çerez (cookie) tabanlı kimlik doğrulama, PBKDF2 ile şifre hash'leme

## Mimari

Katmanlı mimari (N-Tier). Bağımlılık yönü: **WebUI → Business → DataAccess → Entities**

```
src/
├── HastaneRandevu.Entities     → Varlık class'ları ve enum'lar
│   ├── Abstract                → BaseEntity, Kullanici, Calisan
│   ├── Concrete                → Hasta, Doktor, Randevu, Kurum, Sube, Bolum, Hizmet...
│   └── Enums                   → KullaniciRolu, RandevuDurumu, KurumTuru, HizmetKategorisi
├── HastaneRandevu.DataAccess   → Veritabanı erişimi
│   ├── Abstract / Concrete     → Repository interface'leri ve class'ları
│   ├── Context                 → DbContext
│   ├── Migrations              → Veritabanı şeması
│   └── Seed                    → Örnek veriler
├── HastaneRandevu.Business     → İş kuralları
│   ├── Abstract / Concrete     → Service interface'leri ve Manager class'ları
│   ├── Dtos                    → Veri taşıyıcı record'lar
│   └── Results                 → İşlem sonucu class'ları
└── HastaneRandevu.WebUI        → MVC arayüzü (Controller'lar, View'lar)
```

## Nesneye Dayalı Programlama İlkeleri

| İlke | Projede nerede kullanıldı |
|---|---|
| **Kalıtım** | `BaseEntity → Kullanici → Calisan → Doktor` çok seviyeli kalıtım; `GenericRepository<T> → RandevuRepository`; `IslemSonucu → IslemSonucu<T>` |
| **Soyutlama** | `BaseEntity`, `Kullanici`, `Calisan` abstract class'ları; `IGenericRepository<T>`, `IRandevuService` gibi interface'ler |
| **Kapsülleme** | Private alanlar ve doğrulama yapan property'ler (`Ad`, `Eposta`, `TcKimlikNo`, `Ucret`); `Randevu.Durum` ve `SifreHash` gibi `private set` alanlar sadece `Onayla()`, `Reddet()`, `SifreBelirle()` gibi metotlarla değişir |
| **Çok biçimlilik** | Her kullanıcı türünde farklı çalışan `abstract Rol` ve `override BilgiGetir()`; interface üzerinden kullanılan service ve repository'ler (dependency injection) |

Veritabanında kullanıcı hiyerarşisi **Table-Per-Hierarchy (TPH)** ile tek tabloda tutulur; `KullaniciTipi` sütunu satırın hangi class'a ait olduğunu belirtir.

## Kurulum

### Gereksinimler
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)
- EF Core komut aracı: `dotnet tool install --global dotnet-ef`

### Adımlar

1. Projeyi klonlayın:
   ```bash
   git clone https://github.com/melekyolcu/HastaneRandevuSistemi.git
   cd HastaneRandevuSistemi
   ```

2. Veritabanı bağlantısını kaydedin (`SIFRENIZ` yerine kendi PostgreSQL şifrenizi yazın):
   ```bash
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=HastaneRandevuDb;Username=postgres;Password=SIFRENIZ" --project src/HastaneRandevu.WebUI
   ```

3. Veritabanını oluşturun:
   ```bash
   dotnet ef database update --project src/HastaneRandevu.DataAccess --startup-project src/HastaneRandevu.WebUI
   ```

4. Uygulamayı çalıştırın:
   ```bash
   dotnet run --project src/HastaneRandevu.WebUI
   ```

İlk çalıştırmada örnek veriler (bir hastane, iki şube, bölümler, hizmetler, doktorlar ve kullanıcılar) otomatik yüklenir.

### Örnek hesaplar

Bütün örnek hesapların şifresi: **`Sifre123`**

| Rol | E-posta |
|---|---|
| Yönetici | `yonetici@sifasaglik.ornek` |
| Doktor | `ayse.yilmaz@sifasaglik.ornek` |
| Doktor | `mehmet.kaya@sifasaglik.ornek` |
| Sağlık Personeli | `elif.sahin@sifasaglik.ornek` |
| Hasta | `ahmet.celik@ornek.com` |