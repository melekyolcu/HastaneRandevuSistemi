using HastaneRandevu.Entities.Abstract;
using HastaneRandevu.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace HastaneRandevu.DataAccess.Context;


/// Veritabanı ile sınıflarımız arasındaki köprü.
/// Her DbSet bir tabloya karşılık gelir.
public class HastaneRandevuContext : DbContext
{
    public HastaneRandevuContext(DbContextOptions<HastaneRandevuContext> options) : base(options) { }

    // Kullanıcı hiyerarşisi tek tabloda tutulur; alt türler ayrıca sorgulanabilir
    public DbSet<Kullanici> Kullanicilar => Set<Kullanici>();
    public DbSet<Hasta> Hastalar => Set<Hasta>();
    public DbSet<Doktor> Doktorlar => Set<Doktor>();
    public DbSet<SaglikPersoneli> SaglikPersonelleri => Set<SaglikPersoneli>();
    public DbSet<Yonetici> Yoneticiler => Set<Yonetici>();

    public DbSet<Kurum> Kurumlar => Set<Kurum>();
    public DbSet<Sube> Subeler => Set<Sube>();
    public DbSet<CalismaSaati> CalismaSaatleri => Set<CalismaSaati>();
    public DbSet<Bolum> Bolumler => Set<Bolum>();
    public DbSet<Hizmet> Hizmetler => Set<Hizmet>();
    public DbSet<UygunlukZamani> UygunlukZamanlari => Set<UygunlukZamani>();
    public DbSet<Randevu> Randevular => Set<Randevu>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Kullanıcılar: Table-Per-Hierarchy 
        // Hasta, Doktor, SaglikPersoneli, Yonetici tek tabloda;
        // "KullaniciTipi" sütunu satırın hangi sınıfa ait olduğunu belirtir.
        modelBuilder.Entity<Kullanici>(e =>
        {
            e.ToTable("Kullanicilar");
            e.HasDiscriminator<string>("KullaniciTipi")
                .HasValue<Hasta>("Hasta")
                .HasValue<Doktor>("Doktor")
                .HasValue<SaglikPersoneli>("SaglikPersoneli")
                .HasValue<Yonetici>("Yonetici");

            e.Property(k => k.Ad).HasMaxLength(50).IsRequired();
            e.Property(k => k.Soyad).HasMaxLength(50).IsRequired();
            e.Property(k => k.Eposta).HasMaxLength(100).IsRequired();
            e.Property(k => k.Telefon).HasMaxLength(20);
            e.HasIndex(k => k.Eposta).IsUnique();
        });

        modelBuilder.Entity<Hasta>(e =>
        {
            e.Property(h => h.TcKimlikNo).HasMaxLength(11);
            e.HasIndex(h => h.TcKimlikNo).IsUnique();
            e.Property(h => h.DogumTarihi).HasColumnType("date");
        });

        modelBuilder.Entity<Calisan>(e =>
        {
            e.Property(c => c.SicilNo).HasMaxLength(20);
            e.Property(c => c.IseBaslamaTarihi).HasColumnType("date");
        });

        modelBuilder.Entity<Doktor>(e =>
        {
            e.HasOne(d => d.Bolum)
                .WithMany()
                .HasForeignKey(d => d.BolumId)
                .OnDelete(DeleteBehavior.Restrict);

            // Çoka çok: ara tablo otomatik oluşur
            e.HasMany(d => d.Hizmetler)
                .WithMany(h => h.Doktorlar)
                .UsingEntity(j => j.ToTable("DoktorHizmetleri"));
        });

        // ---- Kurum yapısı ----
        modelBuilder.Entity<Kurum>(e =>
        {
            e.Property(k => k.Ad).HasMaxLength(150).IsRequired();
        });

        modelBuilder.Entity<Sube>(e =>
        {
            e.Property(s => s.Ad).HasMaxLength(150).IsRequired();
        });

        modelBuilder.Entity<CalismaSaati>(e =>
        {
            // Bir şubenin her gün için tek bir çalışma saati kaydı olabilir
            e.HasIndex(c => new { c.SubeId, c.Gun }).IsUnique();
        });

        modelBuilder.Entity<Bolum>(e =>
        {
            e.Property(b => b.Ad).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Hizmet>(e =>
        {
            e.Property(h => h.Ad).HasMaxLength(150).IsRequired();
            e.Property(h => h.Ucret).HasPrecision(10, 2);
        });

        modelBuilder.Entity<UygunlukZamani>(e =>
        {
            e.HasIndex(u => new { u.DoktorId, u.Gun });
        });

        // ---- Randevu ----
        modelBuilder.Entity<Randevu>(e =>
        {
            e.Property(r => r.Ucret).HasPrecision(10, 2);
            e.Property(r => r.BaslangicZamani).HasColumnType("timestamp without time zone");
            e.Property(r => r.Durum).HasConversion<string>().HasMaxLength(20);

            // Doktor, hasta veya hizmet silinirse randevular silinmesin
            e.HasOne(r => r.Hasta).WithMany()
                .HasForeignKey(r => r.HastaId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(r => r.Doktor).WithMany(d => d.Randevular)
                .HasForeignKey(r => r.DoktorId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(r => r.Hizmet).WithMany()
                .HasForeignKey(r => r.HizmetId).OnDelete(DeleteBehavior.Restrict);

            // Çakışma kontrolünü hızlandırmak için
            e.HasIndex(r => new { r.DoktorId, r.BaslangicZamani });
        });
    }
}