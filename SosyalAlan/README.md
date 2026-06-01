# SosyalAlan

ASP.NET Core 8 Web API ile geliştirilmiş sosyal ağ uygulaması.

## Teknolojiler

- ASP.NET Core 8 Web API
- Entity Framework Core 8
- SQL Server LocalDB
- JWT Authentication
- BCrypt şifre hashleme
- Swagger / OpenAPI

## Mimari

Controller → Service → DbContext

## API Endpoint'leri

### Kimlik Doğrulama
| Method | Endpoint | Açıklama |
|--------|----------|----------|
| POST | /api/auth/kayit | Kayıt ol |
| POST | /api/auth/giris | Giriş yap, token al |

### Kullanıcı
| Method | Endpoint | Açıklama |
|--------|----------|----------|
| GET | /api/kullanici | Kullanıcıları listele ve ara |
| GET | /api/kullanici/profil | Kendi profilini gör |
| PUT | /api/kullanici/profil | Profilini güncelle |
| PATCH | /api/kullanici/sifre | Şifre değiştir |

### Arkadaşlık
| Method | Endpoint | Açıklama |
|--------|----------|----------|
| POST | /api/arkadaslik | Arkadaşlık isteği gönder |
| PATCH | /api/arkadaslik/{id} | İsteği kabul et veya reddet |
| GET | /api/arkadaslik | Arkadaş listesi |
| GET | /api/arkadaslik/bekleyen | Bekleyen istekler |
| DELETE | /api/arkadaslik/{arkadasId} | Arkadaşlıktan çıkar |

### Mesaj
| Method | Endpoint | Açıklama |
|--------|----------|----------|
| POST | /api/mesaj | Mesaj gönder |
| GET | /api/mesaj/{id} | Konuşmayı görüntüle |
| GET | /api/mesaj/okunmamis | Okunmamış mesaj sayısı |

## Güncelleme Geçmişi

### v2.0
- Servis katmanı eklendi (Controller → Service → DbContext)
- Profil görüntüleme ve güncelleme eklendi
- Şifre değiştirme eklendi
- Bekleyen arkadaşlık istekleri eklendi
- Arkadaşlıktan çıkarma eklendi
- Okunmamış mesaj sayısı eklendi

### v1.0
- Kullanıcı kayıt ve giriş
- Kullanıcı arama
- Arkadaşlık sistemi
- Mesajlaşma sistemi
