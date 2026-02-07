using App.Desktop.Models;

namespace App.Desktop.Services;

public sealed class DashboardService
{
    public IReadOnlyList<DashboardKpi> GetKpis()
    {
        return
        [
            new DashboardKpi("Günlük Sorgu", "128", "+12% • Dünden", "#1DB954"),
            new DashboardKpi("Yeni Şifre", "14", "Bugün • 5 onay bekliyor", "#7B8796"),
            new DashboardKpi("Bulunamadı", "7", "Son 24 saat", "#7B8796"),
            new DashboardKpi("Ort. Yanıt", "1.4 sn", "Telegram • 98% başarı", "#7B8796")
        ];
    }

    public IReadOnlyList<RecentActivity> GetRecentActivities()
    {
        return
        [
            new RecentActivity("🧰 Kofre 12894 şifresi güncellendi", "10:42"),
            new RecentActivity("🔎 !kofre 44521 sorgusu", "10:40"),
            new RecentActivity("✅ Excel import tamamlandı", "09:58"),
            new RecentActivity("⚠️ Eksik şifreli kofre 3 adet", "09:30")
        ];
    }
}
