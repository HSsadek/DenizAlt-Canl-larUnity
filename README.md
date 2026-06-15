# 🐠 Denizaltı Canlıları — Artırılmış Gerçeklik Eğitim Uygulaması

Durağan eğitim kartlarını interaktif birer öğrenme aracına dönüştüren bir **Artırılmış Gerçeklik (AR)** mobil uygulamasıdır. Geleneksel yöntemlerdeki statik bilgiyi, **Image Tracking** teknolojisiyle canlandırarak deniz canlıları hakkında hem görsel hem de işitsel bir deneyim sunar.

---

## 📌 Proje Bağlantıları

| Platform | Bağlantı |
|---|---|
| 🎥 **YouTube** | [Proje Tanıtım Videosu](https://youtu.be/jYT07feivB0?si=RJbThjtEfd0nCMUl) |
| 💻 **GitHub** | [DenizAlt-Canl-larUnity](https://github.com/HSsadek/DenizAlt-Canl-larUnity) |
| 📋 **Trello** | [Proje Panosu](https://trello.com/invite/b/69a924789b8c2940e8650cd6/ATTI5eb3f8804fd07c78bd36e8eeb82890e3C9EA26F2/denizalticanlilar) |

---

## 👥 Grup Üyeleri ve Görev Dağılımı

### Hüseyin Sadık — `220541605`
| Alan | Görevler |
|---|---|
| **Proje Yönetimi** | GitHub repo yönetimi, Trello görev takibi, proje koordinasyonu |
| **3D Modelleme** | Orkinos (TunaFish.fbx) ve Köpek Balığı modellerinin araştırılması, seçimi ve projeye entegrasyonu |
| **Materyal & Texture** | PBR materyal haritalarının (BaseColor, Normal, Metallic, Roughness) yapılandırılması |
| **AR Sahne Kurulumu** | Vuforia Image Target'ların sahneye eklenmesi, 3D modellerin kart üzerine hizalanması |
| **Sunum** | YouTube tanıtım videosu hazırlanması ve anlatımı |

### Muhammed Kardıe — `220541602`
| Alan | Görevler |
|---|---|
| **UI Sistemi** | `UIManager.cs` — Splash, Tarama ve Bilgi paneli ekranlarının programatik olarak oluşturulması |
| **Tema & Tasarım** | `UITheme.cs` — Okyanus temalı renk paleti, tipografi, runtime sprite oluşturma (rounded rect, gradient, circle) |
| **AR Kontrol** | `FishUITrigger.cs` — Vuforia Observer ile tracking durumu yönetimi ve UI tetikleme |
| **AR Kontrol** | `FishARController.cs` — AR Foundation ile alternatif image tracking kontrolü |
| **Ses Sistemi** | `BackgroundMusic.cs` — Arka plan müziği, ses kısma (duck) ve geri yükleme mekanizması |
| **Animasyon** | `PulseAnim.cs` — UI elemanları için nabız animasyonu, tarama çizgisi animasyonu |
| **Durum Yönetimi** | Splash → Scanning → FishFound geçiş mantığı, smoothstep animasyonlar |

### İbrahim Neddef — `220541604`
| Alan | Görevler |
|---|---|
| **SWOT Analizi** | Projenin güçlü/zayıf yönleri, fırsatlar ve tehditler analizi (`docs/SWOT.pdf`) |
| **RAMS Analizi** | Güvenilirlik, Erişilebilirlik, Bakım ve Güvenlik analizi (`docs/RAMS.pdf`) |
| **THS Raporu** | Teknik değerlendirme raporu hazırlanması (`docs/THS_report.pdf`) |
| **Dokümantasyon** | Yazılım gereksinimleri ve proje belgelerinin hazırlanması (`docs/Requirements.md`) |
| **Eğitici İçerik** | Deniz canlıları hakkında çocuklara yönelik bilgi metinlerinin araştırılması ve yazılması |

---

## 🛠️ Teknik Altyapı ve Kullanılan Teknolojiler

| Teknoloji | Versiyon | Kullanım Amacı |
|---|---|---|
| **Unity** | 6000.4.4f1 (Unity 6) | Oyun motoru ve geliştirme ortamı |
| **URP** | Universal Render Pipeline | Mobil optimizasyonlu görsel render |
| **Vuforia Engine** | 11.4.4 | Image Target tanıma ve AR tracking |
| **AR Foundation** | 6.4.2 | Platform bağımsız AR altyapısı |
| **ARCore** | 6.4.2 | Android AR desteği |
| **C#** | — | Uygulama programlama dili |

---

## 🎯 Nasıl Çalışır?

1. Kullanıcı uygulamayı açar → okyanus temalı splash ekranı karşılar
2. Kamera tarama moduna geçer → animasyonlu tarama çerçevesi gösterilir
3. Kullanıcı basılı tanıtım kartını kameraya tutar
4. Sistem kartı tanır → kartın üzerine **3D balık modeli** gerçek zamanlı yerleşir
5. **Bilgi kartı** açılır (canlının adı, özellikleri, eğlenceli bilgiler)
6. **Sesli anlatım** otomatik başlar, arka plan müziği kısılır
7. Kart kameradan çekilince model ve bilgi kaybolur, tarama moduna dönülür

---

## 📂 Proje Yapısı

```
AR1/
├── Assets/
│   ├── Scripts/UI/          # C# kaynak kodları
│   │   ├── UIManager.cs     # Ana UI yöneticisi
│   │   ├── UITheme.cs       # Tasarım sistemi
│   │   ├── FishUITrigger.cs # Vuforia tracking tetikleyici
│   │   ├── BackgroundMusic.cs # Ses yönetimi
│   │   └── PulseAnim.cs     # Animasyon bileşeni
│   ├── FishARController.cs  # AR Foundation kontrolcüsü
│   ├── Char/                # 3D modeller (Orkinos, Köpek Balığı)
│   ├── Audio/               # Arka plan müziği
│   ├── Scenes/              # Unity sahnesi
│   └── images/              # Image Target görselleri
├── docs/                    # SWOT, RAMS, THS, Requirements belgeleri
└── README.md
```
